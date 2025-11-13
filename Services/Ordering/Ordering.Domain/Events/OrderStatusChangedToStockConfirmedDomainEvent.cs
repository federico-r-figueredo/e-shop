using MediatR;

namespace eShop.Services.Ordering.Domain.Events {
    public class OrderStatusChangedToStockConfirmedDomainEvent : INotification {
        private readonly int orderID;

        public OrderStatusChangedToStockConfirmedDomainEvent(int orderID) {
            this.orderID = orderID;
        }

        public int OrderID {
            get { return this.orderID; }
        }
    }
}