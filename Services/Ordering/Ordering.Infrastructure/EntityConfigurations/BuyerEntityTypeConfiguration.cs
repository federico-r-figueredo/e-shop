using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;
using Microsoft.EntityFrameworkCore.Metadata;

namespace eShop.Services.Ordering.Infrastructure.EntityConfigurations {
    internal class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer> {
        public void Configure(EntityTypeBuilder<Buyer> builder) {
            builder.ToTable("Buyers", OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID)
                .UseHiLo("buyer_hilo", OrderingContext.DEFAULT_SCHEMA);

            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.IdentityGUID)
                .HasMaxLength(200)
                .IsRequired();
            builder.HasIndex("IdentityGUID")
                .IsUnique(true);

            builder.Property(x => x.Name);

            builder.HasMany(x => x.PaymentMethods)
                .WithOne()
                .HasForeignKey("BuyerID")
                .OnDelete(DeleteBehavior.Cascade);

            IMutableNavigation navigationProperty = builder
                .Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

            navigationProperty.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}