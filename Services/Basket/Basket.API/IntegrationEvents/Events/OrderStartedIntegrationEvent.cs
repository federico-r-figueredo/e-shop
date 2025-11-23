using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Basket.API.IntegrationEvents.Events {
    public class OrderStartedIntegrationEvent : IntegrationEvent {
        public OrderStartedIntegrationEvent(string userID) {
            UserID = userID;
        }

        public string UserID { get; private set; }
    }
}