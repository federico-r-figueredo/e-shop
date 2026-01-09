using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eShop.Services.Ordering.Domain.Model.BuyerAggregate;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Infrastructure.Repositories {
    public class BuyerRepository : IBuyerRepository {
        private readonly OrderingContext context;

        public BuyerRepository(OrderingContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork {
            get { return this.context; }
        }

        public Buyer Add(Buyer buyer) {
            return buyer.IsTransient()
                ? this.context.Buyers.Add(buyer).Entity
                : buyer;
        }

        public async Task<Buyer> GetByIDAsync(string id) {
            return await this.context.Buyers
                .Include(x => x.PaymentMethods)
                .Where(x => x.ID == int.Parse(id))
                .SingleOrDefaultAsync();
        }

        public async Task<Buyer> GetByGUIDAsync(string guid) {
            return await this.context.Buyers
                .Include(x => x.PaymentMethods)
                .Where(x => x.IdentityGUID == guid)
                .SingleOrDefaultAsync();
        }

        public Buyer Update(Buyer buyer) {
            this.context.Entry(buyer).State = EntityState.Modified;
            return buyer;
        }
    }
}