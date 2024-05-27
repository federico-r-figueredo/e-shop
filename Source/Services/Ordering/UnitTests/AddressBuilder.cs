using EShop.Services.Ordering.Domain.Aggregates.OrderAggregate;

namespace EShop.Services.Ordering.UnitTests {
    internal class AddressBuilder {
        internal Address Build() {
            return new Address("street", "city", "state", "country", "zipcode");
        }
    }
}