using System;
using System.Threading.Tasks;
using EShop.BuildingBlocks.EventBus.EventBus.Abstractions;
using EShop.BuildingBlocks.EventBus.EventBus.Events;
using EShop.BuildingBlocks.EventBus.IntegrationEventLog.Services;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using EShop.Services.Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EShop.Services.Ordering.API.Application.IntegrationEvents {
    internal class OrderingIntegrationEventService : IOrderingIntegrationEventService {
        private readonly Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory;
        private readonly IEventBusClient eventBusClient;
        private readonly OrderingContext orderingContext;
        private readonly IIntegrationEventLogService eventLogService;
        private readonly ILogger<OrderingIntegrationEventService> logger;

        public OrderingIntegrationEventService(IEventBusClient eventBusClient,
            OrderingContext orderingContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<OrderingIntegrationEventService> logger) {
            this.eventBusClient = eventBusClient ?? throw new ArgumentNullException(nameof(eventBusClient));
            this.orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            this.eventLogService = this.integrationEventLogServiceFactory(this.orderingContext.Database.GetDbConnection());
            this.integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionID) {
            throw new NotImplementedException();
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent integrationEvent) {
            throw new NotImplementedException();
        }
    }
}