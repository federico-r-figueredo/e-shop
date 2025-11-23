using System;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.Services.Basket.API.Extensions;
using eShop.Services.Basket.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Services.Basket.API {
    public class Startup {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services
        // to the IoC container.
        public IServiceProvider ConfigureServices(IServiceCollection services) {
            return services.AddMVC(this.configuration)
                .AddOptions(this.configuration)
                .AddRedis(this.configuration)
                .AddSwagger(this.configuration)
                .AddRabbitMQ(this.configuration)
                .AddIntegrationEventHandlers(this.configuration)
                .AddHTTPContextAccessor(this.configuration)
                .AddRepositories(this.configuration)
                .AddIdentityService(this.configuration)
                .AddAutofacModules(this.configuration);
        }

        // This method gets called by the runtime.
        // Use this method to configure the HTTP
        // request processing pipeline.
        public async void Configure(IApplicationBuilder builder, IWebHostEnvironment environment) {
            if (environment.IsDevelopment()) {
                builder.UseDeveloperExceptionPage();
                builder.UseSwagger();
                builder.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            builder.UseRouting();
            builder.UseAuthorization();
            builder.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            IEventBus eventBus = builder.ApplicationServices.GetRequiredService<IEventBus>();
            await eventBus.SubscribeAsync<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
            await eventBus.SubscribeAsync<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        }
    }
}