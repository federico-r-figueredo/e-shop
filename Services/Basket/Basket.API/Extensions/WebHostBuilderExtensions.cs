using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using eShop.Services.Basket.API.Infrastructure.Filters;
using eShop.Services.Basket.API.Infrastructure.Options;
using Microsoft.Extensions.Configuration;

namespace eShop.Services.Basket.API.Extensions {
    internal static class WebHostBuilderExtensions {
        internal static IWebHostBuilder UseFailing(this IWebHostBuilder builder,
            Action<FailingOptions> options) {

            builder.ConfigureServices(services => {
                services.AddSingleton<IStartupFilter>(new FailingStartupFilter(options));
            });

            return builder;
        }

        internal static IWebHostBuilder ConfigureAzureKeyVault(this IWebHostBuilder builder) {
            return builder.ConfigureAppConfiguration((context, configurationBuilder) => {
                IConfigurationRoot configuration = configurationBuilder.Build();

                if (configuration.GetValue<bool>("UseAzureKeyVault", false)) {
                    configurationBuilder.AddAzureKeyVault(
                        $"https://{configuration["AzureKeyVault:Name"]}.vault.azure.net/",
                        configuration["AzureKeyVault:ClientID"],
                        configuration["AzureKeyVault:ClientSecret"]
                    );
                }
            });
        }
    }
}