// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsXml.cs" company="Solidsoft Reply Ltd.">
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

// ReSharper disable UnusedMethodReturnValue.Global

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// A library of helper extension methods.
    /// </summary>
    internal static class ExtensionMethodsXml
    {
        /// <summary>
        /// A BTS type specifier for an XML document in the form of namespace#docElementName.
        /// </summary>
        /// <param name="xmlDocument">An XML document.</param>
        /// <returns>A BTS type specifier.</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public static string TypeSpecifier(this XmlDocument xmlDocument)
        {
            var namespaceSpecifier = string.Empty;
            var docElementName = string.Empty;

            if (xmlDocument.DocumentElement != null)
            {
                namespaceSpecifier = xmlDocument.DocumentElement.NamespaceURI;
                docElementName = xmlDocument.DocumentElement.LocalName;
            }

            if (!string.IsNullOrEmpty(namespaceSpecifier))
            {
                namespaceSpecifier += "#";
            }

            return namespaceSpecifier + docElementName;
        }

/*
        /// <summary>
        /// Converts an XElement to an XML DOM element.
        /// </summary>
        /// <param name="element">The XElement to be converted.</param>
        /// <returns>An XNML DOM element.</returns>
        public static XmlElement ConvertToXmlElement(this XElement element)
        {
            using (var xmlReader = element.CreateReader())
            {
                var xmlDoc = new AsXmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc.DocumentElement;
            }
        }
*/

        /// <summary>
        /// Serializes a CLR object (ref or value) to an XML representation.
        /// </summary>
        /// <param name="clrObject">The CLR object.</param>
        /// <returns>An XML representation of the serialized object.</returns>
        public static XmlElement SerializeClrObjectToXml(this object clrObject)
        {
            var xmlserializer = new NetDataContractSerializer();
            var memStream = new MemoryStream();
            xmlserializer.Serialize(memStream, clrObject);
            var doc = new XmlDocument();
            doc.Load(memStream.StreamAtStart());
            return doc.DocumentElement;
        }
    }
}