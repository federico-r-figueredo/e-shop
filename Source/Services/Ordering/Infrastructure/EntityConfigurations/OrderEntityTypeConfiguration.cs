
using System;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Infrastructure.EntityTypeConfigurations {
    internal class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order> {
        public void Configure(EntityTypeBuilder<Order> builder) {
            builder.ToTable(nameof(OrderingContext.Orders), OrderingContext.DEFAULT_SCHEMA);
            builder.HasKey(x => x.ID);
            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.ID).UseHiLo("OrdersSequence", OrderingContext.DEFAULT_SCHEMA);

            // Address value object is persisted as owned entity type (supported since EF Core 2.0)
            builder.OwnsOne(x => x.Address, x => {
                // Explicit configuration of the shadow property in the owned type
                // as a workaround for a documented issue in EF Core 5 (https://github.com/dotnet/efcore/issues/20740)
                x.Property<int>("OrderID").UseHiLo("OrdersSequence", OrderingContext.DEFAULT_SCHEMA);
                x.WithOwner();
            });

            builder
                .Property<int?>("buyerID")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("BuyerID")
                .IsRequired(false);

            builder
                .Property<DateTime>("orderDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderDate")
                .IsRequired();

            builder
                .Property<int>("orderStatusID")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderStatusID")
                .IsRequired();

            builder
                .Property<int?>("paymentMethodID")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentMethodID")
                .IsRequired(false);

            builder
                .Property<string>("description")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Description")
                .IsRequired(false);

            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));

            // DDD Patterns comment:
            // Set as field (since EF Core 1.1) to access the OrderItem collection property
            // through its field.
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasOne<PaymentMethod>()
                .WithMany()
                // .HasForeignKey("PaymentMethodID")
                .HasForeignKey("paymentMethodID")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne<Buyer>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey("buyerID");

            builder
                .HasOne(x => x.OrderStatus)
                .WithMany()
                .HasForeignKey("orderStatusID");
        }
    }
}