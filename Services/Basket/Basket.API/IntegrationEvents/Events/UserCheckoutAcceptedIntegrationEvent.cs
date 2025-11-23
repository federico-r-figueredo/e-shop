using System;
using eShop.BuildingBlocks.EventBus.Events;
using eShop.Services.Basket.API.Model;

namespace eShop.Services.Basket.API.IntegrationEvents.Events {
    public class UserCheckoutAcceptedIntegrationEvent : IntegrationEvent {
        public UserCheckoutAcceptedIntegrationEvent(string userID, string userName,
            string street, string city, string state, string zipCode, string country,
            string cardNumber, string cardHolderName, DateTime cardExpiration,
            string cardSecurityNumber, int cardTypeID, string buyer, Guid requestID,
            CustomerBasket basket) {
            UserID = userID;
            UserName = userName;
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeID = cardTypeID;
            Buyer = buyer;
            RequestID = requestID;
            Basket = basket;
        }

        public string UserID { get; private set; }
        public string UserName { get; private set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public string Country { get; private set; }
        public string CardNumber { get; private set; }
        public string CardHolderName { get; private set; }
        public DateTime CardExpiration { get; private set; }
        public string CardSecurityNumber { get; private set; }
        public int CardTypeID { get; private set; }
        public string Buyer { get; private set; }
        public Guid RequestID { get; private set; }
        public CustomerBasket Basket { get; private set; }
    }
}