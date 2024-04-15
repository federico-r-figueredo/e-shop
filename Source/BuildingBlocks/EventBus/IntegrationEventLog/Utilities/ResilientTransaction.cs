using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EShop.BuildingBlocks.EventBus.IntegrationEventLog.Utilities {
    internal class ResilientTransaction {
        private readonly DbContext dbContext;

        private ResilientTransaction(DbContext dbContext) {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public static ResilientTransaction Create(DbContext dbContext) {
            return new ResilientTransaction(dbContext);
        }

        public async Task ExecuteAsync(Func<Task> action) {
            // Use of an EF Core resiliency strategy when using multiple DbContexts 
            // within an explicit BeginTransation();
            // See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            IExecutionStrategy executionStrategy = this.dbContext.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () => {
                using (var transaction = await this.dbContext.Database.BeginTransactionAsync()) {
                    await action();
                    await transaction.CommitAsync();
                }
            });
        }
    }
}