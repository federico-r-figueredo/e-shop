using MediatR;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using System.Collections.Generic;
using System.Linq;

namespace eShop.Services.Ordering.Domain.Events {
    public class OrderCancelledDueToStockRejectedDomainEvent : INotification {
        private readonly Order order;
        private readonly IEnumerable<string> itemStockRejectedProductNames;

        public OrderCancelledDueToStockRejectedDomainEvent(Order order, IEnumerable<string> itemStockRejectedProductNames) {
            this.order = order;
            this.itemStockRejectedProductNames = itemStockRejectedProductNames;
        }

        public Order Order {
            get { return this.order; }
        }

        public IReadOnlyCollection<string> ItemStockRejectedProductNames {
            get { return this.itemStockRejectedProductNames.ToList().AsReadOnly(); }
        }
    }
}