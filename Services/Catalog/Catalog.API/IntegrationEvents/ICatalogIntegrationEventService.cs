using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Events;

namespace eShop.Services.Catalog.API.IntegrationEvents {
    public interface ICatalogIntegrationEventService {
        Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent integrationEvent);
        Task PublishThroughEventBusAsync(IntegrationEvent integrationEvent);
    }
}