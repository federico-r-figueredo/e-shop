using System;
using System.Data.Common;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;
using eShop.BuildingBlocks.EventBus.Events;
using eShop.BuildingBlocks.IntegrationEventLogEF.Services;
using eShop.Services.Catalog.API.Infrastructure;
using Microsoft.Extensions.Logging;
using eShop.BuildingBlocks.IntegrationEventLogEF.Utilities;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.Catalog.API.IntegrationEvents {
    internal class CatalogIntegrationEventService :
        ICatalogIntegrationEventService, IDisposable {
        private readonly ILogger<CatalogIntegrationEventService> logger;
        private readonly IEventBus eventBus;
        private readonly CatalogContext catalogContext;
        private Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory;
        private readonly IIntegrationEventLogService eventLogService;
        private volatile bool disposedValue;

        public CatalogIntegrationEventService(ILogger<CatalogIntegrationEventService> logger,
            IEventBus eventBus, CatalogContext catalogContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory) {
            this.logger = logger;
            this.eventBus = eventBus;
            this.catalogContext = catalogContext;
            this.integrationEventLogServiceFactory = integrationEventLogServiceFactory;
            this.eventLogService = this.integrationEventLogServiceFactory(
                this.catalogContext.Database.GetDbConnection()
            );
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent integrationEvent) {
            try {
                this.logger.LogInformation(
                    @"----- Publishing integration event: {IntegrationEventID_Published} 
                    from {AppName} - ({@integrationEvent})", integrationEvent.ID,
                    Program.ApplicationName, integrationEvent
                );

                await this.eventLogService.MarkEventAsInProgressAsync(integrationEvent.ID);
                this.eventBus.Publish(integrationEvent);
                await this.eventLogService.MarkEventAsPublishedAsync(integrationEvent.ID);
            } catch (Exception exception) {
                this.logger.LogError(exception, @"ERROR Publishing integration event: 
                    {IntegrationEventID} from {ApplicationName} - ({@IntegrationEvent})",
                    integrationEvent.ID, Program.ApplicationName, integrationEvent
                );
            }
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent integrationEvent) {
            this.logger.LogInformation(
                @"----- CatalogIntegrationEventService - Saving changes and integratinoEvent: 
                {IntegrationEventID}", integrationEvent.ID
            );

            // Use of an EF Core resiliency strategy when using multiple DbContexts within 
            // an explicit BeginTransaction():
            // See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
            await ResilientTransaction.New(this.catalogContext).ExecuteAsync(async () => {
                // Achieving atomicity between original catalog database operation and the
                // IntegrationEventLog thanks to a local transaction.
                await this.catalogContext.SaveChangesAsync();
                await this.eventLogService.SaveEventAsync(
                    integrationEvent,
                    this.catalogContext.Database.CurrentTransaction
                );
            });
        }

        protected virtual void Dispose(bool disposing) {
            if (this.disposedValue) return;

            if (disposing) {
                (this.eventLogService as IDisposable)?.Dispose();
            }

            this.disposedValue = true;
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}