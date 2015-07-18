// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParametersFactory.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.BizTalk.Orchestration
{
    using System;
    using System.Collections;

    using SolidsoftReply.Esb.Libraries.Resolution.Dictionaries;

    /// <summary>
    /// Helper class.  Use to create Resolver parameters in orchestration code.
    /// </summary>
    [Serializable]
    public class ParametersFactory
    {
        /// <summary>
        /// The internal parameters dictionary.
        /// </summary>
        private readonly Parameters parameters = new Parameters();

        /// <summary>
        /// Gets a parameters collection that can be passed to the Resolver.
        /// </summary>
        public Parameters Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the parameters dictionary.
        /// </summary>
        public ICollection Keys
        {
            get
            {
                return this.parameters.Keys;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the parameters dictionary.
        /// </summary>
        public ICollection Values
        {
            get
            {
                return this.parameters.Values;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the parameters dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                return this.parameters.Count;
            }
        }

        /// <summary>
        /// Create a new instance of a Parameters Factory.
        /// </summary>
        /// <returns>A new instance of a Parameters Factory</returns>
        public static ParametersFactory Create()
        {
            return new ParametersFactory();
        }

        /// <summary>
        /// Implicit conversion operator from parameters factory to parameters dictionary.
        /// </summary>
        /// <param name="parametersFactory">
        /// The parameters factory to be converted to a parameters dictionary.
        /// </param>
        /// <returns>A parameters dictionary.</returns>
        public static implicit operator Parameters(ParametersFactory parametersFactory)
        {
            return parametersFactory.Parameters;
        }
        
        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add. The value can be null for reference types.
        /// </param>
        public void Add(string key, object value)
        {
            this.Parameters.Add(key, value);
        }

        /// <summary>
        /// Removes all keys and values from the parameters dictionary.
        /// </summary>
        public void Clear()
        {
            this.parameters.Clear();
        }

        /// <summary>
        /// Returns the value associated with the specified key
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, 
        /// the method throws a KeyNotFoundException
        /// </returns>
        public object GetValue(string key)
        {
            return this.parameters[key];
        }

        /// <summary>
        /// Determines whether the parameters dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the parameters dictionary.</param>
        /// <returns>
        /// True if the parameters dictionary contains an element with the specified key; 
        /// otherwise, false.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return this.parameters.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the parameters dictionary contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the parameters dictionary. The value can be null 
        /// for reference types.
        /// </param>
        /// <returns>
        /// True if the parameters dictionary contains an element with the specified value; 
        /// otherwise, false.
        /// </returns>
        public bool ContainsValue(object value)
        {
            return this.parameters.ContainsValue(value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the parameters dictionary.
        /// </summary>
        /// <returns>An enumerator that iterates through the parameters dictionary.</returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        /// <summary>
        /// Removes the value with the specified key from the parameters dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public void Remove(string key)
        {
            this.parameters.Remove(key);
        }
    }
}
