
using System.Threading.Tasks;
using EShop.BuildingBlocks.EventBus.EventBus.Abstractions;

namespace EShop.BuildingBlocks.EventBus.Tests {
    internal class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent> {
        private bool handled;

        public TestIntegrationEventHandler() {
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