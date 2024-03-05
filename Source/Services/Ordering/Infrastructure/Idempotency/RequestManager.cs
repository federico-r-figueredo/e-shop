
using System;
using System.Threading.Tasks;
using Dawn;
using EShop.Services.Ordering.Domain.Exceptions;

namespace Ordering.Infrastructure.Idempotency {
    internal class RequestManager : IRequestManager {
        private readonly OrderingContext context;

        public RequestManager(OrderingContext context) {
            this.context = Guard
                .Argument(context, nameof(context))
                .NotNull()
                .Value;
        }

        public async Task<bool> ExistsAsync(Guid id) {
            var request = await this.context.FindAsync<ClientRequest>(id);

            return request != null;
        }

        public async Task<bool> CreateRequestForCommandAsync<T>(Guid id) {
            bool exists = await ExistsAsync(id);

            ClientRequest request = exists
                ? throw new OrderingDomainException($"Request with {id} already exists.")
                : new ClientRequest(id, typeof(T).Name, DateTime.UtcNow);

            this.context.Add(request);

            int result = await this.context.SaveChangesAsync();

            return result > 0;
        }
    }
}