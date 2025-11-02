-   _EventBus_

    -   **IEventBus**: The abstraction over the event bus used to publish events.
    -   **IIntegrationEventHandler**: The abstraction over the event handler that will handle
        events published through the event bus.
    -   **IntegrationEvent**: The base class for all integration events.
    -   **IEventBusSubscriptionManager**: The abstraction over the construct that manages handler
        subscriptions against events.
    -   **InMemoryEventBusSubscriptionManager**: The concrete implementation of the construct that manages handler subscriptions against events. It holds a dictionary where event names are the keys and each key is associates w/ a value that consists on a collection of **SubscriptionInfo** that represents the subscription against a concrete event on behalf of a concrete event handler.
    -   **SubscriptionInfo**: The data type that encapsulates details about a handler's subscription.

-   _EventBusRabbitMQ_

    -   **IRabbitMQPersistentConnection** - An abstraction that represents a connection against the RabbitMQ server.
    -   **DefaultRabbitMQPersistentConnection** - A concrete implementation that represents a connection against the RabbitMQ server.
    -   **EventBusRabbitMQ** - Represents a concrete implementation of the event bus used to publish events. It's composed of a RabbitMQ persistent connection, a subscription manager and a channel that was created using the persistent connection.

-   _IntegrationEventLog_
    -   **IntegrationEventLogEntry** - Represent an integration event log that tracks which integration events have been published and in which state they are. Useful to implement retries and improve resilience in case event publish fails for some reason.
    -   **IntegrationEventLogContext** - The database context used to manage **IntegrationEventLogEntry** records.
    -   **EventStateEnum** - The state of an event.
