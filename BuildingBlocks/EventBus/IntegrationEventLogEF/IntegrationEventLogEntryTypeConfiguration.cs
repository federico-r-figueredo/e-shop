using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.BuildingBlocks.IntegrationEventLogEF {
    internal class IntegrationEventLogEntryTypeConfiguration
        : IEntityTypeConfiguration<IntegrationEventLogEntry> {
        public void Configure(EntityTypeBuilder<IntegrationEventLogEntry> builder) {
            builder.ToTable("IntegrationEventLogs", IntegrationEventLogContext.DEFAULT_SCHEMA);
            builder.HasKey(x => x.EventID);
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.CreationDateTime).IsRequired();
            builder.Property(x => x.State).IsRequired();
            builder.Property(x => x.TimesSent).IsRequired();
            builder.Property(x => x.EventTypeName).IsRequired();
        }
    }
}