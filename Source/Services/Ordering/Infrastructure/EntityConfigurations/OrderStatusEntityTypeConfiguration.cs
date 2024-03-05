
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Infrastructure.EntityTypeConfigurations {
    internal class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatus> {
        public void Configure(EntityTypeBuilder<OrderStatus> builder) {
            builder.ToTable(nameof(OrderingContext.OrderStatuses), OrderingContext.DEFAULT_SCHEMA);
            builder.HasKey(x => x.ID);

            builder
                .Property(x => x.ID)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder
                .Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}