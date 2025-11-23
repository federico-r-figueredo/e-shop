using System;
using eShop.Services.Basket.API.Extensions;
using eShop.Services.Basket.API.Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace eShop.Services.Basket.API.Infrastructure.Filters {
    internal class FailingStartupFilter : IStartupFilter {
        private readonly Action<FailingOptions> options;

        public FailingStartupFilter(Action<FailingOptions> options) {
            this.options = options;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) {
            return app => {
                app.UseFailingMiddleware(options);
                next(app);
            };
        }
    }
}