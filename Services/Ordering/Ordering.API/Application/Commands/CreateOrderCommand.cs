using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using eShop.Services.Ordering.API.Application.DTOs;
using eShop.Services.Ordering.API.Application.Models;
using eShop.Services.Ordering.API.Extensions;
using MediatR;

namespace eShop.Services.Ordering.API.Application.Commands {
    // DDD and CQRS patterns comment: Note that it is recommended to implement immutable
    // Commands. In this case, its immutability is achieved by having all the setters as
    // private plus only being able to update the data just once, when creating the object
    // through its constructor.
    // References to Immutable Commands:
    // http://cqrs.nu/Faq
    // https://docs.spine3.org/motivation/immutability.html 
    // http://blog.gauffin.org/2012/06/griffin-container-introducing-command-support/
    // https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/how-to-implement-a-lightweight-class-with-auto-implemented-properties
    [DataContract]
    public class CreateOrderCommand : IRequest<bool> {
        public CreateOrderCommand() {
            this.orderItems = new List<OrderItemDTO>();
        }

        public CreateOrderCommand(List<BasketItem> basketItems, string userID, string userName,
            string street, string city, string state, string zipCode, string country,
            string cardNumber, string cardHolderName, DateTime cardExpiration,
            string cardSecurityNumber, int cardTypeID) {
            this.orderItems = basketItems.ToOrderItemsDTO().ToList();

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
        }

        [DataMember]
        private readonly List<OrderItemDTO> orderItems;

        [DataMember]
        public string UserID { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public string Street { get; private set; }

        [DataMember]
        public string City { get; private set; }

        [DataMember]
        public string State { get; private set; }

        [DataMember]
        public string ZipCode { get; private set; }

        [DataMember]
        public string Country { get; private set; }

        [DataMember]
        public string CardNumber { get; private set; }

        [DataMember]
        public string CardHolderName { get; private set; }

        [DataMember]
        public DateTime CardExpiration { get; private set; }

        [DataMember]
        public string CardSecurityNumber { get; private set; }

        [DataMember]
        public int CardTypeID { get; private set; }

        [DataMember]
        public IEnumerable<OrderItemDTO> OrderItems {
            get { return this.orderItems; }
        }
    }
}