
using System.Collections.Generic;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using MediatR;

namespace EShop.Services.Ordering.Domain.Events {
    /// <summary>
    /// Event used when the grace period order is confirmed
    /// </summary>
    internal class OrderStatusChangedToAwaitingValidationDomainEvent : INotification {
        private int orderID;
        private IEnumerable<OrderItem> orderItems;

        public OrderStatusChangedToAwaitingValidationDomainEvent(int orderID, IEnumerable<OrderItem> orderItems) {
            this.OrderID = orderID;
            this.OrderItems = orderItems;
        }

        public int OrderID {
            get { return this.orderID; }
            set { this.orderID = value; }
        }

        public IEnumerable<OrderItem> OrderItems {
            get { return this.orderItems; }
            set { this.orderItems = value; }
        }
    }
}