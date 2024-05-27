
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EShop.Services.Ordering.Domain.SeedWork {
    public interface IUnitOfWork : IDisposable {
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}