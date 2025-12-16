using System.Collections.Generic;
using eShop.Services.Ordering.API.Application.DTOs;
using eShop.Services.Ordering.API.Application.Models;

namespace eShop.Services.Ordering.API.Extensions {
    public static class BasketItemExtensions {
        public static IEnumerable<OrderItemDTO> ToOrderItemsDTO(this IEnumerable<BasketItem> basketItems) {
            foreach (BasketItem basketItem in basketItems) {
                yield return basketItem.ToOrderItemDTO();
            }
        }

        public static OrderItemDTO ToOrderItemDTO(this BasketItem basketItem) {
            return new OrderItemDTO() {
                ProductID = basketItem.ProductID,
                ProductName = basketItem.ProductName,
                PictureURL = basketItem.PictureURL,
                UnitPrice = basketItem.UnitPrice,
                Units = basketItem.Quantity
            };
        }
    }
}