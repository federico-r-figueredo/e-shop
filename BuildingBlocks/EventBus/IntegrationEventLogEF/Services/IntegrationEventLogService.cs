using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShop.BuildingBlocks.IntegrationEventLogEF.Services {
    public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable {
        private readonly IntegrationEventLogContext integrationEventLogContext;
        private readonly DbConnection dbConnection;
        private readonly List<Type> eventTypes;
        private volatile bool disposedValue;

        public IntegrationEventLogService(DbConnection dbConnection) {
            this.dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            this.integrationEventLogContext = new IntegrationEventLogContext(
                new DbContextOptionsBuilder<IntegrationEventLogContext>()
                    .UseSqlServer(this.dbConnection)
                    .Options
            );
            this.eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(x => x.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionID) {
            string transactionIDAsString = transactionID.ToString();

            List<IntegrationEventLogEntry> result =
                await this.integrationEventLogContext.IntegrationEventLogs
                    .Where(x => x.TransactionID == transactionIDAsString
                        && x.State == EventStateEnum.NotPublished)
                    .ToListAsync();

            if (result == null || !result.Any()) {
                return new List<IntegrationEventLogEntry>();
            }

            return result.OrderBy(x => x.CreationDateTime)
                .Select(x => x.DeserializeJsonContent(this.eventTypes.Find(y => y.Name == x.EventTypeShortName)));
        }

        public Task<int> SaveEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction transaction) {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            IntegrationEventLogEntry eventLogEntry = new IntegrationEventLogEntry(
                integrationEvent,
                transaction.TransactionId
            );

            this.integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            this.integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

            return this.integrationEventLogContext.SaveChangesAsync();
        }

        public Task<int> MarkEventAsInProgressAsync(Guid integrationEventID) {
            return this.UpdateEventStatus(integrationEventID, EventStateEnum.InProgress);
        }

        public Task<int> MarkEventAsPublishedAsync(Guid integrationEventID) {
            return this.UpdateEventStatus(integrationEventID, EventStateEnum.Published);
        }

        public Task<int> MarkEventAsFailedAsync(Guid integrationEventID) {
            return this.UpdateEventStatus(integrationEventID, EventStateEnum.PublishedFailed);
        }

        private Task<int> UpdateEventStatus(Guid eventID, EventStateEnum status) {
            IntegrationEventLogEntry eventLogEntry = this.integrationEventLogContext
                .IntegrationEventLogs
                .Single(x => x.EventID == eventID);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress) {
                eventLogEntry.TimesSent++;
            }

            this.integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

            return this.integrationEventLogContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing) {
            if (this.disposedValue) return;

            if (disposing) {
                this.integrationEventLogContext?.Dispose();
            }

            this.disposedValue = true;
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}