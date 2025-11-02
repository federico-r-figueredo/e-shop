using System.Threading.Tasks;

namespace eShop.BuildingBlocks.EventBus.Abstractions {
    public interface IDynamicIntegrationEventHandler {
        Task Handle(dynamic integrationEvent);
    }
}