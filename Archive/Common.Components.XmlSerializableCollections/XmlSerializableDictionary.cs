//----------------------------------------------------------------------------
// <copyright file="XmlSerializableDictionary.cs" company="Solidsoft Ltd">
//      (c) Solidsoft Ltd
// </copyright>
// <summary>This file contains the XmlSerializableDictionary class.</summary>
//----------------------------------------------------------------------------

namespace Solidsoft.Common.Components.XmlSerializableCollections
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
    [XmlRoot("Dictionary", Namespace = "http://Solidsoft.dwp.gov.uk/Common/XmlSerialisableDictionary", IsNullable = true)]
    public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
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
        /// <returns>The qualified XML name of of the Dictionary type.</returns>
        public static XmlQualifiedName GetDictionarySchema(XmlSchemaSet schemaSet)
        {
            if (schemaSet == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Properties.Resources.ExSchemaSetIsNull,
                    "GetDictionarySchema");
                var innerException = new ArgumentNullException(
                    "schemaSet",
                    Properties.Resources.ExValueNotNull);

                throw new XmlSerializableDictionaryException(message, innerException);
            }

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(Properties.Resources.FactsDictionarySchema);

            if (stream != null)
            {
                var xsdReader = new XmlTextReader(stream);
                var xs = XmlSchema.Read(xsdReader, null);
                schemaSet.XmlResolver = new XmlUrlResolver();
                schemaSet.Add(xs);
            }

            return new XmlQualifiedName("DictionaryType", Properties.Resources.Namespace);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <returns>Always returns null.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Deserializes the XML representation of the dictionary.
        /// </summary>
        /// <param name="reader">An XML Reader used for deserialization.</param>
        public void ReadXml(XmlReader reader)
        {
            try
            {
                if (reader.NodeType == XmlNodeType.None)
                {
                    reader.Read();

                    if (reader.NodeType == XmlNodeType.None)
                    {
                        throw new XmlSerializableDictionaryException("The reader contains invalid XML.");
                    }

                    if (reader.IsEmptyElement)
                    {
                        throw new XmlSerializableDictionaryException("The reader contains no XML.");
                    }
                }

                while (true)
                {
                    if (reader.LocalName != "Item")
                    {
                        if (!reader.ReadToFollowing("Item"))
                        {
                            break;
                        }
                    }

                    reader.ReadStartElement("Item");

                    if (reader.LocalName != "Key")
                    {
                        if (!reader.ReadToFollowing("Key"))
                        {
                            throw new XmlSerializableDictionaryException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resources.ExDeserializationNoElement,
                                    this.GetType().Name,
                                    "Key"));
                        }
                    }

                    reader.ReadStartElement("Key");
                    TKey key;

                    // Copy the key content to another XmlReader to protect main reader.
                    using (var sr = new StringReader(reader.ReadOuterXml()))
                    {
                        var keyReader = XmlReader.Create(sr);
                        key = this.ReadKey(keyReader);
                    }

                    if (reader.LocalName != "Value")
                    {
                        if (!reader.ReadToFollowing("Value"))
                        {
                            throw new XmlSerializableDictionaryException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resources.ExDeserializationNoElement,
                                    this.GetType().Name,
                                    "Value"));
                        }
                    }

                    reader.ReadStartElement("Value");
                    TValue value;

                    // Copy the key content to another XmlReader to protect main reader.
                    using (var sr = new StringReader(reader.ReadOuterXml()))
                    {
                        var valueReader = XmlReader.Create(sr);
                        value = this.ReadValue(valueReader);
                    }

                    this.Add(key, value);

                    if (reader.LocalName == "Item")
                    {
                        reader.ReadEndElement();
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
                       Properties.Resources.ExUnexpectedError,
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
                    writer.WriteStartElement("Item", Properties.Resources.Namespace);

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
                       Properties.Resources.ExUnexpectedError,
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
                    Properties.Resources.ExReaderIsNull,
                    "ReadKey");
                var innerException = new ArgumentNullException(
                    "reader",
                    Properties.Resources.ExValueNotNull);

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
                    Properties.Resources.ExReaderIsNull,
                    "ReadValue");
                var innerException = new ArgumentNullException(
                    "reader",
                    Properties.Resources.ExValueNotNull);

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
                    Properties.Resources.ExWriterIsNull,
                    "WriteKey");
                var innerException = new ArgumentNullException(
                    "writer",
                    Properties.Resources.ExValueNotNull);

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
                    Properties.Resources.ExWriterIsNull,
                    "WriteValue");
                var innerException = new ArgumentNullException(
                    "writer",
                    Properties.Resources.ExValueNotNull);

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
