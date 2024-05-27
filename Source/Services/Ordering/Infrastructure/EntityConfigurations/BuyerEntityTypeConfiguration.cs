
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Services.Ordering.Infrastructure.EntityTypeConfigurations {
    internal class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer> {
        public void Configure(EntityTypeBuilder<Buyer> builder) {
            builder.ToTable("Buyers", OrderingContext.DEFAULT_SCHEMA);

            builder.Ignore(x => x.DomainEvents);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseHiLo("BuyersSequence", OrderingContext.DEFAULT_SCHEMA);

            builder.Property(x => x.IdentityGUID).HasMaxLength(200).IsRequired();
            builder.HasIndex(nameof(Buyer.IdentityGUID)).IsUnique();

            builder.Property(x => x.Name);

            builder.HasMany(x => x.PaymentMethods)
                .WithOne()
                .HasForeignKey("BuyerID")
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}