using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using eShop.BuildingBlocks.EventBus;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBus.Events;
using eShop.BuildingBlocks.EventBus.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace eShop.BuildingBlocks.EventBusRabbitMQ {
    public class EventBusRabbitMQ : IEventBus, IDisposable {
        const string BROKER_NAME = "eshop_event_bus";
        const string AUTOFAC_SCOPE_NAME = "eshop_event_bus";

        private readonly IRabbitMQPersistentConnection persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> logger;
        private readonly IEventBusSubscriptionManager subscriptionsManager;
        private readonly ILifetimeScope autofac;
        private readonly int retryCount;

        private IChannel consumerChannel;
        private string queueName;
        private readonly SemaphoreSlim subscriptionLock = new SemaphoreSlim(1, 1);

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection,
            ILogger<EventBusRabbitMQ> logger, ILifetimeScope autofac,
            IEventBusSubscriptionManager subscriptionManager, string queueName = null,
            int retryCount = 5) {
            this.persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.subscriptionsManager = subscriptionManager ?? new InMemoryEventBusSubscriptionManager();
            this.queueName = queueName;
            this.autofac = autofac;
            this.retryCount = retryCount;
            this.subscriptionsManager.OnEventRemoved += this.SubscriptionManager_OnEventRemoved;
        }

        private async void SubscriptionManager_OnEventRemoved(object sender, string eventName) {
            if (!this.persistentConnection.IsConnected) {
                await this.persistentConnection.TryConnect();
            }

            using (IChannel channel = await this.persistentConnection.CreateChannelAsync()) {
                await channel.QueueUnbindAsync(
                    queue: this.queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName
                );

                if (this.subscriptionsManager.IsEmpty) {
                    this.queueName = string.Empty;
                    await this.consumerChannel?.CloseAsync();
                }
            }
        }

        public async Task PublishAsync(IntegrationEvent integrationEvent) {
            if (!this.persistentConnection.IsConnected) {
                await this.persistentConnection.TryConnect();
            }

            RetryPolicy policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(this.retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, time) => {
                    this.logger.LogWarning(
                        exception,
                        "Could not publish event: {EventID} after {Timeout}s ({ExceptionMessage})",
                        integrationEvent.ID,
                        $"{time.TotalSeconds:n1}",
                        exception
                    );
                });

            string eventName = integrationEvent.GetType().Name;

            this.logger.LogTrace("Creating RabbitMQ channel to publish event: {EventID} ({EventName})", integrationEvent.ID, eventName);

            using (IChannel channel = await this.persistentConnection.CreateChannelAsync()) {
                this.logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventID}", integrationEvent.ID);

                await channel.ExchangeDeclareAsync(
                    exchange: BROKER_NAME,
                    type: "direct"
                );

                string message = JsonConvert.SerializeObject(integrationEvent);
                byte[] body = Encoding.UTF8.GetBytes(message);

                await policy.Execute(async () => {
                    var properties = new BasicProperties() {
                        DeliveryMode = DeliveryModes.Persistent
                    };

                    this.logger.LogTrace("Publishing event to RabbitMQ: {EventID}", integrationEvent.ID);

                    await channel.BasicPublishAsync(
                        exchange: BROKER_NAME,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body
                    );
                });
            }
        }

        public async Task SubscribeAsync<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {
            await this.subscriptionLock.WaitAsync();
            try {
                string eventName = this.subscriptionsManager.GetEventKey<TIntegrationEvent>();
                await this.DoInternalSubscription(eventName);

                this.logger.LogInformation(
                    "Subscribing to event {EventName} with {EventHandler}",
                    eventName,
                    typeof(TIntegrationEventHandler).GetGenericTypeName()
                );

                this.subscriptionsManager.AddSubscription<TIntegrationEvent, TIntegrationEventHandler>();
                await this.StartBasicConsumeAsync();
            } finally {
                this.subscriptionLock.Release();
            }
        }

        public async Task SubscribeDynamicAsync<TIntegrationEventHandler>(string eventName) where TIntegrationEventHandler : IDynamicIntegrationEventHandler {
            this.logger.LogInformation(
                "Subscribing to dynamic event {EventName} with {EventHandler}",
                eventName,
                typeof(TIntegrationEventHandler).GetGenericTypeName()
            );

            await this.DoInternalSubscription(eventName);

            this.subscriptionsManager.AddDynamicSubscription<TIntegrationEventHandler>(eventName);
            await this.StartBasicConsumeAsync();
        }

        private async Task DoInternalSubscription(string eventName) {
            bool containsKey = this.subscriptionsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey) {
                if (!this.persistentConnection.IsConnected) {
                    await this.persistentConnection.TryConnect();
                }

                using (IChannel channel = await this.persistentConnection.CreateChannelAsync()) {
                    await channel.ExchangeDeclareAsync(
                        exchange: BROKER_NAME,
                        type: "direct"
                    );
                    await channel.QueueDeclareAsync(
                        queue: this.queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );
                    await channel.QueueBindAsync(
                        queue: this.queueName,
                        exchange: BROKER_NAME,
                        routingKey: eventName
                    );
                }
            }
        }

        public void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent> {
            string eventName = this.subscriptionsManager.GetEventKey<TIntegrationEvent>();

            this.logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            this.subscriptionsManager.RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>();
        }

        public void UnsubscribeDynamic<TIntegrationEventHandler>(string eventName)
            where TIntegrationEventHandler : IDynamicIntegrationEventHandler {
            this.subscriptionsManager.RemoveDynamicSubscription<TIntegrationEventHandler>(eventName);
        }

        public void Dispose() {
            this.consumerChannel?.Dispose();

            this.subscriptionsManager.Clear();
        }

        private async Task<IChannel> CreateConsumerChannelAsync() {
            if (!this.persistentConnection.IsConnected) {
                await this.persistentConnection.TryConnect();
            }

            this.logger.LogTrace("Creatting RabbitMQ consumer channel");

            IChannel channel = await this.persistentConnection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(
                exchange: BROKER_NAME,
                type: "direct"
            );
            await channel.QueueDeclareAsync(
                queue: this.queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            channel.CallbackExceptionAsync += async (sender, eventArgs) => {
                this.logger.LogWarning(eventArgs.Exception, "Recreating RabbitMQ consumer channel");

                this.consumerChannel?.Dispose();
                this.consumerChannel = await CreateConsumerChannelAsync();
                await this.StartBasicConsumeAsync();
            };

            return channel;
        }

        private async Task StartBasicConsumeAsync() {
            this.logger.LogTrace("Starting RabbitMQ basic consume");

            if (this.consumerChannel == null) {
                this.consumerChannel = await this.CreateConsumerChannelAsync();
            }

            if (this.consumerChannel == null) {
                this.logger.LogError($"StartBasicConsume can't call on {nameof(this.consumerChannel)} == null");
                return;
            }

            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(this.consumerChannel);
            consumer.ReceivedAsync += this.Consumer_Received;
            await this.consumerChannel.BasicConsumeAsync(
                queue: this.queueName,
                autoAck: false,
                consumer: consumer
            );
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs) {
            string eventName = eventArgs.RoutingKey;
            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            try {
                if (message.ToLowerInvariant().Contains("throw-fake-exception")) {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await this.ProcessEvent(eventName, message);
            } catch (Exception exception) {
                this.logger.LogWarning(exception, "----- ERROR Processing Message \"{Message}\"", message);
            }

            if (this.consumerChannel == null) {
                this.consumerChannel = await CreateConsumerChannelAsync();
            }

            // Even on exception we take the message off the queue. In a REAL WORLD app
            // this should be handled with a Dead Letter Exchange (DLX). For more information
            // see: http://www.rabbitmq.com/dlx.html
            await this.consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message) {
            this.logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            if (!this.subscriptionsManager.HasSubscriptionsForEvent(eventName)) {
                this.logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
                return;
            }

            using (ILifetimeScope scope = this.autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME)) {
                IEnumerable<SubscriptionInfo> subscriptions =
                    this.subscriptionsManager.GetHandlersForEvent(eventName);
                foreach (SubscriptionInfo subscription in subscriptions) {
                    if (subscription.IsDynamic) {
                        IDynamicIntegrationEventHandler handler =
                            scope.ResolveOptional(subscription.HandlerType)
                                as IDynamicIntegrationEventHandler;
                        if (handler == null) continue;
                        dynamic eventData = JObject.Parse(message);

                        await Task.Yield();
                        await handler.Handle(eventData);
                    } else {
                        object handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;
                        Type eventType = this.subscriptionsManager.GetEventTypeByName(eventName);
                        object integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
    }
}
