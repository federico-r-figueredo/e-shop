
using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Services.Ordering.Domain.Exceptions;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.OrderAggregate {
    public class OrderStatus : Enumeration {
        public static OrderStatus Submitted = new OrderStatus(1, nameof(Submitted));
        public static OrderStatus AwaitingValidation = new OrderStatus(2, nameof(AwaitingValidation));
        public static OrderStatus StockConfirmed = new OrderStatus(3, nameof(StockConfirmed));
        public static OrderStatus Paid = new OrderStatus(4, nameof(Paid));
        public static OrderStatus Shipped = new OrderStatus(5, nameof(Shipped));
        public static OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled));

        public OrderStatus(int id, string name) : base(id, name) { }

        public static IEnumerable<OrderStatus> ToEnumerable() {
            return new[] {
                Submitted,
                AwaitingValidation,
                StockConfirmed,
                Paid,
                Shipped,
                Cancelled
            };
        }

        public static OrderStatus FromName(string name) {
            OrderStatus orderStatus = ToEnumerable().SingleOrDefault(x => String.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            GuardAgainstNullOrderStatus(orderStatus);
            return orderStatus;
        }

        public static OrderStatus FromID(int id) {
            OrderStatus orderStatus = ToEnumerable().SingleOrDefault(x => x.ID == id);
            orderStatus = GuardAgainstNullOrderStatus(orderStatus);
            return orderStatus;
        }

        private static OrderStatus GuardAgainstNullOrderStatus(OrderStatus orderStatus) {
            if (orderStatus == null) {
                throw new OrderingDomainException($"Possible values for {nameof(OrderStatus)}: {String.Join(",", ToEnumerable().Select(x => x.Name))}");
            }

            return orderStatus;
        }
    }
}