
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using MediatR;

namespace EShop.Services.Ordering.Domain.Events {
    public class OrderShippedDomainEvent : INotification {
        private Order order;

        public OrderShippedDomainEvent(Order order) {
            this.Order = order;
        }

        public Order Order {
            get { return this.order; }
            set { this.order = value; }
        }
    }
}