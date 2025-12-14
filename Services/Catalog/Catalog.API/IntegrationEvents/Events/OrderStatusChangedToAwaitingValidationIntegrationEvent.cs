using System.Collections.Generic;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Catalog.API.IntegrationEvents.Events {
    public class OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent {
        public OrderStatusChangedToAwaitingValidationIntegrationEvent(int orderID,
            IEnumerable<OrderStockItem> orderStockItems) {
            OrderID = orderID;
            OrderStockItems = orderStockItems;
        }

        public int OrderID { get; set; }
        public IEnumerable<OrderStockItem> OrderStockItems { get; set; }
    }

    public class OrderStockItem {
        public OrderStockItem(int productID, int units) {
            ProductID = productID;
            Units = units;
        }

        public int ProductID { get; set; }
        public int Units { get; set; }
    }
}