
using System.Collections.Generic;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.OrderAggregate {
    internal class Address : ValueObject {
        private string street;
        private string city;
        private string state;
        private string country;
        private string zipCode;

        protected Address() { }

        public Address(string street, string city, string state, string country, string zipCode) {
            this.Street = street;
            this.City = city;
            this.State = state;
            this.Country = country;
            this.ZipCode = zipCode;
        }

        public string Street {
            get { return this.street; }
            private set { this.street = value; }
        }

        public string City {
            get { return this.city; }
            private set { this.city = value; }
        }

        public string State {
            get { return this.state; }
            private set { this.state = value; }
        }

        public string Country {
            get { return this.country; }
            private set { this.country = value; }
        }

        public string ZipCode {
            get { return this.zipCode; }
            private set { this.zipCode = value; }
        }

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return this.Street;
            yield return this.City;
            yield return this.State;
            yield return this.Country;
            yield return this.ZipCode;
        }
    }
}