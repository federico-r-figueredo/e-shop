
using System.Threading.Tasks;
using EShop.BuildingBlocks.EventBus.EventBus.Abstractions;
using EShop.BuildingBlocks.EventBus.Tests;

namespace EShop.BuildingBlocks.EventBus.Tests {
    internal class TestIntegrationOtherEventHandler : IIntegrationEventHandler<TestIntegrationEvent> {
        private bool handled;

        public TestIntegrationOtherEventHandler() {
            this.handled = false;
        }

        public Task Handle(TestIntegrationEvent integrationEvent) {
            this.handled = true;
            return Task.CompletedTask;
        }

        public bool Handled {
            get { return this.handled; }
        }
    }
}