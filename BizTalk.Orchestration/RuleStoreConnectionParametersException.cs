// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleStoreConnectionParametersException.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.BizTalk.Orchestration
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    /// <summary>
    ///    The exception that is thrown when a non-fatal application error occurs.
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    public class RuleStoreConnectionParametersException : EsbBizTalkOrchestrationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleStoreConnectionParametersException"/> class. 
        /// </summary>
        public RuleStoreConnectionParametersException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleStoreConnectionParametersException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">
        /// A message that describes the error.
        /// </param>
        public RuleStoreConnectionParametersException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleStoreConnectionParametersException"/> class with
        /// a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException
        /// parameter is not a null reference, the current exception is raised in a catch
        /// block that handles the inner exception.
        /// </param>
        public RuleStoreConnectionParametersException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleStoreConnectionParametersException"/> class with
        /// serialized data.
        /// </summary>
        /// <param name="info">
        /// The object that holds the serialized object data.
        /// </param>
        /// <param name="context">
        /// The contextual information about the source or destination.
        /// </param>
        protected RuleStoreConnectionParametersException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
