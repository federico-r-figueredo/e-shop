
using System.Collections.Generic;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using MediatR;

namespace EShop.Services.Ordering.Domain.Events {
    /// <summary>
    /// Event used when the order stock items are confirmed
    /// </summary>
    public class OrderStatusChangedToStockConfirmedDomainEvent : INotification {
        private int orderID;
        private IEnumerable<OrderItem> orderItems;

        public OrderStatusChangedToStockConfirmedDomainEvent(int orderID, IEnumerable<OrderItem> orderItems) {
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