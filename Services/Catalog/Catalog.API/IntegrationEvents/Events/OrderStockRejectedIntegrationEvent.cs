using System.Collections.Generic;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Catalog.API.IntegrationEvents.Events {
    public class OrderStockRejectedIntegrationEvent : IntegrationEvent {
        public OrderStockRejectedIntegrationEvent(int orderID,
            List<ConfirmedOrderStockItem> orderStockItems) {
            OrderID = orderID;
            OrderStockItems = orderStockItems;
        }

        public int OrderID { get; set; }
        public List<ConfirmedOrderStockItem> OrderStockItems { get; set; }
    }

    public class ConfirmedOrderStockItem {
        public ConfirmedOrderStockItem(int productID, bool hasStock) {
            ProductID = productID;
            HasStock = hasStock;
        }

        public int ProductID { get; }
        public bool HasStock { get; set; }
    }
}