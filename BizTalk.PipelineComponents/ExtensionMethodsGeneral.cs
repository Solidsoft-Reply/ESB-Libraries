// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsGeneral.cs" company="Solidsoft Reply Ltd.">
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

// ReSharper disable UnusedMethodReturnValue.Global

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A library of helper extension methods.
    /// </summary>
    internal static class ExtensionMethodsGeneral
    {
/*
        /// <summary>
        /// Caches a single value.  Use this in lieu of the the ability to declare unbound 
        /// variables in expressions in C#.
        /// </summary>
        /// <typeparam name="TV">The type of the value.</typeparam>
        /// <param name="value">A value.</param>
        /// <returns>A parameterless function that returns a cached value.</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public static Func<TV> AsCachedVar<TV>(this TV value)
        {
            var cachedValue = default(TV);
            var cachedValueSet = false;

            Func<TV> setCachedValue = () =>
            {
                cachedValue = value;
                cachedValueSet = true;
                return cachedValue;
            };

            return () => cachedValueSet ? cachedValue : setCachedValue();
        }
*/

        /// <summary>
        /// Caches a single value returned by a parameterless function.  Use this in lieu 
        /// of the the ability to declare unbound variables in expressions in C#.
        /// </summary>
        /// <typeparam name="TV">The type of the value.</typeparam>
        /// <param name="function">A parameterless function that returns a value.</param>
        /// <returns>A parameterless function that returns a cached value.</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public static Func<TV> AsCachedVar<TV>(this Func<TV> function)
        {
            var cachedValue = default(TV);
            var cachedValueSet = false;

            Func<TV> setCachedValue = () =>
                {
                    cachedValue = function();
                    cachedValueSet = true;
                    return cachedValue;
                };

            return () => cachedValueSet ? cachedValue : setCachedValue();
        }
    }
}
