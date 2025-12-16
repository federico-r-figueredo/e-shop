
namespace eShop.Services.Ordering.API.Application.DTOs {
    public class OrderItemDTO {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public int Units { get; set; }
        public string PictureURL { get; set; }
    }
}