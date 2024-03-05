
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dawn;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using EShop.Services.Ordering.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ordering.Infrastructure {
    public class OrderingContext : DbContext, IUnitOfWork {
        public const string DEFAULT_SCHEMA = "Ordering";

        private readonly IMediator mediator;
        private IDbContextTransaction currentTransaction;

        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

        public OrderingContext(DbContextOptions<OrderingContext> options, IMediator mediator)
            : base(options) {
            this.mediator = Guard
                .Argument(mediator, nameof(mediator))
                .NotNull()
                .Value;

            System.Diagnostics.Debug.WriteLine($"OrderingContext::ctor -> {this.GetHashCode()}");
        }

        #region Order Aggregate

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }

        #endregion

        #region Buyer Aggregate

        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

        #endregion

        public IDbContextTransaction CurrentTransaction {
            get { return this.currentTransaction; }
        }

        public bool HasActiveTransaction {
            get { return this.currentTransaction != null; }
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default) {
            // Dispatch Domain Events Collection
            // Choices:
            // A) Right BEFORE commiting data (EF SaveChanges) into the DB will make a single
            //    transaction including side effects from the domain event handlers which are
            //    using the same DbContext with "InstancePerLifetimeScope" or "Scoped" lifetime.
            // B) Right AFTER committing data (EF SaveChanges) into the DB will require multiple
            //    transactions.
            // You will need to handle eventual consistency and compensatory actions in case
            // failures in any of the domain event handlers arise.
            await this.mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and  Domain Event
            // Handlers) performed through the DbContext will be committed.
            int result = await base.SaveChangesAsync(cancellationToken);

            return result > 0;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync() {
            if (this.currentTransaction == null) return null;

            this.currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return this.currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction) {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != this.currentTransaction) throw new InvalidOperationException(
                $"Transaction {transaction.TransactionId} is not the current" +
                $" transaction ({this.currentTransaction.TransactionId})");

            try {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            } catch {
                RollbackTransaction();
                throw;
            } finally {
                if (this.currentTransaction != null) {
                    this.currentTransaction.Dispose();
                    this.currentTransaction = null;
                }
            }
        }

        private void RollbackTransaction() {
            try {
                this.currentTransaction?.Rollback();
            } finally {
                if (this.currentTransaction != null) {
                    this.currentTransaction.Dispose();
                    this.currentTransaction = null;
                }
            }
        }
    }
}