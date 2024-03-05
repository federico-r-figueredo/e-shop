
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Infrastructure.EntityTypeConfigurations {
    internal class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem> {
        public void Configure(EntityTypeBuilder<OrderItem> builder) {
            builder.ToTable(nameof(OrderingContext.OrderItems), OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.ID);

            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.ID).UseHiLo("OrderItemSequence");

            builder.Property("OrderID").IsRequired();

            builder
                .Property<decimal>("discount")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Discount")
                .IsRequired();

            // TODO: Review if this is correct or we need to target productID field
            builder
                .Property<int>("ProductID")
                .IsRequired();

            builder
                .Property<string>("productName")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ProductName")
                .IsRequired();

            builder
                .Property<decimal>("unitPrice")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("UnitPrice")
                .IsRequired();

            builder
                .Property<int>("units")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Units")
                .IsRequired();

            builder
                .Property<string>("pictureURL")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PictureURL")
                .IsRequired(false);
        }
    }
}