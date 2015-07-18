// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BamActivityStepValidator.cs" company="Solidsoft Reply Ltd.">
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
    using System.Text;

    using SolidsoftReply.Esb.Libraries.Facts.Properties;

    /// <summary>
    /// Validates a BAM activity step.
    /// </summary>
    [Serializable]
    internal class BamActivityStepValidator
    {
        /// <summary>
        /// List of validation error messages for the BAM activity step.
        /// </summary>
        private readonly StringBuilder errorStrings = new StringBuilder();

        /// <summary>
        /// The BAM activity step to be validated
        /// </summary>
        private readonly BamActivityStep bamActivityStep;

        /// <summary>
        /// Initializes a new instance of the <see cref="BamActivityStepValidator"/> class.
        /// </summary>
        /// <param name="bamActivityStep">
        /// The BAM activity step.
        /// </param>
        public BamActivityStepValidator(BamActivityStep bamActivityStep)
        {
            this.bamActivityStep = bamActivityStep;
        }

        /// <summary>
        /// Gets the current validity errors as text;
        /// </summary>
        public string ValidErrors
        {
            get
            {
                return this.errorStrings.ToString();
            }
        }

        /// <summary>
        /// Validates the Start action for a BAM step.
        /// </summary>
        /// <param name="location">The location name.</param>
        /// <param name="activityIdExtractionInfo">Extraction information for the activity ID.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateStart(object location, object activityIdExtractionInfo)
        {
            var isValid = true;

            if (this.bamActivityStep.ActionCount("Start") > 1)
            {
                isValid = false;

                var message = string.Format(
                    Resources.ExceptionBamStart, 
                    location ?? "<null or empty>", 
                    this.bamActivityStep.ActivityName);

                this.errorStrings.AppendLine(message);
            }

            // Test the step name
            if (!this.ValidateLocation(location, "starting", this.bamActivityStep.ActivityName))
            {
                isValid = false;
            }

            return this.ValidateExtractionInfo(location, "starting", this.bamActivityStep.ActivityName) && isValid;
        }

        /// <summary>
        /// Validates a data extraction action for a BAM step.
        /// </summary>
        /// <param name="itemName">The data item name.</param>
        /// <param name="location">The location name.</param>
        /// <param name="extractionInfo">Extraction information.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateDataExtraction(string itemName, object location, object extractionInfo)
        {
            var isValid = true;
            var actionType = (string)(location ?? "<null or empty>");
            var activityDescription = this.bamActivityStep.ActivityName;

            // Test the item name
            if (string.IsNullOrWhiteSpace(itemName))
            {
                isValid = false;
                var message = string.Format(
                    Resources.ExceptionInvalidDataExtractionItemName,
                    actionType,
                    activityDescription);
                this.errorStrings.AppendLine(message);
            }
            else
            {
                actionType = string.Format("extracting the {0} data item to", itemName);
            }

            // Test the step name
            if (!this.ValidateLocation(location, actionType, activityDescription))
            {
                isValid = false;
            }

            return this.ValidateExtractionInfo(
                location,
                actionType,
                activityDescription) && isValid;
        }

        /// <summary>
        /// Validates a relationship action for a BAM step.
        /// </summary>
        /// <param name="otherActivityName">Name of the related activity</param>
        /// <param name="location">The location name.</param>
        /// <param name="extractionInfo">Extraction information.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateRelationship(string otherActivityName, object location, object extractionInfo)
        {
            var isValid = true;
            var actionType = (string)(location ?? "<null or empty>");
            var activityDescription = this.bamActivityStep.ActivityName;

            // Test the other activity name
            if (string.IsNullOrWhiteSpace(otherActivityName))
            {
                isValid = false;
                var message = string.Format(
                    Resources.ExceptionInvalidRelatedActivityName,
                    actionType,
                    activityDescription);
                this.errorStrings.AppendLine(message);
            }
            else
            {
                actionType = string.Format("relating the {0} activity to", otherActivityName);
            }

            // Test the step name
            if (!this.ValidateLocation(location, actionType, activityDescription))
            {
                isValid = false;
            }

            return this.ValidateExtractionInfo(
                location,
                actionType,
                activityDescription) && isValid;
        }

        /// <summary>
        /// Validates a reference action for a BAM step.
        /// </summary>
        /// <param name="referenceName">Reference name.</param>
        /// <param name="referenceType">Reference type.</param>
        /// <param name="location">The location name.</param>
        /// <param name="extractionInfo">Extraction information.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateReference(string referenceName, string referenceType, object location, object extractionInfo)
        {
            var isValid = true;
            var actionType = (string)(location ?? "<null or empty>");
            var activityDescription = this.bamActivityStep.ActivityName;

            // Test the reference name
            if (string.IsNullOrWhiteSpace(referenceName))
            {
                isValid = false;
                var message = string.Format(
                    Resources.ExceptionInvalidReferenceName,
                    actionType,
                    activityDescription);
                this.errorStrings.AppendLine(message);
            }

            // Test the reference type
            if (string.IsNullOrWhiteSpace(referenceType))
            {
                isValid = false;
                var message = string.Format(
                    Resources.ExceptionInvalidReferenceType,
                    actionType,
                    activityDescription);
                this.errorStrings.AppendLine(message);
            }

            actionType = "registering a reference from";

            // Test the step name
            if (!this.ValidateLocation(location, actionType, activityDescription))
            {
                isValid = false;
            }

            return this.ValidateExtractionInfo(
                location,
                actionType,
                activityDescription) && isValid;
        }

        /// <summary>
        /// Validates an Enable Continue action for a BAM step.
        /// </summary>
        /// <param name="location">The location name.</param>
        /// <param name="continuationTokenExtractionInfo">Extraction information for the continuation token.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateEnableContinuation(object location, object continuationTokenExtractionInfo)
        {
            var isValid = this.ValidateLocation(
                location, 
                "enabling continuation of", 
                this.bamActivityStep.ActivityName);

            return this.ValidateExtractionInfo(
                location,
                "enabling continuation of",
                this.bamActivityStep.ActivityName) && isValid;
        }

        /// <summary>
        /// Validates an Enable Continue action for a BAM step.
        /// </summary>
        /// <param name="location">The location name.</param>
        /// <param name="continuationTokenExtractionInfo">Extraction information for the continuation token.</param>
        /// <param name="prefix">The continuation token prefix.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateEnableContinuation(object location, object continuationTokenExtractionInfo, string prefix)
        {
            var isValid = true;
            var activityDescription = this.bamActivityStep.ActivityName;

            // Test the continuation prefix
            if (this.ValidatePrefix(prefix, "enabling continuation of", activityDescription))
            {
                activityDescription = string.Format(
                    "{0} with prefix {1}",
                    this.bamActivityStep.ActivityName,
                    prefix ?? string.Empty);
            }
            else
            {
                isValid = false;
            }

            // Test the step name
            if (!this.ValidateLocation(location, "enabling continuation of", activityDescription))
            {
                isValid = false;
            }

            return this.ValidateExtractionInfo(
                location,
                "enabling continuation of",
                activityDescription) && isValid;
        }

        /// <summary>
        /// Validates the Continue action for a BAM step.
        /// </summary>
        /// <param name="location">The location name.</param>
        /// <param name="continuationTokenExtractionInfo">Extraction information for the continuation token.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateContinue(object location, object continuationTokenExtractionInfo)
        {
            var isValid = true;

            if (this.bamActivityStep.ActionCount("Start") > 1)
            {
                isValid = false;

                var message = string.Format(
                    Resources.ExceptionBamStart,
                    location ?? "<null or empty>",
                    this.bamActivityStep.ActivityName);

                this.errorStrings.AppendLine(message);
            }

            // Test the step name
            if (!this.ValidateLocation(location, "continuing", this.bamActivityStep.ActivityName))
            {
                isValid = false;
            }

            return this.ValidateExtractionInfo(
                location, 
                "continuing", 
                this.bamActivityStep.ActivityName) && isValid;
        }

        /// <summary>
        /// Validates the Continue action for a BAM step.
        /// </summary>
        /// <param name="location">The location name.</param>
        /// <param name="continuationTokenExtractionInfo">Extraction information for the continuation token.</param>
        /// <param name="prefix">Continuation token prefix.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateContinue(object location, object continuationTokenExtractionInfo, string prefix)
        {
            var isValid = true;

            if (this.bamActivityStep.ActionCount("Start") > 1)
            {
                isValid = false;

                var message = string.Format(
                    Resources.ExceptionBamStart,
                    location ?? "<null or empty>",
                    this.bamActivityStep.ActivityName);

                this.errorStrings.AppendLine(message);
            }

            var activityDescription = this.bamActivityStep.ActivityName;

            // Test the continuation prefix
            if (this.ValidatePrefix(prefix, "continuing", activityDescription))
            {
                activityDescription = string.Format(
                    "{0} with prefix {1}",
                    this.bamActivityStep.ActivityName,
                    prefix ?? string.Empty);
            }
            else
            {
                isValid = false;
            }

            // Test the step name
            if (!this.ValidateLocation(location, "continuing", activityDescription))
            {
                isValid = false;
            }

            return this.ValidateExtractionInfo(
                location, 
                "continuing", 
                activityDescription) && isValid;
        }

        /// <summary>
        /// Validates the Continue action for a BAM step.
        /// </summary>
        /// <param name="step">The extending step.</param>
        /// <param name="extendedStep">The step being extended.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateExtend(string step, string extendedStep)
        {
            var isValid = true;

            if (this.bamActivityStep.ActionCount("Start") > 1)
            {
                isValid = false;

                var message = string.Format(
                    Resources.ExceptionBamStart,
                    step ?? "<null or empty>",
                    this.bamActivityStep.ActivityName);

                this.errorStrings.AppendLine(message);
            }

            var activityDescription = this.bamActivityStep.ActivityName;

            // Step names are case sensitive, so we can perform a simple comparison.
            if (step != extendedStep)
            {
                return this.ValidateLocation(step, "extending", activityDescription) && isValid;
            }

            var invalidBamExtendedStepNameMessage = string.Format(
                Resources.ExceptionInvalidBamExtendedStepName,
                string.Format("Extended step name is same as step name: {0}", extendedStep));

            this.errorStrings.AppendLine(invalidBamExtendedStepNameMessage);

            // Test the step name
            this.ValidateLocation(step, "extending", activityDescription);

            return false;
        }

        /// <summary>
        /// Validates the End action for a BAM step.
        /// </summary>
        /// <param name="location">The location name.</param>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateEnd(object location)
        {
            if (this.bamActivityStep.ActionCount("End") <= 1)
            {
                return this.ValidateLocation(location, "ending", this.bamActivityStep.ActivityName);
            }

            var message = string.Format(
                Resources.ExceptionBamStart,
                location ?? "<null or empty>",
                this.bamActivityStep.ActivityName);

            this.errorStrings.AppendLine(message);

            // Test the step name
            this.ValidateLocation(location, "ending", this.bamActivityStep.ActivityName);
            return false;
        }

        /// <summary>
        /// Validate a location string.
        /// </summary>
        /// <param name="location">The step name.</param>
        /// <param name="actionType">The BAM action type.</param>
        /// <param name="actionDescription">The BAM action description.</param>
        /// <returns>True, if valid; otherwise false.</returns>
        private bool ValidateLocation(object location, string actionType, string actionDescription)
        {
            if (location != null && !string.IsNullOrWhiteSpace((string)location))
            {
                return true;
            }

            var message = string.Format(
                Resources.ExceptionInvalidBamStep,
                actionType,
                actionDescription);
            this.errorStrings.AppendLine(message);
            return false;
        }

        /// <summary>
        /// Validate an extraction information string.
        /// </summary>
        /// <param name="extractionInfo">The extraction information.</param>
        /// <param name="actionType">The BAM action type.</param>
        /// <param name="actionDescription">The BAM action description.</param>
        /// <returns>True, if valid; otherwise false.</returns>
        private bool ValidateExtractionInfo(object extractionInfo, string actionType, string actionDescription)
        {
            if (extractionInfo != null && !string.IsNullOrWhiteSpace((string)extractionInfo))
            {
                return true;
            }

            var message = string.Format(
                Resources.ExceptionInvalidExtractionInfo,
                actionType,
                actionDescription);
            this.errorStrings.AppendLine(message);
            return false;
        }

        /// <summary>
        /// Validate a continuation prefix.
        /// </summary>
        /// <param name="prefix">The continuation prefix.</param>
        /// <param name="actionType">The BAM action type.</param>
        /// <param name="actionDescription">The BAM action description.</param>
        /// <returns>True, if valid; otherwise false.</returns>
        private bool ValidatePrefix(string prefix, string actionType, string actionDescription)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                return true;
            }

            var message = string.Format(
                Resources.ExceptionInvalidPrefix,
                actionType,
                actionDescription);
            this.errorStrings.AppendLine(message);
            return false;
        }
    }
}
