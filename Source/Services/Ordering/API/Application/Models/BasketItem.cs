
namespace EShop.Services.Ordering.API.Application.Models {
    public class BasketItem {
        private string id;
        private int productID;
        private string productName;
        private decimal unitPrice;
        private decimal oldUnitPrice;
        private int units;
        private string pictureURL;

        public BasketItem() {

        }

        public BasketItem(string id, int productID, string productName,
            decimal unitPrice, decimal oldUnitPrice, int units, string pictureURL) {
            this.id = id;
            this.productID = productID;
            this.productName = productName;
            this.unitPrice = unitPrice;
            this.oldUnitPrice = oldUnitPrice;
            this.units = units;
            this.pictureURL = pictureURL;
        }

        public string ID {
            get { return this.id; }
            set { this.id = value; }
        }

        public int ProductID {
            get { return this.productID; }
            set { this.productID = value; }
        }

        public string ProductName {
            get { return this.productName; }
            set { this.productName = value; }
        }

        public decimal UnitPrice {
            get { return this.unitPrice; }
            set { this.unitPrice = value; }
        }

        public int Units {
            get { return this.units; }
            set { this.units = value; }
        }

        public string PictureURL {
            get { return this.pictureURL; }
            set { this.pictureURL = value; }
        }
    }
}