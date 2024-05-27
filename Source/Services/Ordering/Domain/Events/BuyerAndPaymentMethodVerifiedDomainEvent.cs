
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using MediatR;

namespace EShop.Services.Ordering.Domain.Events {
    public class BuyerAndPaymentMethodVerifiedDomainEvent : INotification {
        private readonly Buyer buyer;
        private readonly PaymentMethod paymentMethod;
        private readonly int orderID;

        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod paymentMethod, int orderID) {
            this.buyer = buyer;
            this.paymentMethod = paymentMethod;
            this.orderID = orderID;
        }

        public Buyer Buyer {
            get { return this.buyer; }
        }

        public PaymentMethod PaymentMethod {
            get { return this.paymentMethod; }
        }

        public int OrderID {
            get { return this.orderID; }
        }
    }
}