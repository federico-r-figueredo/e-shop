
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac;
using EShop.BuildingBlocks.EventBus.EventBus;
using EShop.BuildingBlocks.EventBus.EventBus.Abstractions;
using EShop.BuildingBlocks.EventBus.EventBus.Events;
using EShop.BuildingBlocks.EventBus.EventBus.Extensions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using static EShop.BuildingBlocks.EventBus.EventBus.InMemoryEventBusSubscriptionManager;

namespace EShop.BuildingBlocks.EventBus.RabbitMQ {
    public class RabbitMQClient : IEventBusClient, IDisposable {
        const string BROKER_NAME = "eshop-event-bus";
        const string AUTOFAC_SCOPE_NAME = "eshop-event-bus";

        private readonly IRabbitMQPersistentConnection persistentConnection;
        private readonly ILogger<RabbitMQClient> logger;
        private readonly IEventBusSubscriptionsManager subscriptionsManager;
        private readonly ILifetimeScope lifetimeScope;
        private readonly int retryCount;
        private IModel consumerChannel;
        private string queueName;

        public RabbitMQClient(IRabbitMQPersistentConnection persistentConnection,
            ILogger<RabbitMQClient> logger, IEventBusSubscriptionsManager subscriptionsManager,
            ILifetimeScope lifetimeScope, int retryCount = 5, string queueName = null) {
            this.persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.subscriptionsManager = subscriptionsManager ?? new InMemoryEventBusSubscriptionManager();
            this.subscriptionsManager.OnEventRemoved += OnEventRemoved;
            this.lifetimeScope = lifetimeScope;
            this.retryCount = retryCount;
            this.consumerChannel = CreateConsumerChannel();
            this.queueName = queueName;
        }

        public void Publish(IntegrationEvent integrationEvent) {
            if (!this.persistentConnection.IsConnected) this.persistentConnection.TryConnect();

            string eventName = integrationEvent.GetType().Name;
            byte[] body = JsonSerializer.SerializeToUtf8Bytes(
                value: integrationEvent,
                inputType: integrationEvent.GetType(),
                options: new JsonSerializerOptions {
                    WriteIndented = true
                }
            );

            this.logger.LogTrace($"Creating a RabbitMQ channel to publish event: {integrationEvent.Id} {eventName}");
            using (IModel channel = this.persistentConnection.CreateModel()) {

                this.logger.LogTrace($"Declaring RabbitMQ exchange to publish event: {integrationEvent.Id}");
                channel.ExchangeDeclare(exchange: BROKER_NAME, type: ExchangeType.Direct);

                RetryPolicy policy = CreateRetryPolicy(integrationEvent);
                policy.Execute(() => {

                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    this.logger.LogTrace($"Publishing event to RabbitMQ: {integrationEvent.Id}");
                    channel.BasicPublish(
                        exchange: BROKER_NAME,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body
                    );
                });
            }
        }

        public void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {

            string eventName = this.subscriptionsManager.GetEventKey<TIntegrationEvent>();
            DoInternalSubscription(eventName);

            this.logger.LogInformation($"Subscribing to event {eventName} with {typeof(TIntegrationEventHandler).GetGenericTypeName()}");
            this.subscriptionsManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();

            StartBasicConsume();
        }

