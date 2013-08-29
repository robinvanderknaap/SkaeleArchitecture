using System;

namespace Infrastructure.Cache
{
    /// <summary>
    /// Key value cache provider
    /// </summary>
    public interface IKeyValueCache
    {
        /// <summary>
        /// Try to get value from cache
        /// </summary>
        /// <param name="key">Key from cache</param>
        /// <returns>If retrieving value was successfull</returns>
        dynamic Get(string key);

        /// <summary>
        /// Set value
        /// </summary>
        /// <param name="key">Key for in cache</param>
        /// <param name="value">Value for in cache</param>
        void Add(string key, dynamic value);

        /// <summary>
        /// Set value
        /// </summary>
        /// <param name="key">Key for in cache</param>
        /// <param name="value">Value for in cache</param>
        /// <param name="expireOffset">Offset from when the item will expire</param>
        void Add(string key, dynamic value, DateTimeOffset expireOffset);

        /// <summary>
        /// Removes value
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
