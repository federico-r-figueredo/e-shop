using eShop.Services.Catalog.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Services.Catalog.API.Infrastructure.EntityConfigurations {
    internal class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand> {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder) {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseHiLo("catalog_brands_hilo").IsRequired();
            builder.Property(x => x.Brand).IsRequired().HasMaxLength(100);
        }
    }
}