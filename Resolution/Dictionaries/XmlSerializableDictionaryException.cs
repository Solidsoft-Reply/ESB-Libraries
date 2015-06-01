//----------------------------------------------------------------------------
// <copyright file="XmlSerializableDictionaryException.cs" company="Solidsoft Reply Ltd">
//   © 2015 Solidsoft Reply Ltd
//   This software is released under the Apache 2 License.
//   The license can be found in the file LICENSE at the root 
//   directory of the distribution.
// </copyright>
//----------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Resolution.Dictionaries
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an exception raised by the XmlSerializableDictionary.
    /// </summary>
    [Serializable]
    public class XmlSerializableDictionaryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionaryException class.
        /// </summary>
        public XmlSerializableDictionaryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionaryException class.
        /// </summary>
        /// <param name="message">Error message.</param>
        public XmlSerializableDictionaryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionaryException class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception wrapped by XmlSerializableDictionaryException.</param>
        public XmlSerializableDictionaryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionaryException class.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected XmlSerializableDictionaryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
