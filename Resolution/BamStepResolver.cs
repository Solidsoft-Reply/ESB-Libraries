// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BamStepResolver.cs" company="Solidsoft Reply Ltd.">
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
    using System.Collections;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Caching;
    using System.ServiceModel;

    using Microsoft.BizTalk.Bam.EventObservation;

    using SolidsoftReply.Esb.Libraries.Resolution.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution.ResolutionService;

    using ActivityInterceptorConfiguration = Microsoft.BizTalk.Bam.EventObservation.ActivityInterceptorConfiguration;
    using TrackPoint = SolidsoftReply.Esb.Libraries.Resolution.ResolutionService.TrackPoint;

    /// <summary>
    /// Class to resolve BAM step directives.
    /// </summary>
    [Serializable]
    public class BamStepResolver
    {
        /// <summary>
        /// A bam interceptor.
        /// </summary>
        private BAMInterceptor bamInterceptor;

        /// <summary>
        /// Get a named BAM step.
        /// </summary>
        /// <param name="activityName">BAM activity name</param>
        /// <param name="stepName">The BAM step name.</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <returns>A BAM activity step.</returns>
        public static BamActivityStep GetStep(string activityName, string stepName, string policyName, Version version)
        {
            return DoGetStep(activityName, stepName, policyName, version);
        }

        /// <summary>
        /// Gets a BAM interceptor for the step.
        /// </summary>
        /// <param name="activityName">
        /// BAM activity name
        /// </param>
        /// <param name="stepName">
        /// The BAM step name.
        /// </param>
        /// <returns>
        /// Return a BAMInterceptor object
        /// </returns>
        public BAMInterceptor GetInterceptor(string activityName, string stepName)
        {
            return this.DoGetBamInterceptor(activityName, stepName, null, null);
        }

        /// <summary>
        /// Gets a BAM interceptor for the step.
        /// </summary>
        /// <param name="activityName">BAM activity name</param>
        /// <param name="stepName">The BAM step name.</param>
        /// <param name="policyName">Policy name</param>
        /// <returns>Return a BAMInterceptor object</returns>
        public BAMInterceptor GetInterceptor(string activityName, string stepName, string policyName)
        {
            return this.DoGetBamInterceptor(activityName, stepName, policyName, null);
        }

        /// <summary>
        /// Gets a BAM interceptor for the step.
        /// </summary>
        /// <param name="activityName">BAM activity name</param>
        /// <param name="stepName">The BAM step name.</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <returns>Return a BAMInterceptor object</returns>
        public BAMInterceptor GetInterceptor(string activityName, string stepName, string policyName, Version version)
        {
            return this.DoGetBamInterceptor(activityName, stepName, policyName, version);
        }

        /// <summary>
        /// Get a named BAM step.
        /// </summary>
        /// <param name="activityName">BAM activity name</param>
        /// <param name="stepName">The BAM step name.</param>
        /// <param name="policyName">Policy name</param>
        /// <param name="version">Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <returns>A BAM activity step.</returns>
        private static BamActivityStep DoGetStep(string activityName, string stepName, string policyName, Version version)
        {
            // Check parameters
            if (string.IsNullOrEmpty(activityName))
            {
                throw new ArgumentException(Resources.ExceptionBamActivityNameInvalid);
            }

            if (string.IsNullOrEmpty(policyName))
            {
                try
                {
                    policyName = ConfigurationManager.AppSettings[Resources.AppSettingEsbBamDefaultTrackpointPolicy];
                }
                catch
                {
                    // TODO: Configuration error - log this.
                    policyName = string.Empty;
                }

                if (string.IsNullOrEmpty(policyName))
                {
                    // Assume that the current directive name is being used:
                    throw new ArgumentException(Resources.ExceptionBamTrackpointPolicyUndetermined);
                }
            }

            var ver = string.Empty;

            try
            {
                ver = version == null
                          ? ConfigurationManager.AppSettings[Resources.AppSettingEsbBamDefaultTrackpointPolicyVersion]
                          : version.ToString(2);
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // TODO: Configuration error - log this.
            }

            BamActivityStep bamActivityStep;

            var key = string.Format(Resources.BamActivityKey, activityName, stepName, policyName, string.IsNullOrWhiteSpace(ver) ? string.Empty : "_" + ver);

            // Get from the cache if is there
            if (Resolver.DirectivesCache.Contains(key))
            {
                bamActivityStep = Resolver.DirectivesCache.GetBamActivityStep(key);
            }
            else
            {
                // Call web service
                var svc = new ResolverClient();

                try
                {
                    svc.Endpoint.Address = new EndpointAddress(ConfigurationManager.AppSettings[Resources.AppSettingEsbServiceEndPoint]);
                }
                catch
                {
                    // TODO: Configuration error - log this.
                    svc.Endpoint.Address = new EndpointAddress(string.Empty);
                }

                //////svc.Credentials = System.Net.CredentialCache.DefaultCredentials;

                bamActivityStep = svc.GetInterceptionPolicy(activityName, stepName, policyName, ver);

                if (bamActivityStep.TrackPoints == null)
                {
                    // This was originally treated as an exception, but this was too
                    // restrictive.  It is entirely permissable to have no trackpoint
                    // configuration for a given BAM step.
                    bamActivityStep.TrackPoints = new ArrayList(0);
                }
            }

            if (bamActivityStep == null)
            {
                throw new EsbResolutionException(Resources.ExceptionNoBamActivity);
            }

            try
            {
                Resolver.DirectivesCache.Add(
                    key,
                    bamActivityStep,
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
            catch
            {
                // TODO: Configuration error - log this.
            }

            return bamActivityStep;
        }

        /// <summary>
        /// Gets a BAM interceptor for the step.
        /// </summary>
        /// <param name="activityName">BAM activity name</param>
        /// <param name="stepName">The BAM step name.</param>
        /// <param name="policyName">BAM Policy name</param>
        /// <param name="version">BAM Policy version in the format of x.y where x is the major and y is the minor version number</param>
        /// <returns>Return a BAMInterceptor object</returns>
        private BAMInterceptor DoGetBamInterceptor(string activityName, string stepName, string policyName, Version version)
        {
            // TraceHelper.TraceMessage("Resolver.GetInterceptor - In");
            if (this.bamInterceptor != null)
            {
                return this.bamInterceptor;
            }

            var bamActivityStep = DoGetStep(activityName, stepName, policyName, version);
            var interceptor = new BAMInterceptor();

            // Because of a logic error in Microsoft's code, a separate ActivityInterceptorConfiguration must be used 
            // for each location.  The following code extracts only those track points for a given step name (location).
            var trackPointGroup = from TrackPoint tp in bamActivityStep.TrackPoints
                                  where (string)tp.Location == stepName
                                  select tp;
            var bamActivityInterceptorConfig = new ActivityInterceptorConfiguration(activityName);

            foreach (var trackPoint in trackPointGroup)
            {
                switch (trackPoint.Type)
                {
                    case TrackPointType.Start:
                        bamActivityInterceptorConfig.RegisterStartNew(trackPoint.Location, trackPoint.ExtractionInfo);
                        break;
                    case TrackPointType.Reference:
                        bamActivityInterceptorConfig.RegisterReference(trackPoint.ItemName, trackPoint.ReferenceType, trackPoint.Location, trackPoint.ExtractionInfo);
                        break;
                    case TrackPointType.Relationship:
                        bamActivityInterceptorConfig.RegisterRelationship(trackPoint.ItemName, trackPoint.Location, trackPoint.ExtractionInfo);
                        break;
                    case TrackPointType.EnableContinuation:
                        if (string.IsNullOrWhiteSpace(trackPoint.ItemName))
                        {
                            bamActivityInterceptorConfig.RegisterEnableContinuation(trackPoint.Location, trackPoint.ExtractionInfo);
                        }
                        else
                        {
                            bamActivityInterceptorConfig.RegisterEnableContinuation(
                                trackPoint.Location,
                                trackPoint.ExtractionInfo,
                                trackPoint.ItemName);
                        }

                        break;
                    case TrackPointType.Continue:
                        if (string.IsNullOrWhiteSpace(trackPoint.ItemName))
                        {
                            bamActivityInterceptorConfig.RegisterContinue(trackPoint.Location, trackPoint.ExtractionInfo);
                        }
                        else
                        {
                            bamActivityInterceptorConfig.RegisterContinue(trackPoint.Location, trackPoint.ExtractionInfo, trackPoint.ItemName);
                        }

                        break;
                    case TrackPointType.Data:
                        bamActivityInterceptorConfig.RegisterDataExtraction(trackPoint.ItemName, trackPoint.Location, trackPoint.ExtractionInfo);
                        break;
                    case TrackPointType.End:
                        bamActivityInterceptorConfig.RegisterEnd(trackPoint.Location);
                        break;
                }
            }

            bamActivityInterceptorConfig.UpdateInterceptor(interceptor);

            // #if !DEBUG1
            //            // Add to the local cache
            //            DirectivesCache.Add(key.ToString(), interceptor);
            // #endif
            this.bamInterceptor = interceptor;

            Debug.Write("[Resolver] GetInterceptor - Returned a BAM interceptor for activity " + activityName);

            return interceptor;
        }
    }
}
