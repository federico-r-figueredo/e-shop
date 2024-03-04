
using System.Threading.Tasks;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate {
    public interface IBuyerRepository : IRepository<Buyer> {
        Buyer Add(Buyer buyer);
        Buyer Update(Buyer buyer);
        Task<Buyer> FindAsync(string buyerIdentityGUID);
        Task<Buyer> FindByIDAsync(string id);
    }
}