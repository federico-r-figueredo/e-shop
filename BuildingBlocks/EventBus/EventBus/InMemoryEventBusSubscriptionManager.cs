using System;
using System.Collections.Generic;
using System.Linq;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.BuildingBlocks.EventBus {
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager {
        private readonly Dictionary<string, List<SubscriptionInfo>> handlers;
        private readonly List<Type> eventTypes;

        public InMemoryEventBusSubscriptionManager() {
            this.handlers = new Dictionary<string, List<SubscriptionInfo>>();
            this.eventTypes = new List<Type>();
        }

        public bool IsEmpty {
            get { return !this.handlers.Keys.Any(); }
        }

        public event EventHandler<string> OnEventRemoved;

        public void AddSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent> {

            string eventName = this.GetEventKey<TEvent>();

            this.DoAddSubscription(eventName, typeof(TEventHandler), false);

            if (!this.eventTypes.Contains(typeof(TEvent))) {
                this.eventTypes.Add(typeof(TEvent));
            }
        }

        public void AddDynamicSubscription<TEventHandler>(string eventName)
            where TEventHandler : IDynamicIntegrationEventHandler {
            this.DoAddSubscription(eventName, typeof(TEventHandler), true);
        }

        private void DoAddSubscription(string eventName, Type handlerType, bool isDynamic) {
            if (!this.HasSubscriptionsForEvent(eventName)) {
                this.handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (this.handlers[eventName].Any(x => x.HandlerType == handlerType)) {
                throw new ArgumentException($@"Handler Type {handlerType.Name} already 
                    registered for '{eventName}'", nameof(handlerType)
                );
            }

            if (isDynamic) {
                this.handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
            } else {
                this.handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
            }
        }

        public void RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent> {

            string eventName = this.GetEventKey<TEvent>();
            SubscriptionInfo subscription = this.FindSubscriptionToRemove<TEvent, TEventHandler>();

            this.DoRemoveSubscription(eventName, subscription);
        }

        public void RemoveDynamicSubscription<TEventHanlder>(string eventName)
            where TEventHanlder : IDynamicIntegrationEventHandler {

            SubscriptionInfo subscription = FindDynamicSubscriptionToRemove<TEventHanlder>(eventName);

            this.DoRemoveSubscription(eventName, subscription);
        }

        private SubscriptionInfo FindSubscriptionToRemove<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent> {
            string eventName = GetEventKey<TEvent>();
            return DoFindSubscriptionToRemove(eventName, typeof(TEventHandler));
        }

        private SubscriptionInfo FindDynamicSubscriptionToRemove<TEventHandler>(string eventName)
            where TEventHandler : IDynamicIntegrationEventHandler {
            return this.DoFindSubscriptionToRemove(eventName, typeof(TEventHandler));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType) {
            if (!this.HasSubscriptionsForEvent(eventName)) {
                return null;
            }

            return this.handlers[eventName].SingleOrDefault(x => x.HandlerType == handlerType);
        }

        private void DoRemoveSubscription(string eventName, SubscriptionInfo subscriptionToRemove) {
            if (!this.HasSubscriptionsForEvent(eventName)) return;

            if (subscriptionToRemove == null) return;

            this.handlers[eventName].Remove(subscriptionToRemove);
            if (this.handlers[eventName].Any()) return;

            this.handlers.Remove(eventName);
            Type eventType = this.eventTypes.SingleOrDefault(x => x.Name == eventName);
            if (eventType != null) {
                this.eventTypes.Remove(eventType);
            }

            this.RaiseOnEventRemoved(eventName);
        }

        private void RaiseOnEventRemoved(string eventName) {
            EventHandler<string> handler = this.OnEventRemoved;
            handler?.Invoke(this, eventName);
        }

        public void Clear() {
            this.handlers.Clear();
        }

        public string GetEventKey<TEvent>() {
            return typeof(TEvent).Name;
        }

        public Type GetEventTypeByName(string eventName) {
            return this.eventTypes.SingleOrDefault(x => x.Name == eventName);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<TEvent>()
            where TEvent : IntegrationEvent {
            string key = this.GetEventKey<TEvent>();
            return this.GetHandlersForEvent(key);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) {
            return this.handlers[eventName];
        }

        public bool HasSubcriptionsForEvent<TEvent>() where TEvent : IntegrationEvent {
            string key = this.GetEventKey<TEvent>();
            return this.HasSubscriptionsForEvent(key);
        }

        public bool HasSubscriptionsForEvent(string eventName) {
            return this.handlers.ContainsKey(eventName);
        }
    }
}