using System;
using System.Collections.Generic;
using EShop.Services.Ordering.Domain.Aggregates.BuyerAggregate;
using EShop.Services.Ordering.Domain.Exceptions;
using NUnit.Framework;

namespace EShop.Services.Ordering.UnitTests.Domain {
    internal class BuyerAggregateTest {

        #region Buyer

        [Test]
        public void CreateBuyer_WithValidArguments_ShouldReturnNotNullResult() {
            // Arrange
            string identity = new Guid().ToString();
            string name = "fakeUser";

            // Act
            Buyer buyer = new Buyer(identity, name);

            // Assert
            Assert.NotNull(buyer);
        }

        [Test]
        public void CreateBuyer_WhenIdentityIsNull_ShouldThrowArgumentNullException() {
            // Arrange
            string identity = null;
            string name = "fakeUser";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Buyer(identity, name));
        }

        [Test]
        public void CreateBuyer_WhenIdentityIsWhiteSpace_ShouldThrowArgumentException() {
            // Arrange
            string identity = string.Empty;
            string name = "fakeUser";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Buyer(identity, name));
        }

        [Test]
        public void CreateBuyer_WhenNameIsNull_ShouldThrowArgumentNullException() {
            // Arrange
            string identity = new Guid().ToString();
            string name = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Buyer(identity, name));
        }

        [Test]
        public void CreateBuyer_WhenNameIsWhiteSpace_ShouldThrowArgumentException() {
            // Arrange
            string identity = new Guid().ToString();
            string name = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Buyer(identity, name));
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenArgumentsAreValid_ShouldReturnNotNullResult() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act
            PaymentMethod result = sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            );

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenArgumentsAreValid_ShouldRaiseADomainEvent() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act
            sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            );

            // Assert
            Assert.AreEqual(1, sut.DomainEvents.Count);
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenAliasIsNull_ThrowsArgumentNullException() {
            // Arrange
            int cardTypeID = 1;
            string alias = null;
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(alias)} cannot be null");
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenAliasIsAnEmptyString_ThrowsArgumentException() {
            // Arrange
            int cardTypeID = 1;
            string alias = string.Empty;
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(alias)} cannot be empty or consist only of white-space characters");
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenPaymentCardNumberIsNull_ThrowsArgumentNullException() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = null;
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} cannot be null");
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenPaymentCardNumberIsAnEmptyString_ThrowsArgumentException() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = string.Empty;
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} cannot be empty or consist only of white-space characters");
        }

        [Test, TestCaseSource(nameof(PaymentCardNumberBelowMinLengthTestCaseData))]
        public void VerifyOrAddPaymentMethod_WhenPaymentCardNumberIsBelowMinAllowedLength_ThrowsArgumentException(string paymentCardNumberValue) {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = paymentCardNumberValue;
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} must contain 8 to 19 characters");
        }

        private static IEnumerable<TestCaseData> PaymentCardNumberBelowMinLengthTestCaseData {
            get {
                yield return new TestCaseData("1");
                yield return new TestCaseData("12");
                yield return new TestCaseData("123");
                yield return new TestCaseData("1234");
                yield return new TestCaseData("12345");
                yield return new TestCaseData("123456");
                yield return new TestCaseData("1234567");
            }
        }

        [Test, TestCaseSource(nameof(PaymentCardNumberAboveMaxLengthTestCaseSource))]
        public void VerifyOrAddPaymentMethod_WhenPaymentCardNumberIsAboveMaxAllowedLength_ThrowsArgumentException(string paymentCardNumberValue) {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = paymentCardNumberValue;
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} must contain 8 to 19 characters");
        }

        private static IEnumerable<TestCaseData> PaymentCardNumberAboveMaxLengthTestCaseSource {
            get {
                yield return new TestCaseData("12345678912345678912");
                yield return new TestCaseData("123456789123456789123");
                yield return new TestCaseData("1234567891234567891234");
                yield return new TestCaseData("12345678912345678912345");
                yield return new TestCaseData("123456789123456789123456");
                yield return new TestCaseData("1234567891234567891234567");
                yield return new TestCaseData("12345678912345678912345678");
                yield return new TestCaseData("123456789123456789123456789");
            }
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenCardHolderNameIsNull_ThrowsArgumentException() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = null;
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(cardHolderName)} cannot be null");
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenCardHolderNameIsAnEmptyString_ThrowsArgumentException() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = string.Empty;
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(cardHolderName)} cannot be empty or consist only of white-space characters");
        }

        [Test, TestCaseSource(nameof(CardHolderNameNotMatchingRegexPatternTestCaseSource))]
        public void VerifyOrAddPaymentMethod_WhenCardHolderNameDoesNotMatchRegexPattern_ThrowsArgumentException(string cardHolderNameValue) {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = cardHolderNameValue;
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} must contain 8 to 19 characters");
        }

        private static IEnumerable<TestCaseData> CardHolderNameNotMatchingRegexPatternTestCaseSource {
            get {
                yield return new TestCaseData("a");
                yield return new TestCaseData("a a");
                yield return new TestCaseData("aa a");
                yield return new TestCaseData("a aa");
                yield return new TestCaseData("aa aa");
                yield return new TestCaseData("aaa aa");
                yield return new TestCaseData("aa aaa");
            }
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenCardExpirationIsLessOrEqualThanCurrentDate_ThrowsArgumentException() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardcardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(-1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardcardExpiration,
                orderID: orderID
            ), $"Invalid {nameof(cardcardExpiration)}'s value. {nameof(cardcardExpiration)} can't be lesser than current date-time");
        }


        [Test]
        public void VerifyOrAddPaymentMethod_WhenCardVerificationCodeIsNull_ThrowsArgumentNullException() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = null;
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            string cardVerificationCode = null;
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} cannot be null");
        }

        [Test]
        public void VerifyOrAddPaymentMethod_WhenCardVerificationCodeIsAnEmptyString_ThrowsArgumentException() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = string.Empty;
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} cannot be empty or consist only of white-space characters");
        }

        [Test, TestCaseSource(nameof(CardVerificationCodeBelowMinLengthTestCaseData))]
        public void VerifyOrAddPaymentMethod_WithCardVerificationCodeBelowMinLength_ThrowsArgumentException(string cardVerificationCodeValue) {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = cardVerificationCodeValue;
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} must contain 3 to 4 characters");
        }

        private static IEnumerable<TestCaseData> CardVerificationCodeBelowMinLengthTestCaseData {
            get {
                yield return new TestCaseData("");
                yield return new TestCaseData("1");
                yield return new TestCaseData("12");
            }
        }

        [Test, TestCaseSource(nameof(CardVerificationCodeAboveMaxLengthTestCaseSource))]
        public void VerifyOrAddPaymentMethod_WithCardVerificationCodeAboveMaxLength_ThrowsArgumentException(string cardVerificationCodeValue) {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = cardVerificationCodeValue;
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"{nameof(paymentCardNumber)} must contain 3 to 4 characters");
        }

        private static IEnumerable<TestCaseData> CardVerificationCodeAboveMaxLengthTestCaseSource {
            get {
                yield return new TestCaseData("12345");
                yield return new TestCaseData("123456");
                yield return new TestCaseData("1234567");
            }
        }

        [Test, TestCaseSource(nameof(InvalidCardTypeIDTestCaseSource))]
        public void VerifyOrAddPaymentMethod_WithInvalidCardTypeID_ThrowsArgumentException(int cardTypeIDValue) {
            // Arrange
            int cardTypeID = cardTypeIDValue;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            int orderID = 1;
            string name = "fakeUser";
            string identity = new Guid().ToString();
            Buyer sut = new Buyer(identity, name);

            // Act & Assert
            Assert.Throws<OrderingDomainException>(() => sut.VerifyOrAddPaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration,
                orderID: orderID
            ), $"Invalid {cardTypeID}'s value. Possible values: 1,2,3");
        }

        private static IEnumerable<TestCaseData> InvalidCardTypeIDTestCaseSource {
            get {
                yield return new TestCaseData(0);
                yield return new TestCaseData(4);
                yield return new TestCaseData(5);
                yield return new TestCaseData(6);
            }
        }

        #endregion

        #region PaymentMethod

        [Test]
        public void CreatePaymentMethod_WithValidArguments_ShouldReturnNotNullResult() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            string identity = new Guid().ToString();

            // Act
            PaymentMethod result = new PaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration
            );

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void IsEqualTo_GivenSameArgumentsUsedToCreateInstance_ShouldBeTrue() {
            // Arrange
            int cardTypeID = 1;
            string alias = "fakeAlias";
            string paymentCardNumber = "12345678";
            string cardVerificationCode = "123";
            string cardHolderName = "John Doe";
            DateOnly cardExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
            string identity = new Guid().ToString();

            // Act
            PaymentMethod paymentMethod = new PaymentMethod(
                cardTypeID: cardTypeID,
                alias: alias,
                paymentCardNumber: paymentCardNumber,
                cardVerificationCode: cardVerificationCode,
                cardHolderName: cardHolderName,
                cardExpiration: cardExpiration
            );
            bool result = paymentMethod.IsEqualTo(cardTypeID, paymentCardNumber, cardExpiration);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region CardType

        [Test, TestCaseSource(nameof(FromNameTestDataSource))]
        public void FromName_WhenGivenAValidName_ItReturnsAValidCardType(CardType cardType) {
            // Arrange
            CardType expected = cardType;

            // Act
            CardType actual = CardType.FromName(expected.Name);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        public void FromName_WhenGivenAnInvalidName_ItThrowsNullArgumentException() {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CardType.FromName("aaa"));

        }

        [Test, TestCaseSource(nameof(FromNameTestDataSource))]
        public void FromName_WhenGivenAValidID_ItReturnsAValidCardType(CardType cardType) {
            // Arrange
            CardType expected = cardType;

            // Act
            CardType actual = CardType.FromID(expected.ID);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private static IEnumerable<TestCaseData> FromNameTestDataSource {
            get {
                yield return new TestCaseData(CardType.AmericanExpress);
                yield return new TestCaseData(CardType.Visa);
                yield return new TestCaseData(CardType.MasterCard);
            }
        }

        public void FromName_WhenGivenAnInvalidID_ItThrowsNullArgumentException() {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CardType.FromID(99));
        }

        #endregion
    }
}