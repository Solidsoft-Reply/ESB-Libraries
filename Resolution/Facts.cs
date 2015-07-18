// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Facts.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System.Diagnostics.CodeAnalysis;

    using SolidsoftReply.Esb.Libraries.Resolution.Dictionaries;

    /// <summary>
    /// A set of facts asserted to the Resolution Service to resolve directives.
    /// </summary>
    public class Facts
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Facts"/> class.
        /// </summary>
        public Facts()
        {
            this.MessageDirection = MessageDirectionTypes.NotSpecified;
            this.Parameters = new Parameters();
        }

        /// <summary>
        /// Gets or sets the service provider name.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets access information (e.g URL) for the associated Web service.
        /// </summary>
        public string BindingAccessPoint { get; set; }

        /// <summary>
        /// Gets or sets the binding URL type - e.g., HTTP, HTTPS, FTP
        /// </summary>
        public string BindingUrlType { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the service operation name.
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// Gets or sets the message role specifier - e.g., request, response, ack, nack, fault.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public string MessageRole { get; set; }

        /// <summary>
        /// Gets or sets the direction of the message.  This may be unspecified, inbound,
        /// outbound or both.
        /// </summary>
        public MessageDirectionTypes MessageDirection { get; set; }

        /// <summary>
        /// Gets or sets the parameter items.
        /// </summary>
        public Parameters Parameters { get; set; }
    }
}
