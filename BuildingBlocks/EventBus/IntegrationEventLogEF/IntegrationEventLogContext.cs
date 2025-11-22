using Microsoft.EntityFrameworkCore;

namespace eShop.BuildingBlocks.IntegrationEventLogEF {
    public class IntegrationEventLogContext : DbContext {
        public const string DEFAULT_SCHEMA = "Integration";
        public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options)
            : base(options) { }

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IntegrationEventLogContext).Assembly);
        }
    }
}
