using System;
using System.Threading.Tasks;

namespace eShop.Services.Ordering.Infrastructure.Idempotency {
    public interface IRequestManager {
        Task<bool> ExistsAsync(Guid guid);
        Task CreateRequestForCommandAsync<T>(Guid guid);
    }
}