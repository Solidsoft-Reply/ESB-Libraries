// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionHelpers.cs" company="Solidsoft Reply Ltd.">
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
    using System.Runtime.Caching;

    /// <summary>
    /// A library of helper extension methods.
    /// </summary>
    internal static class ExtensionHelpers
    {
        /// <summary>
        /// Returns a schema strong name that has been cached.
        /// </summary>
        /// <param name="schemaStrongName">
        /// The schema strong name to be cached.
        /// </param>
        /// <param name="schemaName">
        /// The schema name, used as a key in the cache.
        /// </param>
        /// <returns>
        /// The cached strong name.
        /// </returns>
        public static string CachedAsSchemaStrongName(this string schemaStrongName, string schemaName)
        {
            Transformer.SchemaStrongNameCache.Add(
                schemaName,
                schemaStrongName,
                new CacheItemPolicy { SlidingExpiration = TimeSpan.FromDays(1) });
            return schemaStrongName;
         }
    }
}
