
using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;
using EShop.Services.Ordering.Domain.Events;
using EShop.Services.Ordering.Domain.Exceptions;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.OrderAggregate {
    public class Order : Entity, IAggregateRoot {
        // DDD Patterns Comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (instead of properties and property collections)
        private DateTime orderDate;
        private string description;
        private int? buyerID;
        private Address address;
        private int? paymentMethodID;
        private int orderStatusID;
        private OrderStatus orderStatus;

        // DDD Patterns Commnet
        // Using a private collection field (better for DDD Aggregate's encapsulation),
        // so OrderItems can't be added from "outside the AggregateRoot" directly to
        // the collection, but only through the method OrderAggregateRoot.AddOrderItem() which includes
        // business behaviour.
        private readonly List<OrderItem> orderItems;

        // Draft orders have this set to true. Currently we don't check anywhere the draft status of an Order,
        // but we could do it if needed.
        private bool isDraft;

        public static Order NewDraft() {
            Order order = new Order {
                isDraft = true
            };
            return order;
        }

        public Order() {
            this.orderItems = new List<OrderItem>();
            this.isDraft = false;
        }

        public Order(string userID, string userName, Address address,
            int cardTypeID, string cardNumber, string cardSecurityCode,
            string cardHolderName, DateOnly cardExpiration,
            int? buyerID = null, int? paymentMethodID = null) : this() {

            this.buyerID = buyerID;
            this.paymentMethodID = paymentMethodID;
            this.OrderStatus = OrderStatus.Submitted;
            this.orderDate = DateTime.UtcNow;
            this.address = address;

            // Add the OrderStartedDomainEvent to the domain events collection
            // to be raised / dispatched when commiting changes into DB 
            // (after DbContext.SaveChanges())
            AddOrderStartedDomainEvent(userID, userName, cardTypeID, cardNumber, cardSecurityCode, cardHolderName, cardExpiration);
        }

        private void AddOrderStartedDomainEvent(string userID, string userName, int cardTypeID,
            string cardNumber, string cardSecurityNumber, string cardHolderName, DateOnly cardExpiration) {
            OrderStartedDomainEvent domainEvent = new OrderStartedDomainEvent(this, userID,
                userName, cardTypeID, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration);

            base.AddDomainEvent(domainEvent);
        }

        private string Description {
            set {
                this.description = Guard
                    .Argument(value, nameof(value))
                    .NotNull()
                    .NotEmpty()
                    .NotWhiteSpace()
                    .Value;
            }
        }

        public int? BuyerID {
            get { return this.buyerID; }
            set { this.buyerID = value; }
        }

        public Address Address {
            get { return this.address; }
            private set { this.address = value; }
        }

        public int? PaymentMethodID {
            get { return this.paymentMethodID; }
            set { this.paymentMethodID = value; }
        }

        #region  Order Status

        public OrderStatus OrderStatus {
            get { return this.orderStatus; }
            private set {
                this.orderStatusID = value.ID;
                this.orderStatus = value;
            }
        }

        public void SetAwaitingValidationStatus() {
            if (this.orderStatusID != OrderStatus.Submitted.ID) {
                throw new OrderingDomainException($"Transition towards " +
                    $"{OrderStatus.AwaitingValidation.Name} is only allowed " +
                    $"if current status isn't {OrderStatus.Submitted.Name}");
            }

            base.AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(this.ID, this.OrderItems));

            this.OrderStatus = OrderStatus.AwaitingValidation;
        }

        public void SetStockConfirmedStatus() {
            if (this.orderStatusID != OrderStatus.AwaitingValidation.ID) {
                throw new OrderingDomainException($"Transition towards " +
                    $"{OrderStatus.StockConfirmed.Name} is only allowed " +
                    $"if current status isn't {OrderStatus.AwaitingValidation.Name}"
                );
            }

            base.AddDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(this.ID, this.orderItems));

            this.OrderStatus = OrderStatus.StockConfirmed;
        }

        public void SetPaidStatus() {
            if (this.orderStatusID != OrderStatus.StockConfirmed.ID) {
                throw new OrderingDomainException($"Transition towards " +
                    $"{OrderStatus.Paid.Name} is not allowed " +
                    $"if current status isn't {OrderStatus.StockConfirmed.Name}"
                );
            }

            base.AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(this.ID, this.orderItems));

            this.OrderStatus = OrderStatus.Paid;
        }

        public void SetShippedStatus() {
            if (this.orderStatusID != OrderStatus.Paid.ID) {
                throw new OrderingDomainException($"Transition towards " +
                    $"{OrderStatus.Shipped.Name} is not allowed " +
                    $"if current status is not {OrderStatus.Paid.Name}"
                );
            }

            this.OrderStatus = OrderStatus.Shipped;
            this.Description = "The order was shipped.";
            base.AddDomainEvent(new OrderShippedDomainEvent(this));
        }

        public void SetCancelledStatus() {
            if (this.orderStatusID == OrderStatus.Paid.ID ||
                this.orderStatusID == OrderStatus.Shipped.ID) {
                throw new OrderingDomainException($"Transition towards " +
                    $"{OrderStatus.Cancelled.Name} is not allowed" +
                    $"if current status is {OrderStatus.Paid.Name} or {OrderStatus.Shipped.Name}"
                );
            }

            this.OrderStatus = OrderStatus.Cancelled;
            this.Description = "The order was cancelled.";
            base.AddDomainEvent(new OrderCancelledDomainEvent(this));
        }

        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems) {
            if (this.orderStatusID != OrderStatus.AwaitingValidation.ID) {
                throw new OrderingDomainException($"Transition towards " +
                    $"{OrderStatus.Cancelled.Name} due to 'Stock is rejected'" +
                    $"is not allowed if current status is not {OrderStatus.AwaitingValidation.Name}"
                );
            }

            this.OrderStatus = OrderStatus.Cancelled;
            IEnumerable<string> itemsStockRejectedItemsProductNames = this.orderItems
                .Where(x => orderStockRejectedItems.Contains(x.ProductID))
                .Select(x => x.ProductName);

            string itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedItemsProductNames);
            this.Description = $"The product items doesn't 've stock : {itemsStockRejectedDescription}";
        }

        #endregion

        #region Order Items

        public IReadOnlyCollection<OrderItem> OrderItems {
            get { return this.orderItems.AsReadOnly(); }
        }

        // DDD Patterns Comment
        // This Order's AggregateRoot's method "AddOrUpdateOrderItem()" should be the only way to add
        // OrderItem to the Order. Therefore, any behaviour (discounts, etc.) and validations
        // are controlled by the AggregateRoot in order to maintain consistency between the
        // the whole Aggregate.
        public void AddOrUpdateOrderItem(int productID, string productName, decimal unitPrice,
            decimal discount, string picureURL, int units = 1) {
            OrderItem existingOrderForProduct = this.orderItems
                .Where(x => x.ProductID == productID)
                .SingleOrDefault();

            if (existingOrderForProduct == null) {
                AddNewOrderItem(productID, productName, unitPrice, discount, picureURL, units);
                return;
            }

            UpdateExistingOrderItem(existingOrderForProduct, discount, units);
        }

        private void AddNewOrderItem(int productID, string productName, decimal unitPrice, decimal discount, string picureURL, int units) {
            OrderItem newOrderItem = new OrderItem(productID, productName, unitPrice, discount, picureURL, units);
            this.orderItems.Add(newOrderItem);
            return;
        }

        private static void UpdateExistingOrderItem(OrderItem existingOrderItem, decimal newDiscount, int units) {
            if (NewDiscountIsGreaterThanCurrentOne(newDiscount, existingOrderItem.Discount)) {
                existingOrderItem.SetNewDiscount(newDiscount);
            }

            existingOrderItem.AddUnits(units);
        }

        private static bool NewDiscountIsGreaterThanCurrentOne(decimal newDiscount, decimal currentDiscount) {
            return newDiscount > currentDiscount;
        }

        public decimal Total {
            get {
                return this.orderItems.Sum(x => x.Units * x.UnitPrice);
            }
        }

        #endregion
    }
}