using System;
using System.Collections.Generic;

namespace EShop.Services.Ordering.API.Application.Queries.ViewModels {
    public class GetOrderByIDResponse {
        private readonly int orderNumber;
        private readonly DateTime date;
        private readonly string status;
        private readonly string description;
        private readonly string street;
        private readonly string city;
        private readonly string zipCode;
        private readonly string country;
        private readonly List<OrderItemViewModel> orderItems;
        private decimal total;

        public GetOrderByIDResponse(int orderNumber, DateTime date, string status,
            string description, string street, string city, string zipCode,
            string country, List<OrderItemViewModel> orderItems, decimal total) {
            this.orderNumber = orderNumber;
            this.date = date;
            this.status = status;
            this.description = description;
            this.street = street;
            this.city = city;
            this.zipCode = zipCode;
            this.country = country;
            this.orderItems = orderItems;
            this.total = total;
        }

        public int OrderNumber {
            get { return this.orderNumber; }
        }
        public DateTime Date {
            get { return this.date; }
        }
        public string Status {
            get { return this.status; }
        }
        public string Description {
            get { return this.description; }
        }
        public string Street {
            get { return this.street; }
        }
        public string City {
            get { return this.city; }
        }
        public string ZipCode {
            get { return this.zipCode; }
        }
        public string Country {
            get { return this.country; }
        }
        public List<OrderItemViewModel> OrderItems {
            get { return this.orderItems; }
        }
        public decimal Total {
            get { return this.total; }
            set { this.total = value; }
        }
    }
}
