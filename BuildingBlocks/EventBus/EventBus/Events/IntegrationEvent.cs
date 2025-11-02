using System;
using Newtonsoft.Json;

namespace eShop.BuildingBlocks.EventBus.Events {
    public class IntegrationEvent {
        public IntegrationEvent() {
            this.ID = Guid.NewGuid();
            this.CreationDateTime = DateTime.UtcNow;
        }

        public IntegrationEvent(Guid id, DateTime creationDateTime) {
            this.ID = id;
            this.CreationDateTime = creationDateTime;
        }

        [JsonProperty]
        public Guid ID { get; set; }

        [JsonProperty]
        public DateTime CreationDateTime { get; set; }
    }
}