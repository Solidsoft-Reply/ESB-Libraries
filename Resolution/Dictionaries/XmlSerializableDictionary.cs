//----------------------------------------------------------------------------
// <copyright file="XmlSerializableDictionary.cs" company="Solidsoft Reply Ltd">
//   © 2015 Solidsoft Reply Ltd
//   This software is released under the Apache 2 License.
//   The license can be found in the file LICENSE at the root 
//   directory of the distribution.
// </copyright>
//----------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Resolution.Dictionaries
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
        private XmlSchema schema;

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
                    Properties.Resources.ExceptionSchemaSetIsNull,
                    "GetDictionarySchema");
                var innerException = new ArgumentNullException(
                    "schemaSet",
                    Properties.Resources.ExceptionValueIsNull);

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            var xs = DoGetSchema();

            if (xs == null)
            {
                return new XmlQualifiedName("DictionaryType", Properties.Resources.DictionaryNamespace);
            }

            schemaSet.XmlResolver = new XmlUrlResolver();
            schemaSet.Add(xs);

            return new XmlQualifiedName("DictionaryType", Properties.Resources.DictionaryNamespace);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>Always returns null.</returns>
        public XmlSchema GetSchema()
        {
            return this.schema ?? (this.schema = DoGetSchema());
        }

        /// <summary>
        /// Deserializes the XML representation of the dictionary.
        /// </summary>
        /// <param name="reader">An XML Reader used for deserialization.</param>
        public void ReadXml(XmlReader reader)
        {
            // Grab the content
            var xmlContent = reader.ReadOuterXml();
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
                        throw new XmlSerializableDictionaryException("The reader contains no XML.");
                    }
                }

                while (true)
                {
                    if (nodeReader.LocalName != "Item")
                    {
                        if (!nodeReader.ReadToFollowing("Item"))
                        {
                            break;
                        }
                    }

                    nodeReader.ReadStartElement("Item");

                    if (nodeReader.LocalName != "Key")
                    {
                        if (!nodeReader.ReadToFollowing("Key"))
                        {
                            throw new XmlSerializableDictionaryException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resources.ExceptionDeserializationNoElement,
                                    this.GetType().Name,
                                    "Key"));
                        }
                    }

                    nodeReader.ReadStartElement("Key");
                    TKey key;

                    // Copy the key content to another XmlReader to protect main reader.
                    using (var sr = new StringReader(nodeReader.ReadOuterXml()))
                    {
                        var keyReader = XmlReader.Create(sr);
                        key = this.ReadKey(keyReader);
                    }

                    if (nodeReader.LocalName != "Value")
                    {
                        if (!nodeReader.ReadToFollowing("Value"))
                        {
                            throw new XmlSerializableDictionaryException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resources.ExceptionDeserializationNoElement,
                                    this.GetType().Name,
                                    "Value"));
                        }
                    }

                    nodeReader.ReadStartElement("Value");
                    TValue value;

                    // Copy the key content to another XmlReader to protect main reader.
                    using (var sr = new StringReader(nodeReader.ReadOuterXml()))
                    {
                        var valueReader = XmlReader.Create(sr);
                        value = this.ReadValue(valueReader);
                    }

                    this.Add(key, value);

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
                       Properties.Resources.ExceptionUnexpectedError,
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
                    writer.WriteStartElement("Item", Properties.Resources.DictionaryNamespace);

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
                       Properties.Resources.ExceptionUnexpectedError,
                       "WriteXml"),
                   ex);
            }
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
                    Properties.Resources.ExceptionReaderIsNull,
                    "ReadKey");
                var innerException = new ArgumentNullException(
                    "reader",
                    Properties.Resources.ExceptionValueIsNull);

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            // The NetDataContractSerializer class is an XML serializer that adds/reads 
            // additional type information, allowing arbitrary types to be serialised.
            var keySerializer = new NetDataContractSerializer();
            reader.Read();

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
                    Properties.Resources.ExceptionReaderIsNull,
                    "ReadValue");
                var innerException = new ArgumentNullException(
                    "reader",
                    Properties.Resources.ExceptionValueIsNull);

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            // The NetDataContractSerializer class is an XML serializer that adds/reads 
            // additional type information, allowing arbitrary types to be serialised.
            var valueSerializer = new NetDataContractSerializer();
            reader.Read();

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
                    Properties.Resources.ExceptionWriterIsNull,
                    "WriteKey");
                var innerException = new ArgumentNullException(
                    "writer",
                    Properties.Resources.ExceptionValueIsNull);

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
                    Properties.Resources.ExceptionWriterIsNull,
                    "WriteValue");
                var innerException = new ArgumentNullException(
                    "writer",
                    Properties.Resources.ExceptionValueIsNull);

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

        /// <summary>
        /// Returns an XSD schema for the serializable dictionary.  This is referenced by the XmlSchemaProvider
        /// attribute on this class in order control the XML format. 
        /// </summary>
        /// <returns>The XML schema of the Dictionary type.</returns>
        private static XmlSchema DoGetSchema()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(Properties.Resources.XsdDictionarySchemaFile);

            if (stream == null)
            {
                return null;
            }

            var xsdReader = new XmlTextReader(stream);
            return XmlSchema.Read(xsdReader, null);
        }
    }
}
