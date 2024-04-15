
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace EShop.BuildingBlocks.EventBus.AzureServiceBus {
    internal class AzureServiceBusPersistentConnection : IAzureServiceBusPersistentConnection {
        private readonly string serviceBusConnectionString;
        private ServiceBusClient serviceBusClient;
        private ServiceBusAdministrationClient serviceBusAdministrationClient;
        private bool disposed;

        public AzureServiceBusPersistentConnection(string serviceBusConnectionString) {
            this.serviceBusConnectionString = serviceBusConnectionString;
            this.serviceBusClient = new ServiceBusClient(this.serviceBusConnectionString);
            this.serviceBusAdministrationClient = new ServiceBusAdministrationClient(this.serviceBusConnectionString);
        }

        public ServiceBusClient ServiceBusClient {
            get {
                if (this.serviceBusClient.IsClosed) {
                    this.serviceBusClient = new ServiceBusClient(this.serviceBusConnectionString);
                }
                return this.serviceBusClient;
            }
        }

        public ServiceBusAdministrationClient ServiceBusAdministrationClient {
            get { return this.serviceBusAdministrationClient; }
        }

        public ServiceBusClient CreateModel() {
            if (this.serviceBusClient.IsClosed) {
                this.serviceBusClient = new ServiceBusClient(this.serviceBusConnectionString);
            }

            return this.serviceBusClient;
        }

        public ValueTask DisposeAsync() {
            throw new System.NotImplementedException();
        }
    }
}