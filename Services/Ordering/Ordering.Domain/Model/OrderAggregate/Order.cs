using System;
using System.Collections.Generic;
using System.Linq;
using eShop.Services.Ordering.Domain.Events;
using eShop.Services.Ordering.Domain.Exceptions;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.OrderAggregate {
    public class Order : Entity, IAggregateRoot {
        // DDD patterns comment:
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (instead of properties and
        // properties collections).
        private DateTime orderDate;

        // Address is a Value Object pattern example persisted as EF Core 2.0 owned entity.
        private Address address;

        // DDD Patterns comment:
        // In this case we opt to not adding a navigation property to entities that don't
        // belong to the same aggregate. Instead, we refer to them just by their ID.
        private int? buyerID;
        private int? paymentMethodID;

        private int orderStatusID;
        private OrderStatus orderStatus;
        private string description;

        // Draft orders have this set to true. Currently we don't check anywhere the draft
        // status of an Order, but we could do it if needed.
        private bool isDraft;

        // DDD Patterns comment:
        // Using a private collection field, better for DDD Aggregate's encapsulation, so
        // OrderItems cannot be added from "outside the AggregateRoot" directly to the
        // collection, but only through the method OrderAggregateRoot.AddOrderItem() which
        // includes behaviour.
        private readonly List<OrderItem> orderItems;

        protected Order() {
            this.orderItems = new List<OrderItem>();
            this.isDraft = false;
        }

        public Order(string userID, string userName, Address address, int cardTypeID,
            string cardNumber, string cardSecurityNumber, string cardHolderName,
            DateTime cardExpirationDate, int? buyerID = null, int? paymentMethodID = null) : this() {

            this.address = address;
            this.buyerID = buyerID;
            this.paymentMethodID = paymentMethodID;

            this.orderStatusID = OrderStatus.Submitted.ID;
            this.orderDate = DateTime.UtcNow;

            // DDD Patterns comment:
            // Add the OrderStartedDomainEvent to the domain events collection to be
            // raised / dispatched when committing changes into the database (after 
            // DbContext.SaveChanges()).
            base.AddDomainEvent(new OrderStartedDomainEvent(
                userID: userID,
                userName: userName,
                cardTypeID: cardTypeID,
                cardNumber: cardNumber,
                cardSecurityNumber: cardSecurityNumber,
                cardHolderName: cardHolderName,
                cardExpirationDate: cardExpirationDate,
                order: this
            ));
        }

        public static Order NewDraft() {
            return new Order {
                isDraft = true
            };
        }

        public Address Address {
            get { return this.address; }
        }

        public int? BuyerID {
            get { return this.buyerID; }
        }

        public OrderStatus OrderStatus {
            get { return OrderStatus.FromID(this.orderStatusID); }
        }

        public IReadOnlyCollection<OrderItem> OrderItems {
            get { return this.orderItems.AsReadOnly(); }
        }

        // DDD Patterns comment:
        // This Order AggregateRoot's method "AddOrderItem()" should be the only way to
        // add OrderItems to the Order, so any behaviour (e.g. discounts) and validations
        // are controlled by the AggregateRoot in order to maintain consitency across the
        // whole Aggregate.
        public void AddOrderItem(int productID, string productName, decimal unitPrice,
            decimal newDiscount, string pictureURL, int units = 1) {

            OrderItem existingOrderItem = this.orderItems
                .Where(x => x.ProductID == productID)
                .SingleOrDefault();

            if (existingOrderItem != null) {
                // If an existing OrderItem exists for the product, then update it with the
                // higher discount and units.
                if (newDiscount > existingOrderItem.CurrentDiscount) {
                    existingOrderItem.SetNewDiscount(newDiscount);
                }

                existingOrderItem.AddUnits(units);
            } else {
                // Add validated new order item
                OrderItem newOrderItem = new OrderItem(
                    productID: productID,
                    productName: productName,
                    unitPrice: unitPrice,
                    discount: newDiscount,
                    pictureURL: pictureURL,
                    units: units
                );
                this.orderItems.Add(newOrderItem);
            }
        }

        public void SetPaymentMethodID(int id) {
            this.paymentMethodID = id;
        }

        public void SetBuyerID(int id) {
            this.buyerID = id;
        }

        public void SetAwaitingValidationStatus() {
            if (this.orderStatusID != OrderStatus.Submitted.ID) {
                throw new OrderingDomainException(
                    "An order can be driven to the 'SetAwaitingValidation' status only " +
                    "if its previous state was 'Submitted'."
                );
            }

            base.AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(
                orderID: this.id,
                orderItems: this.orderItems
            ));

            this.orderStatusID = OrderStatus.AwaitingValidation.ID;
        }

        public void SetStockConfirmedStatus() {
            if (this.orderStatusID != OrderStatus.AwaitingValidation.ID) {
                throw new OrderingDomainException(
                    "An order can be driven to the 'StockConfirmed' status only " +
                    "if its previous state was 'AwaitingValidation'."
                );
            }

            base.AddDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(
                orderID: this.id
            ));

            this.orderStatusID = OrderStatus.StockConfirmed.ID;
            this.description = "All the items were confirmed with available stock";
        }

        public void SetPaidStatus() {
            if (this.orderStatusID != OrderStatus.StockConfirmed.ID) {
                throw new OrderingDomainException(
                    "An order can be driven to the 'Paid' status only " +
                    "if its previous state was 'StockConfirmed'."
                );
            }

            this.orderStatusID = OrderStatus.Paid.ID;
            this.description = "The payment was performed at a simulated 'American Bank " +
                "checking bank account ending on XX35071'";

            base.AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(
                orderID: this.id,
                orderItems: this.orderItems
            ));
        }

        public void SetShippedStatus() {
            if (this.orderStatusID != OrderStatus.Paid.ID) {
                throw new OrderingDomainException(
                    "An order can be driven to the 'Paid' status only " +
                    "if its previous state was 'StockConfirmed'."
                );
            }

            this.orderStatusID = OrderStatus.Shipped.ID;
            this.description = "The order has been shipped";

            base.AddDomainEvent(new OrderShippedDomainEvent(
                order: this
            ));
        }

        public void SetCancelledStatus() {
            if (this.orderStatusID == OrderStatus.Paid.ID ||
                this.orderStatusID == OrderStatus.Shipped.ID) {
                throw new OrderingDomainException(
                    "An order that is in either 'Paid' or 'Shipped' state can't be cancelled"
                );
            }

            this.orderStatusID = OrderStatus.Cancelled.ID;
            this.description = "The order has been cancelled";

            base.AddDomainEvent(new OrderCancelledDomainEvent(
                order: this
            ));
        }

        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems) {
            if (this.orderStatusID != OrderStatus.AwaitingValidation.ID) {
                throw new OrderingDomainException(
                    "An order that isn't in 'AwaitingValidation' status can't " +
                    "be cancelled due to rejected stock"
                );
            }

            this.orderStatusID = OrderStatus.Cancelled.ID;

            IEnumerable<string> itemsStockRejectedProductNames = OrderItems
                .Where(x => orderStockRejectedItems.Contains(x.ProductID))
                .Select(x => x.ProductName);

            string itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
            this.description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";

            base.AddDomainEvent(new OrderCancelledDueToStockRejectedDomainEvent(
                order: this,
                itemStockRejectedProductNames: itemsStockRejectedProductNames
            ));
        }

        public decimal GetTotal() {
            return this.orderItems.Sum(x => x.Units * x.UnitPrice);
        }
    }
}