// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResolver.cs" company="Solidsoft Reply Ltd.">
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
    using System.ServiceModel;

    using SolidsoftReply.Esb.Libraries.Facts;

    /// <summary>
    /// The service contract for the resolver service
    /// </summary>
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [XmlSerializerFormat]
    [ServiceContract(Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", ConfigurationName = "ResolverSoap")]
    public interface IResolver
    {
        // CODEGEN: Parameter 'Interchange' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached.
        /// </summary>
        /// <param name="request">
        /// The request made to the resolver service.
        /// </param>
        /// <returns>
        /// An interchange document containing resolved values.
        /// </returns>
        [OperationContract(Action = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05/Resolve")]
        ResolveResponse Resolve(ResolveRequest request);

        /// <summary>
        /// Obtain a BAM interception policy for a given business activity.
        /// </summary>
        /// <param name="activityName">
        /// Name of the business activity
        /// </param>
        /// <param name="stepName">
        /// The activity step name.
        /// </param>
        /// <param name="policyName">
        /// The policy Name.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// A sub-classed activity interceptor configuration object containing the configuration for tracking points.
        /// </returns>
        [OperationContract(Action = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05/GetInterceptionPolicy")]
        BamActivityStep GetInterceptionPolicy(string activityName, string stepName, string policyName, string version);
    }
}
