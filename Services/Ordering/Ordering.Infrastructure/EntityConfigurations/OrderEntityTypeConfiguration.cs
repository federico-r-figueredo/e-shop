using Microsoft.EntityFrameworkCore;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;

namespace eShop.Services.Ordering.Infrastructure.EntityConfigurations {
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order> {
        public void Configure(EntityTypeBuilder<Order> builder) {
            builder.ToTable("Orders", OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID)
                .UseHiLo("order_hilo", OrderingContext.DEFAULT_SCHEMA);

            builder.Ignore(x => x.DomainEvents);

            builder.Property<DateTime>("orderDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderDate")
                .IsRequired();

            // Address value object persisted as owned 
            // entity type supported since EF Core 2.0
            builder.OwnsOne(x => x.Address, x => {
                x.WithOwner();
            });

            builder.Property<int?>("buyerID")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("BuyerID")
                .IsRequired(false);

            builder.HasOne<Buyer>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey("buyerID");

            builder.Property<int?>("paymentMethodID")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentMethodID")
                .IsRequired(false);

            builder.HasOne<PaymentMethod>()
                .WithMany()
                .HasForeignKey("paymentMethodID")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OrderStatus)
                .WithMany()
                .HasForeignKey("OrderStatusID");

            builder.Property<string>("Description")
                .IsRequired(false);

            builder.Property<bool>("isDraft")
                .IsRequired(true);

            var navigationProperty = builder.Metadata.FindNavigation(nameof(Order.OrderItems));
            // DDD Patterns comment:
            // Set as field (new since EF Core 1.1) to access the OrderItem collection
            // property through its field.
            navigationProperty.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}