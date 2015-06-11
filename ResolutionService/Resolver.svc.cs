// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Resolver.svc.cs" company="Solidsoft Reply Ltd.">
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
    using System;
    using System.ServiceModel;

    /// <summary>
    /// The resolver service
    /// </summary>
    public class Resolver : IResolver
    {
        /// <summary>
        /// Execute the policy and returns the info related, the information is then cached.
        /// </summary>
        /// <param name="request">
        /// The request made to the resolver service.
        /// </param>
        /// <returns>
        /// An interchange document containing resolved values.
        /// </returns>
        public ResolveResponse Resolve(ResolveRequest request)
        {
            var soap = OperationContext.Current.RequestContext.RequestMessage.ToString();
            System.Diagnostics.Trace.WriteLine(soap);

            try
            {
                System.Diagnostics.Debug.Write("[ResolutionService] Resolve called");

                // Check parameters
                if (string.IsNullOrEmpty(request.PolicyName))
                {
                    throw new EsbResolutionServiceException(Properties.Resources.ExceptionPolicyNameInvalid);
                }

                // Is not in the cache we need to excecute the policy on BRE and then put on the cache
                Version ver = null;
                if (!string.IsNullOrEmpty(request.Version))
                {
                    ver = new Version(request.Version);
                }

                // Execute the policy
                var policyResultFacts = BusinessRuleEnginePolicyEvaluator.Eval(
                    request.PolicyName,
                    ver,
                    request.ProviderName,
                    request.ServiceName,
                    request.BindingAccessPoint,
                    request.BindingUrlType,
                    request.MessageType,
                    request.OperationName,
                    request.MessageRole,
                    request.Parameters,
                    request.MessageDirection);

                System.Diagnostics.Debug.Write("[ResolutionService] Resolve - Returned # directives from BRE: '" + policyResultFacts.Directives.Count + "'");

                return new ResolveResponse { Interchange = policyResultFacts };
            }
            catch (Exception ex)
            {
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("Application", ex.ToString(), System.Diagnostics.EventLogEntryType.Error, 1);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }

                throw;
            }
        }

        /// <summary>
        /// Obtain a BAM interception policy for a given business activity.
        /// </summary>
        /// <param name="activityName">
        /// Name of the business activity
        /// </param>
        /// <param name="stepName">
        /// The name of the current BAM activity step.
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
        public Facts.BamActivityStep GetInterceptionPolicy(string activityName, string stepName, string policyName, string version)
        {
            try
            {
                System.Diagnostics.Debug.Write("[ResolutionService] Called GetInterceptionPolicy");

                // Check parameters
                if (string.IsNullOrWhiteSpace(activityName))
                {
                    throw new EsbResolutionServiceException(Properties.Resources.ExceptionBamActivityParameter);
                }

                // Is not in the cache we need to excecute the policy on BRE and then put on the cache
                Version ver = null;

                if (!string.IsNullOrWhiteSpace(version))
                {
                    ver = new Version(version);
                }

                // Execute the policy
                var policyResultFact = BusinessRuleEnginePolicyEvaluator.GetBamActivityPolicy(policyName, ver, activityName, stepName);

                System.Diagnostics.Debug.Write("[ResolutionService] - Returned a BAM interception policy from BRE.");

                return policyResultFact;
            }
            catch (Exception ex)
            {
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("Application", ex.ToString(), System.Diagnostics.EventLogEntryType.Error, 2);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }

                throw;
            }
        }
    }
}
