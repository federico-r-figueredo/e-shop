using System.Threading.Tasks;
using System.Collections.Generic;

namespace eShop.Services.Basket.API.Model {
    public interface IBasketRepository {
        IEnumerable<string> GetUsers();
        Task<CustomerBasket> GetBasketAsync(string customerID);
        Task<CustomerBasket> AddOrUpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string id);
    }
}