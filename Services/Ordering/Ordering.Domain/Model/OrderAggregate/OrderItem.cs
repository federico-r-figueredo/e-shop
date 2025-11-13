using eShop.Services.Ordering.Domain.Exceptions;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.OrderAggregate {
    public class OrderItem : Entity {
        // DDD patterns comment:
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (insteaf of properties and
        // property collections).
        private int productID;
        private string productName;
        private string pictureURL;
        private decimal unitPrice;
        private decimal discount;
        private int units;

        public OrderItem(int productID, string productName, string pictureURL,
            decimal unitPrice, decimal discount, int units = 1) {

            if (units <= 0) {
                throw new OrderingDomainException($"Invalid number of units: {units}");
            }

            if ((unitPrice * units) < discount) {
                throw new OrderingDomainException(
                    "The total of order item is lower than applied discount"
                );
            }

            this.productID = productID;
            this.productName = productName;
            this.pictureURL = pictureURL;
            this.unitPrice = unitPrice;
            this.discount = discount;
            this.units = units;
        }

        public int ProductID {
            get { return this.productID; }
        }

        public int PictureID {
            get { return this.productID; }
        }

        public string ProductName {
            get { return this.productName; }
        }

        public string PictureURL {
            get { return this.pictureURL; }
        }

        public decimal CurrentDiscount {
            get { return this.discount; }
        }

        public decimal UnitPrice {
            get { return this.unitPrice; }
        }

        public int Units {
            get { return this.units; }
        }

        public void SetNewDiscount(decimal discount) {
            if (discount < 0) {
                throw new OrderingDomainException(
                    $"Invalid discount: {discount}. It should be greater or equal than zero."
                );
            }

            this.discount = discount;
        }

        public void AddUnits(int units) {
            if (units < 0) {
                throw new OrderingDomainException(
                    $"Invalid units: {units}. It should be greater or equal than zero."
                );
            }

            this.units += units;
        }
    }
}