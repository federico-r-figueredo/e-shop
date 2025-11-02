using System;

namespace eShop.BuildingBlocks.EventBus {
    public class SubscriptionInfo {
        public bool IsDynamic { get; }
        public Type HandlerType { get; }

        private SubscriptionInfo(bool isDynamic, Type handlerType) {
            this.IsDynamic = isDynamic;
            this.HandlerType = handlerType;
        }

        public static SubscriptionInfo Dynamic(Type handlerType) {
            return new SubscriptionInfo(true, handlerType);
        }

        public static SubscriptionInfo Typed(Type handlerType) {
            return new SubscriptionInfo(false, handlerType);
        }
    }
}