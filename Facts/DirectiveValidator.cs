// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectiveValidator.cs" company="Solidsoft Reply Ltd.">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Validates a directive during an interchange.
    /// </summary>
    [Serializable]
    internal class DirectiveValidator
    {
        /// <summary>
        /// List of validation error messages for the directive
        /// </summary>
        private readonly StringBuilder errorStrings = new StringBuilder();

        /// <summary>
        /// The directive to be validated
        /// </summary>
        private readonly Directive directive;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectiveValidator"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        public DirectiveValidator(Directive directive)
        {
            this.directive = directive;
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
        /// Validates the URI for resolved endpoint.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateKeyName()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("EndPoint") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionEndPoint, this.directive.KeyName));
            }

            // Test to ensure the SOAP action is a URI
            if (Uri.IsWellFormedUriString(this.directive.EndPoint, UriKind.RelativeOrAbsolute))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionEndPointInvalidUri, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the URI for resolved endpoint.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateEndPoint()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("EndPoint") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionEndPoint, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.EndPoint))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidEndpoint, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the transport type specifier.  In the context of BizTalk Server, this should be 
        /// an adapter schema - e.g., 'FILE', 'WCF-BasicHttp'.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public bool ValidateTransportType()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("TransportType") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionTransportType, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.TransportType))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidEndpointTransport, this.directive.KeyName, this.directive.EndPoint));
            return false;
        }

        /// <summary>
        /// Validates the BTS endpoint configuration.  This may be a token or raw configuration text.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateEndPointConfiguration()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("EndPointConfiguration") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionEndPointConfiguration, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.EndPointConfiguration))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(
                string.Format(
                    Properties.Resources.ExceptionInvalidEndpointConfiguration,
                    this.directive.KeyName,
                    this.directive.EndPoint,
                    this.directive.TransportType));

            return false;
        }
        
        /// <summary>
        /// Validates the SOAP action for the endpoint.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateSoapAction()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("SoapAction") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionSoapAction, this.directive.KeyName));
            }

            if (string.IsNullOrWhiteSpace(this.directive.SoapAction))
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidSoapAction, this.directive.KeyName));
            }

            // Test to ensure the SOAP action is a URI
            if (Uri.IsWellFormedUriString(this.directive.SoapAction, UriKind.RelativeOrAbsolute))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionSoapActionInvalidUri, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the name of a map to apply.  In the context of BizTalk Server, this will be the 
        /// fully qualified .NET assembly name and type for map.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateMapToApply()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("MapToApply") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionMapToApply, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.MapToApply))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidTransformationMap, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the format to apply to XML content.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateXmlFormat()
        {
            if (this.directive.PropertySetCount("XmlFormat") <= 1)
            {
                return true;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionXmlFormat, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the name of BAM activity for which a BAM interception tracking point will be created.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateBamActivity()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("BamActivity") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamActivity, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.BamActivity))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamInterceptionActivity, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the name of a BAM step ('location') within an activity.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateBamStepName()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("BamStepName") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamStep, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.BamStepName))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamInterceptionStepName, this.directive.KeyName));

            return false;
        }

        /// <summary>
        /// Validates the name of each BAM step extension within an activity.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateBamStepExtensions()
        {
            var isValid = true;

            if (string.IsNullOrWhiteSpace(this.directive.BamStepName))
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamExtendedStepNoStepName, this.directive.KeyName));
            }

            if (!this.directive.BamStepExtensions.Any(string.IsNullOrWhiteSpace))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamInterceptionStepExtensionName, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the name of each BAM after-map step extension within an activity.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateBamAfterMapStepExtensions()
        {
            var isValid = true;

            if (string.IsNullOrWhiteSpace(this.directive.BamAfterMapStepName))
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamAfterMapExtendedStepNoStepName, this.directive.KeyName));
            }

            if (!this.directive.BamAfterMapStepExtensions.Any(string.IsNullOrWhiteSpace))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamInterceptionAfterMapStepExtensionName, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the name of conceptual BAM step ('location') within an activity that will
        /// be placed after a transformation
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateBamAfterMapStepName()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("BamAfterMapStepName") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamPostTransformationStep, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.BamAfterMapStepName))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamInterceptionPostTransformationStepName, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the connection string for BAM.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateBamConnectionString()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("BamConnectionString") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamConnectionString, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.BamConnectionString))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamConnectionstring, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates a value indicating whether BAM will use a buffered event stream.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateBamIsBuffered()
        {
            if (this.directive.PropertySetCount("BamIsBuffered") <= 1)
            {
                return true;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamIsBufferedFlag, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates a value that determines under what conditions the buffered 
        /// data will be sent to the tracking database.
        /// </summary>
        /// <remarks>
        /// &lt;= 0 This value is not allowed.   If set to 0, the BufferedEventStream 
        ///         would never flush automatically and the application would have to
        ///         call the Flush method explicitly.   There is no obvious way to do
        ///         this in most common resolution scenarios
        /// 1       Each event will be immediately persisted in the BAM database. 
        /// &gt; 1  The BufferedEventStream will accumulate the events in memory until the 
        ///         event count equals or exceeds this threshold; at this point, the Flush 
        ///         method will be called internally. 
        /// </remarks>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateBamFlushThreshold()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("BamFlushThreshold") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamFlushThreshold, this.directive.KeyName));
            }

            if (this.directive.BamFlushThreshold >= 1)
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamFlushThresholdLt1, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the name of the BAM Trackpoint policy.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public bool ValidateBamTrackpointPolicyName()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("BamTrackpointPolicyName") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamTrackpointPolicyName, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.BamTrackpointPolicyName))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamTrackpointPolicyName, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the version of the BAM Trackpoint policy.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public bool ValidateBamTrackpointPolicyVersion()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("BamTrackpointPolicyVersion") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamTrackpointPolicyVersion, this.directive.KeyName));
            }

            if (string.IsNullOrWhiteSpace(this.directive.BamTrackpointPolicyVersion))
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidBamTrackpointPolicyVersion, this.directive.KeyName));
            }

            // Test to ensure the value is a valid Version specifier.
            Version version;

            if (Version.TryParse(this.directive.BamTrackpointPolicyVersion, out version))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionBamTrackpointPolicyVersionInvalid, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the number of retries to perform on failure
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateRetryCount()
        {
            if (this.directive.PropertySetCount("RetryCount") <= 1)
            {
                return true;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionRetryCount, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the number of minutes between each retry.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateRetryInterval()
        {
            if (this.directive.PropertySetCount("RetryInterval") <= 1)
            {
                return true;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionRetryInterval, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the level specifier for a retry.   
        /// </summary>
        /// <remarks>
        /// This may be used in scenarios where the system should perform a series of 
        /// retries at one level, and then 'wrap' these in additional levels of retry 
        /// over successively longer time intervals.  E.g., a policy might state that 
        /// the system should retry three times with an interval of on minute, then 
        /// perform an additional set of retries once an hour for five hours.
        /// </remarks>
        /// <returns>True if valid; otherwise false.</returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public bool ValidateRetryLevel()
        {
            if (this.directive.PropertySetCount("RetryLevel") <= 1)
            {
                return true;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionRetryLevel, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the time at which service window opens.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateServiceWindowStartTime()
        {
            if (this.directive.PropertySetCount("ServiceWindowStartTime") <= 1)
            {
                return true;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionServiceWindowStartTime, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the time at which service window closes
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateServiceWindowStopTime()
        {
            if (this.directive.PropertySetCount("ServiceWindowStopTime") <= 1)
            {
                return true;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionServiceWindowStartTime, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the name of the validation policy.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateValidationPolicyName()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("ValidationPolicyName") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionValidationPolicyName, this.directive.KeyName));
            }

            if (!string.IsNullOrWhiteSpace(this.directive.ValidationPolicyName))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidValidationPolicyName, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates the version of the validation policy.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateValidationPolicyVersion()
        {
            var isValid = true;

            if (this.directive.PropertySetCount("ValidationPolicyVersion") > 1)
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionValidationPolicyVersion, this.directive.KeyName));
            }

            if (string.IsNullOrWhiteSpace(this.directive.ValidationPolicyVersion))
            {
                isValid = false;
                this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionInvalidValidationPolicyVersion, this.directive.KeyName));
            }

            // Test to ensure the value is a valid Version specifier.
            Version version;

            if (Version.TryParse(this.directive.ValidationPolicyVersion, out version))
            {
                return isValid;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionValidationPolicyVersionInvalid, this.directive.KeyName));
            return false;
        }

        /// <summary>
        /// Validates a directive.  This method handles any validations that must compare across
        /// multiple directive instructions.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateDirective()
        {
            var isValid = true;

            if (this.directive.BamStepExtensions.Count > 0)
            {
                isValid = this.ValidateBamStepExtensions();
            }

            if (this.directive.BamAfterMapStepExtensions.Count > 0)
            {
                isValid = this.ValidateBamStepExtensions() && isValid;
            }

            return isValid;
        }

        /// <summary>
        /// Validates a value indicating whether to throw an error if a validation rule policy indicates invalidity.
        /// </summary>
        /// <returns>True if valid; otherwise false.</returns>
        public bool ValidateErrorOnInvalid()
        {
            if (this.directive.PropertySetCount("ErrorOnInvalid") <= 1)
            {
                return true;
            }

            this.errorStrings.AppendLine(string.Format(Properties.Resources.ExceptionErrorOnInvalidFlag, this.directive.KeyName));
            return false;
        }
    }
}
