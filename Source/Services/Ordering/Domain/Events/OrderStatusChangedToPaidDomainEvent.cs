
using System.Collections.Generic;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using MediatR;

namespace EShop.Services.Ordering.Domain.Events {
    /// <summary>
    /// Event used when the order is paid
    /// </summary>
    public class OrderStatusChangedToPaidDomainEvent : INotification {
        private int orderID;
        private IEnumerable<OrderItem> orderItems;

        public OrderStatusChangedToPaidDomainEvent(int orderID, IEnumerable<OrderItem> orderItems) {
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