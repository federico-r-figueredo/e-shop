using System;
using eShop.Services.Ordering.Domain.Exceptions;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.BuyerAggregate {
    public class PaymentMethod : Entity {
        private string alias;
        private string cardNumber;
        private string securityNumber;
        private string cardHolderName;
        private DateTime expirationDate;
        private int cardTypeID;
        private CardType cardType;

        protected PaymentMethod() { }

        public PaymentMethod(string alias, string cardNumber, string securityNumber,
            string cardHolderName, DateTime expirationDate, int cardTypeID) {
            if (string.IsNullOrWhiteSpace(cardNumber)) throw new OrderingDomainException(nameof(cardNumber));
            if (string.IsNullOrWhiteSpace(securityNumber)) throw new OrderingDomainException(nameof(securityNumber));
            if (string.IsNullOrWhiteSpace(cardHolderName)) throw new OrderingDomainException(nameof(cardHolderName));
            if (expirationDate < DateTime.UtcNow) throw new OrderingDomainException(nameof(expirationDate));

            this.alias = alias;
            this.cardNumber = cardNumber;
            this.securityNumber = securityNumber;
            this.cardHolderName = cardHolderName;
            this.expirationDate = expirationDate;
            this.cardTypeID = cardTypeID;
        }

        public CardType CardType {
            get { return this.cardType; }
        }

        public bool IsEqualTo(int cardTypeID, string cardNumber, DateTime expirationDate) {
            return this.cardTypeID == cardTypeID
                && this.cardNumber == cardNumber
                && this.expirationDate == expirationDate;
        }
    }
}