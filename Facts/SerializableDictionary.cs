// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableDictionary.cs" company="Solidsoft Reply Ltd.">
//   (c) 2013 Solidsoft Reply Ltd.
// </copyright>
// <summary>
//   Xml Serialisable generic dictionary.   Inherits from the non-serialisable
//   generic dictionary provided by .NET.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Solidsoft.Esb.BreFacts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Xml Serialisable generic dictionary.   Inherits from the non-serialisable
    /// generic dictionary provided by .NET.
    /// </summary>
    /// <typeparam name="TKey">Type of key value</typeparam>
    /// <typeparam name="TValue">Type of value</typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here."),
     XmlSchemaProvider("GetDictionarySchema")]
    [XmlRoot("Dictionary", Namespace = "http://solidsoft.com/schemas/webservices/esbresolutionservice/2008/05/1", IsNullable = true)]
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        /// <summary>
        /// The namespace.
        /// </summary>
        public const string Namespace = "http://solidsoft.com/schemas/webservices/esbresolutionservice/2008/05/1";

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
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
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
            var stream = assembly.GetManifestResourceStream(Properties.Resources.XsdFileDictionarySchema);
            Debug.Assert(stream != null, Properties.Resources.XsdFileDictionarySchema + " is null");
            var xsdReader = new XmlTextReader(stream);
            var xs = XmlSchema.Read(xsdReader, null);
            xss.XmlResolver = new XmlUrlResolver();
            xss.Add(xs);
            return new XmlQualifiedName(Properties.Resources.QNameDictionaryType, Namespace);
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
                    reader.ReadStartElement("Item", Namespace);

                    reader.ReadStartElement("Key", Namespace);
                    ////reader.ReadStartElement("string", Namespace);
                    


// The ReadElementContentAs method returns an ArgumentNullException if null is passed as the returnType
// argument, but it's OK to pass null to the namespaceResolver argument                  
// ReSharper disable AssignNullToNotNullAttribute 
                    var keySerializer = new XmlSerializer(typeof(TKey));
                    ////var key = (TKey)keySerializer.Deserialize(reader);
                    var key = keySerializer.Deserialize(reader);
                    /////var key = (TKey)reader.ReadElementContentAs(typeof(TKey), null);
// ReSharper restore AssignNullToNotNullAttribute
                    reader.ReadEndElement();

                    reader.ReadStartElement("Value", Namespace);

// ReSharper disable AssignNullToNotNullAttribute
                    var value = (TValue)reader.ReadElementContentAs(typeof(TValue), null);
// ReSharper restore AssignNullToNotNullAttribute
                    this.Add((TKey)key, value);
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
        /// Serialises the dictionary as XML.
        /// </summary>
        /// <param name="writer">An XML writer used for serialisation.</param>
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
    }
}
