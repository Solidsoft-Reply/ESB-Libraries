// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsIBaseMessage.cs" company="Solidsoft Reply Ltd.">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using BTS;

    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.BizTalk.Streaming;
    using Microsoft.XLANGs.BaseTypes;

    /// <summary>
    /// A library of helper extension methods for IBaseMessage and IBaseMessagePart objects.
    /// </summary>
    internal static class ExtensionMethodsIBaseMessage
    {
        /// <summary>
        ///     MessageType BizTalk property.
        /// </summary>
        private static readonly PropertyBase MessageTypeProp = new MessageType();

        /// <summary>
        /// Returns a part in a pipeline message.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <param name="index">The index at which the part is located.</param>
        /// <returns>The part in a pipeline message.</returns>
        public static IBaseMessagePart Part(this IBaseMessage message, int index)
        {
            string partNameOut;
            var inMsgPart = message.GetPartByIndex(index, out partNameOut);
            return inMsgPart;
        }

        /// <summary>
        /// Sets a property in the message context.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="nameSpace">The namespace of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>The pipeline message with amended context.</returns>
        public static IBaseMessage SetProperty(this IBaseMessage message, string name, string nameSpace, object value)
        {
            if (message.Context != null)
            {
                message.Context.Write(name, nameSpace, value);
            }

            return message;
        }

        /// <summary>
        /// Sets a promoted property in the message context.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <param name="name">The name of the promoted property.</param>
        /// <param name="nameSpace">The namespace of the property.</param>
        /// <param name="value">The value of the promoted property.</param>
        /// <returns>The pipeline message with amended context.</returns>
        public static IBaseMessage SetPromotedProperty(this IBaseMessage message, string name, string nameSpace, object value)
        {
            if (message.Context != null)
            {
                message.Context.Promote(name, nameSpace, value);
            }

            return message;
        }

        /// <summary>
        /// Returns an enumerator for the properties in a pipeline message.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <returns>An enumerator for the properties in a pipeline message.</returns>
        public static IEnumerable<MessageProperty> Properties(this IBaseMessage message)
        {
            for (var index = 0; index < message.Context.CountProperties; index++)
            {
                yield return message.Property(index);
            }
        }

        /// <summary>
        /// The message type  property value of a pipeline message.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <returns>The message type property value.</returns>
        public static string MessageType(this IBaseMessage message)
        {
            var messageType = message.Context.Read(
                    MessageTypeProp.Name.Name,
                    MessageTypeProp.Name.Namespace);

            return messageType == null || messageType.ToString() == "#whitespace"
                       ? string.Empty
                       : messageType.ToString();
        }

        /// <summary>
        /// Clones a pipeline message creating an entirely independent and seekable copy.
        /// </summary>
        /// <param name="message">The message to be cloned.</param>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>The cloned message.</returns>
        public static IBaseMessage Clone(this IBaseMessage message, IPipelineContext pc)
        {
            // Create the cloned message
            var messageFactory = pc.GetMessageFactory();
            var clonedMessage = messageFactory.CreateMessage();

            // Clone each part
            for (var partNo = 0; partNo < message.PartCount; partNo++)
            {
                string partName;
                var part = message.GetPartByIndex(partNo, out partName);

                // Create and initilialize the new part
                var newPart = messageFactory.CreateMessagePart();
                newPart.Charset = part.Charset;
                newPart.ContentType = part.ContentType;

                // Get the original uncloned data stream
                var originalStream = part.GetOriginalDataStream();

                // If the original data stream is non-seekable, check the clone
                // returned by the Data property, and failing that, or if the 
                // clone is not seekablem manufacture a seekable stream.
                if (!originalStream.CanSeek)
                {
                    Stream candidateSeekableStream;

                    try
                    {
                        candidateSeekableStream = part.Data;
                    }
                    catch (NotSupportedException)
                    {
                        // Some streams (e.g. ICSharpCode.SharpZipLib.Zip.ZipInputStream) throw 
                        // a System.NotSupportedException when an attempt is made to clone them
                        candidateSeekableStream = null;
                    }

                    if (candidateSeekableStream != null && !candidateSeekableStream.Equals(originalStream))
                    {
                        if (candidateSeekableStream.CanSeek)
                        {
                            originalStream = candidateSeekableStream;
                        }
                        else
                        {
                            originalStream = new ReadOnlySeekableStream(candidateSeekableStream);
                        }
                    }
                    else
                    {
                        originalStream = new ReadOnlySeekableStream(originalStream);
                    }
                }

                // Add the original stream to the Resource tracker to prevent it being
                // disposed, in case we need to clone the same stream multiple times.
                pc.ResourceTracker.AddResource(originalStream);
                originalStream.StreamAtStart();

                // Create the new part with a Virtual Stream, and add the the resource tracker
                newPart.Data = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
                pc.ResourceTracker.AddResource(newPart.Data);

                // Clone the stream, and seek back to the beginning
                originalStream.CopyTo(newPart.Data);
                originalStream.StreamAtStart();

                // Create and populate the property bag for the new message part.
                newPart.Data.StreamAtStart();
                newPart.PartProperties = messageFactory.CreatePropertyBag();
                var partPoperties = part.PartProperties;

                for (var propertyNo = 0; propertyNo < partPoperties.CountProperties; propertyNo++)
                {
                    string propertyName, propertyNamespace;
                    var property = partPoperties.ReadAt(propertyNo, out propertyName, out propertyNamespace);
                    newPart.PartProperties.Write(propertyName, propertyNamespace, property);
                }

                // Add the new part to the cloned message
                clonedMessage.AddPart(partName, newPart, partName == message.BodyPartName);
            }

            // Copy the context from old to new
            clonedMessage.Context = message.Context;

            return clonedMessage;
        }

        /// <summary>
        /// Gets the body part, if it exists, or the first part, or returns a null value
        /// if no part is found.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <returns>The body part, or the first part if no body part is designated, or null if no part exists.</returns>
        public static IBaseMessagePart BodyOrFirstPartOrDefault(this IBaseMessage message)
        {
            try
            {
                IBaseMessagePart part;

                if (message.BodyPart == null)
                {
                    string partName;
                    part = message.GetPartByIndex(0, out partName);
                }
                else
                {
                    part = message.BodyPart;
                }

                return part.WithStreamAtStart();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Populates a body part with content from an XML document.  If no body part exists, a new one
        /// is created.
        /// </summary>
        /// <param name="message">The massage to which the body part will be added.</param>
        /// <param name="xmlDocument">The XML document containing the content</param>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>A message with a body part populated by the XML document content.</returns>
        public static bool PopulateBodyPartFromXmlDocument(this IBaseMessage message, XmlDocument xmlDocument, IPipelineContext pc)
        {
            if (message == null)
            {
                return false;
            }

            // Add a body part if none exists.
            if (message.BodyPart == null)
            {
                message.AddPart("Body", pc.GetMessageFactory().CreateMessagePart(), true);
            }

            // Assign transformed message to outbound message
            message.BodyPart.Data = new MemoryStream().PopulateFromXmlDocument(xmlDocument);

            return true;
        }

        /// <summary>
        /// Populates a body part with content from an XML document.  If no body part exists, a new one
        /// is created.
        /// </summary>
        /// <param name="message">The massage to which the body part will be added.</param>
        /// <param name="part">The existing part</param>
        /// <param name="pc">The pipeline context.</param>
        /// <returns>A message with a body part populated by the XML document content.</returns>
        public static bool PopulateBodyPartFromExistingPart(this IBaseMessage message, IBaseMessagePart part, IPipelineContext pc)
        {
            if (message == null || part == null)
            {
                return false;
            }

            Func<IBaseMessagePart> clonedPart = () =>
            {
                var newPart = pc.GetMessageFactory().CreateMessagePart();
                newPart.Data = part.GetOriginalDataStream().Clone();
                return newPart;
            };

            var outPart = part.IsMutable
                ? clonedPart()
                : part.WithStreamAtStart();

            // Add a body part if none exists.
            if (message.BodyPart == null)
            {
                message.AddPart("Body", outPart, true);
            }

            return true;
        }

        /// <summary>
        /// Returns a message property definition in a pipeline message.
        /// </summary>
        /// <param name="message">The pipeline message.</param>
        /// <param name="index">The index of the property.</param>
        /// <returns>A message property definition in a pipeline message</returns>
        private static MessageProperty Property(this IBaseMessage message, int index)
        {
            string propertyName, propertyNamespace;
            var property = message.Context.ReadAt(index, out propertyName, out propertyNamespace);
            return new MessageProperty(propertyName, propertyNamespace, property, message.Context.IsPromoted(propertyName, propertyNamespace));
        }
    }
}