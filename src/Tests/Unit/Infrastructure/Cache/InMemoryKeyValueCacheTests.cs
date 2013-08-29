using System;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using Infrastructure.Cache;
using Infrastructure.ExtensionMethods;
using NUnit.Framework;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.Cache
{
    class InMemoryKeyValueCacheTests : BaseTestFixture
    {
        [SetUp]
        public void SetUp()
        {
            ClearCache();
        }

        [TearDown]
        public void TearDown()
        {
            ClearCache();
        }

        [Test]
        public void CanGetValueFromCache()
        {
            var cache = new InMemoryKeyValueCache();

            cache.Add("test", "some value");
            Assert.AreEqual("some value", cache.Get("test"));
        }

        [Test]
        public void NonExistingKeyReturnsNull()
        {
            var cache = new InMemoryKeyValueCache();
            Assert.IsNull(cache.Get("test"));
        }
        
        [Test]
        public void CanRemoveValueFromCache()
        {
            var cache = new InMemoryKeyValueCache();

            cache.Add("test", "some value");
            Assert.AreEqual("some value", cache.Get("test"));

            cache.Remove("test");
            Assert.IsNull(cache.Get("test"));
        }

        [Test]
        public void CanAddValueToCacheWithExpiration()
        {
            var cache = new InMemoryKeyValueCache();
            var expiration = 1.Seconds();

            cache.Add("test", "some value", DateTime.Now.AddTicks(expiration.Ticks));
            Assert.AreEqual("some value", cache.Get("test"));

            Thread.Sleep(expiration);

            Assert.IsNull(cache.Get("test"));
        }

        [Test]
        public void CannotAddNullValueToCache()
        {
            Assert.Throws<ArgumentNullException>(() => new InMemoryKeyValueCache().Add("test", null));
        }

        [Test]
        public void KeyCannotBeNullOrEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new InMemoryKeyValueCache().Add(null, "some value"));
            Assert.Throws<ArgumentNullException>(() => new InMemoryKeyValueCache().Add(" ", "some value"));
            Assert.Throws<ArgumentNullException>(() => new InMemoryKeyValueCache().Add(string.Empty, "some value"));
        }

        [Test]
        public void CanDisposeCache()
        {
            var cache = new InMemoryKeyValueCache();

            cache.Add("test", "some value");
            Assert.AreEqual("some value", cache.Get("test"));

            cache.Dispose();

            var cacheNew = new InMemoryKeyValueCache();
            Assert.IsNull(cache.Get("test"));
            Assert.IsNull(cacheNew.Get("test"));
        }

        private static void ClearCache()
        {
            var cacheKeys = MemoryCache.Default.Select(cacheItem => cacheItem.Key).ToList();

            foreach (var cacheKey in cacheKeys)
            {
                MemoryCache.Default.Remove(cacheKey);
            }
        }
    }
}
