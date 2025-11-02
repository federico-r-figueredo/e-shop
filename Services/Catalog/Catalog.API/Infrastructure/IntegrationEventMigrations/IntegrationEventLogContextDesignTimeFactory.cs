using eShop.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eShop.Services.Catalog.API.Infrastructure.IntegrationEventMigrations {
    public class IntegrationEventLogContextDesignTimeFactory
        : IDesignTimeDbContextFactory<IntegrationEventLogContext> {
        public IntegrationEventLogContext CreateDbContext(string[] args) {
            DbContextOptionsBuilder<IntegrationEventLogContext> optionsBuilder =
                new DbContextOptionsBuilder<IntegrationEventLogContext>();

            optionsBuilder.UseSqlServer(
                @"Server=localhost;Initial Catalog=eShop.Services.IntegrationEventLogDB;
                User Id=admin;Password=1234;TrustServerCertificate=True;Encrypt=false;
                Trusted_Connection=True;",
                options => options.MigrationsAssembly(GetType().Assembly.GetName().Name)
            );

            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}