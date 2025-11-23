using System;

namespace eShop.Services.Basket.API.Infrastructure.Exceptions {
    internal class BasketDomainException : Exception {
        internal BasketDomainException() { }
        internal BasketDomainException(string message) : base(message) { }
        internal BasketDomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}