using System.Collections.Generic;
using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.OrderAggregate {
    public class Address : ValueObject {
        private string street;
        private string city;
        private string state;
        private string country;
        private string zipCode;

        public Address() { }

        public Address(string street, string city, string state, string country,
            string zipCode) {
            this.street = street;
            this.city = city;
            this.state = state;
            this.country = country;
            this.zipCode = zipCode;
        }

        protected override IEnumerable<object> GetEqualityComponents() {
            // Using a yield return statement to return each element one at a time
            yield return this.street;
            yield return this.city;
            yield return this.state;
            yield return this.country;
            yield return this.zipCode;
        }
    }
}