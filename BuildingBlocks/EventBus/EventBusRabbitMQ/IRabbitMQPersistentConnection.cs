using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace eShop.BuildingBlocks.EventBusRabbitMQ {
    public interface IRabbitMQPersistentConnection : IDisposable {
        bool IsConnected { get; }
        Task<bool> TryConnect();
        Task<IChannel> CreateChannelAsync();
    }
}