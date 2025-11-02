using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.BuildingBlocks.EventBus.Abstractions {
    public interface IEventBus {
        void Publish(IntegrationEvent integrationEvent);

        void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void SubscribeDynamic<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler;

        void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void UnsubscribeDynamic<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler;
    }
}