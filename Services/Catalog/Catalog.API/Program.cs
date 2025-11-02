using eShop.BuildingBlocks.IntegrationEventLogEF;
using eShop.Services.Catalog.API.Extensions;
using eShop.Services.Catalog.API.Infrastructure;
using eShop.Services.Catalog.API.Settings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eShop.Services.Catalog.API {
    public static class Program {
        private static readonly string @namespace = typeof(Program).Namespace;
        private static readonly string applicationName = @namespace.Substring(
            @namespace.LastIndexOf('.', @namespace.LastIndexOf('.') - 1) + 1
        );

        public static string ApplicationName {
            get { return applicationName; }
        }

        public static void Main(string[] args) {
            IWebHost webHost = CreateWebHostBuilder(args).Build();

            webHost.MigrateDbContext<CatalogContext>((context, services) => {
                IWebHostEnvironment environment = services.GetService<IWebHostEnvironment>();
                IOptions<CatalogSettings> settings = services.GetService<IOptions<CatalogSettings>>();
                ILogger<CatalogContextSeed> logger = services.GetService<ILogger<CatalogContextSeed>>();

                new CatalogContextSeed()
                    .SeedAsync(context, environment, settings, logger)
                    .Wait();
            })
            .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}