using System;
using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Ordering.API.Application.DomainEventHandlers.OrderStartedEvent {
    internal class SendEmailToCustomerWhenOrderStartedDomainEventHandler
        : INotificationHandler<OrderStartedDomainEvent> {
        private ILogger<SendEmailToCustomerWhenOrderStartedDomainEventHandler> logger;

        public SendEmailToCustomerWhenOrderStartedDomainEventHandler(
            ILogger<SendEmailToCustomerWhenOrderStartedDomainEventHandler> logger) {
            this.logger = logger;
        }

        public Task Handle(OrderStartedDomainEvent notification, 
            CancellationToken cancellationToken) {
            this.logger.LogInformation("----- Sending email to {user}", notification.UserName);
            return Task.CompletedTask;
        }
    }
}