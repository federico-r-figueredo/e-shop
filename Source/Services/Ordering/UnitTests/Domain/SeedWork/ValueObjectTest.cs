using System;
using System.Collections.Generic;
using System.Linq;
using EShop.Services.Ordering.Domain.SeedWork;
using NUnit.Framework;

namespace EShop.Services.Ordering.UnitTests.Domain.SeedWork {
    public class ValueObjectTest {

        [Test, TestCaseSource(nameof(EqualValueObjects))]
        public void Equals_EqualValueObjects_ReturnsTrue(ValueObject instanceA, ValueObject instanceB, string reason) {
            // Act
            var result = EqualityComparer<ValueObject>.Default.Equals(instanceA, instanceB);

            // Assert
            Assert.True(result, reason);
        }

        [Test, TestCaseSource(nameof(NonEqualValueObjects))]
        public void Equals_NonEqualValueObjects_ReturnFalse(ValueObject instanceA, ValueObject instanceB, string reason) {
            // Act
            var result = EqualityComparer<ValueObject>.Default.Equals(instanceA, instanceB);

            // Assert
            Assert.False(result, reason);
        }

        private static readonly ValueObject APrettyValueObject = new ValueObjectA(
            1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3")
        );

        public static readonly IEnumerable<TestCaseData> EqualValueObjects = new List<TestCaseData>() {
            new TestCaseData(
                null,
                null,
                "They should be equal because they are both null"
            ),
            new TestCaseData(
                APrettyValueObject,
                APrettyValueObject,
                "They should be equal because they are the same object"
            ),
            new TestCaseData(
                new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3")),
                new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3")),
                "They should be equal because they have equal members"
            ),
            new TestCaseData(
                new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"), notAnEqualityComponent: "xpto"),
                new ValueObjectA(1, "2", Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), new ComplexObject(2, "3"), notAnEqualityComponent: "xpto2"),
                "They should be equal because all equality components are equal, even though an additional member was set"
            ),
            new TestCaseData(
                new ValueObjectB(1, "2", 1, 2, 3 ),
                new ValueObjectB(1, "2", 1, 2, 3 ),
                "They should be equal because all equality components are equal, including the 'C' list"
            )
        };

        public static readonly IEnumerable<TestCaseData> NonEqualValueObjects = new List<TestCaseData>() {
            new TestCaseData(
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                new ValueObjectA(a: 2, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                "They should not be equal because the 'A' member on ValueObjectA is different among them"
            ),
            new TestCaseData(
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                new ValueObjectA(a: 1, b: null, c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                "They should not be equal because the 'B' member on ValueObjectA is different among them"
            ),
            new TestCaseData(
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(3, "3")),
                "They should not be equal because the 'A' member on ValueObjectA's 'D' is different among them"
            ),
            new TestCaseData(
                new ValueObjectA(a: 1, b: "2", c: Guid.Parse("97ea43f0-6fef-4fb7-8c67-9114a7ff6ec0"), d: new ComplexObject(2, "3")),
                new ValueObjectB(a: 1, b: "2"),
                "They should not be equal because they are not the same type"
            ),
            new TestCaseData(
                new ValueObjectB(a: 1, b: "2", 1, 2, 3),
                new ValueObjectB(a: 1, b: "2", 1, 2, 3, 4),
                "They should not be equal because the 'C' list contains one additional value"
            ),
            new TestCaseData(
                new ValueObjectB(a: 1, b: "2", 1, 2, 3, 5),
                new ValueObjectB(a: 1, b: "2", 1, 2, 3),
                "They should not be equal because the 'C' list contains one additional value"
            ),
            new TestCaseData(
                new ValueObjectB(a: 1, b: "2", 1, 2, 3, 5),
                new ValueObjectB(a: 1, b: "2", 1, 2, 3, 4),
                "They should not be equal because the 'C' lists are not equal"
            ),
        };

        private class ValueObjectA : ValueObject {
            private int a;
            private string b;
            private Guid c;
            private ComplexObject d;
            private string notAnEqualityComponent;

            public ValueObjectA(int a, string b, Guid c, ComplexObject d, string notAnEqualityComponent = null) {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
                this.notAnEqualityComponent = notAnEqualityComponent;
            }

            protected override IEnumerable<object> GetEqualityComponents() {
                yield return this.a;
                yield return this.b;
                yield return this.c;
                yield return this.d;
            }
        }

        private class ValueObjectB : ValueObject {
            private readonly int a;
            private readonly string b;
            private readonly List<int> c;

            public ValueObjectB(int a, string b, params int[] c) {
                this.a = a;
                this.b = b;
                this.c = c.ToList();
            }

            protected override IEnumerable<object> GetEqualityComponents() {
                yield return this.a;
                yield return this.b;
                foreach (var item in this.c) {
                    yield return item;
                }
            }
        }

        private class ComplexObject : IEquatable<ComplexObject> {
            private readonly int a;
            private readonly string b;

            public ComplexObject(int a, string b) {
                this.a = a;
                this.b = b;
            }

            public override bool Equals(object obj) {
                return this.Equals(obj as ComplexObject);
            }

            public bool Equals(ComplexObject other) {
                return other != null
                    && this.a == other.a
                    && this.b == other.b;
            }

            public override int GetHashCode() {
                return HashCode.Combine(this.a, this.b);
            }
        }
    }
}