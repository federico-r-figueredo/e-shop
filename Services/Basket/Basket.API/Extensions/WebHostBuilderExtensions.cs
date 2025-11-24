using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using eShop.Services.Basket.API.Infrastructure.Filters;
using eShop.Services.Basket.API.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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

        internal static IWebHostBuilder ConfigurePortsFromConfig(this IWebHostBuilder builder) {
            return builder.ConfigureServices((context, services) => {

                IConfiguration configuration = context.Configuration;

                builder.ConfigureKestrel(options => {
                    (int httpPort, int grpcPort) = GetDefinedPorts(configuration);

                    // HTTP 1.1 + HTTP2
                    options.Listen(IPAddress.Any, httpPort, listenOptions => {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });

                    // gRPC (HTTP/2 only)
                    options.Listen(IPAddress.Any, grpcPort, ListenOptions => {
                        ListenOptions.Protocols = HttpProtocols.Http2;
                    });
                });
            });
        }

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration configuration) {
            int grpcPort = configuration.GetValue("GRPC_PORT", 5002);
            int httpPort = configuration.GetValue("PORT", 7172);
            return (httpPort, grpcPort);
        }
    }
}