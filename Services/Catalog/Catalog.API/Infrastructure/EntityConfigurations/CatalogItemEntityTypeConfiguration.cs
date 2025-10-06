using eShop.Services.Catalog.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Services.Catalog.API.Infrastructure.EntityConfigurations {
    internal class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem> {
        public void Configure(EntityTypeBuilder<CatalogItem> builder) {
            builder.Property(x => x.ID)
                .UseHiLo("catalog_items_hilo")
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Price)
                .IsRequired();

            builder.Property(x => x.PictureFileName)
                .IsRequired();

            builder.Ignore(x => x.PictureURI);

            builder.HasOne(x => x.CatalogBrand)
                .WithMany()
                .HasForeignKey(x => x.CatalogBrandID);

            builder.HasOne(x => x.CatalogType)
                .WithMany()
                .HasForeignKey(x => x.CatalogTypeID);
        }
    }
}