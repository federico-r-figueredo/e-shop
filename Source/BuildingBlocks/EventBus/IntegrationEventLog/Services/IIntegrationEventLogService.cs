
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EShop.BuildingBlocks.EventBus.EventBus.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace EShop.BuildingBlocks.EventBus.IntegrationEventLog.Services {
    public interface IIntegrationEventLogService {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionID);
        Task SaveIntegrationEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid integrationEventID);
        Task MarkEventAsInProgressAsync(Guid integrationEventID);
        Task MarkEventAsFailedAsync(Guid integrationEventID);
    }
}