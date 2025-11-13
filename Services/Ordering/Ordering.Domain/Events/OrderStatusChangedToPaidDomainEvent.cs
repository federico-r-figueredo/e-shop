using System.Collections.Generic;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using MediatR;

namespace eShop.Services.Ordering.Domain.Events {
    public class OrderStatusChangedToPaidDomainEvent : INotification {
        private readonly int orderID;
        private readonly IEnumerable<OrderItem> orderItems;

        public OrderStatusChangedToPaidDomainEvent(int orderID, IEnumerable<OrderItem> orderItems) {
            this.orderID = orderID;
            this.orderItems = orderItems;
        }

        public int OrderID {
            get { return this.orderID; }
        }

        public IEnumerable<OrderItem> OrderItems {
            get { return this.orderItems; }
        }
    }
}