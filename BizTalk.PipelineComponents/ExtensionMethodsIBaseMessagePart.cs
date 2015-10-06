// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsIBaseMessagePart.cs" company="Solidsoft Reply Ltd.">
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

// ReSharper disable UnusedMethodReturnValue.Global

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System.IO;
    using System.Xml;

    using Microsoft.BizTalk.Message.Interop;

    /// <summary>
    /// A library of helper extension methods for IBaseMessage and IBaseMessagePart objects.
    /// </summary>
    internal static class ExtensionMethodsIBaseMessagePart
    {
        /// <summary>
        /// Remove outer nodes from the message part content and leave the body. 
        /// </summary>
        /// <param name="messagePart">The message part</param>
        /// <param name="bodyContainerXPath">The XPath specifying the body of the message.</param>
        /// <returns>The message part with the envelope removed.</returns>
        public static IBaseMessagePart RemoveEnvelope(this IBaseMessagePart messagePart, string bodyContainerXPath)
        {
            if (string.IsNullOrEmpty(bodyContainerXPath))
            {
                return messagePart;
            }

            var bodyXml = messagePart.AsXmlDocument();

            // Assign content back to part
            var contentStream = new MemoryStream();
            var sw = new StreamWriter(contentStream);
            var selectSingleNode = bodyXml.SelectSingleNode(bodyContainerXPath);

            if (selectSingleNode != null)
            {
                sw.Write(selectSingleNode.FirstChild.OuterXml);
            }

            sw.Flush();
            contentStream.StreamAtStart();

            messagePart.Data = contentStream;
            return messagePart;
        }

        /// <summary>
        /// Returns an XmlDocument over the message part content.
        /// </summary>
        /// <param name="messagePart">A pipeline messagePart part.</param>
        /// <returns>An XML document.</returns>
        public static XmlDocument AsXmlDocument(this IBaseMessagePart messagePart)
        {
            var bodyXml = new XmlDocument();

            // Preserve whitespace to render document as original
            bodyXml.PreserveWhitespace = true;

            if (messagePart.Data == null)
            {
                return bodyXml;
            }

            var currentPosition = messagePart.Data.Position;
            messagePart.WithStreamAtStart();

            try
            {
                bodyXml.Load(messagePart.Data);
            }
            catch (XmlException)
            {
                return new XmlDocument();
            }

            messagePart.Data.Seek(currentPosition, SeekOrigin.Begin);
            return bodyXml;
        }

        /// <summary>
        /// Seeks back to the origin of a stream.
        /// </summary>
        /// <param name="messagePart">A pipeline messagePart part.</param>
        /// <returns>A seekable stream set at its origin.</returns>
        // ReSharper disable once UnusedMethodReturnValue.Local
        public static IBaseMessagePart WithStreamAtStart(this IBaseMessagePart messagePart)
        {
            // Obtain a reference to the original message stream
            var originalDataStream = messagePart.GetOriginalDataStream();

            // If the original data stream is seekable, then return it as the caller can simply reset
            // the position once it's been read
            if (originalDataStream.CanSeek)
            {
                messagePart.Data = originalDataStream.StreamAtStart();
            }
            else
            {
                // Attempt to obtain a reference to a clone of the original message stream.
                Stream clonedStream;

                try
                {
                    clonedStream = messagePart.Data;
                }
                catch
                {
                    // Some streams throw an exception (e.g., a System.NotSupportedException)
                    // when an attempt is made to clone them.
                    clonedStream = null;
                }

                // If the theoretical clone is a true clone, then use it.
                if (clonedStream != null && clonedStream.Equals(originalDataStream) == false)
                {
                    clonedStream.Position = 0;
                    messagePart.Data = clonedStream.StreamAtStart();
                }
                else
                {
                    // Otherwise, we need to replace the stream with one which is seekable
                    messagePart.Data = originalDataStream.StreamAtStart();
                }
            }

            return messagePart;
        }
    }
}
