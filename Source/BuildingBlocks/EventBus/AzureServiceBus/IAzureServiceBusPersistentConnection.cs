using System;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace EShop.BuildingBlocks.EventBus.AzureServiceBus {
    public interface IAzureServiceBusPersistentConnection : IAsyncDisposable {
        ServiceBusClient ServiceBusClient { get; }
        ServiceBusAdministrationClient ServiceBusAdministrationClient { get; }
    }
}