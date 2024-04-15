
using System.Threading.Tasks;
using EShop.BuildingBlocks.EventBus.EventBus.Events;

namespace EShop.BuildingBlocks.EventBus.EventBus.Abstractions {
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent {
        Task Handle(TIntegrationEvent integrationEvent);
    }

    public interface IIntegrationEventHandler {

    }
}