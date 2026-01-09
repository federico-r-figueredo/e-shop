using System;
using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.API.Application.IntegrationEvents;
using eShop.Services.Ordering.API.Application.IntegrationEvents.Events;
using eShop.Services.Ordering.Domain.Events;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Ordering.API.Application.DomainEventHandlers.OrderStartedEvent {
    internal class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
        : INotificationHandler<OrderStartedDomainEvent> {
        private readonly IBuyerRepository buyerRepository;
        private readonly IOrderingIntegrationEventService integrationEventService;
        private readonly ILogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler> logger;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(
            IBuyerRepository buyerRepository, IOrderingIntegrationEventService integrationEventService) {
            this.buyerRepository = buyerRepository;
            this.integrationEventService = integrationEventService;
        }

        public async Task Handle(OrderStartedDomainEvent orderStartedDomainEvent, CancellationToken cancellationToken) {
            int cardTypeID = orderStartedDomainEvent.CardTypeID != 0
                ? orderStartedDomainEvent.CardTypeID
                : 1;
            Buyer buyer = await this.buyerRepository.GetByIDAsync(orderStartedDomainEvent.UserID);
            bool buyerDoesNotExist = buyer == null;

            if (buyerDoesNotExist) {
                buyer = new Buyer(orderStartedDomainEvent.UserID, orderStartedDomainEvent.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(
                cardTypeID,
                $"Payment method on {DateTime.Now}",
                orderStartedDomainEvent.CardNumber,
                orderStartedDomainEvent.CardSecurityNumber,
                orderStartedDomainEvent.CardHolderName,
                orderStartedDomainEvent.CardExpiration,
                orderStartedDomainEvent.Order.ID
            );

            Buyer updatedBuyer = buyerDoesNotExist
                ? this.buyerRepository.Add(buyer)
                : this.buyerRepository.Update(buyer);

            await this.buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            await this.integrationEventService.AddAndSaveEventAsync(
                new OrderStatusChangedToSubmittedIntegrationEvent(
                    orderStartedDomainEvent.Order.ID,
                    orderStartedDomainEvent.Order.OrderStatus.Name,
                    buyer.Name
                )
            );
            this.logger.LogTrace(
                "Buyer {buyerID} and related payment method were validated or updated for orderID: {orderID}",
                updatedBuyer.ID,
                orderStartedDomainEvent.Order.ID
            );
        }
    }
}