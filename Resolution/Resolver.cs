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

namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Caching;
    using System.ServiceModel;
    using System.Text;

    using SolidsoftReply.Esb.Libraries.Resolution.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution.ResolutionService;

    using Parameters = SolidsoftReply.Esb.Libraries.Resolution.Dictionaries.Parameters;

    /// <summary>
    /// Class to resolve and get info from the Service Directory
    /// </summary>
    [Serializable]
    public class Resolver
    {
        /// <summary>
        /// The cache of directives retrieved from the Resolution service.
        /// </summary>
        internal static readonly DirectivesCache DirectivesCache = new DirectivesCache();

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
            return Resolve(
                facts.ProviderName,
                facts.ServiceName,
                facts.BindingAccessPoint,
                facts.BindingUrlType,
                facts.MessageType,
                facts.OperationName,
                facts.MessageRole,
                facts.MessageDirection,
                policyName,
                policyVersion,
                facts.Parameters);
        }

        /// <summary>
        /// Invalidate the cache
        /// </summary>
        public static void InvalidateCache()
        {
            DirectivesCache.InvalidateCache();
        }
        
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
        /// <param name="messageDirection">Direction of the message</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="policyVersion">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <param name="parameters">Parameter items</param>
        /// <returns>Return a Directive object with the result</returns>
        private static Directives Resolve(
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            MessageDirectionTypes messageDirection,
            string policyName,
            string policyVersion,
            Parameters parameters)
        {
            // TraceHelper.TraceMessage("Resolver.Resolve - In");

            // Check parameters
            if (string.IsNullOrWhiteSpace(policyName))
            {
                try
                {
                    policyName = ConfigurationManager.AppSettings[Resources.AppSettingEsbDefaultPolicyName];

                    try
                    {
                        policyVersion = ConfigurationManager.AppSettings[Resources.AppSettingEsbDefaultPolicyVersion];
                    }
                    catch (Exception)
                    {
                        policyVersion = string.Empty;
                    }
                }
                catch
                {
                    // TODO: Log the exception
                    throw new ArgumentException(string.Format(Resources.ExceptionEsbPolicyUndetermined, Resources.AppSettingEsbDefaultPolicyName));
                }
            }

            Version version;

            if (!Version.TryParse(policyVersion, out version))
            {
                version = default(Version);
            }

            // Build the key
            var key = new StringBuilder(512);

            if (!string.IsNullOrEmpty(providerName))
            {
                key.Append("1;");
                key.Append(providerName);
                key.Append(";");
            }

            if (!string.IsNullOrEmpty(serviceName))
            {
                key.Append("2;");
                key.Append(serviceName);
                key.Append(";");
            }

            if (!string.IsNullOrEmpty(bindingAccessPoint))
            {
                key.Append("3;");
                key.Append(bindingAccessPoint);
                key.Append(";");
            }

            if (!string.IsNullOrEmpty(bindingUrlType))
            {
                key.Append("4;");
                key.Append(bindingUrlType);
                key.Append(";");
            }

            if (!string.IsNullOrEmpty(messageType))
            {
                key.Append("5;");
                key.Append(messageType);
                key.Append(";");
            }

            if (!string.IsNullOrEmpty(operationName))
            {
                key.Append("6;");
                key.Append(operationName);
                key.Append(";");
            }

            if (!string.IsNullOrEmpty(messageRole))
            {
                key.Append("7;");
                key.Append(messageRole);
                key.Append(";");
            }

            if (!string.IsNullOrEmpty(messageDirection.ToString()))
            {
                key.Append("8;");
                key.Append(messageDirection);
                key.Append(";");
            }

            var keyParams = new StringBuilder(256);

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var paramItem in parameters.Where(paramItem => !string.IsNullOrEmpty(paramItem.Key)))
                {
                    keyParams.Append(paramItem.Key);
                    keyParams.Append(";");

                    if (paramItem.Value == null)
                    {
                        continue;
                    }

                    keyParams.Append(paramItem.Value);
                    keyParams.Append(";");
                }
            }

            if (keyParams.Length > 0)
            {
                key.Append("9;");
                key.Append(keyParams);
                key.Append(";");
            }

            if (!string.IsNullOrEmpty(policyName))
            {
                key.Append("10;");
                key.Append(policyName);
                key.Append(";");
            }

            if (version != null)
            {
                key.Append("11;");
                key.Append(version.ToString(2));
                key.Append(";");
            }

            // Get from the cache if is there
            if (DirectivesCache.Contains(key.ToString()))
            {
                var resolverResults = DirectivesCache.GetResolverResults(key.ToString());
                Debug.Write("[Resolver] Resolve - Returned # elements from the cache: '" + resolverResults.Count + "'");
                return resolverResults;
            }

            // Call web service
            var svc = new ResolverClient();

            try
            {
                svc.Endpoint.Address = new EndpointAddress(ConfigurationManager.AppSettings[Resources.AppSettingEsbServiceEndPoint]);
            }
            catch
            {
                // TODO: Log the error
                throw new EsbResolutionException(
                    string.Format(
                    "No {0} app setting provided for resolution service.  Check the config file.", 
                    Resources.AppSettingEsbServiceEndPoint));
            }

            //////svc.ClientCredentials = System.Net.CredentialCache.DefaultCredentials;
            var ver = version == null ? null : version.ToString(2);

            var resolverResultFromWs = svc.Resolve(
                providerName,
                serviceName,
                bindingAccessPoint,
                bindingUrlType,
                messageType,
                operationName,
                messageRole,
                parameters == null || parameters.Count == 0 ? null : parameters,
                messageDirection,
                policyName,
                ver);

            var resolverLocalResult = new Directives(resolverResultFromWs);

            // Add to the local cache
            try
            {
                DirectivesCache.Add(
                    key.ToString(),
                    resolverLocalResult,
                    new CacheItemPolicy
                        {
                            SlidingExpiration =
                                new TimeSpan(
                                    ConfigurationManager.AppSettings.GetValues(Resources.AppSettingEsbCacheExpiration) == null
                                    ? 24
                                    : Convert.ToInt32(
                                        ConfigurationManager.AppSettings[Resources.AppSettingEsbCacheExpiration]),
                                    0,
                                    0)
                        });
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // TODO: Log the exception
            }

            Debug.Write("[Resolver] Resolve - Returned # elements: " + resolverResultFromWs.Directives == null ? 0 : resolverResultFromWs.Directives.Length);

            return resolverLocalResult;
        }
    }
}
