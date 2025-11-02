using System.Linq;
using Xunit;

namespace eShop.BuildingBlocks.EventBus.UnitTests {
    public class InMemoryEventBusSubscriptionManagerTests {

        [Fact]
        public void AfterCreationShouldBeEmpty() {
            var manager = new InMemoryEventBusSubscriptionManager();
            Assert.True(manager.IsEmpty);
        }

        [Fact]
        public void AfterOneEventSubscriptionShouldContainTheEvent() {
            var manager = new InMemoryEventBusSubscriptionManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(manager.HasSubcriptionsForEvent<TestIntegrationEvent>());
        }

        [Fact]
        public void AfterAllSubscriptionsAreDeletedEventShouldNoLongerExist() {
            var manager = new InMemoryEventBusSubscriptionManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.False(manager.HasSubcriptionsForEvent<TestIntegrationEvent>());
        }

        [Fact]
        public void DeletingLastSubscriptionShouldRaiseOnDeletedEvent() {
            var isRaised = false;
            var manager = new InMemoryEventBusSubscriptionManager();
            manager.OnEventRemoved += (o, e) => isRaised = true;
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(isRaised);
        }

        [Fact]
        public void GetHandlersForEventShouldReturnAllHandlers() {
            var manager = new InMemoryEventBusSubscriptionManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandlerBIS>();
            var handlers = manager.GetHandlersForEvent<TestIntegrationEvent>();
            Assert.Equal(2, handlers.Count());
        }
    }
}