
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Infrastructure.Idempotency;

namespace Ordering.Infrastructure.EntityTypeConfigurations {
    internal class ClientRequestEntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest> {
        public void Configure(EntityTypeBuilder<ClientRequest> builder) {
            builder.ToTable("Requests", OrderingContext.DEFAULT_SCHEMA);
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.RequestDateTime).IsRequired();
        }
    }
}