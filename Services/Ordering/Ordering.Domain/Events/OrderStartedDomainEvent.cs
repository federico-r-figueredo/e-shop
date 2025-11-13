using MediatR;
using System;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;

namespace eShop.Services.Ordering.Domain.Events {
    public class OrderStartedDomainEvent : INotification {
        private readonly string userID;
        private readonly string userName;
        private readonly int cardTypeID;
        private readonly string cardNumber;
        private readonly string cardSecurityNumber;
        private readonly string cardHolderName;
        private readonly DateTime cardExpiration;
        private readonly Order order;

        public OrderStartedDomainEvent(string userID, string userName, int cardTypeID,
            string cardNumber, string cardSecurityNumber, string cardHolderName,
            DateTime cardExpirationDate, Order order) {
            this.userID = userID;
            this.userName = userName;
            this.cardTypeID = cardTypeID;
            this.cardNumber = cardNumber;
            this.cardSecurityNumber = cardSecurityNumber;
            this.cardHolderName = cardHolderName;
            this.cardExpiration = cardExpirationDate;
            this.order = order;
        }

        public string UserID { get { return userID; } }
        public string UserName { get { return userName; } }
        public int CardTypeID { get { return cardTypeID; } }
        public string CardNumber { get { return cardNumber; } }
        public string CardSecurityNumber { get { return cardSecurityNumber; } }
        public string CardHolderName { get { return cardHolderName; } }
        public DateTime CardExpiration { get { return cardExpiration; } }
        public Order Order { get { return order; } }
    }
}