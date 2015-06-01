// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveRequest.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.ResolutionService
{
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.ServiceModel;

    /// <summary>
    /// The message contract representing a resolve operation.
    /// </summary>
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "Resolve",
        WrapperNamespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", IsWrapped = true)]
    public class ResolveRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveRequest"/> class.
        /// </summary>
        public ResolveRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveRequest"/> class.
        /// </summary>
        /// <param name="providerName">
        /// Provider name
        /// </param>
        /// <param name="serviceName">
        /// Service name
        /// </param>
        /// <param name="bindingAccessPoint">
        /// Binding access point
        /// </param>
        /// <param name="bindingUrlType">
        /// Binding URL type
        /// </param>
        /// <param name="messageType">
        /// Message type
        /// </param>
        /// <param name="operationName">
        /// Operation name
        /// </param>
        /// <param name="messageRole">
        /// Message role
        /// </param>
        /// <param name="parameters">
        /// General parameters
        /// </param>
        /// <param name="messageDirection">
        /// Message direction
        /// </param>
        /// <param name="policyName">
        /// Policy name
        /// </param>
        /// <param name="version">
        /// Policy version in the format of x.y where x is the major and y is the minor version number.
        /// </param>
        public ResolveRequest(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            Facts.Dictionaries.ParametersDictionary parameters,
            Facts.Interchange.MessageDirectionTypes messageDirection,
            string policyName,
            string version)
        {
            this.ProviderName = providerName;
            this.ServiceName = serviceName;
            this.BindingAccessPoint = bindingAccessPoint;
            this.BindingUrlType = bindingUrlType;
            this.MessageType = messageType;
            this.OperationName = operationName;
            this.MessageRole = messageRole;
            this.Parameters = parameters;
            this.MessageDirection = messageDirection;
            this.PolicyName = policyName;
            this.Version = version;
        }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 0)]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 1)]
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the binding access point.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 2)]
        public string BindingAccessPoint { get; set; }

        /// <summary>
        /// Gets or sets the binding URL type.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 3)]
        public string BindingUrlType { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 4)]
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets the operation name.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 5)]
        public string OperationName { get; set; }

        /// <summary>
        /// Gets or sets the message role.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 6)]
        public string MessageRole { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 7)]
        public Facts.Dictionaries.ParametersDictionary Parameters { get; set; }

        /// <summary>
        /// Gets or sets the message direction.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 8)]
        public Facts.Interchange.MessageDirectionTypes MessageDirection { get; set; }

        /// <summary>
        /// Gets or sets the policy name.
        /// </summary>
        [MessageBodyMember(
            Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", Order = 9)]
        public string PolicyName { get; set; }

        /// <summary>
        /// Gets or sets the policy version in the format of x.y where x is the major and y is the minor version number.
        /// </summary>
        [MessageBodyMember(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05",
            Order = 10)]
        public string Version { get; set; }
    }
}