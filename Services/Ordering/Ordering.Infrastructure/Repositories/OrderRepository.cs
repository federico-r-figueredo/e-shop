using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eShop.Services.Ordering.Domain.Model.OrderAggregate;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Infrastructure.Repositories {
    public class OrderRepository : IOrderRepository {
        private readonly OrderingContext context;

        public OrderRepository(OrderingContext context) {
            this.context = context;
        }

        public IUnitOfWork UnitOfWork {
            get { return this.context; }
        }

        public Order Add(Order order) {
            return this.context.Orders.Add(order).Entity;
        }

        public async Task<Order> GetByIDAsync(int id) {
            Order order = await this.context.Orders
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.ID == id);

            if (order == null) {
                order = this.context.Orders.Local.FirstOrDefault(x => x.ID == id);
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