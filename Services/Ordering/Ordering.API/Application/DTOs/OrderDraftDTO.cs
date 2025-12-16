using System.Collections.Generic;
using System.Linq;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;

namespace eShop.Services.Ordering.API.Application.DTOs {
    public class OrderDraftDTO {
        public static OrderDraftDTO FromOrder(Order order) {
            return new OrderDraftDTO() {
                OrderItems = order.OrderItems.Select(x => new OrderItemDTO() {
                    Discount = x.CurrentDiscount,
                    ProductID = x.ProductID,
                    UnitPrice = x.UnitPrice,
                    PictureURL = x.PictureURL,
                    Units = x.Units,
                    ProductName = x.ProductName
                }),
                Total = order.GetTotal()
            };
        }

        public IEnumerable<OrderItemDTO> OrderItems { get; set; }
        public decimal Total { get; set; }
    }
}