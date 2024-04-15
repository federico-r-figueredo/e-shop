
using EShop.BuildingBlocks.EventBus.EventBus.Events;

namespace EShop.BuildingBlocks.EventBus.EventBus.Abstractions {
    public interface IEventBusClient {
        void Publish(IntegrationEvent integrationEvent);
        void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
        void SubscribeDynamic<TIntegrationEventHandler>(string integrationEventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler;
        void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
        void UnsubscribeDynamic<TIntegrationEventHandler>(string integrationEventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler;
    }
}