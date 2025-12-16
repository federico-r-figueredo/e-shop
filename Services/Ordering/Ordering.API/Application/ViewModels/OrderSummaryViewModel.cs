using System;

namespace eShop.Services.Ordering.API.Application.ViewModels {
    public class OrderSummaryViewModel {
        public int OrderNumber { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public double Total { get; set; }
    }
}