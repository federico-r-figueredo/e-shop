
using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;
using EShop.Services.Ordering.Domain.Events;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate {
    internal class Buyer : Entity, IAggregateRoot {
        private string identityGUID;
        private string name;
        private readonly List<PaymentMethod> paymentMethods;

        protected Buyer() {
            this.paymentMethods = new List<PaymentMethod>();
        }

        public Buyer(string identityGUID, string name) : this() {
            this.IdentityGUID = identityGUID;
            this.Name = name;
        }

        public string IdentityGUID {
            get { return identityGUID; }
            private set {
                identityGUID = Guard
                    .Argument(value, nameof(this.IdentityGUID))
                    .NotNull()
                    .NotWhiteSpace()
                    .Value;
            }
        }

        public string Name {
            get { return name; }
            private set {
                name = Guard
                    .Argument(value, nameof(this.name))
                    .NotNull()
                    .NotWhiteSpace()
                    .Value;
            }
        }

        public IEnumerable<PaymentMethod> PaymentMethods {
            get { return this.paymentMethods.AsReadOnly(); }
        }

        public PaymentMethod VerifyOrAddPaymentMethod(int cardTypeID, string alias,
            string paymentCardNumber, string securityNumber, string cardHolderNumber,
            DateOnly expirationDate, int orderID) {

            PaymentMethod existingPaymentMethod = this.paymentMethods
                .SingleOrDefault(x => x.IsEqualTo(cardTypeID, paymentCardNumber, expirationDate));

            if (existingPaymentMethod != null) {
                AddBuyerPaymentMethodVerifiedDomainEvent(existingPaymentMethod, orderID);
                return existingPaymentMethod;
            }

            PaymentMethod newPaymentMethod = new PaymentMethod(cardTypeID, alias,
                paymentCardNumber, securityNumber, cardHolderNumber, expirationDate);
            this.paymentMethods.Add(newPaymentMethod);

            AddBuyerPaymentMethodVerifiedDomainEvent(newPaymentMethod, orderID);

            return newPaymentMethod;
        }

        private void AddBuyerPaymentMethodVerifiedDomainEvent(PaymentMethod newPaymentMethod,
            int orderID) {

            base.AddDomainEvent(new BuyerPaymentMethodVerifiedDomainEvent(this,
                newPaymentMethod, orderID));
        }
    }
}