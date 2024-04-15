
using EShop.BuildingBlocks.EventBus.EventBus;
using Moq;
using NUnit.Framework;

namespace EShop.BuildingBlocks.EventBus.Tests {
    [TestFixture]
    internal class InMemoryEventBusSubscriptionManagerTests {
        InMemoryEventBusSubscriptionManager manager;

        [SetUp]
        public void SetUp() {
            Mock<InMemoryEventBusSubscriptionManager> mock = new Mock<InMemoryEventBusSubscriptionManager>();
            this.manager = mock.Object;
        }

        [Test]
        public void AfterCreation_ShouldBeEmpty() {
            // Assert
            Assert.True(this.manager.IsEmpty);
        }

        [Test]
        public void AfterOneEventSubscription_ShouldContainTheEvent() {
            // Act
            this.manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

            // Assert
            Assert.True(this.manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        [Test]
        public void AfterAllSubscriptionsAreDeleted_EventShouldNoLongerExists() {
            // Act
            this.manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            this.manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

            // Assert
            Assert.False(this.manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        [Test]
        public void DeletingLastSubscription_ShouldRaiseOnDeletedEvent() {
            // Arrance
            bool raised = false;
            this.manager.OnEventRemoved += (o, e) => raised = true;

            // Act
            this.manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            this.manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

            // Assert
            Assert.True(raised);
        }
    }
}