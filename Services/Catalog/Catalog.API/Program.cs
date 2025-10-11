using eShop.Services.Catalog.API.Extensions;
using eShop.Services.Catalog.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eShop.Services.Catalog.API {
    public static class Program {
        public static void Main(string[] args) {
            IHost host = CreateHostBuilder(args).Build();

            host.MigrateDbContext<CatalogContext>((context, services) => {
                IWebHostEnvironment environment = services.GetService<IWebHostEnvironment>();
                IOptions<CatalogSettings> settings = services.GetService<IOptions<CatalogSettings>>();
                ILogger<CatalogContextSeed> logger = services.GetService<ILogger<CatalogContextSeed>>();

                new CatalogContextSeed()
                    .SeedAsync(context, environment, settings, logger)
                    .Wait();
            });

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}