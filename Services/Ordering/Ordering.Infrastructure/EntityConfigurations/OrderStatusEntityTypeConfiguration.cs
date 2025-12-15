using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Services.Ordering.Infrastructure.EntityConfigurations {
    public class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatus> {
        public void Configure(EntityTypeBuilder<OrderStatus> builder) {

            builder.ToTable("OrderStatuses", OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}