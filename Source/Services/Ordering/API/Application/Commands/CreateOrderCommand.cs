using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MediatR;
using EShop.Services.Ordering.API.Application.Models;
using System.Linq;

namespace EShop.Services.Ordering.API.Application.Commands {
    [DataContract]
    public class CreateOrderCommand : ICommand {
        [DataMember]
        private readonly string userID;
        [DataMember]
        private readonly string userName;
        [DataMember]
        private readonly string city;
        [DataMember]
        private readonly string street;
        [DataMember]
        private readonly string state;
        [DataMember]
        private readonly string country;
        [DataMember]
        private readonly string zipCode;
        [DataMember]
        private readonly string cardNumber;
        [DataMember]
        private readonly string cardHolderName;
        [DataMember]
        private readonly DateOnly cardExpiration;
        [DataMember]
        private readonly string cardSecurityNumber;
        [DataMember]
        private readonly int cardTypeID;
        [DataMember]
        private readonly List<OrderItemDTO> orderItems;

        public CreateOrderCommand() {
            this.orderItems = new List<OrderItemDTO>();
        }

        public CreateOrderCommand(string userID, string userName, string city,
            string street, string state, string country, string zipCode,
            string cardNumber, string cardHolderName, DateOnly cardExpiration,
            string cardSecurityNumber, int cardTypeID, List<BasketItem> basketItems) {
            this.userID = userID;
            this.userName = userName;
            this.city = city;
            this.street = street;
            this.state = state;
            this.country = country;
            this.zipCode = zipCode;
            this.cardNumber = cardNumber;
            this.cardHolderName = cardHolderName;
            this.cardExpiration = cardExpiration;
            this.cardSecurityNumber = cardSecurityNumber;
            this.cardTypeID = cardTypeID;
            this.orderItems = basketItems.ToOrderItemsDTO().ToList();
        }

        public string UserID {
            get { return this.userID; }
        }

        public string UserName {
            get { return this.userName; }
        }

        public string City {
            get { return this.city; }
        }

        public string Street {
            get { return this.street; }
        }

        public string State {
            get { return this.state; }
        }

        public string Country {
            get { return this.country; }
        }

        public string ZipCode {
            get { return this.zipCode; }
        }

        public string CardNumber {
            get { return this.cardNumber; }
        }

        public string CardHolderName {
            get { return this.cardHolderName; }
        }

        public DateOnly CardExpiration {
            get { return this.cardExpiration; }
        }

        public string CardSecurityNumber {
            get { return this.cardSecurityNumber; }
        }

        public int CardTypeID {
            get { return this.cardTypeID; }
        }

        public IReadOnlyList<OrderItemDTO> OrderItems {
            get { return this.orderItems.AsReadOnly(); }
        }
    }

    public class OrderItemDTO {
        private readonly int productID;
        private readonly string productName;
        private readonly decimal unitPrice;
        private readonly decimal discount;
        private readonly int units;
        private readonly string pictureURL;

        public OrderItemDTO(int productID, string productName, decimal unitPrice,
            int units, string pictureURL, decimal discount = 1.0m) {
            this.productID = productID;
            this.productName = productName;
            this.unitPrice = unitPrice;
            this.discount = discount;
            this.units = units;
            this.pictureURL = pictureURL;
        }

        public int ProductID {
            get { return this.productID; }
        }

        public string ProductName {
            get { return this.productName; }
        }

        public decimal UnitPrice {
            get { return this.unitPrice; }
        }

        public decimal Discount {
            get { return this.discount; }
        }

        public int Units {
            get { return this.units; }
        }

        public string PictureURL {
            get { return this.pictureURL; }
        }
    }
}