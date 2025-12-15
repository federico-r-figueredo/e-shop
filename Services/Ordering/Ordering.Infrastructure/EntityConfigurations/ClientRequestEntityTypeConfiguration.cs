using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eShop.Services.Ordering.Infrastructure.Idempotency;

namespace eShop.Services.Ordering.Infrastructure.EntityConfigurations {
    public class ClientRequestEntityTypeConfiguration :
        IEntityTypeConfiguration<ClientRequest> {
        public void Configure(EntityTypeBuilder<ClientRequest> builder) {
            builder.ToTable("ClientRequests", "Idempotency");

            builder.HasKey(x => x.GUID);

            builder.Property(x => x.Name).IsRequired();

            builder.Property(x => x.DateTime).IsRequired();
        }
    }
}