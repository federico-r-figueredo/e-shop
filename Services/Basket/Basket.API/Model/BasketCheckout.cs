using System;

namespace eShop.Services.Basket.API.Model {
    public class BasketCheckout {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardSecurityNumber { get; set; }
        public int CardTypeID { get; set; }
        public string Buyer { get; set; }
        public Guid RequestGUID { get; set; }
    }
}