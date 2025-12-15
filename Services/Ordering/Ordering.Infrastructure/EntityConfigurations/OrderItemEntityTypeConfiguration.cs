using Microsoft.EntityFrameworkCore;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Services.Ordering.Infrastructure.EntityConfigurations {
    public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem> {
        public void Configure(EntityTypeBuilder<OrderItem> builder) {

            builder.ToTable("OrderItems", OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID)
                .UseHiLo("orderitems_hilo", OrderingContext.DEFAULT_SCHEMA);

            builder.Ignore(x => x.DomainEvents);

            builder.Property<int>("ProductID")
                .IsRequired();

            builder.Property<string>("productName")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ProductName");

            builder.Property<string>("pictureURL")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PictureURL")
                .IsRequired(false);

            builder.Property<decimal>("unitPrice")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("UnitPrice")
                .IsRequired();

            builder.Property<decimal>("discount")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Discount")
                .IsRequired();

            builder.Property<int>("units")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Units")
                .IsRequired();
        }
    }
}