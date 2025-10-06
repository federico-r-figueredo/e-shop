using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eShop.Services.Catalog.API.Infrastructure {
    internal class CatalogContextDesignFactory : IDesignTimeDbContextFactory<CatalogContext> {
        public CatalogContext CreateDbContext(string[] args) {
            DbContextOptionsBuilder<CatalogContext> optionsBuilder =
                new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(
                    @"Server=localhost;Initial Catalog=eShop.Services.CatalogDB;
                    User Id=admin;Password=1234;TrustServerCertificate=True;
                    Encrypt=false;Trusted_Connection=True;"
                );

            return new CatalogContext(optionsBuilder.Options);
        }
    }
}