using System.Collections.Generic;

namespace eShop.Services.Basket.API.Model {
    public class CustomerBasket {
        public CustomerBasket() {
            this.BasketItems = new List<BasketItem>();
        }

        public CustomerBasket(string customerID) {
            this.BuyerID = customerID;
        }

        public string BuyerID { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}