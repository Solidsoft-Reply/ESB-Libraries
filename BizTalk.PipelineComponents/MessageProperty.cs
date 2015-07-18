// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProperty.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    /// <summary>
    /// Descriptor for a pipeline message property.
    /// </summary>
    internal struct MessageProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProperty"/> struct.
        /// </summary>
        /// <param name="name">
        /// The property name.
        /// </param>
        /// <param name="namespace">
        /// The property namespace.
        /// </param>
        /// <param name="value">
        /// The property value.
        /// </param>
        /// <param name="isPromoted">
        /// Indicates if the the property is promoted.
        /// </param>
        public MessageProperty(string name, string @namespace, object value, bool isPromoted)
            : this()
        {
            this.Name = name;
            this.NameSpace = @namespace;
            this.Value = value;
            this.IsPromoted = isPromoted;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the namespace of the property.
        /// </summary>
        public string NameSpace { get; private set; }

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the property is promoted.
        /// </summary>
        public bool IsPromoted { get; private set; }
    }
}
