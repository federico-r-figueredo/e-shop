using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using eShop.Services.Basket.API.Infrastructure.Filters;
using eShop.Services.Basket.API.Infrastructure.Options;

namespace eShop.Services.Basket.API.Extensions {
    internal static class WebHostBuilderExtensions {
        internal static IWebHostBuilder UseFailing(this IWebHostBuilder builder,
            Action<FailingOptions> options) {

            builder.ConfigureServices(services => {
                services.AddSingleton<IStartupFilter>(new FailingStartupFilter(options));
            });

            return builder;
        }
    }
}