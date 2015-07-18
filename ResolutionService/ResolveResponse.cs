// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveResponse.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.ResolutionService
{
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.Xml.Serialization;

    /// <summary>
    /// The message contract for the response from the Resolve operation.
    /// </summary>
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "ResolveResponse", WrapperNamespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", IsWrapped = true)]
    public class ResolveResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveResponse"/> class.
        /// </summary>
        public ResolveResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveResponse"/> class.
        /// </summary>
        /// <param name="interchange">
        /// An interchange that will be resolved using the service directory.
        /// </param>
        public ResolveResponse(Facts.Interchange interchange)
        {
            this.Interchange = interchange;
        }

        /// <summary>
        /// Gets or sets an interchange that will be resolved using the service directory.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 0)]
        [XmlElement(IsNullable = true)]
        public Facts.Interchange Interchange { get; set; }
    }
}