
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using MediatR;

namespace EShop.Services.Ordering.Domain.Events {
    internal class OrderCancelledDomainEvent : INotification {
        private Order order;

        public OrderCancelledDomainEvent(Order order) {
            this.Order = order;
        }

        public Order Order {
            get { return this.order; }
            set { this.order = value; }
        }
    }
}