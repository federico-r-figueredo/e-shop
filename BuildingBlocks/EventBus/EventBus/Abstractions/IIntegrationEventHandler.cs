using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.BuildingBlocks.EventBus.Abstractions {
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent {
        Task Handle(TIntegrationEvent integrationEvent);
    }

    public interface IIntegrationEventHandler { }
}