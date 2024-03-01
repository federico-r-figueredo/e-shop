
using System;

namespace EShop.Services.Ordering.Domain.Events {
    internal abstract class DomainEvent {
        protected static T GuardAgainstNull<T>(T obj) {
            return obj != null ? obj : throw new ArgumentNullException(nameof(T));
        }
    }
}