
using System;
using EShop.Services.Ordering.Infrastructure.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShop.Services.Ordering.Infrastructure.EntityTypeConfigurations {
    internal class ClientRequestEntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest> {
        public void Configure(EntityTypeBuilder<ClientRequest> builder) {
            builder.ToTable("Requests", OrderingContext.DEFAULT_SCHEMA);
            builder.HasKey("id");

            builder
                .Property<Guid>("id")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ID")
                .IsRequired();

            builder
                .Property<string>("name")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Name")
                .IsRequired();

            builder
                .Property<DateTime>("requestDateTime")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("RequestDateTime")
                .IsRequired();
        }
    }
}