using System;
using eShop.Services.Basket.API.Infrastructure.Middleware;
using eShop.Services.Basket.API.Infrastructure.Options;
using Microsoft.AspNetCore.Builder;

namespace eShop.Services.Basket.API.Extensions {
    internal static class ApplicationBuilderExtensions {
        internal static IApplicationBuilder UseFailingMiddleware(this IApplicationBuilder builder) {
            return UseFailingMiddleware(builder, null);
        }

        internal static IApplicationBuilder UseFailingMiddleware(this IApplicationBuilder builder,
            Action<FailingOptions> action) {
            FailingOptions options = new FailingOptions();
            action?.Invoke(options);
            builder.UseMiddleware<FailingMiddleware>(options);
            return builder;
        }
    }
}