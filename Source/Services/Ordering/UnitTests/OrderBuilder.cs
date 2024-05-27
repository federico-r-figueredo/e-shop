
using System;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;

namespace EShop.Services.Ordering.UnitTests {
    internal class OrderBuilder {
        private readonly Order order;

        internal OrderBuilder(Address address) {
            this.order = new Order(
                "userID",
                "fakeName",
                address,
                cardTypeID: 5,
                cardNumber: "12",
                cardSecurityCode: "123",
                cardHolderName: "name",
                cardExpiration: DateOnly.FromDateTime(DateTime.UtcNow)
            );
        }

        internal OrderBuilder AddOne(
            int productID,
            string productName,
            decimal unitPrice,
            decimal discount,
            string pictureURL,
            int units = 1
        ) {
            this.order.AddOrUpdateOrderItem(productID, productName, unitPrice, discount, pictureURL, units);
            return this;
        }

        internal Order Build() {
            return this.order;
        }
    }
}