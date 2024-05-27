
using System;

namespace EShop.Services.Ordering.Domain.Shared {
    public class Error : IEquatable<Error> {
        private readonly string code;
        private readonly string message;

        public static readonly Error None = new Error(string.Empty, string.Empty);
        public static readonly Error NullValue = new Error("Error.NullValue", "The specified result value is null");

        public Error(string code, string message) {
            this.code = code;
            this.message = message;
        }

        public string Code {
            get { return this.code; }
        }

        public string Message {
            get { return this.message; }
        }

        public static implicit operator string(Error error) => error.Code;

        public static bool operator ==(Error a, Error b) {
            if (a is null && b is null) {
                return true;
            }

            if (a is null || b is null) {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Error a, Error b) {
            return !(a == b);
        }

        public virtual bool Equals(Error other) {
            if (other is null) {
                return false;
            }

            return this.code == other.code && this.message == other.message;
        }

        public override bool Equals(object obj) {
            return obj is Error error && this.Equals(error);
        }

        public override int GetHashCode() {
            return HashCode.Combine(this.code, this.message);
        }

        public override string ToString() {
            return this.code;
        }
    }
}