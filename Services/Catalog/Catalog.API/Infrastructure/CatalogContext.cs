using eShop.Services.Catalog.API.Model;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.Catalog.API.Infrastructure {
    public class CatalogContext : DbContext {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

        public const string DEFAULT_SCHEMA = "Catalog";

        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
        }
    }
}