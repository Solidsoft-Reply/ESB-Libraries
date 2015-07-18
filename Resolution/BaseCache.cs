// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseCache.cs" company=" Solidsoft Reply Ltd.">
//   Copyright (c) 2015 Solidsoft Reply Limited. All rights reserved.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Caching;

    using SolidsoftReply.Esb.Libraries.Resolution.Properties;

    /// <summary>
    /// Base class for caches.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the cache item.
    /// </typeparam>
    internal abstract class BaseCache<T> : IEnumerable<KeyValuePair<string, T>> where T : class
    {
        /// <summary>
        /// The memory cache for schema strong names.
        /// </summary>
        private readonly MemoryCache cache;

        /// <summary>
        /// Lock object for accessing the memory cache.
        /// </summary>
        private readonly object cacheLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCache{T}"/> class. 
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="cacheSettings">
        /// The cache Settings.
        /// </param>
        protected BaseCache(string name, NameValueCollection cacheSettings)
        {
            // Create the cacheLock for guarding cache integrity.
            this.cacheLock = new object();

            // Create the cache using the given settings.
            this.cache = new MemoryCache(name, cacheSettings);
        }

        /// <summary>
        /// Creates an enumerator that can be used to iterate through a collection of cache entries.
        /// </summary>
        /// <returns>
        /// The enumerator object that provides access to the cache entries in the cache.
        /// </returns>
        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            // Protect the integrity of the cache.
            lock (this.cacheLock)
            {
                // Create a collection of cache key/directives pairs and return the enumerator.
                return this.cache.Select(
                    keyValuePair => // Create a key/directives pair entry for the collection:
                    new KeyValuePair<string, T>(keyValuePair.Key, (T)keyValuePair.Value))
                    .GetEnumerator();
            }
        }

        /// <summary>
        /// Creates an enumerator that can be used to iterate through a collection of cache entries.
        /// </summary>
        /// <returns>The enumerator object that provides access to the items in the cache.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Protect the integrity of the cache.
            lock (this.cacheLock)
            {
                // Get the enumerator for the cache.
                return ((IEnumerable)this.cache).GetEnumerator();
            }
        }

        /// <summary>
        /// Determines whether a cache entry exists in the Site Location cache.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the cache entry to search for.
        /// </param>
        /// <returns>
        /// true if the cache contains a cache entry whose key matches key; otherwise, false.
        /// </returns>
        internal bool Contains(string key)
        {
            // Protect the integrity of the cache.
            lock (this.cacheLock)
            {
                // Determine if the cache contains the site location.
                return this.cache.Contains(key);
            }
        }

        /// <summary>
        ///     Invalidate the cache.
        /// </summary>
        internal void InvalidateCache()
        {
            lock (this.cacheLock)
            {
                foreach (var item in this.cache)
                {
                    this.cache.Remove(item.Key);
                }
            }
        }

        /// <summary>
        /// Inserts a cache entry into the Site Location cache, specifying information about
        ///  how the entry will be evicted.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the cache entry.
        /// </param>
        /// <param name="cacheItem">
        /// A function that returns the item to be cached. 
        /// </param>
        /// <param name="policy">
        /// An object that contains eviction details for the cache entry. 
        /// This object provides more options for eviction than a simple absolute expiration.
        /// </param>
        /// <returns>
        /// true if insertion succeeded, or false if there is an already an entry 
        /// in the cache that has the same key as item.
        /// </returns>
        protected bool Add(string key, Func<T> cacheItem, CacheItemPolicy policy)
        {
            if (policy.SlidingExpiration.Ticks == 0 && policy.AbsoluteExpiration == DateTime.MinValue)
            {
                return true;
            }

            // Protect the integrity of the cache.
            lock (this.cacheLock)
            {
                // Add the site location to the cache.
                return this.cache.Add(
                    key,
                    cacheItem(),
                    policy);
            }
        }
        
        /// <summary>
        /// Get the item in the cache.
        /// </summary>
        /// <param name="key">
        /// Key for the item.
        /// </param>
        /// <returns>
        /// The item cached.
        /// </returns>
        protected T GetCacheItem(string key)
        {
            T item;

            lock (this.cacheLock)
            {
                item = (T)this.cache[key];
            }

            if (item == null)
            {
                throw new EsbResolutionException(Resources.ExceptionKeyNotFound);
            }

            return item;
        }
    }
}