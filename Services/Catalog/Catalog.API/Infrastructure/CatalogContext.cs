using eShop.Services.Catalog.API.Model;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.Catalog.API.Infrastructure {
    internal class CatalogContext : DbContext {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
        }
    }
}