using System;
using System.Collections.Generic;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.BuildingBlocks.EventBus {
    public interface IEventBusSubscriptionManager {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void AddDynamicSubscription<TEventHandler>(string eventName)
            where TEventHandler : IDynamicIntegrationEventHandler;

        void RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void RemoveDynamicSubscription<TEventHanlder>(string eventName)
            where TEventHanlder : IDynamicIntegrationEventHandler;

        bool HasSubcriptionsForEvent<TEvent>()
            where TEvent : IntegrationEvent;

        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<TEvent>()
            where TEvent : IntegrationEvent;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey<TEvent>();
    }
}