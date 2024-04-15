
using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EShop.BuildingBlocks.EventBus.RabbitMQ {
    internal class RabbitMQPersistentConnection : IRabbitMQPersistentConnection {
        private readonly IConnectionFactory connectionFactory;
        private readonly ILogger<RabbitMQPersistentConnection> logger;
        private readonly int retryCount;
        private IConnection connection;
        private bool disposed;
        readonly object syncRoot = new object();

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory,
            ILogger<RabbitMQPersistentConnection> logger, int retryCount) {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.retryCount = retryCount;
        }

        public bool IsConnected {
            get {
                return this.connection.IsOpen && !this.disposed;
            }
        }

        public IModel CreateModel() {
            if (!this.IsConnected) {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return this.connection.CreateModel();
        }

        public void Dispose() {
            if (this.disposed) return;

            this.disposed = true;

            try {
                this.connection.ConnectionShutdown -= OnConnectionShutdown;
                this.connection.CallbackException -= OnCallbackException;
                this.connection.ConnectionBlocked -= OnConnectionBlocked;
                this.connection.Dispose();
            } catch (IOException ex) {
                this.logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect() {
            this.logger.LogInformation("RabbitMQ client is trying to connect");

            lock (this.syncRoot) {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(
                        retryCount: this.retryCount,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (ex, time) => {
                            this.logger.LogWarning(ex, $"RabbitMQ Client could not connect after {time.TotalNanoseconds:n1}s ({ex.Message})");
                        });

                policy.Execute(() => {
                    this.connection = this.connectionFactory.CreateConnection();
                });

                if (!this.IsConnected) {
                    this.logger.LogCritical("FATAL ERROR: RabbitMQ connnection could not be created and opened");
                    return false;
                }

                this.connection.ConnectionShutdown += OnConnectionShutdown;
                this.connection.CallbackException += OnCallbackException;
                this.connection.ConnectionBlocked += OnConnectionBlocked;

                this.logger.LogInformation($"RabbitMQ client acquired a persistent connection to '{this.connection.Endpoint.HostName}'");

                return true;
            }
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e) {
            if (this.disposed) {
                return;
            }

            this.logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e) {
            if (this.disposed) {
                return;
            }

            this.logger.LogWarning("A RabbitMQ connection throwed an exception. Trying to re-connect...");
            TryConnect();
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e) {
            if (this.disposed) {
                return;
            }

            this.logger.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");
            TryConnect();
        }
    }
}