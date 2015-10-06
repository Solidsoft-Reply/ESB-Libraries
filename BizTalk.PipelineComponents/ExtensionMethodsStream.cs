// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsStream.cs" company="Solidsoft Reply Ltd.">
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
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Microsoft.BizTalk.Streaming;

    /// <summary>
    /// A library of helper extension methods.
    /// </summary>
    internal static class ExtensionMethodsStream
    {
/*
        /// <summary>
        /// Populate a stream from an XML Document.
        /// </summary>
        /// <param name="stream">The stream to be populated.</param>
        /// <param name="originalStream">The original stream containing the content.</param>
        /// <returns>A stream populated from an XML document.</returns>
        public static Stream PopulateFromStream(this Stream stream, Stream originalStream)
        {
            return originalStream == null || stream == null ? new MemoryStream() : originalStream.Clone();
        }
*/

        /// <summary>
        /// Seeks back to the origin of a stream.
        /// </summary>
        /// <param name="stream">A stream.</param>
        /// <returns>A seekable stream set at its origin.</returns>
        public static Stream StreamAtStart(this Stream stream)
        {
            var outStream = stream;

            if (outStream == null)
            {
                return null;
            }

            if (!outStream.CanSeek)
            {
                // If the stream is not seekable, create a seekable stream over a virtual stream to provide buffering and large message handling
                outStream = new ReadOnlySeekableStream(stream, new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk), 0x1000);
            }

            outStream.Position = 0;
            return outStream;
        }

        /// <summary>
        /// Clones a stream.
        /// </summary>
        /// <param name="stream">The stream to be cloned.</param>
        /// <returns>The cloned stream.</returns>
        public static Stream Clone(this Stream stream)
        {
            var outStream = new MemoryStream();

            if (stream == null)
            {
                return outStream;
            }

            var currentPos = stream.Position;
            stream.StreamAtStart().CopyTo(outStream);
            stream.Position = currentPos;

            return outStream.StreamAtStart();
        }

        /// <summary>
        /// Populate a stream from an XML Document.
        /// </summary>
        /// <param name="stream">The stream to be populated.</param>
        /// <param name="xmlDocument">The XML document containing the content.</param>
        /// <returns>A stream populated from an XML document.</returns>
        public static Stream PopulateFromXmlDocument(this Stream stream, XmlDocument xmlDocument)
        {
            if (stream == null)
            {
                stream = new MemoryStream();
            }

            var streamOut = stream.StreamAtStart();

            if (xmlDocument.HasChildNodes)
            {
                // Get the XML encoding.  Defaults to UTF-8
                Func<Encoding> xmlEncoding = () =>
                    (xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration) && 
                    !string.IsNullOrWhiteSpace(((XmlDeclaration)xmlDocument.FirstChild).Encoding)
                    ? Encoding.GetEncoding(((XmlDeclaration)xmlDocument.FirstChild).Encoding.ToLower())
                    : Encoding.UTF8;

                var xmlWriter = new XmlTextWriter(streamOut, xmlEncoding());

                // Assign transformed message to outbound message
                xmlDocument.WriteContentTo(xmlWriter);
                xmlWriter.Flush();
            }
            else
            {
                streamOut = new MemoryStream();
            }

            return streamOut.StreamAtStart();
        }
    }
}
