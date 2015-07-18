// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BamActivityStep.cs" company="Solidsoft Reply Ltd.">
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
        /// The BAM activity step validator.
        /// </summary>
        private readonly BamActivityStepValidator bamActivityStepValidator;

        /// <summary>
        /// Dictionary of the number of times a given property has been set in this directive.
        /// </summary>
        private readonly Dictionary<string, int> actionCounts = new Dictionary<string, int>();

        /// <summary>
        /// Indicates whether the BAM activity step is valid;
        /// </summary>
        private bool isValid = true;

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
            this.bamActivityStepValidator = new BamActivityStepValidator(this);
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
        /// Gets or sets the extended step name (extensions only).
        /// </summary>
        public string ExtendedStepName { get; set; }

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
        /// Gets a value indicating whether the BAM step is valid;
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.isValid;
            }
        }

        /// <summary>
        /// Gets the current validity errors as text;
        /// </summary>
        public string ValidErrors
        {
            get
            {
                return this.isValid ? string.Empty : this.bamActivityStepValidator.ValidErrors;
            }
        }

        /// <summary>
        /// Registers point in the application where the activity begins.
        /// </summary>
        /// <param name="location">The identifier of a location (step) in the application code.</param>
        /// <param name="activityIdExtractionInfo">The callback argument used to extract the activity identifier.</param>
        public new void RegisterStartNew(object location, object activityIdExtractionInfo)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateStart(location, activityIdExtractionInfo));
            this.IncrementActionCount("Start");
            base.RegisterStartNew(location, activityIdExtractionInfo);
        }

        /// <summary>
        /// Registers the track point in the application where the activity events cease.
        /// </summary>
        /// <param name="location">The identifier of a location (step) in the application code.</param>
        public new void RegisterEnd(object location)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateEnd(location));
            this.IncrementActionCount("End");
            base.RegisterEnd(location);
        }

        /// <summary>
        /// Registers step in the application where the activity continues, using a continuation token.
        /// </summary>
        /// <param name="location">The identifier of a location (step) in the application code.</param>
        /// <param name="continuationTokenExtractionInfo">The callback argument to extract the Correlation token.</param>
        public new void RegisterContinue(object location, object continuationTokenExtractionInfo)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateContinue(location, continuationTokenExtractionInfo));
            this.IncrementActionCount("Start");
            base.RegisterContinue(location, continuationTokenExtractionInfo);
        }

        /// <summary>
        /// Registers step in the application where the activity continues, using a continuation token.
        /// </summary>
        /// <param name="location">The identifier of a location (step) in the application code.</param>
        /// <param name="continuationTokenExtractionInfo">The callback argument to extract the Correlation token.</param>
        /// <param name="prefix">
        /// The prefix that makes the ID used as the activity ID and continuation ID unique among 
        /// components of the application that process the same activity instances.
        /// </param>
        public new void RegisterContinue(object location, object continuationTokenExtractionInfo, string prefix)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateContinue(location, continuationTokenExtractionInfo, prefix));
            this.IncrementActionCount("Start");
            base.RegisterContinue(location, continuationTokenExtractionInfo, prefix);
        }

        /// <summary>
        /// Registers the extraction of the continuation token.
        /// </summary>
        /// <param name="location">The identifier of a location (step) in the application code.</param>
        /// <param name="continuationTokenExtractionInfo">The callback argument to extract the Correlation token.</param>
        public new void RegisterEnableContinuation(object location, object continuationTokenExtractionInfo)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateEnableContinuation(location, continuationTokenExtractionInfo));
            base.RegisterEnableContinuation(location, continuationTokenExtractionInfo);
        }

        /// <summary>
        /// Registers the extraction of the continuation token.
        /// </summary>
        /// <param name="location">The identifier of a location (step) in the application code.</param>
        /// <param name="continuationTokenExtractionInfo">The callback argument to extract the Correlation token.</param>
        /// <param name="prefix">
        /// The prefix that makes the ID used as the activity ID and continuation ID unique among 
        /// components of the application that process the same activity instances.
        /// </param>
        public new void RegisterEnableContinuation(object location, object continuationTokenExtractionInfo, string prefix)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateEnableContinuation(location, continuationTokenExtractionInfo, prefix));
            base.RegisterEnableContinuation(location, continuationTokenExtractionInfo, prefix);
        }

        /// <summary>
        /// Registers extraction of a Business Activity Monitoring (BAM) activity item, such as a milestone or data.
        /// </summary>
        /// <param name="itemName">The BAM activity item name. This is the database column name.</param>
        /// <param name="location">Identifier of a location (step) in the application code.</param>
        /// <param name="extractionInfo">The callback argument indicating how to extract the data item.</param>
        public new void RegisterDataExtraction(string itemName, object location, object extractionInfo)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateDataExtraction(itemName, location, extractionInfo));
            base.RegisterDataExtraction(itemName, location, extractionInfo);
        }

        /// <summary>
        /// Registers extraction of activity relationship.
        /// </summary>
        /// <param name="otherActivityName">The name of the other activity item.</param>
        /// <param name="location">The identifier of a location (step) in the application code.</param>
        /// <param name="otherIdExtractionInfo">The callback argument to extract the instance ID for the other activity item.</param>
        public new void RegisterRelationship(string otherActivityName, object location, object otherIdExtractionInfo)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateRelationship(otherActivityName, location, otherIdExtractionInfo));
            base.RegisterRelationship(otherActivityName, location, otherIdExtractionInfo);
        }

        /// <summary>
        /// Registers the reference associated with this instance.
        /// </summary>
        /// <param name="referenceName">The name of the reference.</param>
        /// <param name="referenceType">The type of the reference.</param>
        /// <param name="location">The identifier of a location (step) in the application code.</param>
        /// <param name="instanceIdExtractionInfo">The callback argument to extract the instance ID for the other activity item.</param>
        public new void RegisterReference(string referenceName, string referenceType, object location, object instanceIdExtractionInfo)
        {
            if (location != null && (location.ToString() != this.StepName))
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateReference(referenceName, referenceType, location, instanceIdExtractionInfo));
            base.RegisterReference(referenceName, referenceType, location, instanceIdExtractionInfo);
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
            if (step != this.StepName)
            {
                return;
            }

            this.SetValidity(this.bamActivityStepValidator.ValidateExtend(step, extendedStep));
            this.IncrementActionCount("Start");
            this.RegisterContinueEx(
                step, 
                null,
                string.Format("extends_{0}", extendedStep));
            this.ExtendedStepName = extendedStep;
        }

        /// <summary>
        /// Returns the number of times a given action has been set.
        /// </summary>
        /// <param name="actionName">The name of the action.</param>
        /// <returns>A count of the number of times the action has been set.</returns>
        internal int ActionCount(string actionName)
        {
            int count;

            return this.actionCounts.TryGetValue(actionName, out count) ? count : 0;
        }

        /// <summary>
        /// Increments the number of times a given action has been set.
        /// </summary>
        /// <param name="actionName">The name of the property.</param>
        private void IncrementActionCount(string actionName)
        {
            if (!this.actionCounts.ContainsKey(actionName))
            {
                this.actionCounts.Add(actionName, 0);
            }

            this.actionCounts[actionName] = this.actionCounts[actionName] + 1;
        }

        /// <summary>
        /// Sets the validity flag to false if invalidity is detected.
        /// </summary>
        /// <param name="valid">Validity of a specific validation test.</param>
        private void SetValidity(bool valid)
        {
            if (!valid)
            {
                this.isValid = false;
            }
        }
    }
}
