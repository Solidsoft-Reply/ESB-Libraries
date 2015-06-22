// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableDictionary.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Xml Serialisable generic dictionary.   Inherits from the non-serialisable
    /// generic dictionary provided by .NET.
    /// </summary>
    /// <typeparam name="TKey">Type of key value</typeparam>
    /// <typeparam name="TValue">Type of value</typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [XmlSchemaProvider("GetDictionarySchema")]
    [XmlRoot("Dictionary", Namespace = "http://solidsoftreply.com/schemas/esbresolution/2015/05", IsNullable = true)]
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        /// <summary>
        /// The namespace for the serialised dictionary.
        /// </summary>
        public const string Namespace = "http://solidsoftreply.com/schemas/esbresolution/2015/05";

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class.
        /// </summary>
        public SerializableDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDictionary{TKey,TValue}"/> class. 
        /// Used during deserialization. For use with BinaryFormatter.
        /// </summary>
        /// <param name="info">
        /// Serialization Info
        /// </param>
        /// <param name="context">
        /// Streaming Context
        /// </param>
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentException(string.Format(Properties.Resources.ExceptionInfoNull, "constructor"));
            }

            var xmlStream = new MemoryStream(500);
            var xmlStrWri = new StreamWriter(xmlStream, Encoding.UTF8);
            var dictionaryXml = (string)info.GetValue("DictionaryXml", typeof(string));

            if (string.IsNullOrEmpty(dictionaryXml))
            {
                return;
            }

            xmlStrWri.Write(dictionaryXml);
            xmlStrWri.Flush();
            xmlStream.Seek(0, SeekOrigin.Begin);

            var xmlTextRead = new XmlTextReader(xmlStream);
            this.ReadXml(xmlTextRead);
        }

        /// <summary>
        /// The get dictionary schema.
        /// </summary>
        /// <param name="xss">
        /// The xss.
        /// </param>
        /// <returns>
        /// The <see cref="XmlQualifiedName"/>.
        /// </returns>
        public static XmlQualifiedName GetDictionarySchema(XmlSchemaSet xss)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(Properties.Resources.XmlFileDictionarySchema);
            Debug.Assert(stream != null, string.Format("{0} is null.", Properties.Resources.XmlFileDictionarySchema));
            var xsdReader = new XmlTextReader(stream);
            var xs = XmlSchema.Read(xsdReader, null);
            xss.XmlResolver = new XmlUrlResolver();
            xss.Add(xs);
            return new XmlQualifiedName("DictionaryType", Namespace);
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
        /// Deserialises the XML representation of the dictionary.
        /// </summary>
        /// <param name="reader">An XML Reader used for deserialistion.</param>
        public void ReadXml(XmlReader reader)
        {
            var wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
            {
                return;
            }

            while (true)
            {
                try
                {
                    reader.ReadStartElement("Item");

                    reader.ReadStartElement("Key");

// The ReadElementContentAs method returns an ArgumentNullException if null is passed as the returnType
// argument, but it's OK to pass null to the namespaceResolver argument                  
// ReSharper disable AssignNullToNotNullAttribute
                    var key = (TKey)reader.ReadElementContentAs(typeof(TKey), null);

// ReSharper restore AssignNullToNotNullAttribute
                    reader.ReadEndElement();

                    reader.ReadStartElement("Value");

// ReSharper disable AssignNullToNotNullAttribute
                    var value = (TValue)reader.ReadElementContentAs(typeof(TValue), null);
// ReSharper restore AssignNullToNotNullAttribute
                    this.Add(key, value);
                    reader.ReadEndElement();

                    reader.ReadEndElement();
                }
                catch
                {
                    break;
                }
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// Serializes the dictionary as XML.
        /// </summary>
        /// <param name="writer">An XML writer used for serialization.</param>
        public void WriteXml(XmlWriter writer)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));
            var xsns = new XmlSerializerNamespaces();
            xsns.Add(string.Empty, Namespace);

            foreach (var key in this.Keys)
            {
                writer.WriteStartElement("Item", Namespace);

                writer.WriteStartElement("Key");
                keySerializer.Serialize(writer, key, xsns);
                writer.WriteEndElement();

                writer.WriteStartElement("Value");
                var value = this[key];
                valueSerializer.Serialize(writer, value, xsns);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Serialise the dictionary.  For use with BinaryFormatter.
        /// </summary>
        /// <param name="info">Serialization Info</param>
        /// <param name="context">Streaming Context</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info == null)
            {
                throw new ArgumentException(string.Format(Properties.Resources.ExceptionInfoNull, "GetObjectData"));
            }

            var xmlStream = new MemoryStream(500);
            var xmlTextWriter = new XmlTextWriter(xmlStream, Encoding.UTF8);
            xmlStream.Seek(0, SeekOrigin.Begin);

            this.WriteXml(xmlTextWriter);
            var xmlStrRead = new StreamReader(xmlStream, Encoding.UTF8);

            string xmlText = xmlStrRead.ReadToEnd();
            xmlStrRead.Close();

            info.AddValue("DictionaryXml", xmlText);
        }
    }
}
