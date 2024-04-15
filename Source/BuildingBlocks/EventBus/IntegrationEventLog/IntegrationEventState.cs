using System;

namespace EShop.BuildingBlocks.EventBus.IntegrationEventLog {
    public enum IntegrationEventState {
        NotPublished,
        InProgress,
        Published,
        PublishedFailed
    }
}
