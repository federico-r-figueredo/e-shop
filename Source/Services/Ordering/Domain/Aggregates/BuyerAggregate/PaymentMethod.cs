
using System;
using Dawn;
using EShop.Services.Ordering.Domain.SeedWork;

namespace EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate {
    internal class PaymentMethod : Entity {
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

        protected PaymentMethod() { }

        public PaymentMethod(int cardTypeID, string alias, string paymentCardNumber,
            string cardHolderName, string cardVerificationCode, DateOnly cardExpiration) {
            this.Alias = alias;
            this.PaymentCardNumber = paymentCardNumber;
            this.CardHolderName = cardHolderName;
            this.CardExpiration = cardExpiration;
            this.CardVerificationCode = cardVerificationCode;
            this.CardTypeID = cardTypeID;
        }

        public string Alias {
            get { return this.alias; }
            private set {
                this.alias = Guard
                    .Argument(() => alias)
                    .NotNull()
                    .NotWhiteSpace()
                    .Value;
            }
        }

        public string PaymentCardNumber {
            get { return this.paymentCardNumber; }
            private set {
                this.paymentCardNumber = Guard
                    .Argument(value, nameof(PaymentCardNumber))
                    .NotNull()
                    .NotWhiteSpace()
                    .LengthInRange(
                        MIN_PAYMENT_CARD_NUMBER_LENGTH,
                        MAX_PAYMENT_CARD_NUMBER_LENGTH)
                    .Value;
            }
        }

        public string CardVerificationCode {
            get { return this.cardVerificationCode; }
            private set {
                this.cardVerificationCode = Guard
                .Argument(value, nameof(CardVerificationCode))
                .NotNull()
                .NotWhiteSpace()
                .LengthInRange(
                    MIN_CARD_VERIFICATION_CODE_LENGTH,
                    MAX_CARD_VERIFICATION_CODE_LENGTH)
                .Value;
            }
        }

        public string CardHolderName {
            get { return this.cardHolderName; }
            private set {
                this.cardHolderName = Guard
                    .Argument(value, nameof(CardHolderName))
                    .NotNull()
                    .NotWhiteSpace()
                    .Matches(@"/^([A-Za-z]{3, })\s([A-Za-z]{3, })$/@").Value;
            }
        }

        public DateOnly CardExpiration {
            get { return this.cardExpiration; }
            private set {
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                this.cardExpiration = Guard
                    .Argument(value, nameof(CardExpiration))
                    .GreaterThan(currentDate);
            }
        }

        public int CardTypeID {
            get { return this.cardTypeID; }
            private set {
                this.cardTypeID = Guard
                    .Argument(value, nameof(CardTypeID))
                    .IsValidCardTypeID()
                    .Value;

            }
        }

        public CardType CardType {
            get { return this.cardType; }
            private set { this.cardType = CardType.FromName(value.Name); }
        }

        public bool IsEqualTo(int cardTypeID, string cardNumber, DateOnly expiration) {
            return this.cardTypeID == cardTypeID
                && this.paymentCardNumber == cardNumber
                && this.cardExpiration == expiration;
        }
    }
}