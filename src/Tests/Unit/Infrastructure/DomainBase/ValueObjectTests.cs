using System;
using System.Collections.Generic;
using Infrastructure.DomainBase;
using NUnit.Framework;
using Tests.Unit.Infrastructure.DomainBase.TestClasses;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.DomainBase
{
    public class ValueObjectTests : BaseTestFixture
    {
        [Test]
        public void ValueObjectsWithSameReferenceAreEqual()
        {
            var testValueObject = new TestValueObject { Firstname = "Robin", Lastname = "van der Knaap" };
            var testValueObject2 = testValueObject;

            Assert.AreEqual(testValueObject, testValueObject2);
            Assert.IsTrue(testValueObject == testValueObject2);
            Assert.IsFalse(testValueObject != testValueObject2);

            var valueObjectSet = new HashSet<ValueObject> {testValueObject, testValueObject2};

            Assert.AreEqual(1, valueObjectSet.Count);
        }
        
        [Test]
        public void ValueObjectsAreEqualWhenAllPropertiesHaveSameValue()
        {
            var testValueObject = new TestValueObject { Firstname = "Robin", Lastname = "van der Knaap" };
            var testValueObject2 = new TestValueObject { Firstname = "Robin", Lastname = "van der Knaap" };

            Assert.AreEqual(testValueObject, testValueObject2);
            Assert.IsTrue(testValueObject == testValueObject2);
            Assert.IsFalse(testValueObject != testValueObject2);
            Assert.AreEqual(testValueObject.GetHashCode(), testValueObject2.GetHashCode());

            var valueObjectSet = new HashSet<ValueObject> {testValueObject, testValueObject2};

            Assert.AreEqual(1, valueObjectSet.Count);
        }

        [Test]
        public void ValueObjectsAreNotEqualWhenNotAllPropertiesHaveSameValue()
        {
            var testValueObject = new TestValueObject { Firstname = "Robin", Lastname = "van der Knaap" };
            var testValueObject2 = new TestValueObject { Firstname = "Daan", Lastname = "le Duc" };

            Assert.AreNotEqual(testValueObject, testValueObject2);
            Assert.IsFalse(testValueObject == testValueObject2);
            Assert.IsTrue(testValueObject != testValueObject2);
            Assert.AreNotEqual(testValueObject.GetHashCode(), testValueObject2.GetHashCode());

            var valueObjectSet = new HashSet<ValueObject> {testValueObject, testValueObject2};

            Assert.AreEqual(2, valueObjectSet.Count);
        }

        [Test]
        public void ValueObjectsAreNotAllowedToContainDomainSignatureAttributes()
        {
            var invalidTestValueObject = new InvalidTestValueObject();
            var invalidTestValueObject2 = new InvalidTestValueObject();
            bool dummy;
            Assert.Throws<InvalidOperationException>(() => dummy = invalidTestValueObject == invalidTestValueObject2);
        }

        [Test]
        public void ValueObjectsAreEqualWhenBothAreNull()
        {
            TestValueObject testValueObject1 = null;
            TestValueObject testValueObject2 = null;

            Assert.AreEqual(testValueObject1, testValueObject2);
            Assert.IsTrue(testValueObject1 == testValueObject2);
            Assert.IsFalse(testValueObject1 != testValueObject2);
        }
    }
}
