
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ordering.Infrastructure {
    internal class OrderingContextDesignFactory : IDesignTimeDbContextFactory<OrderingContext> {
        public OrderingContext CreateDbContext(string[] args) {
            DbContextOptionsBuilder<OrderingContext> optionsBuilder =
                new DbContextOptionsBuilder<OrderingContext>()
                    .UseSqlServer("Server=.;Initial Catalog=eShop.Services.Ordering;IntegratedSecurity=true");
            return new OrderingContext(optionsBuilder.Options, new NoMediator());
        }

        private class NoMediator : IMediator {
            public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default) {
                return default(IAsyncEnumerable<TResponse>);
            }

            public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default) {
                return default(IAsyncEnumerable<object>);
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default) {
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) {
                return Task.FromResult<TResponse>(default(TResponse));
            }

            public Task<object> Send(object request, CancellationToken cancellationToken = default) {
                return Task.FromResult(default(object));
            }
        }
    }
}