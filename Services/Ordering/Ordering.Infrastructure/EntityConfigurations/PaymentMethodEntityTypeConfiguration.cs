using System;
using Microsoft.EntityFrameworkCore;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Services.Ordering.Infrastructure.EntityConfigurations {
    public class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod> {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder) {

            builder.ToTable("PaymentMethods", OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID)
                .UseHiLo("payment_hilo", OrderingContext.DEFAULT_SCHEMA);

            builder.Ignore(x => x.DomainEvents);

            // Shadow state property created uniquely to configure the many-to-one
            // relationship in Buyer entity.
            builder.Property<int>("BuyerID")
                .IsRequired();

            builder.Property<string>("alias")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Alias")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<string>("cardNumber")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardNumber")
                .HasMaxLength(19)
                .IsRequired();

            builder.Property<string>("securityNumber")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("SecurityNumber")
                .HasMaxLength(4)
                .IsRequired();

            builder.Property<string>("cardHolderName")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardHolderName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property<DateTime>("expirationDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ExpirationDate")
                .HasMaxLength(25)
                .IsRequired();

            builder.Property<int>("cardTypeID")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardTypeID")
                .IsRequired();

            builder.HasOne(x => x.CardType)
                .WithMany()
                .HasForeignKey("cardTypeID");
        }
    }
}