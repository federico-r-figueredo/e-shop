
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Infrastructure.EntityTypeConfigurations {
    internal class CardTypeEntityTypeConfiguration : IEntityTypeConfiguration<CardType> {
        public void Configure(EntityTypeBuilder<CardType> builder) {
            builder.ToTable(nameof(OrderingContext.CardTypes), OrderingContext.DEFAULT_SCHEMA);
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).HasDefaultValue(1).ValueGeneratedNever().IsRequired();
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        }
    }
}