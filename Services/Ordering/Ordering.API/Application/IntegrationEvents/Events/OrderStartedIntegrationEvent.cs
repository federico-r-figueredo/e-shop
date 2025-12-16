using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Ordering.API.Application.IntegrationEvents.Events {
    public class OrderStartedIntegrationEvent : IntegrationEvent {
        public OrderStartedIntegrationEvent(string userID) {
            UserID = userID;
        }

        public string UserID { get; }
    }
}