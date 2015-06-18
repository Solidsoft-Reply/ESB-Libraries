// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Transformer.cs" company="Solidsoft Reply Ltd.">
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
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Xsl;

    /// <summary>
    /// Transform helper class
    /// </summary>
    [Serializable]
    public class Transformer
    {
        /// <summary>
        /// Cache of strong names of schemas.
        /// </summary>
        internal static readonly SchemaStrongNameCache SchemaStrongNameCache = new SchemaStrongNameCache();

        /// <summary>
        /// Apply a map to a an aggregation of two XML messages
        /// </summary>
        /// <param name="mapFullName">Map full name (Class, strong name)</param>
        /// <param name="messageIn1">The first XML message to be transformed</param>
        /// <param name="messageIn2">The second XML message to be transformed</param>
        /// <returns>The transformed xml document</returns>
        public static TransformResults Transform(string mapFullName, XmlDocument messageIn1, XmlDocument messageIn2)
        {
            var merge = new XmlDocument();
            var xml = new StringBuilder();

            xml.Append(string.Format("<r:Root xmlns:r='{0}'>", Properties.Resources.UriAggSchema));
            xml.Append("<InputMessagePart_0>");

            if (messageIn1.DocumentElement != null)
            {
                xml.Append(messageIn1.DocumentElement.OuterXml);
            }

            xml.Append("</InputMessagePart_0>");
            xml.Append("<InputMessagePart_1>");

            if (messageIn2.DocumentElement != null)
            {
                xml.Append(messageIn2.DocumentElement.OuterXml);
            }

            xml.Append("</InputMessagePart_1>");
            xml.Append("</r:Root>");

            merge.LoadXml(xml.ToString());

            return Transform(mapFullName, merge);
        }

        /// <summary>
        /// Apply a map to a xml message
        /// </summary>
        /// <param name="mapFullName">Map full name (Class, strong name)</param>
        /// <param name="messageIn">Xml message to be transformed</param>
        /// <returns>The transformed xml document</returns>
        public static TransformResults Transform(string mapFullName, XmlDocument messageIn)
        {
            if (string.IsNullOrWhiteSpace(mapFullName))
            {
                return new TransformResults();
            }

            var msgOut = new XmlDocument();            
            StringWriter writer = null;

            System.Diagnostics.Debug.Write("[Resolver] Transform using the map in " + mapFullName);

            // Check parameters
            if (messageIn == null)
            {
                throw new ArgumentNullException("messageIn");
            }

            if (string.IsNullOrEmpty(mapFullName))
            {
                throw new ArgumentNullException("mapFullName");
            }

            // Extract the class name and the strong name from the MapFullName
            var className = mapFullName.Split(',')[0];
            var pos = mapFullName.IndexOf(',');

            if (pos == -1)
            {
                throw new ArgumentException(string.Format(Properties.Resources.ExceptionMapFullName, mapFullName));
            }

            var strongName = mapFullName.Substring(pos);
            strongName = strongName.Trim();

            if (strongName.StartsWith(","))
            {
                strongName = strongName.Substring(1);
            }

            strongName = strongName.Trim();

            TransformResults transformResults;

            try
            {
                // Load the map
                var mapAssembly = Assembly.Load(strongName);
                var map = mapAssembly.CreateInstance(className);

                if (map == null)
                {
                    throw new EsbResolutionException(
                        string.Format(Properties.Resources.ExceptionMapClassInvalid, className));
                }
                
                // Extract the xslt
                var xmlContentProp = map.GetType().GetProperty("XmlContent");
                var xsl = xmlContentProp.GetValue(map, null);

                // Extract xsl and extension objects
                var xsltArgumentsProp = map.GetType().GetProperty("XsltArgumentListContent");
                var xsltArguments = xsltArgumentsProp.GetValue(map, null);

                // Extract source schemas
                var sourceSchemasProp = map.GetType().GetProperty("SourceSchemas");
                var sourceSchemas = (string[])sourceSchemasProp.GetValue(map, null);

                // Extract target schemas
                var targetSchemasProp = map.GetType().GetProperty("TargetSchemas");
                var targetSchemas = (string[])targetSchemasProp.GetValue(map, null);

                // Load all the external assemblies
                var xmlExtension = new XmlDocument();
                var xslArgList = new XsltArgumentList();

                if (xsltArguments != null)
                {
                    // Load the argument list and create all the needed instances
                    xmlExtension.LoadXml(xsltArguments.ToString());
                    var xmlExtensionNodes = xmlExtension.SelectNodes(Properties.Resources.XPathExtensionObject);

                    if (xmlExtensionNodes != null)
                    {
                        foreach (XmlNode extObjNode in xmlExtensionNodes)
                        {
                            var extAttributes = extObjNode.Attributes;

                            if (extAttributes == null)
                            {
                                continue;
                            }

                            var namespaceNode = extAttributes.GetNamedItem("Namespace");
                            var assemblyNode = extAttributes.GetNamedItem("AssemblyName");
                            var classNode = extAttributes.GetNamedItem("ClassName");
                            var extAssembly = Assembly.Load(assemblyNode.InnerText);
                            var extObj = extAssembly.CreateInstance(classNode.InnerText);

                            if (extObj != null)
                            {
                                xslArgList.AddExtensionObject(namespaceNode.InnerText, extObj);
                            }
                        }
                    }
                }

                // Apply xsl to msg in
                var xslDoc = new XmlDocument();
                xslDoc.LoadXml(xsl.ToString());

                var settings = new XsltSettings(true, true);
                var xlsTrans = new XslCompiledTransform();
                xlsTrans.Load(xslDoc, settings, new XmlUrlResolver());

                writer = new StringWriter();

                if (messageIn.DocumentElement != null)
                {
                    xlsTrans.Transform(new XmlNodeReader(messageIn.DocumentElement), xslArgList, writer);
                }

                writer.Flush();

                // Return the msg out                
                msgOut.LoadXml(writer.ToString());

                transformResults = new TransformResults(messageIn, msgOut, xslDoc, xslArgList, sourceSchemas, targetSchemas, (from schemaName in targetSchemas select SchemaStrongNameCache.GetSchemaStrongName(map.GetType(), schemaName)).ToList<string>());
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }

            return transformResults;
        }
    }
}
