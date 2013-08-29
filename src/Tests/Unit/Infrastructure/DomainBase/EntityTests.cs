using System.Collections.Generic;
using Infrastructure.DomainBase;
using NUnit.Framework;
using Tests.Unit.Infrastructure.DomainBase.TestClasses;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.DomainBase
{
    class EntityTests : BaseTestFixture
    {
        [Test]
        public void TwoEntitiesWithSameReferenceAreEqual()
        {
            var testEntity1 = new TestEntity(0);
            var testEntity2 = testEntity1;

            Assert.AreEqual(testEntity1, testEntity2);
            Assert.IsTrue(testEntity1.Equals(testEntity2));

            var entitySet = new HashSet<Entity>();
            entitySet.Add(testEntity1);
            entitySet.Add(testEntity2);

            Assert.AreEqual(1, entitySet.Count);
        }

        [Test]
        public void TwoEntitiesWithDifferingTypesAreNotEqual()
        {
            var testEntity = new TestEntity(0);
            var otherTestEnity = new OtherTestEntity();

            Assert.AreNotEqual(testEntity, otherTestEnity);
            Assert.IsFalse(testEntity.Equals(otherTestEnity));

            var entitySet = new HashSet<Entity>();
            entitySet.Add(testEntity);
            entitySet.Add(otherTestEnity);

            Assert.AreEqual(2, entitySet.Count);
        }

        [Test]
        public void TwoEntitiesWithSameIdAreEqual()
        {
            var testEntity1 = new TestEntity(1) { Firstname = "Robin", Lastname= "van der Knaap" };
            var testEntity2 = new TestEntity(1) { Firstname = "Daan", Lastname = "le Duc" };

            Assert.AreEqual(testEntity1, testEntity2);

            var entitySet = new HashSet<Entity>();
            entitySet.Add(testEntity1);
            entitySet.Add(testEntity2);

            Assert.AreEqual(1, entitySet.Count);
        }

        [Test]
        public void TwoEntitiesWithDifferentIdsAreNotEqual()
        {
            var testEntity1 = new TestEntity(1) { Firstname = "Robin", Lastname = "van der Knaap" };
            var testEntity2 = new TestEntity(2) { Firstname = "Robin", Lastname = "van der Knaap" };

            Assert.AreNotEqual(testEntity1, testEntity2); 
            Assert.IsFalse(testEntity1.Equals(testEntity2));

            var entitySet = new HashSet<Entity>();
            entitySet.Add(testEntity1);
            entitySet.Add(testEntity2);

            Assert.AreEqual(2, entitySet.Count);
        }

        [Test]
        public void TwoEntitiesWithSameDomainSignatureAreEqualWhenBothTransient()
        {
            var testEntity1 = new TestEntity(0) { Firstname = "Robin", Lastname = "van der Knaap" };
            var testEntity2 = new TestEntity(0) { Firstname = "Robin", Lastname = "van der Knaap" };

            Assert.AreEqual(testEntity1, testEntity2);
            Assert.IsTrue(testEntity1.Equals(testEntity2));

            var entitySet = new HashSet<Entity>();
            entitySet.Add(testEntity1);
            entitySet.Add(testEntity2);

            Assert.AreEqual(1, entitySet.Count);
        }

        [Test]
        public void DomainSignatureCanContainNullValues()
        {
            var testEntity1 = new TestEntity(0) { Firstname = "Robin", Lastname = null };
            var testEntity2 = new TestEntity(0) { Firstname = "Robin", Lastname = null };

            Assert.AreEqual(testEntity1, testEntity2);
            Assert.IsTrue(testEntity1.Equals(testEntity2));

            var entitySet = new HashSet<Entity>();
            entitySet.Add(testEntity1);
            entitySet.Add(testEntity2);

            Assert.AreEqual(1, entitySet.Count);
        }

        [Test]
        public void TwoEntitiesAreNotEqualWhenDomainSignaturesDoNotMatch()
        {
            var testEntity1 = new TestEntity(0) { Firstname = "Robin", Lastname = "van der Knaap" };
            var testEntity2 = new TestEntity(0) { Firstname = "Daan", Lastname = "le Duc" };

            Assert.AreNotEqual(testEntity1, testEntity2);
            Assert.IsFalse(testEntity1.Equals(testEntity2));

            var entitySet = new HashSet<Entity>();
            entitySet.Add(testEntity1);
            entitySet.Add(testEntity2);

            Assert.AreEqual(2, entitySet.Count);
        }

        [Test]
        public void EntitiesAreEqualWhenBothAreNull()
        {
            TestEntity testEntity1 = null;
            TestEntity testEntity2 = null;

            Assert.AreEqual(testEntity1, testEntity2);
        }

        [Test]
        public void TestEntityHashCodeCache()
        {
            var entitySet = new HashSet<Entity>();

            var testEntity = new TestEntity(1) { Firstname = "Robin", Lastname = "van der Knaap" };

            entitySet.Add(testEntity);
            entitySet.Add(testEntity);

            Assert.AreEqual(1, entitySet.Count);
        }
    }
}
