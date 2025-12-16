using System;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Ordering.API.Application.IntegrationEvents {
    public interface IOrderingIntegrationEventService {
        Task PublishEventsThroughEventBusAsync(Guid transactionID);
        Task AddAndSaveEventAsync(IntegrationEvent integrationEvent);
    }
}