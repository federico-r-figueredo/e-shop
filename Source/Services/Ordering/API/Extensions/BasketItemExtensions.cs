
using System.Collections.Generic;
using EShop.Services.Ordering.API.Application.Commands;
using EShop.Services.Ordering.API.Application.Models;

namespace EShop.Services.Ordering.API {
    internal static class BasketItemExtensions {
        public static IEnumerable<OrderItemDTO> ToOrderItemsDTO(this IEnumerable<BasketItem> basketItems) {
            foreach (BasketItem basketItem in basketItems) {
                yield return basketItem.ToOrderItemDTO();
            }
        }

        public static OrderItemDTO ToOrderItemDTO(this BasketItem basketItem) {
            return new OrderItemDTO(
                basketItem.ProductID,
                basketItem.ProductName,
                basketItem.UnitPrice,
                basketItem.Units,
                basketItem.PictureURL
            );
        }
    }
}