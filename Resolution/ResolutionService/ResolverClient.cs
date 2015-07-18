// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolverClient.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Resolution.ResolutionService
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// Resolver client for convenient invocation of resolver service.
    /// </summary>
    public partial class ResolverClient
    {
        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="serviceName">Service name</param>
        /// <param name="bindingAccessPoint">Binding access point</param>
        /// <param name="bindingUrlType">Binding URL type</param>
        /// <param name="messageType">Message type</param>
        /// <param name="operationName">Operation name</param>
        /// <param name="messageRole">Message role</param>
        /// <param name="parameters">Parameter items</param>
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <returns>Return a Directive object with the result</returns>
        public Interchange Resolve(
            string providerName, 
            string serviceName, 
            string bindingAccessPoint, 
            string bindingUrlType, 
            string messageType, 
            string operationName, 
            string messageRole,
            Dictionaries.Parameters parameters,
            Resolution.MessageDirectionTypes messageDirection, 
            string policyName, 
            string version)
        {
            Parameters[] parametersItems = null;

            if (parameters != null)
            {
                parametersItems = new Parameters[parameters.Count];
                var index = 0;

                foreach (var parametersItem in parameters)
                {
                    var valueSerializer = new NetDataContractSerializer();

                    using (var ms = new MemoryStream())
                    {
                        valueSerializer.Serialize(ms, parametersItem.Value);
                        ms.Position = 0;
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(ms);

                        parametersItems.SetValue(
                            new Parameters
                                {
                                    Key =
                                        new ParametersKey
                                            {
                                                @string =
                                                    parametersItem
                                                    .Key
                                            },
                                    Value = xmlDoc.DocumentElement
                                },
                            index++);
                    }
                }
            }

            MessageDirectionTypes interchangeMessageDirectionType;

            switch (messageDirection)
            {
                case Resolution.MessageDirectionTypes.Both:
                    interchangeMessageDirectionType = MessageDirectionTypes.Both;
                    break;
                case Resolution.MessageDirectionTypes.MsgIn:
                    interchangeMessageDirectionType = MessageDirectionTypes.MsgIn;
                    break;
                case Resolution.MessageDirectionTypes.MsgOut:
                    interchangeMessageDirectionType = MessageDirectionTypes.MsgOut;
                    break;
                default:
                    interchangeMessageDirectionType = MessageDirectionTypes.NotSpecified;
                    break;
            }

            return this.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                parametersItems,
                interchangeMessageDirectionType,
                policyName,
                version);
        }
    }
}