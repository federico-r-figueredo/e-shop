using MediatR;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Infrastructure.Extensions {
    internal static class MediatorExtensions {
        internal static async Task DispatchDomainEventsAsync(this IMediator mediator,
            OrderingContext context) {
            IEnumerable<EntityEntry<Entity>> domainEntities = context.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents.Any());

            IEnumerable<INotification> domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities
                .ToList()
                .ForEach(x => x.Entity.ClearDomainEvents());

            foreach (INotification domainEvent in domainEvents) {
                await mediator.Publish(domainEvent);
            }
        }
    }
}