        public void SubscribeDynamic<TIntegrationEventHandler>(string integrationEventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler {

            this.logger.LogInformation($"Subscribing to dynamic event {integrationEventName} with {typeof(TIntegrationEventHandler).GetGenericTypeName()}");
            DoInternalSubscription(integrationEventName);

            this.subscriptionsManager.AddDynamicSubscription<TIntegrationEventHandler>(integrationEventName);

            StartBasicConsume();
        }

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {

            var eventName = this.subscriptionsManager.GetEventKey<TIntegrationEvent>();
            this.logger.LogInformation($"Unsubscribing from event {eventName}");

            this.subscriptionsManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void UnsubscribeDynamic<TIntegrationEventHandler>(string integrationEventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler {

            this.subscriptionsManager.RemoveDynamicSubscription<TIntegrationEventHandler>(integrationEventName);
        }

        public void Dispose() {
            if (this.consumerChannel != null) {
                this.consumerChannel.Dispose();
            }

            this.subscriptionsManager.Clear();
        }

        private void OnEventRemoved(object sender, string eventName) {
            if (!this.persistentConnection.IsConnected) {
                this.persistentConnection.TryConnect();
            }

            using (IModel channel = this.persistentConnection.CreateModel()) {
                channel.QueueBind(
                    queue: this.queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName
                );

                if (this.subscriptionsManager.IsEmpty) {
                    this.queueName = string.Empty;
                    this.consumerChannel.Close();
                }
            }
        }

        private IModel CreateConsumerChannel() {
            if (!this.persistentConnection.IsConnected) {
                this.persistentConnection.TryConnect();
            }

            this.logger.LogTrace("Creating RabbitMQ consumer channel");

            IModel channel = this.persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: this.queueName, type: "direct");
            channel.QueueDeclare(queue: this.queueName, durable: true, exclusive: true, autoDelete: true, arguments: null);
            channel.CallbackException += (sender, eventArgs) => {
                this.logger.LogWarning(eventArgs.Exception, "Recreating RabbitMQ consumer channel");
                this.consumerChannel.Dispose();
                this.consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private RetryPolicy CreateRetryPolicy(IntegrationEvent integrationEvent) {
            return RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(
                    retryCount: this.retryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (ex, time) => {
                        this.logger.LogWarning(ex, $"Couldn't publish event: {integrationEvent.Id} after {time.TotalSeconds:n1}s {ex.Message}");
                    });
        }

        private void DoInternalSubscription(string eventName) {
            bool containsKey = this.subscriptionsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey) {
                if (!this.persistentConnection.IsConnected) {
                    this.persistentConnection.TryConnect();
                }

                this.consumerChannel.QueueBind(
                    queue: this.queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName
                );
            }
        }

        private void StartBasicConsume() {
            this.logger.LogTrace("Starting RabbitMQ basic consume");

            if (this.consumerChannel == null) {
                this.logger.LogError("StartBasicConsume can't call on consumerChannel == null");
                return;
            }

            var consumer = new AsyncEventingBasicConsumer(this.consumerChannel);

            consumer.Received += ConsumerReceived;

            this.consumerChannel.BasicConsume(
                queue: this.queueName,
                autoAck: false,
                consumer: consumer
            );
        }

        private async Task ConsumerReceived(object sender, BasicDeliverEventArgs eventArgs) {
            string eventName = eventArgs.RoutingKey;
            string message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try {
                if (message.ToLowerInvariant().Contains("throw-fake-exception")) {
                    throw new InvalidOperationException($"Fake exception requested: '{message}'");
                }

                await ProcesEvent(eventName, message);
            } catch (Exception ex) {
                this.logger.LogWarning(ex, $"----- ERROR Processing message '{message}'");
            }

            // Even on exception we take the message off the queue.
            // In a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX).
            // For more information see: https://www.rabbitmq.com/dlx.html
            this.consumerChannel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        }

        private async Task<bool> ProcesEvent(string eventName, string message) {
            this.logger.LogTrace($"Processing RabbitMQ event: {eventName}");
            bool processed = false;

            if (!this.subscriptionsManager.HasSubscriptionsForEvent(eventName)) {
                this.logger.LogWarning($"No subscription for RabbitMQ event: {eventName}");
                return processed;
            }

            using (ILifetimeScope scope = this.lifetimeScope.BeginLifetimeScope(AUTOFAC_SCOPE_NAME)) {
                IEnumerable<SubscriptionInfo> subscriptions = this.subscriptionsManager.GetHandlersForEvent(eventName);

                foreach (SubscriptionInfo subscription in subscriptions) {
                    if (subscription.IsDynamic) {
                        if (!(scope.ResolveOptional(subscription.HandlerType) is IDynamicIntegrationEventHandler handler)) {
                            continue;
                        }

                        using (dynamic eventData = JsonDocument.Parse(message)) {
                            await Task.Yield();
                            await handler.Handle(eventData);
                        }
                    } else {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;
                        Type eventType = this.subscriptionsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions {
                            PropertyNameCaseInsensitive = true
                        });
                        Type concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }

            processed = true;
            return processed;
        }
    }
}