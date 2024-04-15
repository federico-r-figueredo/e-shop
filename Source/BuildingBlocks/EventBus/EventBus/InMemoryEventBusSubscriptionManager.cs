using System;
using System.Collections.Generic;
using System.Linq;
using EShop.BuildingBlocks.EventBus.EventBus.Abstractions;
using EShop.BuildingBlocks.EventBus.EventBus.Events;

namespace EShop.BuildingBlocks.EventBus.EventBus {
    public partial class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionsManager {
        private readonly Dictionary<string, List<SubscriptionInfo>> eventHandlers;
        private readonly List<Type> eventTypes;

        public InMemoryEventBusSubscriptionManager() {
            this.eventHandlers = new Dictionary<string, List<SubscriptionInfo>>();
            this.eventTypes = new List<Type>();
        }

        public bool IsEmpty {
            get { return this.eventHandlers.Count == 0; }
        }

        public event EventHandler<string> OnEventRemoved;

        public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {
            string eventName = this.GetEventKey<TIntegrationEvent>();

            DoAddSubscription(typeof(TIntegrationEventHandler), eventName, false);

            if (!this.eventTypes.Contains(typeof(TIntegrationEvent))) {
                this.eventTypes.Add(typeof(TIntegrationEvent));
            }
        }

        public void AddDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler {
            DoAddSubscription(typeof(TDynamicIntegrationEventHandler), eventName, true);
        }

        public void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {
            SubscriptionInfo handlerToRemove = FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>();
            string eventName = GetEventKey<TIntegrationEvent>();
            DoRemoveHandler(eventName, handlerToRemove);
        }

        public void RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler {
            SubscriptionInfo handlerToRemove = FindDynamicSubscriptionToRemove<TDynamicIntegrationEventHandler>(eventName);
            DoRemoveHandler(eventName, handlerToRemove);
        }

        public void Clear() {
            this.eventHandlers.Clear();
        }

        public Type GetEventTypeByName(string eventName) {
            return this.eventTypes.SingleOrDefault(x => x.Name == eventName);
        }

        public IEnumerable<SubscriptionInfo> GetHandlerForEvent<TIntegrationEvent>()
            where TIntegrationEvent : IntegrationEvent {
            string eventKey = GetEventKey<TIntegrationEvent>();
            return GetHandlersForEvent(eventKey);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) {
            return this.eventHandlers[eventName];
        }

        public bool HasSubscriptionsForEvent<TIntegrationEvent>()
            where TIntegrationEvent : IntegrationEvent {
            string eventKey = GetEventKey<TIntegrationEvent>();
            return HasSubscriptionsForEvent(eventKey);
        }

        public bool HasSubscriptionsForEvent(string eventName) {
            return this.eventHandlers.ContainsKey(eventName);
        }

        public string GetEventKey<TIntegrationEvent>() {
            return typeof(TIntegrationEvent).Name;
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic) {
            if (!HasSubscriptionsForEvent(eventName)) {
                this.eventHandlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (this.eventHandlers[eventName].Any(x => x.HandlerType == handlerType)) {
                throw new ArgumentException($"Hanlder type {handlerType.Name} has been already registered for '{eventName}'", nameof(handlerType));
            }

            this.eventHandlers[eventName].Add(isDynamic ? SubscriptionInfo.Dynamic(handlerType) : SubscriptionInfo.Typed(handlerType));
        }

        private SubscriptionInfo FindSubscriptionToRemove<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {
            string eventName = GetEventKey<TIntegrationEvent>();
            return DoFindSubscriptionToRemove(eventName, typeof(TIntegrationEventHandler));
        }

        private SubscriptionInfo FindDynamicSubscriptionToRemove<TDynamicIntegrationEventHandler>(string eventName)
            where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler {
            return DoFindSubscriptionToRemove(eventName, typeof(TDynamicIntegrationEventHandler));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type eventHandlerType) {
            if (!HasSubscriptionsForEvent(eventName)) {
                return null;
            }

            return this.eventHandlers[eventName].SingleOrDefault(x => x.HandlerType == eventHandlerType);
        }

        private void DoRemoveHandler(string eventName, SubscriptionInfo handlerToRemove) {
            if (handlerToRemove == null) return;

            this.eventHandlers[eventName].Remove(handlerToRemove);

            if (this.eventHandlers[eventName].Any()) return;

            this.eventHandlers.Remove(eventName);

            Type eventType = this.eventTypes.SingleOrDefault(x => x.Name == eventName);
            if (eventType != null) {
                this.eventTypes.Remove(eventType);
            }

            RaiseOnEventRemoved(eventName);
        }

        private void RaiseOnEventRemoved(string eventName) {
            this.OnEventRemoved?.Invoke(this, eventName);
        }
    }
}