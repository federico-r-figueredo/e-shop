using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eShop.Services.Ordering.API.Application.ViewModels;

namespace eShop.Services.Ordering.API.Application.Queries {
    internal interface IOrderQueries {
        Task<OrderViewModel> GetOrderAsync(int id);
        Task<IEnumerable<OrderSummaryViewModel>> GetOrdersFromUserAsync(Guid guid);
        Task<IEnumerable<CardTypeViewModel>> GetCardTypesAsync();
    }
}