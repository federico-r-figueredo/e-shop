using System;
using System.Collections.Generic;

namespace eShop.Services.Ordering.API.Application.ViewModels {
    public class OrderViewModel {
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
        public decimal Total { get; set; }
    }
}