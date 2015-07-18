// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessPointUseType.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Uddi
{
    /// <summary>
    /// Pre-defined values for the UDDI AccessPoint use type.
    /// </summary>
    public enum AccessPointUseType
    {
        /// <summary>
        /// Designates that the AccessPoint use type is unspecified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Designates that the AccessPoint points to the actual service endpoint, 
        /// i.e. the network address at which the Web service can be invoked
        /// </summary>
        EndPoint,

        /// <summary>
        /// Designates that the AccessPoint contains a BindingKey that points to a 
        /// different BindingTemplate entry. The value in providing this facility is 
        /// seen when a business or entity wants to expose a service description (e.g. 
        /// advertise that they have a service available that suits a specific purpose) 
        /// that is actually a service that is described in a separate BindingTemplate 
        /// record. This might occur when many service descriptions could benefit from 
        /// a single service description.
        /// </summary>
        BindingTemplate,

        /// <summary>
        /// Designates that the AccessPoint can only be determined by querying another 
        /// UDDI registry. This might occur when a service is remotely hosted
        /// </summary>
        HostingRedirector,

        /// <summary>
        /// Designates that the AccessPoint points to a remotely hosted WSDL document 
        /// that already contains the necessary binding information, including the 
        /// actual service endpoint
        /// </summary>
        WsdlDeployment
    }
}
