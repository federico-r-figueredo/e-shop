using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Catalog.API.IntegrationEvents.Events {
    public class OrderStockConfirmedIntegrationEvent : IntegrationEvent {
        public OrderStockConfirmedIntegrationEvent(int orderID) {
            OrderID = orderID;
        }

        public int OrderID { get; set; }
    }
}