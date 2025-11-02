using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Catalog.API.IntegrationEvents.Events {

    // Integration Event notes:
    // An Event is "something that has happened in the past", therefore, its name has to
    // be past tense. An Integration Event is an event that can cause side effects to other
    // microservices, bounded-contexts or external systems.
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent {
        private int productID;
        private decimal newPrice;
        private decimal oldPrice;

        public ProductPriceChangedIntegrationEvent(int productID, decimal newPrice,
            decimal oldPrice) {
            this.productID = productID;
            this.newPrice = newPrice;
            this.oldPrice = oldPrice;
        }

        public decimal OldPrice {
            get { return this.oldPrice; }
        }

        public decimal NewPrice {
            get { return this.newPrice; }
        }

        public int ProductID {
            get { return this.productID; }
        }
    }
}