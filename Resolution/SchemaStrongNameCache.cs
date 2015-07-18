// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaStrongNameCache.cs" company=" Solidsoft Reply Ltd.">
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
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;

    /// <summary>
    /// Schema strong name cache class
    /// </summary>
    [Serializable]
    internal class SchemaStrongNameCache : BaseCache<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaStrongNameCache"/> class. 
        /// </summary>
        public SchemaStrongNameCache()
            : base(
            "EsbSchemaStrongNames",
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
        /// <param name="schemaReference">
        /// A unique identifier for the cache entry.
        /// </param>
        ///  <param name="schemaStrongName">
        /// The schema strong name to be cached 
        /// </param>
        /// <param name="policy">
        /// An object that contains eviction details for the cache entry. 
        /// This object provides more options for eviction than a simple absolute expiration.
        /// </param>
        /// <returns>
        /// true if insertion succeeded, or false if there is an already an entry 
        /// in the cache that has the same key as item.
        /// </returns>
        internal bool Add(string schemaReference, string schemaStrongName, CacheItemPolicy policy)
        {
            if (policy.SlidingExpiration.Ticks == 0 && policy.AbsoluteExpiration == DateTime.MinValue)
            {
                return true;
            }

            Func<string> addDirectiveCacheItem =
                () => schemaStrongName;

            // Add the site location to the cache.
            return this.Add(
                schemaReference,
                addDirectiveCacheItem,
                policy);
        }

        /// <summary>
        /// Returns the strong name for a target schema for a map type.
        /// </summary>
        /// <param name="mapType">The map type for which the strong name of a target schema is required.</param>
        /// <param name="schemaReference">The reference name of the target schema.</param>
        /// <returns>The strong name for a target schema for a map type.</returns>
        internal string GetSchemaStrongName(Type mapType, string schemaReference)
        {
            return this.FirstOrDefault(pair => pair.Key == schemaReference).Value
                   ?? StrongName(mapType, ((Type)SchemaAttributeArg(mapType, schemaReference).Value).FullName)
                          .CachedAsSchemaStrongName(schemaReference);
        }

        /// <summary>
        /// Returns a schema attribute argument representing a schema reference.
        /// </summary>
        /// <param name="mapType">The type representing a map.</param>
        /// <param name="schemaReference">The schema reference.</param>
        /// <returns>A schema attribute argument representing a schema name.</returns>
        private static CustomAttributeTypedArgument SchemaAttributeArg(MemberInfo mapType, string schemaReference)
        {
            return (from attribute in mapType.CustomAttributes
                    where
                        attribute.AttributeType.Name == "SchemaReferenceAttribute"
                        && attribute.ConstructorArguments[0].Value.ToString() == schemaReference
                    select attribute.ConstructorArguments[1]).FirstOrDefault();
        }

        /// <summary>
        /// Returns a schema strong name.
        /// </summary>
        /// <param name="mapType">The type representing a map.</param>
        /// <param name="schemaReference">The schema reference.</param>
        /// <returns>A schema strong name.</returns>
        private static string StrongName(Type mapType, string schemaReference)
        {
            // [var] A .NET type in the map assembly that matches the schema name.
            var matchingType =
                (from type in mapType.Assembly.DefinedTypes where type.FullName == schemaReference select type)
                    .FirstOrDefault();

            // If a matching .NET type was found in the map assembly, return it.
            if (matchingType != null)
            {
                return matchingType.AssemblyQualifiedName;
            }

            // [var] A collection of assembly names for assemblies referenced by the map assembly.
            var assemblyNames = from referencedAssemblyname in mapType.Assembly.GetReferencedAssemblies()
                                select referencedAssemblyname.FullName;

            // Search through the referenced assemblies to find the first .NET type that matches the schema name.
            foreach (var assemblyName in assemblyNames)
            {
                try
                {
                    // search for a matching type.
                    matchingType =
                        (from type in Assembly.Load(assemblyName).DefinedTypes
                         where type.FullName == schemaReference
                         select type).FirstOrDefault();

                    // If a matching .NET type was found in the assembly, return it.
                    if (matchingType != null)
                    {
                        return matchingType.AssemblyQualifiedName;
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // Do nothing here 
                }
            }

            // Return an empty string if no match was found for the schema name.
            return string.Empty;
        }
    }
}