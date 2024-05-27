
namespace EShop.Services.Ordering.API.Application.IntegrationEvents.Events {
    internal class OrderStatusChangedToSubmittedIntegrationEvent {
        private readonly int orderID;
        private readonly string orderStatus;
        private readonly string buyerName;

        public OrderStatusChangedToSubmittedIntegrationEvent(int orderID, string orderStatus, string buyerName) {
            this.orderID = orderID;
            this.orderStatus = orderStatus;
            this.buyerName = buyerName;
        }

        public int OrderID {
            get { return this.orderID; }
        }

        public string OrderStatus {
            get { return this.orderStatus; }
        }

        public string BuyerName {
            get { return this.buyerName; }
        }
    }
}