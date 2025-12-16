using System;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBus.Extensions;
using eShop.Services.Ordering.API.Application.Commands;
using eShop.Services.Ordering.API.Application.IntegrationEvents.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Ordering.API.Application.IntegrationEvents.EventHandling {
    public class UserCheckoutAcceptedIntegrationEventHandler
        : IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent> {
        private readonly IMediator mediator;
        private readonly ILogger<UserCheckoutAcceptedIntegrationEventHandler> logger;

        public UserCheckoutAcceptedIntegrationEventHandler(IMediator mediator,
            ILogger<UserCheckoutAcceptedIntegrationEventHandler> logger) {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Integration event handler which starts the order creation process
        /// </summary>
        /// <param name="integrationEvent">
        /// Integration event message which is sent by the
        /// Basket.API once it has successfully processed
        /// the order items.
        /// </param>
        /// <returns></returns>
        public async Task Handle(UserCheckoutAcceptedIntegrationEvent integrationEvent) {
            Console.WriteLine($"I'm handling {typeof(UserCheckoutAcceptedIntegrationEvent).Name}");

            if (integrationEvent.RequestID == Guid.Empty) {
                this.logger.LogWarning(
                    "Invalid IntegrationEvent - RequestID is missing - {@IntegrationEvent}",
                    integrationEvent
                );
                return;
            }

            CreateOrderCommand createOrderCommand = new CreateOrderCommand(
                integrationEvent.Basket.BasketItems,
                integrationEvent.UserID,
                integrationEvent.UserName,
                integrationEvent.Street,
                integrationEvent.City,
                integrationEvent.State,
                integrationEvent.ZipCode,
                integrationEvent.Country,
                integrationEvent.CardNumber,
                integrationEvent.CardHolderName,
                integrationEvent.CardExpiration,
                integrationEvent.CardSecurityNumber,
                integrationEvent.CardTypeID
            );

            IdentifiedCommand<CreateOrderCommand, bool> identifiedCreateOrderCommand =
                new IdentifiedCommand<CreateOrderCommand, bool>(
                    createOrderCommand,
                    integrationEvent.RequestID
                );

            this.logger.LogInformation(
                "----- Sending command: {commandName} - {idProperty}: {commandID} ({@command})",
                identifiedCreateOrderCommand.GetGenericTypeName(),
                nameof(identifiedCreateOrderCommand.ID),
                identifiedCreateOrderCommand.ID,
                identifiedCreateOrderCommand
            );

            bool result = await this.mediator.Send(identifiedCreateOrderCommand);

            if (result) {
                this.logger.LogInformation(
                    "----- CreateOrderCommand succeeded - RequestID: {requestID}",
                    integrationEvent.RequestID
                );
            } else {
                this.logger.LogWarning(
                    "CreateOrderCommand failed - RequestID: {requestID}",
                    integrationEvent.RequestID
                );
            }
        }
    }
}