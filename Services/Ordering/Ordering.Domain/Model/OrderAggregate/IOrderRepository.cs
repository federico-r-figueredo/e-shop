using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.OrderAggregate {
    // This is just the repository contract defined at the Domain Layer as requisite for 
    // the Order Aggregate
    public interface IOrderRepository : IRepository<Order> {
        Order Add(Order order);
        void Update(Order order);
        Task<Order> GetByIDAsync(int orderID);
    }
}