// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Parameters.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Resolution.Dictionaries
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using AssemblyProperties = SolidsoftReply.Esb.Libraries.Resolution.Properties;

    /// <summary>
    /// Xml Serialisable dictionary for parameters.   Inherits from the serialisable
    /// generic dictionary.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [XmlSchemaProvider("GetDictionarySchema")]
    [XmlRoot("Parameters", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", IsNullable = true)]
    [Serializable]
    public class Parameters : DictionaryBase<object>
    {
        /// <summary>
        /// The XML schema for the dictionary.
        /// </summary>
        private XmlSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        public Parameters()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        public Parameters(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public Parameters(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public Parameters(int capacity, IEqualityComparer<string> comparer)
            : base(capacity, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public Parameters(IDictionary<string, object> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public Parameters(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected Parameters(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The SerializationInfo to populate with data. </param>
        /// <param name="context">The destination (see StreamingContext) for this serialization. </param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the schema for the current dictionary.
        /// </summary>
        /// <returns>The XSD schema for the current dictionary.</returns>
        public override XmlSchema GetSchema()
        {
            return this.schema ?? (this.schema = GetSchema(AssemblyProperties.Resources.XsdParameterSchemaFile));
        }

        /// <summary>
        /// Returns an XSD schema for the serialisable facts dictionary.  This is referenced by the XmlSchemaProvider
        /// attribute on this class in order control the XML format. 
        /// </summary>
        /// <param name="schemaSet">A cache of XSD schemas.</param>
        /// <returns>The qualified XML name of of the FactsDictionary type.</returns>
        public static new XmlQualifiedName GetDictionarySchema(XmlSchemaSet schemaSet)
        {
            return GetDictionarySchema(
                schemaSet,
                "ParametersType",
                AssemblyProperties.Resources.DictionaryNamespace,
                AssemblyProperties.Resources.XsdParameterSchemaFile);
        }

        /// <summary>
        /// Reads a key value as a string.
        /// </summary>
        /// <param name="reader">An XML reader containing the serialized key.</param>
        /// <returns>A string key value.</returns>
        protected override string ReadKey(XmlReader reader)
        {
            return this.ReadKey(reader, "Parameters");
        }

        /// <summary>
        /// Writes a key value as a string.
        /// </summary>
        /// <param name="writer">An XML writer for the serializable dictionary.</param>
        /// <param name="key">The key value to be serialized.</param>
        protected override void WriteKey(XmlWriter writer, string key)
        {
            this.WriteKey(writer, key, "Parameters", AssemblyProperties.Resources.DictionaryNamespace);
        }
    }
}
