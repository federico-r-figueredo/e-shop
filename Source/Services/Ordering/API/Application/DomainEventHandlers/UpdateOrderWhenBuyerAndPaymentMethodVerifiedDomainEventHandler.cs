using MediatR;
using EShop.Services.Ordering.Domain.Events;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;

namespace EShop.Services.Ordering.API.Application.DomainEventHandlers {
    internal class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler
        : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent> {
        private readonly IOrderRepository orderRepository;
        private readonly ILoggerFactory logger;

        public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
            IOrderRepository orderRepository, ILoggerFactory logger
        ) {
            this.orderRepository = orderRepository;
            this.logger = logger;
        }

        // Domain Logic Comment:
        // When the Buyer and Buyer's payment method have been created or verified that they existed,
        // then we can update the original Order with the BuyerID and PaymentID (foreign keys)
        public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent domainEvent, CancellationToken cancellationToken) {
            Order orderToUpdate = await this.orderRepository.FindByIDAsync(domainEvent.OrderID);
            orderToUpdate.SetBuyerID(domainEvent.Buyer.ID);
            orderToUpdate.SetPaymentMethodID(domainEvent.PaymentMethod.ID);

            this.logger
                .CreateLogger<UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler>()
                .LogTrace($"Order with ID: {domainEvent.OrderID} has been succesfully updated with a payment method {nameof(domainEvent.PaymentMethod)} ({domainEvent.PaymentMethod.ID})");
        }
    }
}