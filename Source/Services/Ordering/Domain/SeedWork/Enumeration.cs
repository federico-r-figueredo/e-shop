
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EShop.Services.Ordering.Domain.SeedWork {
    public class Enumeration : IComparable {
        private readonly int id;
        private readonly string name;

        public Enumeration(int id, string name) {
            this.id = id;
            this.name = name;
        }

        public int ID {
            get { return this.id; }
        }

        public string Name {
            get { return this.name; }
        }

        public override string ToString() {
            return this.name;
        }

        public override bool Equals(object obj) {
            if (!(obj is Enumeration)) {
                return false;
            }

            bool typeMatches = this.GetType().Equals(obj.GetType());
            bool valueMatches = this.id.Equals(((Enumeration)obj).ID);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() {
            return this.id.GetHashCode();
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration {
            IEnumerable<T> enumerations = typeof(T)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Select(field => field.GetValue(null))
                .Cast<T>();

            return enumerations;
        }

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue) {
            int absoluteDifference = Math.Abs(firstValue.id - secondValue.id);
            return absoluteDifference;
        }

        public static T FromValue<T>(int value) where T : Enumeration {
            T matchingItem = Parse<T, int>(value, "value", item => item.id == value);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration {
            T matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null) {
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
            }

            return matchingItem;
        }

        public int CompareTo(object obj) {
            int result = this.id.CompareTo(((Enumeration)obj).id);
            return result;
        }
    }
}