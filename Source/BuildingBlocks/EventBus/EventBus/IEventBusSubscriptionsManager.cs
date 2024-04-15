
using System;
using System.Collections.Generic;
using EShop.BuildingBlocks.EventBus.EventBus.Abstractions;
using EShop.BuildingBlocks.EventBus.EventBus.Events;
using static EShop.BuildingBlocks.EventBus.EventBus.InMemoryEventBusSubscriptionManager;

namespace EShop.BuildingBlocks.EventBus.EventBus {
    public interface IEventBusSubscriptionsManager {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
        void AddDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;
        void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;
        void RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;
        bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlerForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<TIntegrationEvent>();
    }
}