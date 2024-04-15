
using System;
using System.Text.Json.Serialization;

namespace EShop.BuildingBlocks.EventBus.EventBus.Events {
    public class IntegrationEvent {
        private Guid id;
        private DateTime creationDate;

        public IntegrationEvent() {
            this.id = Guid.NewGuid();
            this.creationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime creationDate) {
            this.id = id;
            this.creationDate = creationDate;
        }

        [JsonInclude]
        public Guid Id {
            get {
                return this.id;
            }
        }

        [JsonInclude]
        public DateTime CreationDate {
            get {
                return this.creationDate;
            }
        }
    }
}