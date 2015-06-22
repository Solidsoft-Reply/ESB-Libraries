// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryBase.cs" company="Solidsoft Reply Ltd.">
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
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using AssemblyProperties = SolidsoftReply.Esb.Libraries.Resolution.Properties;

    /// <summary>
    /// Xml Serialisable dictionary base class.   Inherits from the serialisable
    /// generic dictionary.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values stored in the dictionary
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here."),]
    [XmlSchemaProvider("GetDictionarySchema")]
    [XmlRoot("DictionaryBase", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", IsNullable = true)]
    [Serializable]
    public class DictionaryBase<T> : XmlSerializableDictionary<string, T>
    {
        /// <summary>
        /// The XML schema for the dictionary.
        /// </summary>
        private XmlSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBase{T}"/> class. 
        /// </summary>
        public DictionaryBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBase{T}"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        public DictionaryBase(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBase{T}"/> class.
        /// </summary>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public DictionaryBase(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBase{T}"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public DictionaryBase(int capacity, IEqualityComparer<string> comparer)
            : base(capacity, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBase{T}"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public DictionaryBase(IDictionary<string, T> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBase{T}"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public DictionaryBase(IDictionary<string, T> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBase{T}"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected DictionaryBase(SerializationInfo info, StreamingContext context)
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
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the schema for the current dictionary.
        /// </summary>
        /// <returns>The XSD schema for the current dictionary.</returns>
        public override XmlSchema GetSchema()
        {
            return this.schema ?? (this.schema = XmlSerializableDictionary<string, T>.GetSchema(AssemblyProperties.Resources.XsdDictionarySchemaFile));
        }

        /// <summary>
        /// Returns an XSD schema for the serialisable facts dictionary.  This is referenced by the XmlSchemaProvider
        /// attribute on this class in order control the XML format. 
        /// </summary>
        /// <param name="schemaSet">A cache of XSD schemas.</param>
        /// <param name="schemaName">The name of the schema.</param>
        /// <param name="schemaNamespace">The namespace of the schema.</param>
        /// <param name="schemaFilename">The schema filename.</param>
        /// <returns>The qualified XML name of of the FactsDictionary type.</returns>
        protected internal static XmlQualifiedName GetDictionarySchema(XmlSchemaSet schemaSet, string schemaName, string schemaNamespace, string schemaFilename)
        {
            if (schemaSet == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    AssemblyProperties.Resources.ExceptionSchemaSetIsNull,
                    "GetDictionarySchema");
                var innerException = new ArgumentNullException(
                    "schemaSet",
                    AssemblyProperties.Resources.ExceptionValueIsNull);

                throw new EsbResolutionException(message, innerException);
            }

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(schemaFilename);

            if (stream == null)
            {
                throw new EsbResolutionException(AssemblyProperties.Resources.ExceptionSchemaResourceNotFound);
            }

            var xsdReader = new XmlTextReader(stream);
            var xs = XmlSchema.Read(xsdReader, null);
            schemaSet.XmlResolver = new XmlUrlResolver();
            schemaSet.Add(xs);

            return new XmlQualifiedName(schemaName, schemaNamespace);
        }

        /// <summary>
        /// Reads a key value as a string.
        /// </summary>
        /// <param name="reader">An XML reader containing the serialized key.</param>
        /// <param name="dictionaryType">The type of dictionary.</param>
        /// <returns>A string key value.</returns>
        protected internal string ReadKey(XmlReader reader, string dictionaryType)
        {
            if (!reader.ReadToFollowing("string", string.Empty))
            {
                throw new EsbResolutionException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        AssemblyProperties.Resources.ExceptionDeserializationInvalidKeyElement,
                        dictionaryType));
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            var key = (string)reader.ReadElementContentAs(typeof(string), null);
            return key;
        }

        /// <summary>
        /// Writes a key value as a string.
        /// </summary>
        /// <param name="writer">An XML writer for the serializable dictionary.</param>
        /// <param name="key">The key value to be serialized.</param>
        /// <param name="dictionaryType">The type of dictionary.</param>
        /// <param name="dictionaryNamespace">The namespace of the dictionary.</param>
        protected internal void WriteKey(XmlWriter writer, string key, string dictionaryType, string dictionaryNamespace)
        {
            try
            {
                var keySerializer = new XmlSerializer(typeof(string));
                var xsns = new XmlSerializerNamespaces();
                xsns.Add(string.Empty, dictionaryNamespace);
                keySerializer.Serialize(writer, key, xsns);
            }
            catch (Exception ex)
            {
                throw new EsbResolutionException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        AssemblyProperties.Resources.ExceptionSerialization,
                        dictionaryType),
                    ex);
            }
        }
    }
}
