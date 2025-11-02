using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShop.BuildingBlocks.IntegrationEventLogEF.Services {
    public interface IIntegrationEventLogService {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionID);
        Task<int> SaveEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction transaction);
        Task<int> MarkEventAsInProgressAsync(Guid integrationEventID);
        Task<int> MarkEventAsPublishedAsync(Guid integrationEventID);
        Task<int> MarkEventAsFailedAsync(Guid integrationEventID);
    }
}