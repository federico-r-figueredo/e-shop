using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBus.Events;
using eShop.Services.Catalog.API.Infrastructure;
using eShop.Services.Catalog.API.IntegrationEvents.Events;
using eShop.Services.Catalog.API.Model;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Catalog.API.IntegrationEvents.EventHandling {
    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler
        : IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent> {

        private readonly CatalogContext catalogContext;
        private readonly ICatalogIntegrationEventService catalogIntegrationEventService;
        private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(
            CatalogContext catalogContext,
            ICatalogIntegrationEventService catalogIntegrationEventService,
            ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger) {

            this.catalogContext = catalogContext;
            this.catalogIntegrationEventService = catalogIntegrationEventService;
            this.logger = logger;
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent integrationEvent) {
            this.logger.LogInformation(
                "----- Handling integration event: {IntegrationEventID} at {AppName} - ({@IntegrationEvent})",
                integrationEvent.ID,
                Program.ApplicationName,
                integrationEvent
            );

            List<ConfirmedOrderStockItem> confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

            foreach (OrderStockItem orderStockItem in integrationEvent.OrderStockItems) {
                CatalogItem catalogItem = this.catalogContext.CatalogItems.Find(orderStockItem.ProductID);
                bool hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                ConfirmedOrderStockItem confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.ID, hasStock);

                confirmedOrderStockItems.Add(confirmedOrderStockItem);
            }

            var confirmedIntegrationEvent = confirmedOrderStockItems.Any(x => !x.HasStock)
                ? (IntegrationEvent)new OrderStockRejectedIntegrationEvent(integrationEvent.OrderID, confirmedOrderStockItems)
                : new OrderStockConfirmedIntegrationEvent(integrationEvent.OrderID);

            await this.catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(confirmedIntegrationEvent);
            await this.catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);
        }
    }
}