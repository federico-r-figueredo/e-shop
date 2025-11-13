using System;
using System.Collections.Generic;
using System.Linq;
using eShop.Services.Ordering.Domain.Exceptions;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.OrderAggregate {
    public class OrderStatus : Enumeration {
        public static OrderStatus Submitted = new OrderStatus(1, nameof(Submitted).ToLowerInvariant());
        public static OrderStatus AwaitingValidation = new OrderStatus(1, nameof(AwaitingValidation).ToLowerInvariant());
        public static OrderStatus StockConfirmed = new OrderStatus(1, nameof(StockConfirmed).ToLowerInvariant());
        public static OrderStatus Paid = new OrderStatus(1, nameof(Paid).ToLowerInvariant());
        public static OrderStatus Shipped = new OrderStatus(1, nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Cancelled = new OrderStatus(1, nameof(Cancelled).ToLowerInvariant());

        protected OrderStatus(int id, string name) : base(id, name) { }

        public static IEnumerable<OrderStatus> List() {
            return new[] { Submitted, AwaitingValidation, StockConfirmed, Paid, Shipped, Cancelled };
        }

        public static OrderStatus FromName(string name) {
            OrderStatus orderStatus = List().SingleOrDefault(
                x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase)
            );

            if (orderStatus == null) throw new OrderingDomainException(
                $"Possible values for OrderStatus are: {string.Join(",", List().Select(x => x.Name))}"
            );

            return orderStatus;
        }

        public static OrderStatus FromID(int id) {
            OrderStatus orderStatus = List().SingleOrDefault(x => x.ID == id);

            if (orderStatus == null) throw new OrderingDomainException(
                $"Possible values for OrderStatus are: {string.Join(",", List().Select(x => x.Name))}"
            );

            return orderStatus;
        }
    }
}