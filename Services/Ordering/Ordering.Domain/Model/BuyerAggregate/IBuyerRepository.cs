using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.BuyerAggregate {
    // This is just the repository contract defined at the Domain Layer as requisite for 
    // the Buyer Aggregate
    public interface IBuyerRepository : IRepository<Buyer> {
        Buyer Add(Buyer buyer);
        Buyer Update(Buyer buyer);
        Task<Buyer> GetByIDAsync(string id);
        Task<Buyer> GetByGUIDAsync(string guid);
    }
}