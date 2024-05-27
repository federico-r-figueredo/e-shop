
using System;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using MediatR;

namespace EShop.Services.Ordering.Domain.Events {
    /// <summary>
    /// Event used when an order is created
    /// </summary>
    public class OrderStartedDomainEvent : INotification {
        private readonly Order order;
        private readonly string userID;
        private readonly string userName;
        private readonly int cardTypeID;
        private readonly string paymentCardNumber;
        private readonly string cardVerificationCode;
        private readonly string cardHolderName;
        private readonly DateOnly cardExpiration;

        public OrderStartedDomainEvent(Order order, string userID, string userName,
            int cardTypeID, string cardNumber, string cardVerificatinCode,
            string cardHolderName, DateOnly cardExpiration) {
            this.order = order;
            this.userID = userID;
            this.userName = userName;
            this.cardTypeID = cardTypeID;
            this.paymentCardNumber = cardNumber;
            this.cardVerificationCode = cardVerificatinCode;
            this.cardHolderName = cardHolderName;
            this.cardExpiration = cardExpiration;
        }

        public string UserID {
            get { return this.userID; }
        }

        public string UserName {
            get { return this.userName; }
        }

        public int CardTypeID {
            get { return this.cardTypeID; }
        }

        public string CardNumber {
            get { return this.paymentCardNumber; }
        }

        public string CardSecurityNumber {
            get { return this.cardVerificationCode; }
        }

        public string CardHolderName {
            get { return this.cardHolderName; }
        }

        public DateOnly CardExpiration {
            get { return this.cardExpiration; }
        }

        public Order Order {
            get { return this.order; }
        }
    }
}