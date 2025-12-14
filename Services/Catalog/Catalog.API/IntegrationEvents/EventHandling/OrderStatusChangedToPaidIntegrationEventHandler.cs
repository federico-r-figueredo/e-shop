using System;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.Services.Catalog.API.Infrastructure;
using eShop.Services.Catalog.API.IntegrationEvents.Events;
using eShop.Services.Catalog.API.Model;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Catalog.API.IntegrationEvents.EventHandling {
    public class OrderStatusChangedToPaidIntegrationEventHandler
    : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent> {

        private readonly CatalogContext catalogContext;
        private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger;

        public OrderStatusChangedToPaidIntegrationEventHandler(CatalogContext catalogContext,
            ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger) {
            this.catalogContext = catalogContext;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent integrationEvent) {
            this.logger.LogInformation(
                "----- Handling integration event: {IntegrationEventID} at {AppName} - ({@IntegrationEvent})",
                integrationEvent.ID,
                Program.ApplicationName,
                integrationEvent
            );

            // we're not blocking stock / inventory
            foreach (OrderStockItem orderStockItem in integrationEvent.OrderStockItems) {
                CatalogItem catalogItem = this.catalogContext.CatalogItems.Find(orderStockItem.ProductID);
                catalogItem.RemoveStock(orderStockItem.Units);
            }

            await this.catalogContext.SaveChangesAsync();
        }
    }
}