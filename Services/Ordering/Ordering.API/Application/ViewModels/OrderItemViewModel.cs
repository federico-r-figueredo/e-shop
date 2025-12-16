
namespace eShop.Services.Ordering.API.Application.ViewModels {
    public class OrderItemViewModel {
        public string ProductName { get; set; }
        public int Units { get; set; }
        public double UnitPrice { get; set; }
        public string PictureURL { get; set; }
    }
}