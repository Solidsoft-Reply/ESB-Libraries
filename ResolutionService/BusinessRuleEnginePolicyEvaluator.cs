// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusinessRuleEnginePolicyEvaluator.cs" company="Solidsoft Reply Ltd.">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using Microsoft.RuleEngine;
    using Microsoft.Win32;

    /// <summary>
    /// Internal class to access the BRE
    /// </summary>
    internal class BusinessRuleEnginePolicyEvaluator
    {
        /// <summary>
        /// Dictionary of rule engine configuration values.
        /// </summary>
        private static readonly IDictionary ConfigValues;

        /// <summary>
        /// Initializes static members of the <see cref="BusinessRuleEnginePolicyEvaluator"/> class.
        /// </summary>
        static BusinessRuleEnginePolicyEvaluator()
        {
            ConfigValues = ConfigurationManager.GetSection("Microsoft.RuleEngine") as IDictionary;
        }

        /// <summary>
        /// Evaluate the policy
        /// </summary>
        /// <param name="policyName">
        /// The name of the policy.
        /// </param>
        /// <param name="version">
        /// The policy version.
        /// </param>
        /// <param name="providerName">
        /// The provider name
        /// </param>
        /// <param name="serviceName">
        /// The service name
        /// </param>
        /// <param name="bindingAccessPoint">
        /// The binding access point.
        /// </param>
        /// <param name="bindingUrlType">
        /// The binding URL type.
        /// </param>
        /// <param name="messageType">
        /// The message type.
        /// </param>
        /// <param name="operationName">
        /// The operation name
        /// </param>
        /// <param name="messageRole">
        /// The role of the message.
        /// </param>
        /// <param name="parameters">
        /// A dictionary of parameters.
        /// </param>
        /// <param name="messageDirection">
        /// The direction of the message flow.
        /// </param>
        /// <returns>
        /// An interchange object containing the information resolved.
        /// </returns>
        internal static Facts.Interchange Eval(
            string policyName,
            Version version,
            string providerName,
            string serviceName,
            string bindingAccessPoint,
            string bindingUrlType,
            string messageType,
            string operationName,
            string messageRole,
            Facts.Dictionaries.ParametersDictionary parameters,
            Facts.Interchange.MessageDirectionTypes messageDirection)
        {
            var trace = Convert.ToBoolean(ConfigurationManager.AppSettings[Properties.Resources.AppSettingsEsbBreTrace]);
            var tempFileName = string.Empty;

            if (trace)
            {
                var traceFileFolder = ConfigurationManager.AppSettings[Properties.Resources.AppSettingsEsbBreTraceFileLocation];

                if (!string.IsNullOrWhiteSpace(traceFileFolder))
                {
                    while (traceFileFolder.EndsWith(@"\"))
                    {
                        traceFileFolder = traceFileFolder.Substring(0, traceFileFolder.Length - 1);
                    }
                }

                if (string.IsNullOrWhiteSpace(traceFileFolder))
                {
                    traceFileFolder = @".";
                }

                tempFileName = string.Format(
                    @"{0}\DirectivePolicyTrace_{1}_{2}.txt",
                    traceFileFolder,
                    DateTime.Now.ToString("yyyyMMdd"),
                    Guid.NewGuid());
            }

            // Create the array of short-term facts  
            var interchange = new Facts.Interchange 
            {
                ProviderName = providerName,
                ServiceName = serviceName,
                BindingAccessPoint = bindingAccessPoint,
                BindingUrlType = bindingUrlType,
                MessageType = messageType,
                OperationName = operationName,
                MessageRole = messageRole,
                Parameters = parameters,
                MessageDirection = messageDirection
            };

            Func<object> createUddiFactObject = () =>
                {
                    try
                    {
                        var uddiObjHandle = Activator.CreateInstance(Properties.Resources.UddiAssembly, Properties.Resources.UddiInquiryService);

                        return uddiObjHandle != null ? uddiObjHandle.Unwrap() : new object();
                    }
                    catch
                    {
                        return new object();
                    }
                };

            // Determine if static support is being used by rule engine and only assert InquiryServices if not.
            var shortTermFacts = IsStaticSupport() 
                ? new object[] { interchange }
                : new[] { interchange, createUddiFactObject()};

            if (Convert.ToBoolean(ConfigurationManager.AppSettings[Properties.Resources.AppSettingsEsbBrePolicyTester]))
            {
                PolicyTester policyTester = null;

                int min;
                int maj;

                if (version == null)
                {
                    maj = 1;
                    min = 0;
                }
                else
                {
                    maj = version.Major;
                    min = version.Minor;
                }

                try
                {
                    // Use PolicyTester
                    var srs = new SqlRuleStore(GetRuleStoreConnectionString());
                    var rsi = new RuleSetInfo(policyName, maj, min);
                    var ruleSet = srs.GetRuleSet(rsi);

                    if (ruleSet == null)
                    {
                        throw new EsbResolutionServiceException(string.Format(Properties.Resources.ExceptionRsNotInStore, policyName, maj, min));
                    }

                    policyTester = new PolicyTester(ruleSet);

                    if (trace)
                    {
                        // Create the debug tacking object
                        var dti = new DebugTrackingInterceptor(tempFileName);

                        // Execute the policy with trace
                        policyTester.Execute(shortTermFacts, dti);

                        Trace.Write("[BusinessRuleEnginePolicyEvaluator] Eval Trace: " + dti.GetTraceOutput());
                    }
                    else
                    {
                        // Execute the policy
                        policyTester.Execute(shortTermFacts);
                    }
                }
                finally
                {
                    if (policyTester != null)
                    {
                        policyTester.Dispose();
                    }
                }
            }
            else
            {
                var policy = version == null ? new Policy(policyName) : new Policy(policyName, version.Major, version.Minor);

                try
                {
                    if (trace)
                    {
                        // Create the debug tacking object
                        var dti = new DebugTrackingInterceptor(tempFileName);

                        // Execute the policy with trace
                        policy.Execute(shortTermFacts, dti);

                        Trace.Write("[BusinessRuleEnginePolicyEvaluator] Eval Trace: " + dti.GetTraceOutput());
                    }
                    else
                    {
                        // Execute the policy
                        policy.Execute(shortTermFacts);
                    }
                }
                finally
                {
                    policy.Dispose();
                }
            }

            Debug.Write("[BusinessRuleEnginePolicyEvaluator] Eval - Returned # directives: " + interchange.Directives.Count);

            // If any directives are invalid, raise an exception.
            var validityStrings = from directive in interchange.Directives
                                  where !directive.Value.IsValid
                                  select directive.Value.ValidErrors;

            var validityStringList = validityStrings as IList<string> ?? validityStrings.ToList();

            if (validityStringList.Any())
            {
                throw new EsbResolutionServiceException(validityStringList.Aggregate((s1, s2) => s1 + s2).Trim());
            }

            return interchange;
        }

        /// <summary>
        /// Return the BAM policy for a given business activity.
        /// </summary>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version</param>
        /// <param name="activityName">Activity Name</param>
        /// <param name="stepName">Step name</param>
        /// <returns>A BamActivityStep object containing the resolved interceptor configuration information</returns>
        internal static Facts.BamActivityStep GetBamActivityPolicy(string policyName, Version version, string activityName, string stepName)
        {
            var trace = Convert.ToBoolean(ConfigurationManager.AppSettings[Properties.Resources.AppSettingsEsbBreTrace]);
            var tempFileName = string.Empty;

            if (trace)
            {
                var traceFileFolder = ConfigurationManager.AppSettings[Properties.Resources.AppSettingsEsbBreTraceFileLocation];

                if (!string.IsNullOrWhiteSpace(traceFileFolder))
                {
                    while (traceFileFolder.EndsWith(@"\"))
                    {
                        traceFileFolder = traceFileFolder.Substring(0, traceFileFolder.Length - 1);
                    }
                }

                if (string.IsNullOrWhiteSpace(traceFileFolder))
                {
                    traceFileFolder = @".";
                }

                tempFileName = string.Format(
                    @"{0}\BAMPolicyTrace_{1}_{2}.txt",
                    traceFileFolder,
                    DateTime.Now.ToString("yyyyMMdd"),
                    Guid.NewGuid());
            }

            // Create the array of short-term facts  
            var bamActivityStep = new Facts.BamActivityStep(activityName, stepName);
            var shortTermFacts = new object[] { bamActivityStep };

            if (Convert.ToBoolean(ConfigurationManager.AppSettings[Properties.Resources.AppSettingsEsbBrePolicyTester]))
            {
                PolicyTester policyTester = null;

                int min;
                int maj;

                if (version == null)
                {
                    maj = 1;
                    min = 0;
                }
                else
                {
                    maj = version.Major;
                    min = version.Minor;
                }

                try
                {
                    // Use PolicyTester
                    var srs = new SqlRuleStore(GetRuleStoreConnectionString());
                    var rsi = new RuleSetInfo(policyName, maj, min);
                    var ruleSet = srs.GetRuleSet(rsi);

                    if (ruleSet == null)
                    {
                        throw new EsbResolutionServiceException(
                            string.Format(Properties.Resources.ExceptionRsNotInStore, policyName, maj, min));
                    }

                    policyTester = new PolicyTester(ruleSet);

                    if (trace)
                    {
                        // Create the debug tracking object
                        var dti = new DebugTrackingInterceptor(tempFileName);

                        // Execute the policy with trace
                        policyTester.Execute(shortTermFacts, dti);

                        Trace.Write("[BusinessRuleEnginePolicyEvaluator] GetBamActivityPolicy Trace: " + dti.GetTraceOutput());
                    }
                    else
                    {
                        // Execute the policy
                        policyTester.Execute(shortTermFacts);
                    }
                }
                finally
                {
                    if (policyTester != null)
                    {
                        policyTester.Dispose();
                    }
                }
            }
            else
            {
                var policy = version == null
                                 ? new Policy(policyName)
                                 : new Policy(policyName, version.Major, version.Minor);

                try
                {
                    if (trace)
                    {
                        // Create the debug tacking object
                        var dti = new DebugTrackingInterceptor(tempFileName);

                        // Execute the policy with trace
                        policy.Execute(shortTermFacts, dti);

                        Trace.Write("[BusinessRuleEnginePolicyEvaluator] GetBamActivityPolicy Trace: " + dti.GetTraceOutput());
                    }
                    else
                    {
                        // Execute the policy
                        policy.Execute(shortTermFacts);
                    }
                }
                finally
                {
                    policy.Dispose();
                }
            }

            Debug.Write("[BusinessRuleEnginePolicyEvaluator] GetBamActivityPolicy - Returned a BAM policy.");

            return bamActivityStep;
        }

        /// <summary>
        /// Indicates whether static support has been configured for the rule engine. 
        /// </summary>
        /// <returns>
        /// True, if static support is being used.  Otherwise false.
        /// </returns>
        private static bool IsStaticSupport()
        {
            const string Key = "StaticSupport";

            if (ConfigValues != null && ConfigValues.Contains(Key))
            {
                return int.Parse((string)ConfigValues[Key], CultureInfo.CurrentCulture) > 0;
            }

            // Test width of integer pointer to determine 32 or 64 bit OS.
            var size = IntPtr.Size == 8;
            var registryKey = Registry.LocalMachine.OpenSubKey(
                size
                ? "Software\\Wow6432Node\\Microsoft\\BusinessRules\\3.0"
                : "Software\\Microsoft\\BusinessRules\\3.0");

            if (registryKey == null)
            {
                return false;
            }

            var value = registryKey.GetValue(Key);
            registryKey.Close();

            if (value != null)
            {
                return (int)value > 0;
            }

            return false;
        }

        /// <summary>
        /// Build the connection string to the business rule store
        /// </summary>
        /// <returns>Connection string for the business rule store</returns>
        private static string GetRuleStoreConnectionString()
        {
            RegistryKey regKey = null;
            object dataBaseName;
            object dataBaseServer;

            try
            {
                regKey = Registry.LocalMachine.OpenSubKey(Properties.Resources.RegKey);

                if (regKey == null)
                {
                    regKey = Registry.LocalMachine.OpenSubKey(Properties.Resources.RegKeyWow6432);

                    if (regKey == null)
                    {
                        throw new EsbResolutionServiceException(Properties.Resources.ExceptionNoConnectionParameters);
                    }
                }

                dataBaseName = regKey.GetValue(Properties.Resources.RegKeyDatabaseName);
                dataBaseServer = regKey.GetValue(Properties.Resources.RegKeyDatabaseServer);

                if (dataBaseName == null || dataBaseServer == null)
                {
                    throw new EsbResolutionServiceException(Properties.Resources.ExceptionNoConnectionParameters);
                }
            }
            finally
            {
                if (regKey != null)
                {
                    regKey.Close();
                }
            }

            return string.Format(Properties.Resources.ConnectionString, dataBaseServer, dataBaseName);
        }
    }
}