
using System.Collections.Generic;
using System.Linq;

namespace EShop.Services.Ordering.Domain.SeedWork {
    public abstract class ValueObject {
        protected static bool EqualOperator(ValueObject left, ValueObject right) {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null)) {
                return false;
            }

            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right) {
            return !EqualOperator(left, right);
        }

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != this.GetType()) {
                return false;
            }

            ValueObject other = (ValueObject)obj;
            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode() {
            int hashCode = this
                .GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);

            return hashCode;
        }

        public ValueObject GetCopy() {
            return (ValueObject)this.MemberwiseClone();
        }
    }
}