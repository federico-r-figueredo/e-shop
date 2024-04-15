using System;
using System.Linq;
using System.Text.Json;
using EShop.BuildingBlocks.EventBus.EventBus.Events;

namespace EShop.BuildingBlocks.EventBus.IntegrationEventLog {
    internal class IntegrationEventLogEntry {
        private Guid integrationEventID;
        private string eventTypeName;
        private IntegrationEvent @event;
        private IntegrationEventState state;
        private int timesSent;
        private DateTime creationTime;
        private string content;
        private string transactionID;

        private IntegrationEventLogEntry() { }

        public IntegrationEventLogEntry(IntegrationEvent integrationEvent, Guid transactionID) {
            this.integrationEventID = integrationEvent.Id;
            this.eventTypeName = integrationEvent.GetType().FullName;
            this.state = IntegrationEventState.NotPublished;
            this.timesSent = 0;
            this.creationTime = integrationEvent.CreationDate;
            this.content = JsonSerializer.Serialize(
                integrationEvent,
                integrationEvent.GetType(),
                new JsonSerializerOptions() {
                    WriteIndented = true
                }
            );
            this.transactionID = transactionID.ToString();
        }

        public Guid IntegrationEventID {
            get { return this.integrationEventID; }
        }

        public string EventTypeName {
            get { return this.eventTypeName; }
        }

        public string EventTypeShortName {
            get { return this.eventTypeName.Split('.')?.Last(); }
        }

        public IntegrationEvent IntegrationEvent {
            get { return this.@event; }
        }

        public IntegrationEventState State {
            get { return this.state; }
            set { this.state = value; }
        }

        public int TimesSent {
            get { return this.timesSent; }
            set { this.timesSent = value; }
        }

        public DateTime CreationTime {
            get { return this.creationTime; }
        }

        public string Content {
            get { return this.content; }
        }

        public string TransactionID {
            get { return this.transactionID; }
        }

        public IntegrationEventLogEntry DeserializeJsonContent(Type type) {
            this.@event = JsonSerializer.Deserialize(this.content, type, new JsonSerializerOptions() {
                PropertyNameCaseInsensitive = true
            }) as IntegrationEvent;

            return this;
        }
    }
}