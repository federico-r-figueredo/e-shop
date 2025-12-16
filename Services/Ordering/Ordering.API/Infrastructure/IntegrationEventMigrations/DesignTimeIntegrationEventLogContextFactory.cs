using eShop.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eShop.Services.Ordering.API.Infrastructure.IntegrationEventMigrations {
    public class DesignTimeIntegrationEventLogContextFactory
        : IDesignTimeDbContextFactory<IntegrationEventLogContext> {
        public IntegrationEventLogContext CreateDbContext(string[] args) {
            DbContextOptionsBuilder<IntegrationEventLogContext> optionsBuilder =
                new DbContextOptionsBuilder<IntegrationEventLogContext>();

            optionsBuilder.UseSqlServer(
                @"Server=localhost;Initial Catalog=eShop.Services.OrderingDB;
                User Id=admin;Password=1234;TrustServerCertificate=True;Encrypt=false;
                Trusted_Connection=True;",
                options => {
                    options.MigrationsAssembly(this.GetType().Assembly.GetName().Name);
                    options.MigrationsHistoryTable("MigrationsHistory", "EFCore");
                }
            );

            return new IntegrationEventLogContext(optionsBuilder.Options);
        }
    }
}