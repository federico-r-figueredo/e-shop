using System;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.Services.Ordering.API.Application.IntegrationEvents.EventHandling;
using eShop.Services.Ordering.API.Application.IntegrationEvents.Events;
using eShop.Services.Ordering.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Services.Ordering.API {
    public class Startup {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the
        // IoC container.
        public IServiceProvider ConfigureServices(IServiceCollection services) {
            return services.AddMVC(this.configuration)
                .AddDBContext(this.configuration)
                .AddSwagger(this.configuration)
                .AddOptions(this.configuration)
                .AddEventBus(this.configuration)
                .AddIntegrationEventHandlers(this.configuration)
                .AddIntegrationEventServices(this.configuration)
                .AddAutofacModules(this.configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTPS
        // request processing pipeline.
        public async void Configure(IApplicationBuilder applcation, IWebHostEnvironment environment) {
            // Configure the HTTP request pipeline.
            if (environment.IsDevelopment()) {
                applcation.UseDeveloperExceptionPage();
                applcation.UseSwagger();
                applcation.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            applcation.UseRouting();
            applcation.UseAuthorization();
            applcation.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            IEventBus eventBus = applcation.ApplicationServices.GetRequiredService<IEventBus>();
            await eventBus.SubscribeAsync<UserCheckoutAcceptedIntegrationEvent, UserCheckoutAcceptedIntegrationEventHandler>();
        }
    }
}