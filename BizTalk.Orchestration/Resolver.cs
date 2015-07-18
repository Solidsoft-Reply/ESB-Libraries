// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Resolver.cs" company="Solidsoft Reply Ltd.">
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

    /// <summary>
    /// Class to resolve and get info from the Service Directory
    /// </summary>
    [Serializable]
    public class Resolver
    {
        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="facts">Facts to be asserted to the Resolution Service.</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(Facts facts)
        {
            return Resolve(
                facts,
                string.Empty,
                string.Empty);
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="facts">Facts to be asserted to the Resolution Service.</param>
        /// <param name="policyName">Policy name</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(Facts facts, string policyName)
        {
            return Resolve(
                facts,
                policyName,
                string.Empty);
        }

        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached
        /// </summary>
        /// <param name="facts">Facts to be asserted to the Resolution Service.</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="policyVersion">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <returns>Return a Directive object with the result</returns>
        public static Directives Resolve(Facts facts, string policyName, string policyVersion)
        {
            var resolverFacts = new Resolution.Facts
                                    {
                                        ServiceName = facts.ServiceName,
                                        ProviderName = facts.ServiceName,
                                        BindingAccessPoint = facts.BindingAccessPoint,
                                        BindingUrlType = facts.BindingUrlType,
                                        MessageType = facts.MessageType,
                                        OperationName = facts.OperationName,
                                        MessageRole = facts.MessageRole,
                                        MessageDirection = facts.MessageDirection,
                                        Parameters =
                                            facts.Parameters == null || facts.Parameters.Count == 0
                                                ? null
                                                : facts.Parameters
                                    };

            return new Directives(Resolution.Resolver.Resolve(
                resolverFacts,
                policyName,
                policyVersion));
        }

        /// <summary>
        /// Invalidate the cache
        /// </summary>
        public static void InvalidateCache()
        {
            Resolution.Resolver.InvalidateCache();
        }
    }
}
