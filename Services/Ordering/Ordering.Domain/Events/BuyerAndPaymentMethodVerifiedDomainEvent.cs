using MediatR;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;

namespace eShop.Services.Ordering.Domain.Events {
    public class BuyerAndPaymentMethodVerifiedDomainEvent : INotification {
        private readonly Buyer buyer;
        private readonly PaymentMethod paymentMethod;
        private readonly int orderID;

        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer,
            PaymentMethod paymentMethod, int orderID) {
            this.buyer = buyer;
            this.paymentMethod = paymentMethod;
            this.orderID = orderID;
        }

        public Buyer Buyer { get { return buyer; } }
        public PaymentMethod PaymentMethod { get { return paymentMethod; } }
        public int OrderID { get { return orderID; } }
    }
}