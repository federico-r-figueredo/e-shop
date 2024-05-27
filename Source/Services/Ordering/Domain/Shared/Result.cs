using System;

namespace EShop.Services.Ordering.Domain.Shared {
    public class Result {
        private bool isSuccess;
        private Error error;

        protected internal Result(bool isSuccess, Error error) {
            if (this.isSuccess && this.error != Error.None) {
                throw new InvalidOperationException();
            }

            if (!isSuccess && error == Error.None) {
                throw new InvalidOperationException();
            }

            this.isSuccess = isSuccess;
            this.error = error;
        }

        public bool IsSuccess {
            get { return this.isSuccess; }
        }

        public bool IsFailure {
            get { return !this.isSuccess; }
        }

        public Error Error {
            get { return this.error; }
        }

        public static Result Success() {
            return new Result(true, Error.None);
        }

        public static Result<TValue> Success<TValue>(TValue value) {
            return new Result<TValue>(value, true, Error.None);
        }

        public static Result Failure(Error error) {
            return new Result(false, error);
        }

        public static Result<TValue> Failure<TValue>(Error error) {
            return new Result<TValue>(default, false, error);
        }

        public static Result<TValue> Create<TValue>(TValue value) {
            return value != null
                ? Result.Success(value) : Result.Failure<TValue>(Error.NullValue);
        }
    }

    public class Result<TValue> : Result {
        private readonly TValue value;

        protected internal Result(TValue value, bool isSuccess, Error error) : base(isSuccess, error) {
            this.value = value;
        }

        public TValue Value {
            get {
                return base.IsSuccess
                ? this.value
                : throw new InvalidOperationException("The value of a failure result can't be accesed.");
            }
        }
    }
}