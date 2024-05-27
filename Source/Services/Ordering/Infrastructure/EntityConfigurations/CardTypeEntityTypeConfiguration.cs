
using System;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Services.Ordering.Infrastructure.EntityTypeConfigurations {
    internal class CardTypeEntityTypeConfiguration : IEntityTypeConfiguration<CardType> {
        public void Configure(EntityTypeBuilder<CardType> builder) {
            builder.ToTable(nameof(OrderingContext.CardTypes), OrderingContext.DEFAULT_SCHEMA);
            builder.HasKey("id");
            builder
                .Property<int>("id")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ID")
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();
            builder
                .Property<string>("name")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}