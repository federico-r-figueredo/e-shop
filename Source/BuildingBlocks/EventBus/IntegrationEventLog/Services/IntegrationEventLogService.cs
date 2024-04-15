
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EShop.BuildingBlocks.EventBus.EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EShop.BuildingBlocks.EventBus.IntegrationEventLog.Services {
    internal class IntegrationEventLogService : IIntegrationEventLogService, IDisposable {
        private readonly IntegrationEventLogContext integrationEventLogContext;
        private readonly DbConnection dbConnection;
        private List<Type> eventTypes;
        private volatile bool disposedValue;

        public IntegrationEventLogService(DbConnection dbConnection) {
            this.dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            this.integrationEventLogContext = new IntegrationEventLogContext(
                new DbContextOptionsBuilder<IntegrationEventLogContext>()
                    .UseSqlServer(this.dbConnection)
                    .Options);
            this.eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(x => x.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionID) {
            string txid = transactionID.ToString();

            List<IntegrationEventLogEntry> result = await this.integrationEventLogContext.IntegrationEventLogs
                .Where(x => x.TransactionID == txid && x.State == IntegrationEventState.NotPublished).ToListAsync();

            if (!result.Any()) {
                return new List<IntegrationEventLogEntry>();
            }

            return result
                .OrderBy(x => x.CreationTime)
                .Select(x => x.DeserializeJsonContent(this.eventTypes.Find(y => y.Name == x.EventTypeShortName)));
        }

        public Task SaveIntegrationEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction transaction) {
            if (transaction == null) {
                throw new ArgumentNullException(nameof(transaction));
            }

            IntegrationEventLogEntry integrationEventLogEntry = new IntegrationEventLogEntry(integrationEvent, transaction.TransactionId);
            this.integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            this.integrationEventLogContext.IntegrationEventLogs.Add(integrationEventLogEntry);

            return this.integrationEventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsFailedAsync(Guid integrationEventID) {
            return UpdateIntegrationEventStatus(integrationEventID, IntegrationEventState.PublishedFailed);
        }

        public Task MarkEventAsInProgressAsync(Guid integrationEventID) {
            return UpdateIntegrationEventStatus(integrationEventID, IntegrationEventState.InProgress);
        }

        public Task MarkEventAsPublishedAsync(Guid integrationEventID) {
            return UpdateIntegrationEventStatus(integrationEventID, IntegrationEventState.Published);
        }

        private Task UpdateIntegrationEventStatus(Guid integrationEventID, IntegrationEventState status) {
            IntegrationEventLogEntry integrationEventLogEntry = this.integrationEventLogContext
                .IntegrationEventLogs.Single(x => x.IntegrationEventID == integrationEventID);
            integrationEventLogEntry.State = status;

            if (status == IntegrationEventState.InProgress) {
                integrationEventLogEntry.TimesSent++;
            }

            this.integrationEventLogContext.IntegrationEventLogs.Update(integrationEventLogEntry);
            return this.integrationEventLogContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing) {
            if (!this.disposedValue) {
                if (disposing) {
                    this.integrationEventLogContext?.Dispose();
                }

                this.disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}