
namespace eShop.Services.Ordering.API.Application.Models {
    public class BasketItem {
        public string ID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OldUnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureURL { get; set; }
    }
}