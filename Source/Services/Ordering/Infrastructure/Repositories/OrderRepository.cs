
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;
using EShop.Services.Ordering.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace EShop.Services.Ordering.Infrastructure.Repositories {
    public class OrderRepository : IOrderRepository {
        private readonly OrderingContext context;

        public OrderRepository(OrderingContext context) {
            this.context = Guard
                .Argument(context, nameof(context))
                .NotNull()
                .Value;
        }

        public Order Add(Order order) {
            return this.context.Orders.Add(order).Entity;
        }

        public async Task<Order> FindByIDAsync(int orderID) {
            Order order = await this.context.Orders
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.ID == orderID);

            if (order == null) {
                order = this.context.Orders
                    .Local.FirstOrDefault(x => x.ID == orderID);
            }

            if (order != null) {
                await this.context.Entry(order).Collection(x => x.OrderItems).LoadAsync();
                await this.context.Entry(order).Reference(x => x.OrderStatus).LoadAsync();
            }

            return order;
        }

        public void Update(Order order) {
            this.context.Entry(order).State = EntityState.Modified;
        }
    }
}