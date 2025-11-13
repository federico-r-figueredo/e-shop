using eShop.Services.Ordering.Domain.SeedWork;

namespace eShop.Services.Ordering.Domain.Model.BuyerAggregate {
    /// <remarks>
    /// Card type class should be marked as abstract with protected constructor to 
    /// encapsulat known enum types. This is currently not possible as OrderingContextSeed
    /// uses this constructor to load card types from CSV file.
    /// </remarks>
    public class CardType : Enumeration {
        public static CardType AmericanExpress = new CardType(1, "AmericanExpress");
        public static CardType Visa = new CardType(2, "Visa");
        public static CardType MasterCard = new CardType(3, "MasterCard");

        public CardType(int id, string name) : base(id, name) { }
    }
}