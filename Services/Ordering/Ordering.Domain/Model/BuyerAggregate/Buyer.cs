using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eShop.Services.Ordering.Domain.Events;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.BuyerAggregate {
    public class Buyer : Entity, IAggregateRoot {
        private string identityGUID;
        private string name;
        private List<PaymentMethod> paymentMethods;

        protected Buyer() {
            this.paymentMethods = new List<PaymentMethod>();
        }

        public Buyer(string identityGUID, string name) : this() {
            if (string.IsNullOrWhiteSpace(identityGUID)) throw new ArgumentNullException(nameof(identityGUID));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            this.identityGUID = identityGUID;
            this.name = name;
        }

        public string IdentityGUID {
            get { return this.identityGUID; }
        }

        public string Name {
            get { return this.name; }
        }

        public ReadOnlyCollection<PaymentMethod> PaymentMethods {
            get { return this.paymentMethods.AsReadOnly(); }
        }

        public PaymentMethod VerifyOrAddPaymentMethod(int cardTypeID, string alias,
            string cardNumber, string securityNumber, string cardHolderName,
            DateTime expirationDate, int orderID) {
            PaymentMethod existingPaymentMethod = this.paymentMethods.SingleOrDefault(
                x => x.IsEqualTo(cardTypeID, cardNumber, expirationDate)
            );

            if (existingPaymentMethod != null) {
                base.AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(
                    this,
                    existingPaymentMethod,
                    orderID
                ));
                return existingPaymentMethod;
            }

            PaymentMethod newPaymentMethod = new PaymentMethod(
                alias: alias,
                cardNumber: cardNumber,
                securityNumber: securityNumber,
                cardHolderName: cardHolderName,
                expirationDate: expirationDate,
                cardTypeID: cardTypeID
            );

            this.paymentMethods.Add(newPaymentMethod);

            base.AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(
                this,
                newPaymentMethod,
                orderID
            ));

            return newPaymentMethod;
        }
    }
}