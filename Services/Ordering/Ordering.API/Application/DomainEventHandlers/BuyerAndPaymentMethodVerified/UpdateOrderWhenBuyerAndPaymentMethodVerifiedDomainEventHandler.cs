using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using eShop.Services.Ordering.Domain.Events;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;

namespace eShop.Services.Ordering.API.Application.DomainEventHandlers.BuyerAndPaymentMethodVerified {
    public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler
        : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent> {
        private readonly IOrderRepository orderRepository;
        private readonly ILogger<UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler> logger;

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
            IOrderRepository orderRepository,
            ILogger<UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler> logger) {
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Domain logic comment:
        // When the Buyer and Buyer's payment method have been created or its existence
        // has been verified, then we can update the original Order with the BuyerID and
        // PaymentID foreign keys.
        public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent domainEvent,
            CancellationToken cancellationToken) {
            Order orderToUpdate = await this.orderRepository.GetByIDAsync(domainEvent.OrderID);
            orderToUpdate.SetBuyerID(domainEvent.Buyer.ID);
            orderToUpdate.SetPaymentMethodID(domainEvent.PaymentMethod.ID);

            this.logger.LogTrace(
                "Order with ID: {orderID} has been successfully updated with a payment method {paymentMethod} ({id})",
                domainEvent.OrderID,
                nameof(domainEvent.PaymentMethod),
                domainEvent.PaymentMethod.ID
            );
        }
    }
}