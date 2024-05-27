using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using EShop.Services.Ordering.Domain.Events;
using EShop.Services.Ordering.Domain.Exceptions;
using NUnit.Framework;

namespace EShop.Services.Ordering.UnitTests.Domain {
    internal class OrderAggregateTest {
        #region OrderItem

        [Test]
        public void CreateOrderItem_WithValidArguments_ShouldReturnNotNullResult() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            // Act
            OrderItem orderItem = new OrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Assert
            Assert.NotNull(orderItem);
        }

        [Test]
        public void CreateOrderItem_WithInvalidNumberOfUnits_ShouldThrowArgumentOutOfRangeException() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(productID, productName, unitPrice, discount, pictureURL, units));
        }

        [Test]
        public void CreateOrderItem_WithTotalOfOrderItemsLowerThanDiscountApplied_ShouldThrowOrderingDomainException() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 1;

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => new OrderItem(productID, productName, unitPrice, discount, pictureURL, units));
        }

        [Test]
        public void CreateOrderItem_WithInvalidDiscountSetting_ShouldThrowArgumentOutOfRangeException() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            // Act
            OrderItem orderItem = new OrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.SetNewDiscount(-1));
        }

        [Test]
        public void CreateOrderItem_WithInvalidAddUnitsArgument_ShouldThrowArgumentOutOfRangeException() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            // Act
            OrderItem orderItem = new OrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.AddUnits(0));
        }

        [Test]
        public void CreateOrderItem_WithInvalidRemoveUnitsArgument_ShouldThrowArgumentOutOfRangeException() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            // Act
            OrderItem orderItem = new OrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => orderItem.RemoveUnits(5));
        }

        #endregion

        #region CreateOrder

        [Test]
        public void CreateOrder_WhenAddTwoTimesOnTheSameItem_TotalOfOrderShouldBeTheSumOfTheTwoItems() {
            // Arrange & Act
            Address address = new AddressBuilder().Build();
            Order order = new OrderBuilder(address)
                .AddOne(1, "cup", 10.0m, 0, string.Empty)
                .AddOne(1, "cup", 10.0m, 0, string.Empty)
                .Build();

            // Assert
            Assert.AreEqual(20.0m, order.Total);
        }

        [Test]
        public void NewDraft_WithNoArguments_ShouldReturnNotNullObject() {
            // Act
            Order order = Order.NewDraft();

            // Assert
            Assert.IsNotNull(order);
        }

        [Test]
        public void CreateOrder_WithParameterlessContstructor_ShouldReturnNotNullObject() {
            // Act
            Order order = new Order();

            // Assert
            Assert.IsNotNull(order);
        }

        [Test]
        public void CreateOrder_WhenCreatingNewInstance_ShouldRaiseNewOrderStartedDomainEvent() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int expectedDomainEventsCount = 1;

            // Act
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Assert
            Assert.AreEqual(expectedDomainEventsCount, order.DomainEvents.Count);
        }

        [Test]
        public void CreateOrder_WhenEventOrderAddedExplicitly_ShouldRaiseNewOrderStartedDomainEvent() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int expectedDomainEventsCount = 2;

            // Act
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );
            order.AddDomainEvent(new OrderStartedDomainEvent(order, "fakeName", "1", cardTypeID, cardNumber, cardSecurityCode, cardHolderName, cardExpiration));

            // Assert
            Assert.AreEqual(expectedDomainEventsCount, order.DomainEvents.Count);
        }

        [Test]
        public void CreateOrder_WhenEventOrderRemovedExplicitly_ShouldRaiseOrderStartedDomainEventOnce() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int expectedDomainEventsCount = 1;
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );
            OrderStartedDomainEvent orderStartedDomainEvent = new OrderStartedDomainEvent(
                order,
                "fakeName",
                "1",
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.AddDomainEvent(orderStartedDomainEvent);
            order.RemoveDomainEvent(orderStartedDomainEvent);

            // Assert
            Assert.AreEqual(expectedDomainEventsCount, order.DomainEvents.Count);
        }

        [Test]
        public void CreateOrder_WhenCreatingNewInstance_ShouldHaveSubmittedOrderStatus() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            // Act
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Assert
            Assert.AreEqual(OrderStatus.Submitted, order.OrderStatus);
        }

        #endregion

        #region SetAwaitingValidationStatus

        [Test]
        public void SetAwaitingValidationStatus_WithSubmittedOrderStatus_ShouldRaiseDomainEventAndUpdateStatusAndDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();

            // Assert
            Assert.AreEqual(OrderStatus.AwaitingValidation, order.OrderStatus);
            Assert.AreEqual(2, order.DomainEvents.Count);
            Assert.IsInstanceOf<OrderStatusChangedToAwaitingValidationDomainEvent>(order.DomainEvents.Last());
        }

        [Test]
        public void SetAwaitingValidationStatus_WithAwaitingValidationOrderStatus_ShouldRaiseDomainEventAndUpdateStatusAndDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetAwaitingValidationStatus());
        }

        [Test]
        public void SetAwaitingValidationStatus_WithStockConfirmedOrderStatus_ShouldRaiseDomainEventAndUpdateStatusAndDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetAwaitingValidationStatus());
        }

        [Test]
        public void SetAwaitingValidationStatus_WithPaidOrderStatus_ShouldRaiseDomainEventAndUpdateStatusAndDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetAwaitingValidationStatus());
        }

        [Test]
        public void SetAwaitingValidationStatus_WithShippedOrderStatus_ShouldRaiseDomainEventAndUpdateStatusAndDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();
            order.SetShippedStatus();

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetAwaitingValidationStatus());
        }

        [Test]
        public void SetAwaitingValidationStatus_WithCancelledOrderStatus_ShouldRaiseDomainEventAndUpdateStatusAndDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetCancelledStatus();

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetAwaitingValidationStatus());
        }

        #endregion

        #region SetStockConfirmedStatus

        [Test]
        public void SetStockConfirmedStatus_WithAwaitingValidationOrderStatus_ShouldRaiseDomainEventAndUpdateDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();

            // Assert
            Assert.AreEqual(3, order.DomainEvents.Count);
            Assert.IsInstanceOf<OrderStatusChangedToStockConfirmedDomainEvent>(order.DomainEvents.Last());
        }

        [Test]
        public void SetStockConfirmedStatus_WithSubmittedOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetStockConfirmedStatus());
        }

        [Test]
        public void SetStockConfirmedStatus_WithStockConfirmedOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetStockConfirmedStatus());
        }

        [Test]
        public void SetStockConfirmedStatus_WithPaidOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetStockConfirmedStatus());
        }

        [Test]
        public void SetStockConfirmedStatus_WithShippedOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();
            order.SetShippedStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetStockConfirmedStatus());
        }

        [Test]
        public void SetStockConfirmedStatus_WithCancelledOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetCancelledStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetStockConfirmedStatus());
        }

        #endregion

        #region SetPaidStatus

        [Test]
        public void SetPaidStatus_WithStockConfirmedOrderStatus_ShouldRaiseDomainEventAndUpdateDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();

            // Assert
            Assert.AreEqual(4, order.DomainEvents.Count);
            Assert.IsInstanceOf<OrderStatusChangedToPaidDomainEvent>(order.DomainEvents.Last());
        }

        [Test]
        public void SetPaidStatus_WithSubmittedOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetPaidStatus());
        }

        [Test]
        public void SetPaidStatus_WithAwaitingValidationOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetPaidStatus());
        }

        [Test]
        public void SetPaidStatus_WithPaidOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetPaidStatus());
        }

        [Test]
        public void SetPaidStatus_WithShippedOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();
            order.SetShippedStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetPaidStatus());
        }

        [Test]
        public void SetPaidStatus_WithCancelledOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetCancelledStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetPaidStatus());
        }

        #endregion

        #region SetShippedStatus

        [Test]
        public void SetShippedStatus_WithPaidOrderStatus_ShouldRaiseDomainEventAndUpdateDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();
            order.SetShippedStatus();

            // Assert
            Assert.AreEqual(5, order.DomainEvents.Count);
            Assert.IsInstanceOf<OrderShippedDomainEvent>(order.DomainEvents.Last());
        }

        [Test]
        public void SetShippedStatus_WithSubmittedStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetShippedStatus());
        }

        [Test]
        public void SetShippedStatus_WithStockConfirmedStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetShippedStatus());
        }

        [Test]
        public void SetShippedStatus_WithAwaitingValidationStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetShippedStatus());
        }

        [Test]
        public void SetShippedStatus_WithCancelledStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetCancelledStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetShippedStatus());
        }

        [Test]
        public void SetShippedStatus_WithShippedStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();
            order.SetShippedStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetShippedStatus());
        }

        #endregion

        #region SetCancelledStatus

        [Test]
        public void SetCancelledStatus_WithPaidOrderStatus_ShouldRaiseDomainEventAndUpdateDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetCancelledStatus();

            // Assert
            Assert.AreEqual(4, order.DomainEvents.Count);
            Assert.IsInstanceOf<OrderCancelledDomainEvent>(order.DomainEvents.Last());
        }

        [Test]
        public void SetCancelledStatus_WithPaidStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => order.SetCancelledStatus());
        }

        [Test]
        public void SetCancelledStatus_WithAwaitingValidationStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();
            order.SetShippedStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetCancelledStatus());
        }

        #endregion

        #region SetCancelledStatusWhenStockIsRejected

        [Test]
        public void SetCancelledStatusWhenStockIsRejected_WithPaidOrderStatus_ShouldRaiseDomainEventAndUpdateDescription() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetCancelledStatusWhenStockIsRejected(new List<int>() { productID });

            // Assert
            Assert.AreEqual(3, order.DomainEvents.Count);
            Assert.IsInstanceOf<OrderCancelledDomainEvent>(order.DomainEvents.Last());
        }

        [Test]
        public void SetCancelledStatusWhenStockIsRejected_WithStockConfirmedOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetCancelledStatusWhenStockIsRejected(new List<int>() { productID }));
        }

        [Test]
        public void SetCancelledStatusWhenStockIsRejected_WithPaidOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetCancelledStatusWhenStockIsRejected(new List<int>() { productID }));
        }

        [Test]
        public void SetCancelledStatusWhenStockIsRejected_WithShippedOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();
            order.SetShippedStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetCancelledStatusWhenStockIsRejected(new List<int>() { productID }));
        }

        [Test]
        public void SetCancelledStatusWhenStockIsRejected_WithCancelledOrderStatus_ShouldThrowOrderingDomainException() {
            // Arrange
            string street = "fakeStreet";
            string city = "fakeCity";
            string state = "fakeState";
            string country = "fakeCountry";
            string zipCode = "fakeZipCode";
            int cardTypeID = 5;
            string cardNumber = "12";
            string cardSecurityCode = "123";
            string cardHolderName = "fakeName";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            Order order = new Order(
                "1",
                "fakeName",
                new Address(street, city, state, country, zipCode),
                cardTypeID,
                cardNumber,
                cardSecurityCode,
                cardHolderName,
                cardExpiration
            );

            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetCancelledStatus();

            // Assert
            Assert.Throws<OrderingDomainException>(() => order.SetCancelledStatusWhenStockIsRejected(new List<int>() { productID }));
        }

        #endregion

        #region AddOrUpdateOrderItem


        [Test]
        public void AddOrUpdateOrderItem_WhenAddingNewOrderItem_ShouldAddNewOrderItemToCollection() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;
            OrderItem expected = new OrderItem(productID, productName, unitPrice, discount, pictureURL, units);
            Order order = new Order();

            // Act
            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Assert
            Assert.AreEqual(1, order.OrderItems.Count);

            OrderItem actual = order.OrderItems.Last();
            Assert.AreEqual(expected.ProductID, actual.ProductID);
            Assert.AreEqual(expected.ProductName, actual.ProductName);
            Assert.AreEqual(expected.UnitPrice, actual.UnitPrice);
            Assert.AreEqual(expected.Discount, actual.Discount);
            Assert.AreEqual(expected.PictureURL, actual.PictureURL);
            Assert.AreEqual(expected.Units, actual.Units);
        }

        [Test]
        public void AddOrUpdateOrderItem_WhenUpdatingExistingOrderItem_ShouldUpdateExistingOrderItemToCollection() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;

            string productNameBIS = "FakeProductName";
            int unitPriceBIS = 12;
            int discountBIS = 15;
            string pictureURLBIS = "FakePictureURL";
            int unitsBIS = 10;

            OrderItem expected = new OrderItem(productID, productNameBIS, unitPriceBIS, discountBIS, pictureURLBIS, unitsBIS);
            Order order = new Order();

            // Act
            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);
            order.AddOrUpdateOrderItem(productID, productNameBIS, unitPriceBIS, discountBIS, pictureURLBIS, units);

            // Assert
            Assert.AreEqual(1, order.OrderItems.Count);

            OrderItem actual = order.OrderItems.Last();
            Assert.AreEqual(expected.ProductID, actual.ProductID);
            Assert.AreEqual(expected.ProductName, actual.ProductName);
            Assert.AreEqual(expected.UnitPrice, actual.UnitPrice);
            Assert.AreEqual(expected.Discount, actual.Discount);
            Assert.AreEqual(expected.PictureURL, actual.PictureURL);
            Assert.AreEqual(expected.Units, actual.Units);
        }

        #endregion

        #region Total

        [Test]
        public void AddOrUpdateOrderItem_WithMultipleOrderItem_ShouldReturnAggregatedUnitsTimesPrice() {
            // Arrange
            int productID = 1;
            string productName = "FakeProductName";
            int unitPrice = 12;
            int discount = 15;
            string pictureURL = "FakePictureURL";
            int units = 5;
            Order order = new Order();

            // Act
            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);
            order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);

            // Assert
            Assert.AreEqual(10 * 12, order.Total);
        }

        #endregion
    }
}