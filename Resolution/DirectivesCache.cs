// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectivesCache.cs" company=" Solidsoft Reply Ltd.">
//   Copyright 2015 Solidsoft Reply Limited.
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
    using System.Collections.Specialized;
    using System.Runtime.Caching;

    using SolidsoftReply.Esb.Libraries.Resolution.ResolutionService;

    /// <summary>
    /// DirectivesCache class
    /// </summary>
    internal class DirectivesCache : BaseCache<DirectiveCacheItem>
    {
         /// <summary>
         /// Initializes a new instance of the <see cref="DirectivesCache"/> class. 
         /// </summary>
         public DirectivesCache()
             : base(
                "EsbDirectives",
                new NameValueCollection(3)
                    {
                    { "cacheMemoryLimitMegabytes", "0" },
                    { "physicalMemoryPercentage", "0" },
                    { "pollingInterval", "00:02:00" }
                    })
        {
        }

         /// <summary>
         /// Inserts a cache entry into the Site Location cache, specifying information about
         ///  how the entry will be evicted.
         /// </summary>
         /// <param name="key">
         /// A unique identifier for the cache entry.
         /// </param>
         /// <param name="directives">
         /// The UDDI site location to insert. 
         /// </param>
         /// <param name="policy">
         /// An object that contains eviction details for the cache entry. 
         /// This object provides more options for eviction than a simple absolute expiration.
         /// </param>
         /// <returns>
         /// true if insertion succeeded, or false if there is an already an entry 
         /// in the cache that has the same key as item.
         /// </returns>
         internal bool Add(string key, Directives directives, CacheItemPolicy policy)
         {
             if (policy.SlidingExpiration.Ticks == 0 && policy.AbsoluteExpiration == DateTime.MinValue)
             {
                 return true;
             }

             Func<DirectiveCacheItem> addDirectiveCacheItem =
                 () => new DirectiveCacheItem { KeyName = key, Directives = directives };

             // Add the site location to the cache.
             return this.Add(
                 key,
                 addDirectiveCacheItem,
                 policy);
         }

         /// <summary>
         /// Inserts a cache entry into the Site Location cache, specifying information about
         ///  how the entry will be evicted.
         /// </summary>
         /// <param name="key">
         /// A unique identifier for the cache entry.
         /// </param>
         /// <param name="bamActivityStep">
         /// The BAM activity step insert. 
         /// </param>
         /// <param name="policy">
         /// An object that contains eviction details for the cache entry. 
         /// This object provides more options for eviction than a simple absolute expiration.
         /// </param>
         /// <returns>
         /// true if insertion succeeded, or false if there is an already an entry 
         /// in the cache that has the same key as item.
         /// </returns>
         internal bool Add(string key, BamActivityStep bamActivityStep, CacheItemPolicy policy)
         {
             if (policy.SlidingExpiration.Ticks == 0 && policy.AbsoluteExpiration == DateTime.MinValue)
             {
                 return true;
             }

             Func<DirectiveCacheItem> addDirectiveCacheItem =
                 () => new DirectiveCacheItem { KeyName = key, BamActivityStep = bamActivityStep };

             // Add the site location to the cache.
             return this.Add(
                 key,
                 addDirectiveCacheItem,
                 policy);
         }

        /// <summary>
        /// Get the BAM activity step.
        /// </summary>
        /// <param name="key">
        /// Key of the BAM activity step.
        /// </param>
        /// <returns>
        /// A BamActivityStep object.
        /// </returns>
        internal BamActivityStep GetBamActivityStep(string key)
        {
            return this.GetCacheItem(key).BamActivityStep;
        }

        /// <summary>
        /// Get resolution results from the cache.
        /// </summary>
        /// <param name="key">
        /// A string containing the key to retrieve the element from the cache.
        /// </param>
        /// <returns>
        /// A Directives object containing the directives.
        /// </returns>
        internal Directives GetResolverResults(string key)
        {
            return this.GetCacheItem(key).Directives;
        }
    }
}