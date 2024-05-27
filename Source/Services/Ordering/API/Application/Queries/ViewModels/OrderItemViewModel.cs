namespace EShop.Services.Ordering.API.Application.Queries.ViewModels {
    public class OrderItemViewModel {
        private readonly string productName;
        private readonly int units;
        private readonly double unitPrice;
        private readonly string pictureURL;

        public OrderItemViewModel(string productName, int units, double unitPrice,
            string pictureURL) {
            this.productName = productName;
            this.units = units;
            this.unitPrice = unitPrice;
            this.pictureURL = pictureURL;
        }

        public string ProductName {
            get { return this.productName; }
        }
        public int Units {
            get { return this.units; }
        }
        public double UnitPrice {
            get { return this.unitPrice; }
        }
        public string PictureURL {
            get { return this.pictureURL; }
        }
    }
}
