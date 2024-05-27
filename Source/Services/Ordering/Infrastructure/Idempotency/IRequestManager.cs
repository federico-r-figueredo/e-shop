
using System;
using System.Threading.Tasks;

namespace EShop.Services.Ordering.Infrastructure.Idempotency {
    public interface IRequestManager {
        Task<bool> ExistsAsync(Guid id);
        Task<bool> CreateRequestForCommandAsync<T>(Guid id);
    }
}