using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using EShop.BuildingBlocks.EventBus.EventBus;
using EShop.BuildingBlocks.EventBus.EventBus.Abstractions;
using EShop.BuildingBlocks.EventBus.EventBus.Events;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using static EShop.BuildingBlocks.EventBus.EventBus.InMemoryEventBusSubscriptionManager;

namespace EShop.BuildingBlocks.EventBus.AzureServiceBus {
    public class AzureServiceBusClient : IEventBusClient, IAsyncDisposable {
        private const string TOPIC_NAME = "eshop-event-bus";
        private const string AUTOFAC_SCOPE_NAME = "eshop-event-bus";
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

        private readonly IAzureServiceBusPersistentConnection serviceBusPersistentConnection;
        private readonly ILogger<AzureServiceBusClient> logger;
        private readonly IEventBusSubscriptionsManager subscriptionsManager;
        private readonly ILifetimeScope autofac;
        private readonly string subscriptionName;
        private readonly ServiceBusSender sender;
        private readonly ServiceBusProcessor processor;

        public AzureServiceBusClient(IAzureServiceBusPersistentConnection serviceBusPersistentConnection,
            ILogger<AzureServiceBusClient> logger, IEventBusSubscriptionsManager subscriptionsManager,
            ILifetimeScope autofac, string subscriptionName) {
            this.serviceBusPersistentConnection = serviceBusPersistentConnection;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.subscriptionsManager = subscriptionsManager ?? new InMemoryEventBusSubscriptionManager();
            this.autofac = autofac;
            this.subscriptionName = subscriptionName;
            this.sender = this.serviceBusPersistentConnection.ServiceBusClient.CreateSender(TOPIC_NAME);
            this.processor = this.serviceBusPersistentConnection.ServiceBusClient.CreateProcessor(TOPIC_NAME, subscriptionName, new ServiceBusProcessorOptions {
                MaxConcurrentCalls = 10,
                AutoCompleteMessages = true
            });

            RemoveDefaultRule();
            RegisterSubscriptionClientMessageHandlerAsync().GetAwaiter().GetResult();
        }

        public void Publish(IntegrationEvent integrationEvent) {
            string eventName = integrationEvent.GetType().Name.Replace(oldValue: INTEGRATION_EVENT_SUFFIX, newValue: "");
            string jsonMessage = JsonSerializer.Serialize(value: integrationEvent, inputType: integrationEvent.GetType());
            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

            ServiceBusMessage message = new ServiceBusMessage {
                MessageId = Guid.NewGuid().ToString(),
                Subject = eventName,
                Body = new BinaryData(body)
            };

            this.sender.SendMessageAsync(message)
                .GetAwaiter()
                .GetResult();
        }

        public void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {
            string integrationEventName = typeof(TIntegrationEvent).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

            bool containsKey = this.subscriptionsManager.HasSubscriptionsForEvent<TIntegrationEvent>();
            if (!containsKey) {
                try {
                    this.serviceBusPersistentConnection.ServiceBusAdministrationClient.CreateRuleAsync(TOPIC_NAME, this.subscriptionName, new CreateRuleOptions {
                        Filter = new CorrelationRuleFilter() {
                            Subject = integrationEventName
                        },
                        Name = integrationEventName
                    }).GetAwaiter().GetResult();
                } catch (ServiceBusException) {
                    this.logger.LogWarning($"The messaging entity {integrationEventName} already exists.");
                }
            }

            this.logger.LogInformation($"Subscribing to event {integrationEventName} with {typeof(TIntegrationEventHandler).Name}");
            this.subscriptionsManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void SubscribeDynamic<TIntegrationEventHandler>(string integrationEventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler {
            this.logger.LogInformation($"Subscribing to dynamic event {integrationEventName} with {typeof(TIntegrationEventHandler).Name}");
            this.subscriptionsManager.AddDynamicSubscription<TIntegrationEventHandler>(integrationEventName);
        }

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {
            string integrationEventName = typeof(TIntegrationEvent).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");

            try {
                this.serviceBusPersistentConnection
                    .ServiceBusAdministrationClient
                    .DeleteRuleAsync(TOPIC_NAME, this.subscriptionName, integrationEventName)
                    .GetAwaiter()
                    .GetResult();
            } catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound) {
                this.logger.LogWarning($"The messaging entity {integrationEventName} could not be found");
            }

            this.logger.LogInformation($"Ubsubscribing from event {integrationEventName}");
            this.subscriptionsManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void UnsubscribeDynamic<TIntegrationEventHandler>(string integrationEventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler {
            this.logger.LogInformation($"Ubsubscribing from dynamic event {integrationEventName}");
            this.subscriptionsManager.RemoveDynamicSubscription<TIntegrationEventHandler>(integrationEventName);
        }

        private void RemoveDefaultRule() {
            try {
                this.serviceBusPersistentConnection
                    .ServiceBusAdministrationClient
                    .DeleteRuleAsync(TOPIC_NAME, this.subscriptionName, RuleProperties.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            } catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound) {
                this.logger.LogWarning($"The messaging entity {RuleProperties.DefaultRuleName} couldn't be found");
            }
        }

        private async Task RegisterSubscriptionClientMessageHandlerAsync() {
            this.processor.ProcessMessageAsync += async (args) => {
                string eventName = $"{args.Message.Subject}{INTEGRATION_EVENT_SUFFIX}";
                string messageData = args.Message.Body.ToString();

                // Complete the message so that it isn't received again
                if (await ProcessEvent(eventName, messageData)) {
                    await args.CompleteMessageAsync(args.Message);
                }
            };

            this.processor.ProcessErrorAsync += ErrorHandler;
            await this.processor.StartProcessingAsync();
        }

        private async Task<bool> ProcessEvent(string eventName, string message) {
            this.logger.LogTrace($"Processing Azure ServiceBus event: {eventName}");
            bool processed = false;

            if (!this.subscriptionsManager.HasSubscriptionsForEvent(eventName)) {
                this.logger.LogWarning($"No subscription for Azure ServiceBus event: {eventName}");
                return processed;
            }

            using (var scope = this.autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME)) {
                IEnumerable<SubscriptionInfo> subscriptions = this.subscriptionsManager.GetHandlersForEvent(eventName);

                foreach (SubscriptionInfo subscription in subscriptions) {
                    if (subscription.IsDynamic) {
                        if (!(scope.ResolveOptional(subscription.HandlerType) is IDynamicIntegrationEventHandler handler)) {
                            continue;
                        }

                        using (dynamic eventData = JsonDocument.Parse(message)) {
                            await handler.Handle(eventData);
                        }
                    } else {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;
                        Type eventType = this.subscriptionsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType);
                        Type concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                    }
                }
            }

            processed = true;
            return processed;
        }

        private Task ErrorHandler(ProcessErrorEventArgs args) {
            Exception exception = args.Exception;
            ServiceBusErrorSource errorSource = args.ErrorSource;

            this.logger.LogError(exception, $"ERROR handling message: {exception.Message} - Context: {errorSource}");
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync() {
            this.subscriptionsManager.Clear();
            await this.processor.CloseAsync();
        }
    }
}