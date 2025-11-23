using System;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;

namespace eShop.Services.Basket.API.IntegrationEvents.Events {
    public class ProductPriceChangedIntegrationEventHandler
        : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent> {
        public Task Handle(ProductPriceChangedIntegrationEvent integrationEvent) {
            Console.WriteLine($"I'm handling {typeof(ProductPriceChangedIntegrationEvent).Name}");
            return Task.CompletedTask;
        }
    }
}