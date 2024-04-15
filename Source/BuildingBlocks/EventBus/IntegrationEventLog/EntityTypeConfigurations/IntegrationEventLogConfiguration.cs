
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.BuildingBlocks.EventBus.IntegrationEventLog.EntityTypeConfigurations {
    internal class IntegrationEventLogConfiguration : IEntityTypeConfiguration<IntegrationEventLogEntry> {
        public void Configure(EntityTypeBuilder<IntegrationEventLogEntry> builder) {
            builder.ToTable(nameof(IntegrationEventLogEntry), "IntegrationEvent");
            builder.HasKey(x => x.IntegrationEventID);
            builder.Property(x => x.IntegrationEventID).IsRequired();
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.State).IsRequired();
            builder.Property(x => x.TimesSent).IsRequired();
            builder.Property(x => x.EventTypeName).IsRequired();
        }
    }
}