using System;

namespace EShop.BuildingBlocks.EventBus.EventBus {
    public partial class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionsManager {
        public class SubscriptionInfo {
            private readonly bool isDynamic;
            private readonly Type handlerType;

            private SubscriptionInfo(bool isDynamic, Type handlerType) {
                this.isDynamic = isDynamic;
                this.handlerType = handlerType;
            }

            public bool IsDynamic {
                get { return this.isDynamic; }
            }

            public Type HandlerType {
                get { return this.handlerType; }
            }

            public static SubscriptionInfo Dynamic(Type handlerType) {
                return new SubscriptionInfo(true, handlerType);
            }

            public static SubscriptionInfo Typed(Type handlerType) {
                return new SubscriptionInfo(false, handlerType);
            }
        }
    }
}