using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace eShop.BuildingBlocks.EventBusRabbitMQ {
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection {
        private readonly IConnectionFactory connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> logger;
        private readonly int retryCount;
        IConnection connection;
        bool isDisposed;

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory,
            ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5) {
            this.connectionFactory = connectionFactory ?? throw new ArgumentException(nameof(connectionFactory));
            this.logger = logger ?? throw new ArgumentException(nameof(logger));
            this.retryCount = retryCount;
        }

        public bool IsConnected {
            get {
                return this.connection != null && this.connection.IsOpen && !this.isDisposed;
            }
        }

        public async Task<IChannel> CreateChannelAsync() {
            if (!this.IsConnected) {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");
            }

            return await this.connection.CreateChannelAsync();
        }

        public void Dispose() {
            if (this.isDisposed) return;

            this.isDisposed = true;

            try {
                this.connection?.Dispose();
            } catch (IOException exception) {
                this.logger.LogCritical(exception.ToString());
            }
        }

        public async Task<bool> TryConnect() {
            this.logger.LogInformation("RabbitMQ Client is trying to connnect");

            await this.semaphoreSlim.WaitAsync();
            try {
                AsyncRetryPolicy policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetryAsync(this.retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, time) => {
                        this.logger.LogWarning(exception, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", exception.Message);
                    });

                await policy.ExecuteAsync(async () => {
                    this.connection = await this.connectionFactory.CreateConnectionAsync();
                });

                if (this.IsConnected) {
                    this.connection.ConnectionShutdownAsync += this.OnConnectionShutdownAsync;
                    this.connection.CallbackExceptionAsync += this.OnCallbackExceptionAsync;
                    this.connection.ConnectionBlockedAsync += this.OnConnectionBlockedAsync;

                    this.logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", this.connection.Endpoint.HostName);

                    return true;
                } else {
                    this.logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            } finally {
                this.semaphoreSlim.Release();
            }
        }

        private async Task OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs eventArgs) {
            if (this.isDisposed) return;

            this.logger.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");

            await this.TryConnect();
        }

        private async Task OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs eventArgs) {
            if (this.isDisposed) return;

            this.logger.LogWarning("A RabbitMQ connection throwed and exception. Trying to re-connect...");

            await this.TryConnect();
        }

        private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs reason) {
            if (this.isDisposed) return;

            this.logger.LogWarning("A RabbitMQ connection was shutdown. Trying to re-connect...");

            await this.TryConnect();
        }
    }
}