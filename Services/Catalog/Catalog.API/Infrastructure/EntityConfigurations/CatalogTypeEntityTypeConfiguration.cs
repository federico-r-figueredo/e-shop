using eShop.Services.Catalog.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Services.Catalog.API.Infrastructure.EntityConfigurations {
    internal class CatalogTypeEntityTypeConfiguration : IEntityTypeConfiguration<CatalogType> {
        public void Configure(EntityTypeBuilder<CatalogType> builder) {
            builder.ToTable("CatalogTypes", CatalogContext.DEFAULT_SCHEMA);

            builder.HasKey(x => x.ID);

            builder.Property(x => x.ID)
                .UseHiLo("catalog_types_hilo")
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}