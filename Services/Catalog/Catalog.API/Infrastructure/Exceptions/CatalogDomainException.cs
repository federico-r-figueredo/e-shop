using System;

namespace eShop.Services.Catalog.API.Infrastructure.Exceptions {
    ///<summary>
    /// Exception type for catalog domain exceptions
    /// </summary>
    internal class CatalogDomainException : Exception {
        internal CatalogDomainException() { }
        internal CatalogDomainException(string message) : base(message) { }
        internal CatalogDomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}