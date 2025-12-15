using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;

namespace eShop.Services.Ordering.Infrastructure.EntityConfigurations {
    public class CardTypeEntityTypeConfiguration : IEntityTypeConfiguration<CardType> {
        public void Configure(EntityTypeBuilder<CardType> builder) {
            builder.ToTable("CardTypes", OrderingContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}