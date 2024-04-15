
using Microsoft.EntityFrameworkCore;

namespace EShop.BuildingBlocks.EventBus.IntegrationEventLog {
    internal class IntegrationEventLogContext : DbContext {
        private DbSet<IntegrationEventLogEntry> integrationEventLogs;

        public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options)
            : base(options) { }

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogs {
            get { return this.integrationEventLogs; }
            set { this.integrationEventLogs = value; }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IntegrationEventLogContext).Assembly);
        }
    }
}