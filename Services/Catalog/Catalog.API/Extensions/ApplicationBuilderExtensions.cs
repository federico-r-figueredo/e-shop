using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.Services.Catalog.API.IntegrationEvents.EventHandling;
using eShop.Services.Catalog.API.IntegrationEvents.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Services.Catalog.API.Extensions {
    internal static class ApplicationBuilderExtensions {
        internal static async void ConfigureEventBus(this IApplicationBuilder app) {
            IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            await eventBus.SubscribeAsync<
                OrderStatusChangedToAwaitingValidationIntegrationEvent,
                OrderStatusChangedToAwaitingValidationIntegrationEventHandler
            >();
            await eventBus.SubscribeAsync<
                OrderStatusChangedToPaidIntegrationEvent,
                OrderStatusChangedToPaidIntegrationEventHandler
            >();
        }
    }
}