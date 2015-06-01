//----------------------------------------------------------------------------
// <copyright file="XmlSerializableDictionaryException.cs" company="Solidsoft Ltd">
//      (c) Solidsoft Ltd
// </copyright>
// <summary>This file contains the XmlSerializableDictionaryException interface.</summary>
//----------------------------------------------------------------------------

namespace Solidsoft.Common.Components.XmlSerializableCollections
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
        public XmlSerializableDictionaryException(string message) : base(message) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the XmlSerializableDictionaryException class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception wrapped by XmlSerializableDictionaryException.</param>
        public XmlSerializableDictionaryException(string message, Exception innerException) : base(message, innerException) 
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
