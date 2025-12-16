using System;
using eShop.BuildingBlocks.EventBus.Events;
using eShop.Services.Ordering.API.Application.Models;

namespace eShop.Services.Ordering.API.Application.IntegrationEvents.Events {
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

        public string UserID { get; set; }
        public string UserName { get; set; }
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
        public Guid RequestID { get; set; }
        public CustomerBasket Basket { get; set; }
    }
}