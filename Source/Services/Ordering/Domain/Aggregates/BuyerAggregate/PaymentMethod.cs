using System;
using Dawn;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate {
    public class PaymentMethod : Entity {
        private const byte MIN_PAYMENT_CARD_NUMBER_LENGTH = 8;
        private const byte MAX_PAYMENT_CARD_NUMBER_LENGTH = 19;

        private const byte MIN_CARD_VERIFICATION_CODE_LENGTH = 3;
        private const byte MAX_CARD_VERIFICATION_CODE_LENGTH = 4;

        private string alias;
        private string paymentCardNumber;
        private string cardHolderName;
        private DateOnly cardExpiration;
        private string cardVerificationCode;
        private int cardTypeID;
        private CardType cardType;

        public PaymentMethod(int cardTypeID, string alias, string paymentCardNumber,
            string cardVerificationCode, string cardHolderName, DateOnly cardExpiration) {
            this.alias = Guard
                .Argument(alias, nameof(alias))
                .NotNull()
                .NotWhiteSpace()
                .Value;
            this.paymentCardNumber = Guard
                .Argument(paymentCardNumber, nameof(paymentCardNumber))
                .NotNull()
                .NotWhiteSpace()
                .LengthInRange(
                    MIN_PAYMENT_CARD_NUMBER_LENGTH,
                    MAX_PAYMENT_CARD_NUMBER_LENGTH)
                .Value;
            this.cardHolderName = Guard
                .Argument(cardHolderName, nameof(cardHolderName))
                .NotNull()
                .NotWhiteSpace()
                .Matches(@"^([A-Za-z]{3,})\s([A-Za-z]{3,})$").Value;
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            this.cardExpiration = Guard
                .Argument(cardExpiration, nameof(cardExpiration))
                .GreaterThan(currentDate);
            this.cardVerificationCode = Guard
                .Argument(cardVerificationCode, nameof(cardVerificationCode))
                .NotNull()
                .NotWhiteSpace()
                .LengthInRange(
                    MIN_CARD_VERIFICATION_CODE_LENGTH,
                    MAX_CARD_VERIFICATION_CODE_LENGTH)
                .Value;
            this.cardTypeID = Guard
                .Argument(cardTypeID, nameof(cardTypeID))
                .IsValidCardTypeID()
                .Value;
        }

        public string Alias {
            get { return this.alias; }
        }

        public string PaymentCardNumber {
            get { return this.paymentCardNumber; }
        }

        public string CardVerificationCode {
            get { return this.cardVerificationCode; }
        }

        public string CardHolderName {
            get { return this.cardHolderName; }
        }

        public DateOnly CardExpiration {
            get { return this.cardExpiration; }
        }

        public int CardTypeID {
            get { return this.cardTypeID; }
        }

        public CardType CardType {
            get { return this.cardType; }
        }

        public bool IsEqualTo(int cardTypeID, string paymentCardNumber, DateOnly cardExpiration) {
            return this.cardTypeID == cardTypeID
                && this.paymentCardNumber == paymentCardNumber
                && this.cardExpiration == cardExpiration;
        }
    }
}