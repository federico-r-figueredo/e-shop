
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Services.Ordering.Infrastructure.EntityTypeConfigurations {
    internal class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatus> {
        public void Configure(EntityTypeBuilder<OrderStatus> builder) {
            builder.ToTable(nameof(OrderingContext.OrderStatuses), OrderingContext.DEFAULT_SCHEMA);
            builder.HasKey("id");

            builder
                .Property<int>("id")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ID")
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder
                .Property<string>("name")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}