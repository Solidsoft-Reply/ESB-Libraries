//----------------------------------------------------------------------------
// <copyright file="XmlSerializableDictionaryException.cs" company="Solidsoft Reply Ltd">
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
