using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using eShop.Services.Ordering.Domain.SeedWork;
using eShop.Services.Ordering.Infrastructure.Extensions;
using eShop.Services.Ordering.Infrastructure.Idempotency;

namespace eShop.Services.Ordering.Infrastructure {
    public class OrderingContext : DbContext, IUnitOfWork {
        private readonly IMediator mediator;
        private IDbContextTransaction currentTransaction;

        public const string DEFAULT_SCHEMA = "Ordering";

        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options) { }

        public OrderingContext(DbContextOptions<OrderingContext> options, IMediator mediator)
            : base(options) {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            Debug.WriteLine("OrderingContext::ctor -> " + base.GetHashCode());
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<ClientRequest> ClientRequests { get; set; }

        public IDbContextTransaction CurrentTransaction {
            get { return this.currentTransaction; }
        }

        public bool HasActiveTransaction {
            get { return this.currentTransaction != null; }
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.ApplyConfigurationsFromAssembly(typeof(OrderingContext).Assembly);
        }

        public async Task<bool> SaveEntitiesAsync(
            CancellationToken cancellationToken = default(CancellationToken)) {
            // Dispatch Domain Events collection
            // Choices:
            // A) Right BEFORE committing data (DbContext::SaveChanges) into the DB will
            // require a single transaction including side effects from the domain event 
            // handlers which are using the same DbContext with "InstancePerLifetimeScope" 
            // or "Scoped" lifetime.
            // B) Right AFTER committing data (DbContext::SaveChanges) into the DB which
            // will require multiple transactions. You will need to handle eventual
            // consistency and compensatory actions in case of failures in any of the
            // handlers.
            await this.mediator.DispatchDomainEventsAsync(this);

            // After executing this line, all changes (from the Command Handler and Domain
            // Event Handlers) performed through the DbContext will be committed.
            return await base.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync() {
            if (this.currentTransaction != null) return null;

            this.currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return this.currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction) {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != this.currentTransaction) throw new InvalidOperationException(
                $"Transaction {transaction.TransactionId} is not the current transaction"
            );

            try {
                await base.SaveChangesAsync();
                transaction.Commit();
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

        public void RollbackTransaction() {
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