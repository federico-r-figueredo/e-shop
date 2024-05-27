
using System;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Services.Ordering.Infrastructure.EntityTypeConfigurations {
    internal class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod> {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder) {
            builder.ToTable(nameof(OrderingContext.PaymentMethods), OrderingContext.DEFAULT_SCHEMA);
            builder.HasKey(x => x.ID);
            builder.Ignore(x => x.DomainEvents);

            builder
                .Property(x => x.ID)
                .UseHiLo("PaymentMethodSequence", OrderingContext.DEFAULT_SCHEMA);

            builder
                .Property<string>("alias")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Alias")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property<string>("paymentCardNumber")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentCardNumber")
                .HasMaxLength(25)
                .IsRequired();

            builder
                .Property<string>("cardHolderName")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardHolderName")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property<DateOnly>("cardExpiration")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardExpiration")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property<string>("cardVerificationCode")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardVerificationCode")
                .HasMaxLength(25)
                .IsRequired();

            builder
                .Property<int>("cardTypeID")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardTypeID")
                .IsRequired();

            builder
                .HasOne(x => x.CardType)
                .WithMany()
                .HasForeignKey("cardTypeID");
        }
    }
}