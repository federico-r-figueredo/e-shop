using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using eShop.BuildingBlocks.EventBus.Events;
using Newtonsoft.Json;

namespace eShop.BuildingBlocks.IntegrationEventLogEF {
    public class IntegrationEventLogEntry {
        private IntegrationEventLogEntry() { }

        public IntegrationEventLogEntry(IntegrationEvent integrationEvent,
            Guid transactionID) {
            this.EventID = integrationEvent.ID;
            this.CreationDateTime = integrationEvent.CreationDateTime;
            this.EventTypeName = integrationEvent.GetType().FullName;
            this.Content = JsonConvert.SerializeObject(integrationEvent);
            this.State = EventStateEnum.NotPublished;
            this.TimesSent = 0;
            this.TransactionID = transactionID.ToString();
        }

        public Guid EventID { get; private set; }
        public string EventTypeName { get; private set; }
        [NotMapped]
        public string EventTypeShortName {
            get {
                return this.EventTypeName.Split('.')?.Last();
            }
        }
        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; set; }
        public EventStateEnum State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string Content { get; set; }
        public string TransactionID { get; set; }

        public IntegrationEventLogEntry DeserializeJsonContent(Type type) {
            this.IntegrationEvent = JsonConvert.DeserializeObject(this.Content, type) as IntegrationEvent;
            return this;
        }
    }
}