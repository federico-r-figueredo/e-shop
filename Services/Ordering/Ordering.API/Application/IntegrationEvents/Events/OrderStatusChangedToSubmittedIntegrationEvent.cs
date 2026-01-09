using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Ordering.API.Application.IntegrationEvents.Events {
    public class OrderStatusChangedToSubmittedIntegrationEvent : IntegrationEvent {
        public OrderStatusChangedToSubmittedIntegrationEvent(int orderID, string orderStatus,
            string buyerName) {
            OrderID = orderID;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }

        public int OrderID { get; set; }
        public string OrderStatus { get; set; }
        public string BuyerName { get; set; }
    }
}