
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using EShop.Services.Ordering.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Ordering.Infrastructure.Repositories {
    internal class BuyerRepository : IBuyerRepository {
        private readonly OrderingContext context;

        public BuyerRepository(OrderingContext context) {
            this.context = Guard
                .Argument(context, nameof(context))
                .NotNull()
                .Value;
        }

        public IUnitOfWork UnitOfWork {
            get { return this.context; }
        }

        public Buyer Add(Buyer buyer) {
            if (buyer.IsTransient()) {
                return this.context.Buyers.Add(buyer).Entity;
            }

            return buyer;
        }

        public async Task<Buyer> FindAsync(string identityGUID) {
            Buyer buyer = await this.context.Buyers
                .Include(x => x.PaymentMethods)
                .Where(x => x.IdentityGUID == identityGUID)
                .SingleOrDefaultAsync();

            return buyer;
        }

        public async Task<Buyer> FindByIDAsync(int id) {
            Buyer buyer = await this.context.Buyers
                .Include(x => x.PaymentMethods)
                .Where(x => x.ID == id)
                .SingleOrDefaultAsync();

            return buyer;
        }

        public Buyer Update(Buyer buyer) {
            return this.context.Buyers
                .Update(buyer)
                .Entity;
        }
    }
}