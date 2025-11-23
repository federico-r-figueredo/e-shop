using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Basket.API.IntegrationEvents.Events {
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent {
        public ProductPriceChangedIntegrationEvent(int productID, decimal newPrice,
            decimal oldPrice) {
            ProductID = productID;
            NewPrice = newPrice;
            OldPrice = oldPrice;
        }

        public int ProductID { get; private set; }
        public decimal NewPrice { get; private set; }
        public decimal OldPrice { get; private set; }
    }
}