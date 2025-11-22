using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.BuildingBlocks.EventBus.Abstractions {
    public interface IEventBus {
        Task PublishAsync(IntegrationEvent integrationEvent);

        Task SubscribeAsync<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        Task SubscribeDynamicAsync<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler;

        void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

        void UnsubscribeDynamic<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler;
    }
}