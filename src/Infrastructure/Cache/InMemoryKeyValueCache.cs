using System;
using System.Runtime.Caching;

namespace Infrastructure.Cache
{
    /// <summary>
    /// In memory key value cache provider
    /// </summary>
    /// http://msdn.microsoft.com/en-us/library/system.runtime.caching.memorycache.aspx
    public class InMemoryKeyValueCache : IKeyValueCache, IDisposable
    {
        private readonly MemoryCache _cacheObject = new MemoryCache("InMemoryKeyValueCacheProvider");

        public dynamic Get(string key)
        {
            dynamic value = null;

            if (_cacheObject.Contains(key))
            {
                value = _cacheObject.Get(key);
            }

            return value;
        }

        public void Add(string key, dynamic value)
        {
            Add(key, value, DateTime.MaxValue);
        }

        public void Add(string key, dynamic value, DateTimeOffset expireOffset)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            Remove(key);
        
            _cacheObject.Add(key, value, expireOffset);
        }

        public void Remove(string key)
        {
            if (_cacheObject.Contains(key))
            {
                _cacheObject.Remove(key);
            }
        }

        public void Dispose()
        {
            _cacheObject.Dispose();
        }
    }
}
