using System;
using System.Threading.Tasks;
using EShop.BuildingBlocks.EventBus.EventBus.Events;

namespace EShop.Services.Ordering.API.Application.IntegrationEvents {
    internal interface IOrderingIntegrationEventService {
        Task PublishEventsThroughEventBusAsync(Guid transactionID);
        Task AddAndSaveEventAsync(IntegrationEvent integrationEvent);
    }
}