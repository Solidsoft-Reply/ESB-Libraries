// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interchange.cs" company="Solidsoft Reply Ltd.">
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
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Microsoft.XLANGs.BaseTypes;

    using SolidsoftReply.Esb.Libraries.Facts.Dictionaries;

    /// <summary>
    /// Represents an interchange that will be resolved using the service directory.
    /// Objects of this class can be asserted to a policy engine (e.g., MS BRE) as facts.
    /// </summary>
    [XmlRoot("Interchange", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", IsNullable = true)]
    [Serializable]
    public class Interchange
    {
        /// <summary>
        /// Provider name .
        /// </summary>
        private string providerName;

        /// <summary>
        /// Service name.
        /// </summary>
        private string serviceName;

        /// <summary>
        /// Binding access point.
        /// </summary>
        private string bindingAccessPoint;

        /// <summary>
        /// Binding URL type.
        /// </summary>
        private string bindingUrlType;

        /// <summary>
        /// Message type.
        /// </summary>
        private string messageType;

        /// <summary>
        /// Operation name.
        /// </summary>
        private string operationName;

        /// <summary>
        /// Message label.
        /// </summary>
        private string messageRole;

        /// <summary>
        /// Message direction.
        /// </summary>
        private MessageDirectionTypes messageDirection;

        /// <summary>
        /// Collection of general purpose parameters.
        /// </summary>
        private Parameters parameters;

        /// <summary>
        /// Timestamp value.
        /// </summary>
        private DateTime timestamp;

        /// <summary>
        /// A collection of directives representing the results of resolution.
        /// </summary>
        private DirectivesDictionary directives;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interchange"/> class. 
        /// </summary>
        public Interchange()
        {
            this.directives = new DirectivesDictionary();
            this.parameters = new Parameters(); 
        }

        /// <summary>
        /// Enumeration for Message Direction values
        /// </summary>
        public enum MessageDirectionTypes
        {
            /// <summary>
            /// The message direction is not specified.
            /// </summary>
            NotSpecified,

            /// <summary>
            /// The message direction is in.
            /// </summary>
            MsgIn,

            /// <summary>
            /// The message direction is out.
            /// </summary>
            MsgOut,

            /// <summary>
            /// The message direction is both in and out.
            /// </summary>
            Both
        }

        /// <summary>
        /// Categories of settings within a directive
        /// </summary>
        [Flags]
        public enum Categories
        {
            /// <summary>
            /// Endpoint directives.
            /// </summary>
            Endpoint = 1,

            /// <summary>
            /// Transformation directives.
            /// </summary>
            Transformation = 2,

            /// <summary>
            /// BAM interception directives.
            /// </summary>
            BamInterception = 4,

            /// <summary>
            /// Validation directives.
            /// </summary>
            Validation = 8,

            /// <summary>
            /// Retry directives.
            /// </summary>
            Retry = 16,

            /// <summary>
            /// Service window directives.
            /// </summary>
            ServiceWindow = 32
        }

        /* The following properties represent UDDI-style attributes
         * which we use a criterion in deciding the policy which
         * will be applied.   They refelect direct attributes of 
         * UDDI Provider, Service and Binding entities.   Language
         * specifiers are not supported in this version.
         * */

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        public string ProviderName
        {
            get { return this.providerName; }
            set { this.providerName = value; }
        }

        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        public string ServiceName
        {
            get { return this.serviceName; }
            set { this.serviceName = value; }
        }

        /// <summary>
        /// Gets or sets the binding access point.
        /// </summary>
        public string BindingAccessPoint
        {
            get { return this.bindingAccessPoint; }
            set { this.bindingAccessPoint = value; }
        }

        /// <summary>
        /// Gets or sets the binding URL type.
        /// </summary>
        public string BindingUrlType
        {
            get { return this.bindingUrlType; }
            set { this.bindingUrlType = value; }
        }

        /* End of UDDI-style attributes */

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        /// <remarks>
        /// This is a fully qualified type specification for a message.
        /// Will typically be used in respect to BTS messageTypes
        /// </remarks>
        public string MessageType
        {
            get { return this.messageType; }
            set { this.messageType = value; }
        }

        /// <summary>
        /// Gets or sets the operation name.
        /// </summary>
        public string OperationName
        {
            get { return this.operationName; }
            set { this.operationName = value; }
        }

        /// <summary>
        /// Gets or sets the message role.
        /// </summary>
        /// <remarks>
        /// A name that identifies the role of a message within a MEP
        /// for a specific interface operation.  Equivalent to 
        /// messageLabel in WSDL 2.0
        /// </remarks>
        public string MessageRole
        {
            get { return this.messageRole; }
            set { this.messageRole = value; }
        }

        /// <summary>
        /// Gets or sets the message direction
        /// </summary>
        public MessageDirectionTypes MessageDirection
        {
            get { return this.messageDirection; }
            set { this.messageDirection = value; }
        }

        /// <summary>
        /// Gets or sets the collection of general purpose parameters.
        /// </summary>
        public Parameters Parameters
        {
            get { return this.parameters; }
            set { this.parameters = value; }
        }

        /// <summary>
        /// Gets the date and time at which this property is first evaluated.  This can be used to
        /// create rules for service windows.
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                if (this.timestamp == DateTime.MinValue)
                {
                    this.timestamp = DateTime.Now;
                }

                return this.timestamp;
            }
        }

        /// <summary>
        /// Gets or sets a collection of directives representing the results of resolution.
        /// </summary>
        public DirectivesDictionary Directives
        {
            get { return this.directives; }
            set { this.directives = value; }
        }

        /// <summary>
        /// Returns a parameter value from the collection of general purpose parameters.
        /// </summary>
        /// <param name="key">Key value.</param>
        /// <returns>Value of keyed parameter as string.</returns>
        public string GetParameterValue(string key)
        {
            object outValue;
            this.parameters.TryGetValue(key, out outValue);
            return outValue != null ? outValue.ToString() : string.Empty;
        }
        
        /// <summary>
        /// Set the SOAP action for a directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="soapAction">A URI that indicates the intent of the SOAP HTTP request.</param>
        public void SetSoapAction(string directiveKey, string soapAction)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(Properties.Resources.ExceptionInvalidDirective, "SOAP action", soapAction ?? "<null>"));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.SoapAction = soapAction;
            directive.DirectiveCategories |= Categories.Endpoint;
        }

        /// <summary>
        /// Set the resolved endpoint for a directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="endPoint">URI for resolved endpoint.</param>
        public void SetEndpoint(string directiveKey, string endPoint)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(Properties.Resources.ExceptionInvalidDirective, "endpoint", endPoint ?? "<null>"));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.EndPoint = endPoint;
            directive.DirectiveCategories |= Categories.Endpoint;
        }

        /// <summary>
        /// Set the resolved endpoint for a directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="endPoint">URI for resolved endpoint.</param>
        /// <param name="transportType">BTS transport type for resolved endpoint.</param>
        public void SetEndpoint(string directiveKey, string endPoint, string transportType)
        {
            this.SetEndpoint(directiveKey, endPoint);
            var directive = this.GetDirective(directiveKey);
            directive.TransportType = transportType;
        }

        /// <summary>
        /// Set the resolved endpoint for a directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="endPoint">URI for resolved endpoint.</param>
        /// <param name="transportType">BTS transport type for resolved endpoint.</param>
        /// <param name="configurationToken">Configuration token.  This way be literal configuration or a reference.</param>
        public void SetEndpoint(string directiveKey, string endPoint, string transportType, string configurationToken)
        {
            this.SetEndpoint(directiveKey, endPoint, transportType);
            var directive = this.GetDirective(directiveKey);
            directive.EndPointConfiguration = configurationToken;
        }

        /// <summary>
        /// Set the resolved transformation map for a directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="mapToApply">Fully qualified .NET assembly and type name of a BTS map.</param>
        public void SetTransformation(string directiveKey, string mapToApply)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(
                        Properties.Resources.ExceptionInvalidDirective,
                        "transformation map",
                        mapToApply ?? "<null>"));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.MapToApply = mapToApply;
            directive.DirectiveCategories |= Categories.Transformation;
        }

        /// <summary>
        /// Set the resolved XML content formatting requirement for a directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="xmlFormat">Formatting required for XML content.</param>
        public void SetXmlFormat(string directiveKey, XmlFormat xmlFormat)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(
                        Properties.Resources.ExceptionInvalidDirective,
                        "XML formatting of type",
                        xmlFormat));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.XmlFormat = xmlFormat;

            if (xmlFormat != XmlFormat.None)
            {
                directive.DirectiveCategories |= Categories.Transformation;
            }
        }

        /// <summary>
        /// Sets validation policy as part of the policy.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="policyName">Name of the validation policy.</param>
        public void SetValidationPolicy(string directiveKey, string policyName)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidDirective, "Validation policy", policyName ?? "<null>"));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.ValidationPolicyName = policyName;

            directive.DirectiveCategories |= Categories.Validation;
        }

        /// <summary>
        /// Sets validation policy as part of the policy.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="policyName">Name of the validation policy.</param>
        /// <param name="errorOnInvalid">Indicates whether to throw an error if a validation rule policy indicates invalidity.</param>
        public void SetValidationPolicy(string directiveKey, string policyName, bool errorOnInvalid)
        {
            this.SetValidationPolicy(directiveKey, policyName);
            var directive = this.GetDirective(directiveKey);
            directive.ErrorOnInvalid = errorOnInvalid;

            // We will not set DirectiveCategories here, as the configuration is of no use 
            // unless we are also doing BAM interception
        }

        /// <summary>
        /// Sets validation policy as part of the policy.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="policyName">Name of the validation policy.</param>
        /// <param name="policyVersion">Validation policy version.</param>
        public void SetValidationPolicy(string directiveKey, string policyName, string policyVersion)
        {
            this.SetValidationPolicy(directiveKey, policyName);
            var directive = this.GetDirective(directiveKey);
            directive.ValidationPolicyVersion = policyVersion;

            directive.DirectiveCategories |= Categories.Validation;
        }

        /// <summary>
        /// Sets validation policy as part of the policy.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="policyName">Name of the validation policy.</param>
        /// <param name="policyVersion">Validation policy version.</param>
        /// <param name="errorOnInvalid">Indicates whether to throw an error if a validation rule policy indicates invalidity.</param>
        public void SetValidationPolicy(string directiveKey, string policyName, string policyVersion, bool errorOnInvalid)
        {
            this.SetValidationPolicy(directiveKey, policyName, policyVersion);
            var directive = this.GetDirective(directiveKey);
            directive.ErrorOnInvalid = errorOnInvalid;

            // We will not set DirectiveCategories here, as the configuration is of no use 
            // unless we are also doing BAM interception
        }

        /// <summary>
        /// Set a BAM interception tracking point for a directive.   This interception point will be 
        /// placed before any transformation step on the same directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="bamActivity">BAM activity name.</param>
        /// <param name="bamStepName">Name of a conceptual step within the activity.</param>
        public void SetBamInterception(string directiveKey, string bamActivity, string bamStepName)
        {
            this.SetBamInterception(directiveKey, bamActivity, bamStepName, false);
        }

        /// <summary>
        /// Set a post-transformation BAM interception tracking point for a directive.   This 
        /// interception point will be placed before any transformation step on the same directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="bamActivity">BAM activity name.</param>
        /// <param name="bamStepName">Name of a conceptual step within the activity.</param>
        public void SetBamInterceptionAfterMap(string directiveKey, string bamActivity, string bamStepName)
        {
            this.SetBamInterception(directiveKey, bamActivity, bamStepName, true);
        }

        /// <summary>
        /// Set a BAM interception step extension for a directive.  The directive must set a BAM step to be extended.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="bamExtensionStepName">The name of the BAM step extension.</param>
        public void SetBamInterceptionExtension(string directiveKey, string bamExtensionStepName)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                var extensionStepName = string.Format("extension {0}", bamExtensionStepName ?? "<null>");
                var message = string.Format(
                    Properties.Resources.ExceptionInvalidDirective,
                    "BAM interception step extension",
                    extensionStepName);
                throw new EsbFactsException(message);
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.BamStepExtensions.Add(bamExtensionStepName);

            directive.DirectiveCategories |= Categories.BamInterception;
        }

        /// <summary>
        /// Set a BAM interception step extension for a directive.  The directive must set a BAM step to be extended.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="bamExtensionStepName">The name of the BAM step extension.</param>
        public void SetBamInterceptionExtensionAfterMap(string directiveKey, string bamExtensionStepName)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                var extensionStepName = string.Format("extension {0}", bamExtensionStepName ?? "<null>");
                var message = string.Format(
                    Properties.Resources.ExceptionInvalidDirective,
                    "BAM post-transformation interception step extension",
                    extensionStepName);
                throw new EsbFactsException(message);
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.BamStepExtensions.Add(bamExtensionStepName);

            directive.DirectiveCategories |= Categories.BamInterception;
        }

        /// <summary>
        /// Sets BAM configuration as part of the policy.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="bamConnectionstring">Database connection string to message box or primary import database.</param>
        /// <param name="bamIsBuffered">Flag indicates the type of event stream to use.</param>
        /// <param name="bamFlushThreshold">
        /// Value that determines under what conditions the buffered data will be sent to the tracking database.
        /// </param>
        public void SetBamConfiguration(string directiveKey, string bamConnectionstring, bool bamIsBuffered, int bamFlushThreshold)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(
                        Properties.Resources.ExceptionInvalidDirective,
                        "BAM configuration",
                        bamConnectionstring ?? "<null>"));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.BamConnectionString = bamConnectionstring;
            directive.BamIsBuffered = bamIsBuffered;
            directive.BamFlushThreshold = bamFlushThreshold;

            // We will not set DirectiveCategories here, as the configuration is of no use 
            // unless we are also doing BAM interception
        }

        /// <summary>
        /// Sets BAM Trackpoint policy as part of the policy.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="policyName">Database connection string to message box or primary import database.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void SetBamTrackpointPolicy(string directiveKey, string policyName)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidDirective, "BAM trackpoint policy", policyName ?? "<null>"));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.BamTrackpointPolicyName = policyName;

            // We will not set DirectiveCategories here, as the trackpoint policy is of no use 
            // unless we are also doing BAM interception
        }

        /// <summary>
        /// Sets BAM Trackpoint policy as part of the policy.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="policyName">Database connection string to message box or primary import database.</param>
        /// <param name="policyVersion">Flag indicates the type of event stream to use.</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void SetBamTrackpointPolicy(string directiveKey, string policyName, string policyVersion)
        {
            this.SetBamTrackpointPolicy(directiveKey, policyName);
            var directive = this.GetDirective(directiveKey);
            directive.BamTrackpointPolicyVersion = policyVersion;

            // We will not set DirectiveCategories here, as the trackpoint policy is of no use 
            // unless we are also doing BAM interception
        }
        
        /// <summary>
        /// Set the policy for performing retries on failure.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="retryCount">The number of retries to perform.</param>
        /// <param name="retryInterval">The interval between each retry.</param>
        public void SetRetryPolicy(string directiveKey, int retryCount, int retryInterval)
        {
            this.SetRetryPolicy(directiveKey, retryCount, retryInterval, 0);
        }

        /// <summary>
        /// Set the policy for performing retries on failure.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="retryCount">The number of retries to perform.</param>
        /// <param name="retryInterval">The interval between each retry.</param>
        /// <param name="retryLevel">The temporal level of retrys.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void SetRetryPolicy(string directiveKey, int retryCount, int retryInterval, int retryLevel)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(
                        Properties.Resources.ExceptionInvalidDirective,
                        "retry policy",
                        string.Format(
                            "of {0} retries with an interval of {1} at level {2}",
                            retryCount,
                            retryInterval,
                            retryLevel)));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.RetryCount = retryCount;
            directive.RetryInterval = retryInterval;
            directive.RetryLevel = retryLevel;
            directive.DirectiveCategories |= Categories.Retry;
        }

        /// <summary>
        /// Set service window start and stop times.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="startTime">Time at which the service window opens.</param>
        /// <param name="stopTime">Time at which the service window closes.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        public void SetServiceWindow(string directiveKey, DateTime startTime, DateTime stopTime)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(
                        Properties.Resources.ExceptionInvalidDirective,
                        "service window",
                        string.Format(
                            "that starts at {0} and stops at {1}",
                            startTime,
                            stopTime)));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;
            directive.ServiceWindowStartTime = startTime;
            directive.ServiceWindowStopTime = stopTime;
            directive.DirectiveCategories |= Categories.ServiceWindow;
        }

        /// <summary>
        /// Set a BTS property name-value pair for a directive.   This property will 
        /// be generally created in message context.   NB.   This property assumes  
        /// that the name-value pair is for a property defined in 
        /// Microsoft.BizTalk.GlobalPropertySchemas.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="name">BTS property name.</param>
        /// <param name="value">Value to assign to the property.</param>
        public void SetBtsGlobalProperty(string directiveKey, string name, string value)
        {
            this.SetBtsGlobalProperty(directiveKey, name, value, true);
        }

        /// <summary>
        /// Set a BTS property name-value pair for a directive.   This property will 
        /// be generally created in message context.   NB.   This property assumes  
        /// that the name-value pair is for a property defined in 
        /// Microsoft.BizTalk.GlobalPropertySchemas.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="name">BTS property name.</param>
        /// <param name="value">Value to assign to the property.</param>
        /// <param name="promoted">Flag indicates if property should be promoted.</param>
        public void SetBtsGlobalProperty(string directiveKey, string name, string value, bool promoted)
        {
            var propOjectHandle = Activator.CreateInstance(Properties.Resources.AssemblyQNameMicrosoftBizTalkGlobalPropertySchemas, name);

            if (propOjectHandle == null)
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidProperty, name));
            }

            var prop = propOjectHandle.Unwrap() as PropertyBase;

            if (prop == null)
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidProperty, "ARG0"));
            }

            this.SetBtsProperty(directiveKey, prop.Name.Name, value, prop.Name.Namespace, promoted);
        }

        /// <summary>
        /// Set a BTS property name-value pair (including namespace) for a directive.   This property will 
        /// be generally created in message context.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="name">BTS property name.</param>
        /// <param name="value">Value to assign to the property.</param>
        /// <param name="namespace">XML namespace of the property</param>
        public void SetBtsProperty(string directiveKey, string name, string value, string @namespace)
        {
            this.SetBtsProperty(directiveKey, name, value, @namespace, true);
        }

        /// <summary>
        /// Set a BTS property name-value pair (including namespace) for a directive.   This property will 
        /// be generally created in message context.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="name">BTS property name.</param>
        /// <param name="value">Value to assign to the property.</param>
        /// <param name="namespace">XML namespace of the property</param>
        /// <param name="promoted">Flag indicates if property should be promoted.</param>
        public void SetBtsProperty(string directiveKey, string name, string value, string @namespace, bool promoted)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(
                        Properties.Resources.ExceptionInvalidDirective,
                        "BizTalk property", 
                        name ?? "<null>"));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidBizTalkPropertyName, directiveKey));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidBizTalkPropertyValue, directiveKey));
            }

            if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidBizTalkPropertyNamespace, directiveKey));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;

            Directive.BtsProperty origPropertyDef;
            var propertyDef = new Directive.BtsProperty(name, value, @namespace, promoted);

            // Remove any existing property definition for this property
            // SolidsoftReply.ESB.Libraries.BizTalk.Facts.Interchange.KeyValue 
            if (directive.BtsProperties.TryGetValue(name, out origPropertyDef))
            {
                directive.BtsProperties.Remove(name);
            }

            directive.BtsProperties.Add(name, propertyDef);
        }

        /// <summary>
        /// Set a general-purpose property name-value pair for a directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="name">Property name.</param>
        /// <param name="value">Value to assign to the property.</param>
        public void SetProperty(string directiveKey, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(
                        Properties.Resources.ExceptionInvalidDirective,
                        "property", 
                        name ?? "<null>"));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidPropertyName, directiveKey));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new EsbFactsException(string.Format(Properties.Resources.ExceptionInvalidPropertyValue, directiveKey));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;

            Directive.Property origPropertyDef;
            var propertyDef = new Directive.Property(name, value);

            // Remove any existing property definition for this property
            if (directive.Properties.TryGetValue(name, out origPropertyDef))
            {
                directive.Properties.Remove(name);
            }

            directive.Properties.Add(name, propertyDef);
        }

        /// <summary>
        /// Performs validation at the directive level.  These validations cover situations
        /// where one instruction must be validated against another.
        /// </summary>
        public void ValidateDirectives()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            this.directives.All(kvp => kvp.Value.Validate());
        }

        /// <summary>
        /// This method is reserved and is not used. 
        /// </summary>
        /// <returns>
        /// An XmlSchema that describes the XML representation of the object that is produced 
        /// by the WriteXml method and consumed by the ReadXml method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The XmlReader stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            throw new Exception(Properties.Resources.ExceptionNotImplemented);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The XmlWriter stream to which the object is serialized. </param>
        public void WriteXml(XmlWriter writer)
        {
            throw new Exception(Properties.Resources.ExceptionNotImplemented);
        }

        /// <summary>
        /// Set a BAM interception tracking point for a directive.   This interception point may be 
        /// placed either before or after any transformation step on the same directive.
        /// </summary>
        /// <param name="directiveKey">Key name that identifies a directive.</param>
        /// <param name="bamActivity">BAM activity name.</param>
        /// <param name="bamStepName">Name of a conceptual step within the activity.</param>
        /// <param name="afterMap">If true, indicates that the interception tracking point should be set 
        /// after any transformation defined on this same directive.</param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        private void SetBamInterception(string directiveKey, string bamActivity, string bamStepName, bool afterMap)
        {
            if (string.IsNullOrWhiteSpace(directiveKey))
            {
                throw new EsbFactsException(
                    string.Format(
                        Properties.Resources.ExceptionInvalidDirective,
                        "BAM interception",
                        string.Format(
                            "{0}step {1} for activity {2}",
                            afterMap ? "after-map " : string.Empty,
                            bamStepName ?? "<null>",
                            bamActivity ?? "<null>")));
            }

            var directive = this.GetDirective(directiveKey);
            directive.KeyName = directiveKey;

            if (!string.IsNullOrWhiteSpace(directive.BamActivity) && directive.BamActivity != bamActivity)
            {
                throw new EsbFactsException(
                    string.Format(Properties.Resources.ExceptionBamActivityName, directiveKey));
            }

            if (!afterMap)
            {
                // Set the BAM Activity on a pre-transformation step
                directive.BamActivity = bamActivity;
            }
            else
            {
                // Set the BAM Activity on a post-transformation step
                if (string.IsNullOrWhiteSpace(directive.BamActivity))
                {
                    directive.BamActivity = bamActivity;
                }

                if (!string.IsNullOrWhiteSpace(bamActivity) && 
                    directive.BamActivity != bamActivity)
                {
                    throw new EsbFactsException(
                        string.Format(Properties.Resources.ExceptionBamNonMatchingStepNames, 
                        directiveKey,
                        directive.BamActivity,
                        string.IsNullOrWhiteSpace(bamActivity) ? "<null or empty>": bamActivity));
                }
            }

            if (afterMap)
            {
                if (!string.IsNullOrWhiteSpace(directive.BamAfterMapStepName)
                    && directive.BamAfterMapStepName != bamStepName)
                {
                    throw new EsbFactsException(
                        string.Format(Properties.Resources.ExceptionBamAfterMapStepName, directiveKey));
                }

                directive.BamAfterMapStepName = bamStepName;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(directive.BamStepName) && directive.BamStepName != bamStepName)
                {
                    throw new EsbFactsException(
                        string.Format(Properties.Resources.ExceptionBamStepName, directiveKey));
                }

                directive.BamStepName = bamStepName;
            }

            directive.DirectiveCategories |= Categories.BamInterception;
        }

        /// <summary>
        /// Gets a directive from the directive collection.    If the directive does
        /// not exist, it is created.
        /// </summary>
        /// <param name="keyName">Directive key name.</param>
        /// <returns>A directive for the given key name.</returns>
        private Directive GetDirective(string keyName)
        {
            Directive directive;

            if (this.directives.TryGetValue(keyName, out directive))
            {
                return directive;
            }

            directive = new Directive(keyName) { KeyName = keyName };
            this.directives.Add(keyName, directive);

            return directive;
        }
    }
}
