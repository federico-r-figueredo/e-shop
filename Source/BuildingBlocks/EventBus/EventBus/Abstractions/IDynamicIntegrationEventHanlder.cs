
using System.Threading.Tasks;

namespace EShop.BuildingBlocks.EventBus.EventBus.Abstractions {
    public interface IDynamicIntegrationEventHandler {
        Task Handle(dynamic eventData);
    }
}