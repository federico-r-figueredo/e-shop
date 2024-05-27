using System;

namespace EShop.Services.Ordering.API.Application.Queries.ViewModels {
    public class OrderSummaryViewModel {
        private readonly int orderNumber;
        private readonly DateTime date;
        private readonly string status;
        private readonly double total;

        public OrderSummaryViewModel(int orderNumber, DateTime date, string status,
            double total) {
            this.orderNumber = orderNumber;
            this.date = date;
            this.status = status;
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
        public double Total {
            get { return this.total; }
        }
    }
}
