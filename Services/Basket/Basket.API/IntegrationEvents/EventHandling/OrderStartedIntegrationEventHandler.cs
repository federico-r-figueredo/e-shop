using System;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;

namespace eShop.Services.Basket.API.IntegrationEvents.Events {
    public class OrderStartedIntegrationEventHandler
        : IIntegrationEventHandler<OrderStartedIntegrationEvent> {
        public Task Handle(OrderStartedIntegrationEvent integrationEvent) {
            Console.WriteLine($"I'm handling {typeof(OrderStartedIntegrationEvent).Name}");
            return Task.CompletedTask;
        }
    }
}