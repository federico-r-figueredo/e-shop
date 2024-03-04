
using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate {
    public class CardType : Enumeration {
        public static CardType AmericanExpress = new CardType(1, nameof(AmericanExpress));
        public static CardType Visa = new CardType(2, nameof(Visa));
        public static CardType MasterCard = new CardType(3, nameof(MasterCard));

        public CardType(int id, string name) : base(id, name) { }

        public static IEnumerable<CardType> ToEnumerable() {
            return new[] {
                AmericanExpress,
                Visa,
                MasterCard
            };
        }

        public static CardType FromName(string name) {
            CardType cardType = ToEnumerable().SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            Guard.Argument(cardType, string.Concat(nameof(CardType), ".", nameof(CardType.Name))).NotNull();
            return cardType;
        }

        public static CardType FromID(int id) {
            CardType cardType = ToEnumerable().SingleOrDefault(x => x.ID == id);
            Guard.Argument(cardType, string.Concat(nameof(CardType), ".", nameof(CardType.ID))).NotNull();
            return cardType;
        }
    }
}