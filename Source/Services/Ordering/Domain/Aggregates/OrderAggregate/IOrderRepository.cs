
using System.Threading.Tasks;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.OrderAggregate {
    public interface IOrderRepository : IRepository<Order> {
        Order Add(Order order);
        void Update(Order order);
        Task<Order> GetAsync(int orderID);
    }
}