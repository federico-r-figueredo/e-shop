using System;
using System.Threading.Tasks;
using eShop.Services.Ordering.Domain.Exceptions;

namespace eShop.Services.Ordering.Infrastructure.Idempotency {
    public class RequestManager : IRequestManager {
        private readonly OrderingContext context;

        public RequestManager(OrderingContext context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistsAsync(Guid guid) {
            ClientRequest request = await this.context.FindAsync<ClientRequest>(guid);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid guid) {
            bool exists = await this.ExistsAsync(guid);

            ClientRequest request = exists
                ? throw new OrderingDomainException($"Request with {guid} already exists.")
                : new ClientRequest() {
                    GUID = guid,
                    Name = typeof(T).Name,
                    DateTime = DateTime.UtcNow
                };

            this.context.Add(request);

            await this.context.SaveChangesAsync();
        }
    }
}