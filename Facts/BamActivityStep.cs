// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BamActivityStep.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Facts
{
    using System;
    using System.Collections;
    using System.Collections.Generic; 
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    using Microsoft.BizTalk.Bam.EventObservation;
    
    /// <summary>
    /// Represents an activity as a fact.  The policy for the named activity
    /// will register BAM tracking points to pass back to the resolver.
    /// </summary>
    /// <remarks>
    /// This class subclasses the ActivityInterceptorConfiguration and surfaces
    /// the activity name in order to support simple rules that test for the activity name.
    /// </remarks>
    [Serializable]
    [XmlInclude(typeof(TrackPoint))]
    public class BamActivityStep : ActivityInterceptorConfiguration
    {
        /// <summary>
        /// A list of steps that extend the step specified in the StepName property.
        /// </summary>
        private readonly List<string> extensionSteps = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BamActivityStep"/> class. 
        /// </summary>
        public BamActivityStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BamActivityStep"/> class. 
        /// Constructor.
        /// </summary>
        /// <param name="activityName">
        /// Name of activity.
        /// </param>
        /// <param name="stepName">
        /// Name of activity step.
        /// </param>
        public BamActivityStep(string activityName, string stepName)
            : base(activityName)
        {
            this.ActivityName = activityName;
            this.StepName = stepName;
        }
        
        /// <summary>
        /// Gets or sets the activity name.
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// Gets or sets the BAM step name.
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// Gets or sets a list of steps that extend the step specified in the StepName property.
        /// </summary>
        public List<string> ExtensionSteps
        {
            // NB. The type is List<string> rather than IList<string> in order to be serialisable.
            get
            {
                return this.extensionSteps;
            }

            set
            {
                this.extensionSteps.Clear();

                if (value == null)
                {
                    return;
                }

                foreach (var item in value)
                {
                    this.extensionSteps.Add(item);
                }
            }
        }

        /// <summary>
        /// Gets the TrackPoints.
        /// </summary>
        [XmlElement("TrackPoint", typeof(TrackPoint))]
        public new ArrayList TrackPoints
        {
            get
            {
                return base.TrackPoints;
            }
        }

        /// <summary>
        /// Registers the start of a new activity.
        /// </summary>
        /// <param name="location">The named location.</param>
        /// <param name="activityIdExtractionInfo">The extraction information of the activity ID.</param>
        public void RegisterStartNewEx(object location, object activityIdExtractionInfo)
        {
            this.RegisterStartNew(location, activityIdExtractionInfo);
            this.RegisterEnd(location);
        }

        /// <summary>
        /// Registers the continuation of an existing activity.
        /// </summary>
        /// <param name="location">The named location.</param>
        /// <param name="continuationTokenExtractionInfo">The extraction information of the continuation ID.</param>
        public void RegisterContinueEx(object location, object continuationTokenExtractionInfo)
        {
            this.RegisterContinueEx(location, continuationTokenExtractionInfo, null);
        }

        /// <summary>
        /// Registers the continuation of an existing activity.
        /// </summary>
        /// <param name="location">The named location.</param>
        /// <param name="continuationTokenExtractionInfo">The extraction information of the activity ID.</param>
        /// <param name="prefix">A unique prefix used to convert the activity ID to a continuation ID.</param>
        public void RegisterContinueEx(object location, object continuationTokenExtractionInfo, string prefix)
        {
            if (location == null || string.IsNullOrWhiteSpace((string)location))
            {
                throw new EsbFactsException(
                    string.Format(Properties.Resources.ExceptionInvalidBamStep, "continuation", prefix ?? string.Empty));
            }

            // Tokenise the whitespace in a string.  We will deliberately replace each individual
            // whitespace character with an underscore, rather han grouping whitespace characters.
            // This handles situations where a developer decides to treat whitespace as significant.
            Func<string, string> tokenizeWhitespace = str => Regex.Replace(str, @"\s", "_");

            this.RegisterContinue(
                location, 
                continuationTokenExtractionInfo,
                prefix == null ? null : tokenizeWhitespace(prefix));
            this.RegisterEnd(location);
        }

        /// <summary>
        /// Registers the extension of an existing activity.  This is really a continuation.  The prefix is 
        /// manufactured using the name of another step.
        /// </summary>
        /// <param name="step">The name of step to be extended.</param>
        /// <param name="extendedStep">The name of the step that is being extended.</param>
        public void RegisterExtendStep(string step, string extendedStep)
        {
            if (string.IsNullOrEmpty(extendedStep))
            {
                throw new EsbFactsException(
                    string.Format(Properties.Resources.ExceptionInvalidBamExtendedStepName, "Extended step name is null or empty."));
            }

            // Step names are case sensitive, so we can perform a simple comparison.
            if (step == extendedStep)
            {
                throw new EsbFactsException(
                    string.Format(Properties.Resources.ExceptionInvalidBamExtendedStepName, string.Format("Extended step name is same as step name: {0}", extendedStep)));
            }

            this.RegisterContinueEx(
                step, 
                null,
                string.Format("extends_{0}", extendedStep));

            this.ExtensionSteps.Add(step);
        }
    }
}
