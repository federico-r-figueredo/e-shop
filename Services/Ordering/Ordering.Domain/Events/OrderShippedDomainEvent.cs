using MediatR;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;

namespace eShop.Services.Ordering.Domain.Events {
    public class OrderShippedDomainEvent : INotification {
        private readonly Order order;

        public OrderShippedDomainEvent(Order order) {
            this.order = order;
        }

        public Order Order {
            get { return this.order; }
        }
    }
}