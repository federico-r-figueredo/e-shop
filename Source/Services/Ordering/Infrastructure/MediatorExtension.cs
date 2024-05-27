
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Services.Ordering.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EShop.Services.Ordering.Infrastructure {
    static class MediatorExtension {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderingContext context) {
            IEnumerable<EntityEntry<Entity>> domainEntities = context
                .ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            List<INotification> domainEvents = domainEntities
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