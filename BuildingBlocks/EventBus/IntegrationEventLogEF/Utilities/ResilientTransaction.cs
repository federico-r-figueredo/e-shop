using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShop.BuildingBlocks.IntegrationEventLogEF.Utilities {
    public class ResilientTransaction {
        private DbContext context;

        private ResilientTransaction(DbContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static ResilientTransaction New(DbContext context) {
            return new ResilientTransaction(context);
        }

        public async Task ExecuteAsync(Func<Task> action) {
            // Use of an EF Core resiliency strategy when using multiple DbContexts within
            // an explicit BeginTransaction(). 
            // See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            IExecutionStrategy strategy = this.context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {
                using (IDbContextTransaction transaction = await
                    this.context.Database.BeginTransactionAsync()) {
                    await action();
                    transaction.Commit();
                }
            });
        }
    }
}