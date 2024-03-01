
using System;
using System.Linq;
using Dawn;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using EShop.Services.Ordering.Domain.Exceptions;

namespace EShop.Services.Ordering.Domain {
    internal static class GuardExtensions {
        public static ref readonly Guard.ArgumentInfo<CardType> NotNull(in this Guard.ArgumentInfo<CardType> argument) {
            if (argument.Value == null) {
                if (argument.Name.Equals(nameof(CardType.Name), StringComparison.InvariantCultureIgnoreCase)) {
                    throw Guard.Fail(new OrderingDomainException(GetInvalidCardTypeName(argument)));
                } else if (argument.Name == nameof(CardType.ID)) {
                    throw Guard.Fail(new OrderingDomainException(GetInvalidCardTypeIDMessage(argument)));
                } else {
                    throw Guard.Fail(new OrderingDomainException(string.Concat(GetInvalidCardTypeIDMessage(argument), GetInvalidCardTypeName(argument))));
                }
            }

            return ref argument;
        }

        private static string GetInvalidCardTypeIDMessage(Guard.ArgumentInfo<CardType> argument) {
            return $"Invalid {nameof(argument.Name)}'s value. Possible values: {String.Join(",", CardType.ToEnumerable().Select(x => x.ID))}";
        }

        private static string GetInvalidCardTypeName(Guard.ArgumentInfo<CardType> argument) {
            return $"Invalid {nameof(argument.Name)}. Possible values: {String.Join(",", CardType.ToEnumerable().Select(x => x.Name))}.";
        }

        public static ref readonly Guard.ArgumentInfo<int> IsValidCardTypeID(in this Guard.ArgumentInfo<int> argument) {
            int id = argument.Value;
            if (!CardType.ToEnumerable().Any(x => x.ID == id)) {
                throw Guard.Fail(new OrderingDomainException($"Invalid {nameof(argument.Name)}'s value. Possible values: {String.Join(",", CardType.ToEnumerable().Select(x => x.ID))}"));
            }

            return ref argument;
        }

        public static ref readonly Guard.ArgumentInfo<DateOnly> GreaterThan(in this Guard.ArgumentInfo<DateOnly> argument, DateOnly dateTime) {
            if (argument.Value < dateTime) {
                throw Guard.Fail(new OrderingDomainException($"Invalid {nameof(argument.Name)}'s value. {argument.Name} can't be lesser than current date-time (${dateTime})"));
            }

            return ref argument;
        }
    }
}