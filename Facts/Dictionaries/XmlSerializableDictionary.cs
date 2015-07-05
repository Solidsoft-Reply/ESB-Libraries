//----------------------------------------------------------------------------
// <copyright file="XmlSerializableDictionary.cs" company="Solidsoft Reply Ltd.">
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
//----------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Facts.Dictionaries
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using AssemblyProperties = SolidsoftReply.Esb.Libraries.Facts.Properties;

    /// <summary>
    /// Xml-serializable generic dictionary.   Inherits from the generic dictionary provided by .NET.
    /// </summary>
    /// <typeparam name="TKey">Type of key value.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    [Serializable]
    [XmlSchemaProvider("GetDictionarySchema")]
    [XmlRoot("Dictionary", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", IsNullable = true)]
    public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        /// <summary>
        /// The XML schema for the dictionary.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static XmlSchema schema;

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionary class.
        /// </summary>
        public XmlSerializableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionary class.
        /// </summary>
        /// <param name="capacity">Initial capacity of dictionary.</param>
        public XmlSerializableDictionary(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionary class.
        /// </summary>
        /// <param name="comparer">Equality comparer.</param>
        public XmlSerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionary class.
        /// </summary>
        /// <param name="capacity">Capacity of the dictionary.</param>
        /// <param name="comparer">Equality comparer.</param>
        public XmlSerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionary class.
        /// </summary>
        /// <param name="dictionary">Copies dictionary to new dictionary.</param>
        public XmlSerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionary class.
        /// </summary>
        /// <param name="dictionary">Copies dictionary to new dictionary.</param>
        /// <param name="comparer">Equality comparer.</param>
        public XmlSerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionary class.
        /// </summary>
        /// <param name="info">serialization information.</param>
        /// <param name="context">serialization context.</param>
        protected XmlSerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Returns an XSD schema for the serializable dictionary.  This is referenced by the XmlSchemaProvider
        /// attribute on this class in order control the XML format. 
        /// </summary>
        /// <param name="schemaSet">A cache of XSD schemas.</param>
        /// <returns>The qualified XML name of the Dictionary type.</returns>
        public static XmlQualifiedName GetDictionarySchema(XmlSchemaSet schemaSet)
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

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            var xs = schema ?? (schema = GetSchema(AssemblyProperties.Resources.XsdDictionarySchemaFile) ?? new XmlSchema());

            if (xs == null)
            {
                return new XmlQualifiedName("DictionaryType", AssemblyProperties.Resources.DictionaryNamespace);
            }

            schemaSet.XmlResolver = new XmlUrlResolver();
            schemaSet.Add(xs);

            return new XmlQualifiedName("DictionaryType", AssemblyProperties.Resources.DictionaryNamespace);
        }

        /// <summary>
        /// Returns the schema for the current dictionary.
        /// </summary>
        /// <returns>The XSD schema for the current dictionary.</returns>
        public virtual XmlSchema GetSchema()
        {
            return schema ?? (schema = GetSchema(AssemblyProperties.Resources.XsdDictionarySchemaFile) ?? new XmlSchema());
        }

        /// <summary>
        /// Deserializes the XML representation of the dictionary.
        /// </summary>
        /// <param name="reader">An XML Reader used for deserialization.</param>
        public void ReadXml(XmlReader reader)
        {
            // Grab the content
            var xmlContent = reader.ReadOuterXml();

            // ReSharper disable once PossibleNullReferenceException
            var targetNamespace = this.GetSchema() == null ? string.Empty : this.GetSchema().TargetNamespace;

            var contentReader = new StringReader(xmlContent);
            var nodeReader = new XmlTextReader(contentReader);

            try
            {
                if (nodeReader.NodeType == XmlNodeType.None)
                {
                    nodeReader.Read();

                    if (nodeReader.NodeType == XmlNodeType.None)
                    {
                        throw new XmlSerializableDictionaryException("The reader contains invalid XML.");
                    }

                    if (nodeReader.IsEmptyElement)
                    {
                        // The following line was removed - July 2015.  No exception is necessary here!
                        // throw new XmlSerializableDictionaryException("The reader contains no XML.");
                        return;
                    }
                }

                while (true)
                {
                    if (nodeReader.LocalName != "Item")
                    {
                        if (!nodeReader.ReadToFollowing("Item", targetNamespace))
                        {
                            break;
                        }
                    }

                    nodeReader.ReadStartElement("Item", targetNamespace);

                    if (nodeReader.LocalName != "Key")
                    {
                        if (!nodeReader.ReadToFollowing("Key", targetNamespace))
                        {
                            throw new XmlSerializableDictionaryException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    AssemblyProperties.Resources.ExceptionDeserializationNoElement,
                                    this.GetType().Name,
                                    "Key"));
                        }
                    }

                    var key = default(TKey);
                    var keyInitialised = false;
                    var innerKeyXml = nodeReader.ReadInnerXml().Trim();

                    if (!string.IsNullOrWhiteSpace(innerKeyXml))
                    {
                        // Copy the key content to another XmlReader to protect main reader.
                        using (var sr = new StringReader(innerKeyXml))
                        {
                            var keyReader = XmlReader.Create(sr);
                            key = this.ReadKey(keyReader);
                            keyInitialised = true;
                        }
                    }

                    if (nodeReader.LocalName != "Value")
                    {
                        if (!nodeReader.ReadToFollowing("Value", targetNamespace))
                        {
                            throw new XmlSerializableDictionaryException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    AssemblyProperties.Resources.ExceptionDeserializationNoElement,
                                    this.GetType().Name,
                                    "Value"));
                        }
                    }

                    var value = default(TValue);
                    var innerValueXml = nodeReader.ReadInnerXml().Trim();

                    Func<TKey> throwExceptionForInvalidKey = () =>
                        {
                            throw new EsbFactsException("The key value is invalid.");
                        };

                    Action<TValue> addEntry = v =>
                        this.Add(keyInitialised ? key : throwExceptionForInvalidKey(), v);

                    if (!string.IsNullOrWhiteSpace(innerValueXml))
                    {
                        // Copy the key content to another XmlReader to protect main reader.
                        using (var sr = new StringReader(innerValueXml))
                        {
                            var valueReader = XmlReader.Create(sr);
                            value = this.ReadValue(valueReader);
                        }
                    }

                    addEntry(value);

                    if (nodeReader.LocalName == "Item")
                    {
                        nodeReader.ReadEndElement();
                    }
                }
            }
            catch (XmlSerializableDictionaryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new XmlSerializableDictionaryException(
                   string.Format(
                       CultureInfo.CurrentCulture,
                       AssemblyProperties.Resources.ExceptionUnexpectedError,
                       "ReadXml"),
                   ex);
            }
        }

        /// <summary>
        /// Serializes the dictionary as XML.
        /// </summary>
        /// <param name="writer">An XML writer used for serialization.</param>
        /// <remarks>
        /// This code is generic.  Override ReadKey, ReadValue, WriteKey 
        /// and/or WriteValue on derived dictionary classes to control serialization.
        /// </remarks>
        public void WriteXml(XmlWriter writer)
        {
            try
            {
                foreach (var key in this.Keys)
                {
                    writer.WriteStartElement("Item", AssemblyProperties.Resources.DictionaryNamespace);

                    using (var ms = new MemoryStream())
                    {
                        writer.WriteStartElement("Key");
                        var encoding = Encoding.UTF8;

                        if (writer.Settings != null)
                        {
                            encoding = writer.Settings.Encoding;
                        }

                        var settings = new XmlWriterSettings
                        {
                            OmitXmlDeclaration = true,
                            Encoding = encoding
                        };
                        var keyWriter = XmlWriter.Create(ms, settings);

                        this.WriteKey(keyWriter, key);
                        keyWriter.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        var sr = new StreamReader(ms);
                        writer.WriteRaw(sr.ReadToEnd());
                        writer.WriteEndElement();
                    }

                    using (var ms = new MemoryStream())
                    {
                        writer.WriteStartElement("Value");

                        var settings = new XmlWriterSettings
                        {
                            OmitXmlDeclaration = true
                        };
                        var valueWriter = XmlWriter.Create(ms, settings);

                        this.WriteValue(valueWriter, this[key]);
                        valueWriter.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        var sr = new StreamReader(ms);
                        writer.WriteRaw(sr.ReadToEnd());
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
            }
            catch (Exception ex)
            {
                throw new XmlSerializableDictionaryException(
                   string.Format(
                       CultureInfo.CurrentCulture,
                       AssemblyProperties.Resources.ExceptionUnexpectedError,
                       "WriteXml"),
                   ex);
            }
        }

        /// <summary>
        /// Returns an XSD schema for the serializable dictionary.  This is referenced by the XmlSchemaProvider
        /// attribute on this class in order control the XML format. 
        /// </summary>
        /// <param name="xsdSchemaFile">XSD schema file resource string</param>
        /// <returns>The XML schema of the Dictionary type.</returns>
        internal static XmlSchema GetSchema(string xsdSchemaFile)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(xsdSchemaFile);

            if (stream == null)
            {
                return null;
            }

            var xsdReader = new XmlTextReader(stream);
            return XmlSchema.Read(xsdReader, null);
        }
        
        /// <summary>
        /// Reads a key value as an arbitrary object type.
        /// </summary>
        /// <param name="reader">An XML reader containing the serialized key.</param>
        /// <returns>A key value of arbitrary type.</returns>
        protected virtual TKey ReadKey(XmlReader reader)
        {
            if (reader == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    AssemblyProperties.Resources.ExceptionReaderIsNull,
                    "ReadKey");
                var innerException = new ArgumentNullException(
                    "reader",
                    AssemblyProperties.Resources.ExceptionValueIsNull);

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            // The NetDataContractSerializer class is an XML serializer that adds/reads 
            // additional type information, allowing arbitrary types to be serialised.
            var keySerializer = new NetDataContractSerializer();

            while (reader.LocalName == string.Empty)
            {
                try
                {
                    reader.Read();
                }
                catch (XmlException)
                {
                    // If we never find an element with a local name, this indicates that there is no content,
                    // so catch and consume the error and return a default key.
                    return default(TKey);
                }
            }

            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms);
                sw.Write(reader.ReadOuterXml());
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                return (TKey)keySerializer.Deserialize(ms);
            }
        }

        /// <summary>
        /// Reads a key value as an arbitrary object type.
        /// </summary>
        /// <param name="reader">An XML reader containing the serialized value.</param>
        /// <returns>A key value of arbitrary type.</returns>
        protected virtual TValue ReadValue(XmlReader reader)
        {
            if (reader == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    AssemblyProperties.Resources.ExceptionReaderIsNull,
                    "ReadValue");
                var innerException = new ArgumentNullException(
                    "reader",
                    AssemblyProperties.Resources.ExceptionValueIsNull);

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            // The NetDataContractSerializer class is an XML serializer that adds/reads 
            // additional type information, allowing arbitrary types to be serialised.
            var valueSerializer = new NetDataContractSerializer();

            while (reader.LocalName == string.Empty)
            {
                try
                {
                    reader.Read();
                }
                catch (XmlException)
                {
                    // If we never find an element with a local name, this indicates that there is no content,
                    // so catch and consume the error and return a default value.
                    return default(TValue);
                }
            }

            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms);
                sw.Write(reader.ReadOuterXml());
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                return (TValue)valueSerializer.Deserialize(ms);
            }
        }

        /// <summary>
        /// Writes a key value of arbitrary type to the serializable dictionary.
        /// </summary>
        /// <param name="writer">An XML writer for the serializable dictionary.</param>
        /// <param name="key">The key value to be serialized.</param>
        protected virtual void WriteKey(XmlWriter writer, TKey key)
        {
            if (writer == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    AssemblyProperties.Resources.ExceptionWriterIsNull,
                    "WriteKey");
                var innerException = new ArgumentNullException(
                    "writer",
                    AssemblyProperties.Resources.ExceptionValueIsNull);

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            var keySerializer = new NetDataContractSerializer();

            using (var ms = new MemoryStream())
            {
                keySerializer.Serialize(ms, key);
                ms.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(ms);
                writer.WriteRaw(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// Writes a value of arbitrary type to the serializable dictionary.
        /// </summary>
        /// <param name="writer">An XML writer for the serializable dictionary.</param>
        /// <param name="value">The value to be serialized.</param>
        protected virtual void WriteValue(XmlWriter writer, TValue value)
        {
            if (writer == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    AssemblyProperties.Resources.ExceptionWriterIsNull,
                    "WriteValue");
                var innerException = new ArgumentNullException(
                    "writer",
                    AssemblyProperties.Resources.ExceptionValueIsNull);

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            var valueSerializer = new NetDataContractSerializer();

            using (var ms = new MemoryStream())
            {
                valueSerializer.Serialize(ms, value);
                ms.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(ms);
                writer.WriteRaw(sr.ReadToEnd());
            }
        }
    }
}
