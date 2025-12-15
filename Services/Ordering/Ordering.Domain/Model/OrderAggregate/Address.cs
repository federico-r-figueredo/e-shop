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

        // The only reason why these properties has been added is that EF Core, by default, 
        // won't create the table columns for an owned property private fields unless they 
        // have have public access properties.
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
            // Using a yield return statement to return each element one at a time
            yield return this.street;
            yield return this.city;
            yield return this.state;
            yield return this.country;
            yield return this.zipCode;
        }
    }
}