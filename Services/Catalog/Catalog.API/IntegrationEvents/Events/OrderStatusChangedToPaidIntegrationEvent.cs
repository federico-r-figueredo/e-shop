using System.Collections.Generic;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Catalog.API.IntegrationEvents.Events {
    public class OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent {
        public OrderStatusChangedToPaidIntegrationEvent(int orderID,
            IEnumerable<OrderStockItem> orderStockItem) {
            OrderID = orderID;
            OrderStockItems = orderStockItem;
        }

        public int OrderID { get; set; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; set; }
    }
}