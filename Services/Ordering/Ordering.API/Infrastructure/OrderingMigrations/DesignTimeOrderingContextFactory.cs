using System.Threading;
using System.Threading.Tasks;
using eShop.Services.Ordering.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eShop.Services.Ordering.API.Infrastructure.OrderingMigrations {
    public class DesignTimeOrderingContextFactory
        : IDesignTimeDbContextFactory<OrderingContext> {
        public OrderingContext CreateDbContext(string[] args) {
            DbContextOptionsBuilder<OrderingContext> optionsBuilder =
                new DbContextOptionsBuilder<OrderingContext>()
                    .UseSqlServer(
                        @"Server=localhost;Initial Catalog=eShop.Services.OrderingDB;
                        User Id=admin;Password=1234;TrustServerCertificate=True;
                        Encrypt=false;Trusted_Connection=True;",
                        options => {
                            options.MigrationsAssembly(this.GetType().Assembly.GetName().Name);
                            options.MigrationsHistoryTable("MigrationsHistory", "EFCore");
                        }
                    );

            return new OrderingContext(optionsBuilder.Options, new NoMediator());
        }

        private class NoMediator : IMediator {
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
                return Task.FromResult<object>(default(object));
            }
        }
    }
}