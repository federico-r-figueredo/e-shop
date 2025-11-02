using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;

namespace eShop.BuildingBlocks.EventBus.UnitTests {
    internal class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent> {

        public TestIntegrationEventHandler() {
            this.IsHandled = false;
        }

        public bool IsHandled { get; set; }

        public async Task Handle(TestIntegrationEvent integrationEvent) {
            this.IsHandled = true;
        }
    }
}