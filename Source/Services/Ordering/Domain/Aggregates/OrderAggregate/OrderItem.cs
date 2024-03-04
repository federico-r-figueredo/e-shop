
using System;
using Dawn;
using EShop.Services.Ordering.Domain.Exceptions;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.OrderAggregate {
    public class OrderItem : Entity {
        private readonly int productID;
        private readonly string productName;
        private readonly string pictureURL;
        private readonly decimal unitPrice;
        private decimal discount;
        private int units;

        protected OrderItem() { }

        public OrderItem(int productID, string productName, decimal unitPrice, decimal discount,
            string pictureURL, int units = 1) {
            GuardAgainstTotalOrderItemCostLesserThanAppliedDiscount(unitPrice, units, discount);

            this.productID = productID;
            this.productName = productName;
            this.pictureURL = pictureURL;
            this.unitPrice = unitPrice;
            this.Discount = discount;
            this.Units = units;
        }

        private static void GuardAgainstTotalOrderItemCostLesserThanAppliedDiscount(decimal unitPrice, int units, decimal discount) {
            if ((unitPrice * units) < discount) {
                throw new OrderingDomainException($"The total {nameof(OrderItem)} cost is lower thant the applied discount");
            }
        }

        public int ProductID {
            get { return this.productID; }
        }

        public string ProductName {
            get { return this.productName; }
        }

        public string PictureURL {
            get { return this.pictureURL; }
        }

        public decimal UnitPrice {
            get { return this.unitPrice; }
        }

        #region Discount

        public decimal Discount {
            get { return this.discount; }
            private set {
                this.discount = Guard
                    .Argument(value, nameof(this.discount))
                    .GreaterThan(-1)
                    .Value;
            }
        }

        public void SetNewDiscount(decimal newDiscount) {
            this.discount = Guard
                .Argument(newDiscount, nameof(newDiscount))
                .GreaterThan(-1)
                .Value;
        }

        #endregion

        #region Units

        public int Units {
            get { return units; }
            private set {
                this.units = Guard
                    .Argument(value, nameof(this.units))
                    .GreaterThan(0)
                    .Value;
            }
        }

        public void AddUnits(int units) {
            this.units += Guard
                .Argument(units, nameof(units))
                .GreaterThan(0)
                .Value;
        }

        public void RemoveUnits(int units) {
            this.units -= Guard
                .Argument(this.units - units, nameof(units))
                .GreaterThan(0)
                .Value;
        }

        #endregion
    }
}