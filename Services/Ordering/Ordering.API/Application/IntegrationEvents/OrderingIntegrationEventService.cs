using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBus.Events;
using eShop.BuildingBlocks.IntegrationEventLogEF;
using eShop.BuildingBlocks.IntegrationEventLogEF.Services;
using eShop.Services.Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShop.Services.Ordering.API.Application.IntegrationEvents {
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService {
        private readonly IEventBus eventBus;
        private OrderingContext orderingContext;
        private readonly Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory;
        private readonly IIntegrationEventLogService integrationEventLogService;
        private readonly ILogger<OrderingIntegrationEventService> logger;

        public OrderingIntegrationEventService(IEventBus eventBus, OrderingContext orderingContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            ILogger<OrderingIntegrationEventService> logger) {
            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this.orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            this.integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            this.integrationEventLogService = this.integrationEventLogServiceFactory(
                this.orderingContext.Database.GetDbConnection()
            );
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionID) {
            IEnumerable<IntegrationEventLogEntry> pendingIntegrationEventLogs =
                await this.integrationEventLogService.RetrieveEventLogsPendingToPublishAsync(transactionID);

            foreach (IntegrationEventLogEntry pendingIntegrationEventLog in pendingIntegrationEventLogs) {
                this.logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventID} from {ApplicationName} - ({@IntegrationEvent})",
                    pendingIntegrationEventLog.EventID,
                    Program.ApplicationName,
                    pendingIntegrationEventLog.IntegrationEvent
                );

                try {
                    await this.integrationEventLogService.MarkEventAsInProgressAsync(pendingIntegrationEventLog.EventID);
                    await this.eventBus.PublishAsync(pendingIntegrationEventLog.IntegrationEvent);
                    await this.integrationEventLogService.MarkEventAsPublishedAsync(pendingIntegrationEventLog.EventID);
                } catch (Exception exception) {
                    this.logger.LogError(
                        exception,
                        "ERROR publishing integration event: {IntegrationEventID} from {ApplicationName}",
                        pendingIntegrationEventLog.EventID,
                        Program.ApplicationName
                    );

                    await this.integrationEventLogService.MarkEventAsFailedAsync(pendingIntegrationEventLog.EventID);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent integrationEvent) {
            this.logger.LogInformation(
                "----- Enqueuing integration event {IntegrationEventID} to repository ({@IntegrationEvent})",
                integrationEvent.ID,
                integrationEvent
            );

            await this.integrationEventLogService.SaveEventAsync(
                integrationEvent,
                this.orderingContext.CurrentTransaction
            );
        }
    }
}