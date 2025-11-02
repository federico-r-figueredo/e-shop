using System;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.Services.Catalog.API.Infrastructure;
using eShop.Services.Catalog.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Catalog.API.IntegrationEvents.EventHandling {
    public class ProductPriceChangedIntegrationEventHandler
        : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent> {
        private readonly CatalogContext catalogContext;
        private readonly ILogger<ProductPriceChangedIntegrationEventHandler> logger;

        public ProductPriceChangedIntegrationEventHandler(CatalogContext catalogContext, ILogger<ProductPriceChangedIntegrationEventHandler> logger) {
            this.catalogContext = catalogContext;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(ProductPriceChangedIntegrationEvent integrationEvent) {
            Console.WriteLine($"I'm handling {typeof(ProductPriceChangedIntegrationEvent).Name}!");
            return Task.CompletedTask;
        }
    }
}