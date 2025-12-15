using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace eShop.Services.Ordering.Domain.SeedWork {
    public class Enumeration : IComparable {
        private int id;
        private string name;

        protected Enumeration(int id, string name) {
            this.id = id;
            this.name = name;
        }

        public int ID {
            get { return this.id; }
            // This private setter is required so EF Core design time tools won't fail with
            // "No backing field could be found for property '<EnumerationChild>.ID' and 
            // the property does not have a setter".
            private set { this.id = value; }
        }

        public string Name {
            get { return this.name; }
        }

        public override string ToString() {
            return this.name;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration {
            FieldInfo[] fields = typeof(T).GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly
            );

            return fields.Select(x => x.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj) {
            Enumeration other = (Enumeration)obj;

            if (other == null) return false;

            bool typeMatches = this.GetType().Equals(obj.GetType());
            bool valueMatches = this.id.Equals(other.id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() {
            return this.id.GetHashCode();
        }

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue) {
            return Math.Abs(firstValue.id - secondValue.id);
        }

        public static T FromValue<T>(int value) where T : Enumeration {
            return Parse<T, int>(value, "value", x => x.id == value);
        }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration {
            return Parse<T, string>(displayName, "display name", x => x.name == displayName);
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate)
            where T : Enumeration {
            T matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null) throw new InvalidOperationException(
                $"'{value}' is not valid {description} in {typeof(T)}"
            );

            return matchingItem;
        }

        public int CompareTo(object obj) {
            return this.id.CompareTo(((Enumeration)obj).id);
        }
    }
}