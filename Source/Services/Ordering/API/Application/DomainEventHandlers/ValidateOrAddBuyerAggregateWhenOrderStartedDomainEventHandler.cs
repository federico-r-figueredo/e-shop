using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using EShop.Services.Ordering.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.API.Application.DomainEventHandlers {
    internal class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent> {
        private readonly IBuyerRepository buyerRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerFactory loggerFactory;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(IBuyerRepository buyerRepository,
            IUnitOfWork unitOfWork, ILoggerFactory loggerFactory) {
            this.buyerRepository = buyerRepository;
            this.unitOfWork = unitOfWork;
            this.loggerFactory = loggerFactory;
        }

        public async Task Handle(OrderStartedDomainEvent domainEvent, CancellationToken cancellationToken) {
            var cardTypeID = (domainEvent.CardTypeID != 0) ? domainEvent.CardTypeID : 1;
            var buyer = await this.buyerRepository.FindAsync(domainEvent.UserID);
            bool buyerOriginallyExisted = (buyer == null) ? false : true;

            if (!buyerOriginallyExisted) {
                buyer = new Buyer(domainEvent.UserID, domainEvent.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(
                cardTypeID,
                $"Payment method on {DateTime.UtcNow}",
                domainEvent.CardNumber,
                domainEvent.CardSecurityNumber,
                domainEvent.CardHolderName,
                domainEvent.CardExpiration,
                domainEvent.Order.ID
            );

            Buyer buyerUpdated = buyerOriginallyExisted
                ? this.buyerRepository.Update(buyer)
                : this.buyerRepository.Add(buyer);

            await this.unitOfWork.SaveEntitiesAsync(cancellationToken);

            // TODO send OrderStatusChangedToSubmittedIntegrationEvent

            // this.loggerFactory
            //     .CreateLogger<OrderStartedDomainEventHandler>()
            //     .LogTrace($"Buyer {buyerUpdated.ID} and related payment method were validated or updated of orderID = {domainEvent.Order.ID}");
        }
    }
}