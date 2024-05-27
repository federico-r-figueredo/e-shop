using System;
using System.Threading;
using System.Threading.Tasks;
using Dawn;
using EShop.Services.Ordering.Domain.SeedWork;
using MediatR;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EShop.Services.Ordering.Infrastructure {
    public class UnitOfWork : IUnitOfWork {
        private readonly OrderingContext context;
        private IDbContextTransaction currentTransaction;
        private readonly IMediator mediator;
        private bool disposed;

        public UnitOfWork(OrderingContext context, IMediator mediator) {
            this.context = context;
            this.disposed = false;

            this.mediator = Guard
                .Argument(mediator, nameof(mediator))
                .NotNull()
                .Value;

            System.Diagnostics.Debug.WriteLine($"OrderingContext::ctor -> {this.GetHashCode()}");
        }

        public IDbContextTransaction CurrentTransaction {
            get { return this.currentTransaction; }
        }

        public bool HasActiveTransaction {
            get { return this.currentTransaction != null; }
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            return await this.context.SaveChangesAsync(cancellationToken) > 0;
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
            await this.mediator.DispatchDomainEventsAsync(this.context);

            // After executing this line all the changes (from the Command Handler and  Domain Event
            // Handlers) performed through the DbContext will be committed.
            return await this.context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync() {
            if (this.currentTransaction == null) return null;

            this.currentTransaction = await this.context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

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

        protected virtual void Dispose(bool disposing) {
            if (!this.disposed && disposing) {
                this.context.Dispose();
            }

            this.disposed = true;
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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