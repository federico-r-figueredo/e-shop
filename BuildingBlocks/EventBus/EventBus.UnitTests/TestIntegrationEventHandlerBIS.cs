using System.Threading.Tasks;
using eShop.BuildingBlocks.EventBus.Abstractions;

namespace eShop.BuildingBlocks.EventBus.UnitTests {
    internal class TestIntegrationEventHandlerBIS : IIntegrationEventHandler<TestIntegrationEvent> {

        public TestIntegrationEventHandlerBIS() {
            this.IsHandled = false;
        }

        public bool IsHandled { get; set; }

        public async Task Handle(TestIntegrationEvent integrationEvent) {
            this.IsHandled = true;
        }
    }
}