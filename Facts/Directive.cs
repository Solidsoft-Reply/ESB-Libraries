// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Directive.cs" company="Solidsoft Reply Ltd.">
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
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// A directive assigned as a result of resolution.    Directives are created by rules
    /// within the service directory.   An interchange fact asserted to the rule engine may
    /// be populated with zero or more directives.   A single directive can carry information
    /// for multiple categories.
    /// </summary>
    [Serializable]
    [DataContract(Name = "Directive", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05")]
    [XmlRoot("Directive", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", IsNullable = true)]
    public class Directive
    {
        /// <summary>
        /// The directive validator.
        /// </summary>
        private readonly DirectiveValidator directiveValidator;

        /// <summary>
        /// Dictionary of the number of times a given property has been set in this directive.
        /// </summary>
        private readonly Dictionary<string, int> propertySetCounts = new Dictionary<string, int>();

        /// <summary>
        /// Indicates whether the directive is valid;
        /// </summary>
        private bool isValid = true;

        /// <summary>
        /// URI for resolved endpoint.
        /// </summary>
        private string endPoint;

        /// <summary>
        /// BTS Transport type (adapter 'scheme') specifier.  E.g., 'FILE', 'WSE'.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private string transportType;

        /// <summary>
        /// Configuration token for communication with the endpoint.
        /// </summary>
        private string endPointConfiguration;

        /// <summary>
        /// The SOAP Action to use in a SOAP interchange.
        /// </summary>
        private string soapAction;

        /// <summary>
        /// Fully qualified .NET assembly name and type for BTS map.
        /// </summary>
        private string mapToApply;

        /// <summary>
        /// Format specifier for XML content.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private XmlFormat xmlFormat;

        /// <summary>
        /// Name of BAM activity for which a BAM interception tracking point will be created.
        /// </summary>
        private string bamActivity;

        /// <summary>
        /// Name of conceptual BAM step ('location') within an activity.
        /// </summary>
        private string bamStepName;

        /// <summary>
        /// List of steps that extend the step specified in the StepName property.
        /// </summary>
        private BamStepExtensionsType bamStepExtensions = new BamStepExtensionsType();

        /// <summary>
        /// List of steps that extend the post-transformation step specified in the AfterMapStepName property.
        /// </summary>
        private BamStepExtensionsType bamAfterMapStepExtensions = new BamStepExtensionsType();
        
        /// <summary>
        /// Name of conceptual BAM step ('location') within an activity that will
        /// be placed after a transformation.
        /// </summary>
        private string bamAfterMapStepName;

        /// <summary>
        /// Connection string for BAM.
        /// </summary>
        private string bamConnectionString;

        /// <summary>
        /// Indicates if BAM will use a buffered event stream.
        /// </summary>
        private bool bamIsBuffered = true;

        /// <summary>
        /// A value that determines under what conditions the buffered 
        /// data will be sent to the tracking database.
        /// </summary>
        private int bamFlushThreshold = 1;

        /// <summary>
        /// The BAM Trackpoint policy name.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private string bamTrackpointPolicyName;

        /// <summary>
        /// The BAM Trackpoint policy version.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private string bamTrackpointPolicyVersion;

        /// <summary>
        /// Number of retries to perform on failure.
        /// </summary>
        private int retryCount;

        /// <summary>
        /// Number of minutes between each retry.
        /// </summary>
        private int retryInterval;

        /// <summary>
        /// Level specifier for a retry.   
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private int retryLevel;

        /// <summary>
        /// Time at which service window opens.
        /// </summary>
        private DateTime serviceWindowStartTime;

        /// <summary>
        /// Time at which service window closes
        /// </summary>
        private DateTime serviceWindowStopTime;

        /// <summary>
        /// The validation policy name.
        /// </summary>
        private string validationPolicyName;

        /// <summary>
        /// The validation policy version.
        /// </summary>
        private string validationPolicyVersion;

        /// <summary>
        /// Indicates if an error will be thrown if a validation rule policy indicates invalidity.
        /// </summary>
        private bool errorOnInvalid = true;

        /// <summary>
        /// Collection of BTS property name-value pairs (with namespaces).
        /// </summary>
        [NonSerialized]
        private Dictionaries.BtsProperties btsProperties;

        /// <summary>
        /// Collection of general purpose property name-value pairs.
        /// </summary>
        [NonSerialized]
        private Dictionaries.Properties properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="Directive"/> class. 
        /// </summary>
        /// <param name="keyName">
        /// Key name used to uniquely represent the directive.
        /// </param>
        public Directive(string keyName)
        {
            this.KeyName = keyName;
            this.btsProperties = new Dictionaries.BtsProperties();
            this.properties = new Dictionaries.Properties();
            this.directiveValidator = new DirectiveValidator(this);
        }

        /// <summary>
        /// Gets or sets the key name used to uniquely represent the directive.   Resolution may result
        /// in several directives being created - e.g., for request and response messages.
        /// </summary>
        [DataMember(Name = "KeyName", Order = 0)]
        [XmlElement("KeyName", Order = 0)]
        public string KeyName { get; set; }

        /// <summary>
        /// Gets or sets a set of flags indicating the categories of settings within this directive
        /// </summary>
        [DataMember(Name = "DirectiveCategories", Order = 1)]
        [XmlElement("DirectiveCategories", Order = 1)]
        public Interchange.Categories DirectiveCategories { get; set; }

        #region Category: EndPoint

        /// <summary>
        /// Gets or sets the URI for resolved endpoint.
        /// </summary>
        [DataMember(Name = "EndPoint", Order = 2)]
        [XmlElement("EndPoint", Order = 2)]
        public string EndPoint
        {
            get
            {
                return this.endPoint;
            }

            set
            {
                this.endPoint = value;
                this.IncrementPropertySetCount("EndPoint");
                this.SetValidity(this.directiveValidator.ValidateEndPoint());
            }
        }

        /// <summary>
        /// Gets or sets the transport type specifier.  In the context of BizTalk Server, this should be 
        /// an adapter schema - e.g., 'FILE', 'WCF-BasicHttp'.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [DataMember(Name = "TransportType", Order = 3)]
        [XmlElement("TransportType", Order = 3)]
        public string TransportType
        {
            get
            {
                return this.transportType;
            }

            set
            {
                this.transportType = value;
                this.IncrementPropertySetCount("TransportType");
                this.SetValidity(this.directiveValidator.ValidateTransportType());
            }
        }

        /// <summary>
        /// Gets or sets the BTS endpoint configuration.  This may be a token or raw configuration text.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [DataMember(Name = "EndPointConfiguration", Order = 4)]
        [XmlElement("EndPointConfiguration", Order = 4)]
        public string EndPointConfiguration
        {
            get
            {
                return this.endPointConfiguration;
            }

            set
            {
                this.endPointConfiguration = value;
                this.IncrementPropertySetCount("EndPointConfiguration");
                this.SetValidity(this.directiveValidator.ValidateEndPointConfiguration());
            }
        }
        
        /// <summary>
        /// Gets or sets the SOAP action for the endpoint.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [DataMember(Name = "SoapAction", Order = 5)]
        [XmlElement("SoapAction", Order = 5)]
        public string SoapAction
        {
            get
            {
                return this.soapAction;
            }

            set
            {
                this.soapAction = value;
                this.IncrementPropertySetCount("SoapAction");
                this.SetValidity(this.directiveValidator.ValidateSoapAction());
            }
        }

        #endregion

        #region Category: Transformation

        /// <summary>
        /// Gets or sets the name of a map to apply.  In the context of BizTalk Server, this will be the 
        /// fully qualified .NET assembly name and type for map.
        /// </summary>
        [DataMember(Name = "MapToApply", Order = 6)]
        [XmlElement("MapToApply", Order = 6)]
        public string MapToApply
        {
            get
            {
                return this.mapToApply;
            }

            set
            {
                this.mapToApply = value;
                this.IncrementPropertySetCount("MapToApply");
                this.SetValidity(this.directiveValidator.ValidateMapToApply());
            }
        }

        /// <summary>
        /// Gets or sets the format specifier for XML content.
        /// </summary>
        [DataMember(Name = "XmlFormat", Order = 7)]
        [XmlElement("XmlFormat", Order = 7)]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public XmlFormat XmlFormat
        {
            get
            {
                return this.xmlFormat;
            }

            set
            {
                this.xmlFormat = value;
                this.IncrementPropertySetCount("XmlFormat");
                this.SetValidity(this.directiveValidator.ValidateXmlFormat());
            }
        }

        #endregion

        #region Category: BamInterceptor

        /// <summary>
        /// Gets or sets the name of BAM activity for which a BAM interception tracking point will be created.
        /// </summary>
        [DataMember(Name = "BamActivity", Order = 8)]
        [XmlElement("BamActivity", Order = 8)]
        public string BamActivity
        {
            get
            {
                return this.bamActivity;
            }

            set
            {
                this.bamActivity = value;
                this.IncrementPropertySetCount("BamActivity");
                this.SetValidity(this.directiveValidator.ValidateBamActivity());
            }
        }

        /// <summary>
        /// Gets or sets the name of conceptual BAM step ('location') within an activity.
        /// </summary>
        [DataMember(Name = "BamStepName", Order = 9)]
        [XmlElement("BamStepName", Order = 9)]
        public string BamStepName
        {
            get
            {
                return this.bamStepName;
            }

            set
            {
                this.bamStepName = value;
                this.IncrementPropertySetCount("BamStepName");
                this.SetValidity(this.directiveValidator.ValidateBamStepName());
            }
        }

        /// <summary>
        /// Gets or sets a list of steps that extend the step specified in the StepName property.
        /// </summary>
        [DataMember(Name = "BamStepExtensions", Order = 10)]
        [XmlElement("BamStepExtensions", Order = 10)]
        public BamStepExtensionsType BamStepExtensions
        {
            get
            {
                return this.bamStepExtensions;
            }

            set
            {
                this.bamStepExtensions.Clear();

                if (value == null)
                {
                    return;
                }

                foreach (var item in value)
                {
                    this.bamStepExtensions.Add(item);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of conceptual BAM step ('location') within an activity that will
        /// be placed after a transformation
        /// </summary>
        [DataMember(Name = "BamAfterMapStepName", Order = 11)]
        [XmlElement("BamAfterMapStepName", Order = 11)]
        public string BamAfterMapStepName
        {
            get
            {
                return this.bamAfterMapStepName;
            }

            set
            {
                this.bamAfterMapStepName = value;
                this.IncrementPropertySetCount("BamAfterMapStepName");
                this.SetValidity(this.directiveValidator.ValidateBamAfterMapStepName());
            }
        }

        /// <summary>
        /// Gets or sets a list of steps that extend the post-transformation step specified in the AfterMapStepName property.
        /// </summary>
        [DataMember(Name = "BamAfterMapStepExtensions", Order = 12)]
        [XmlElement("BamAfterMapStepExtensions", Order = 12)]
        public BamStepExtensionsType BamAfterMapStepExtensions
        {
            get
            {
                return this.bamAfterMapStepExtensions;
            }

            set
            {
                this.bamAfterMapStepExtensions.Clear();

                if (value == null)
                {
                    return;
                }

                foreach (var item in value)
                {
                    this.bamAfterMapStepExtensions.Add(item);
                }
            }
        }

        /// <summary>
        /// Gets or sets the connection string for BAM.
        /// </summary>
        [DataMember(Name = "BamConnectionString", Order = 13)]
        [XmlElement("BamConnectionString", Order = 13)]
        public string BamConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(this.bamConnectionString))
                {
                    return this.bamConnectionString;
                }

                try
                {
                    this.bamConnectionString = this.BamIsBuffered ? ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbBamBufferedConnectionString].Trim() : ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbBamDirectConnectionString].Trim();
                }
                catch
                {
                    this.bamConnectionString = string.Empty;
                }

                if (!string.IsNullOrEmpty(this.bamConnectionString))
                {
                    return this.bamConnectionString;
                }

                const string DefaultBufferedCs = "Data Source=.;Initial Catalog=BizTalkMsgBoxDB;Integrated Security=SSPI;";
                const string DefaultDirectCs = "Data Source=.;Initial Catalog=BAMPrimaryImport;Integrated Security=SSPI;";

                this.bamConnectionString = this.bamIsBuffered ? DefaultBufferedCs : DefaultDirectCs;

                return this.bamConnectionString;
            }

            set
            {
                this.bamConnectionString = value;
                this.IncrementPropertySetCount("BamConnectionString");
                this.SetValidity(this.directiveValidator.ValidateBamConnectionString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether BAM will use a buffered event stream.
        /// </summary>
        [DataMember(Name = "BamIsBuffered", Order = 14)]
        [XmlElement("BamIsBuffered", Order = 14)]
        public bool BamIsBuffered
        {
            get
            {
                // If not already set, get the value from the config file
                if (this.PropertySetCount("BamIsBuffered") > 0)
                {
                    return this.bamIsBuffered;
                }

                var bamIsBufferedString = string.Empty;

                try
                {
                    bamIsBufferedString = ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbBamIsBuffered].Trim().ToLower();
                }
                catch
                {
                    this.bamIsBuffered = !(bamIsBufferedString == "false" || bamIsBufferedString == "0" || bamIsBufferedString == "no");
                }

                return this.bamIsBuffered;
            }

            set
            {
                this.bamIsBuffered = value;
                this.IncrementPropertySetCount("BamIsBuffered");
                this.SetValidity(this.directiveValidator.ValidateBamIsBuffered());
            }
        }

        /// <summary>
        /// Gets or sets a value that determines under what conditions the buffered 
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
        [DataMember(Name = "BamFlushThreshold", Order = 15)]
        [XmlElement("BamFlushThreshold", Order = 15)]
        public int BamFlushThreshold
        {
            get
            {
                // If not already set, get the value from the config file
                if (this.PropertySetCount("BamIsBuffered") > 0)
                {
                    return this.bamFlushThreshold;
                }

                try
                {
                    var bamFlushThresholdString = ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbBamFlushThreshold].Trim();
                    this.bamFlushThreshold = Convert.ToInt32(bamFlushThresholdString);
                }
                catch
                {
                    this.bamFlushThreshold = 1;
                }

                return this.bamFlushThreshold;
            }

            set
            {
                this.bamFlushThreshold = value;
                this.IncrementPropertySetCount("BamFlushThreshold");
                this.SetValidity(this.directiveValidator.ValidateBamFlushThreshold());
            }
        }

        /// <summary>
        /// Gets or sets the name of the BAM Trackpoint policy.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [DataMember(Name = "BamTrackpointPolicyName", Order = 16)]
        [XmlElement("BamTrackpointPolicyName", Order = 16)]
        public string BamTrackpointPolicyName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.bamTrackpointPolicyName))
                {
                    return this.bamTrackpointPolicyName;
                }

                try
                {
                    this.bamTrackpointPolicyName = ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbBamDefaultTrackpointPolicyName].Trim();
                }
                catch
                {
                    this.bamTrackpointPolicyName = string.Empty;
                }

                return !string.IsNullOrEmpty(this.bamTrackpointPolicyName) ? this.bamTrackpointPolicyName : string.Empty;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                this.bamTrackpointPolicyName = value;
                this.IncrementPropertySetCount("BamTrackpointPolicyName");
                this.SetValidity(this.directiveValidator.ValidateBamTrackpointPolicyName());
            }
        }

        /// <summary>
        /// Gets or sets the version of the BAM Trackpoint policy.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [DataMember(Name = "BamTrackpointPolicyVersion", Order = 17)]
        [XmlElement("BamTrackpointPolicyVersion", Order = 17)]
        public string BamTrackpointPolicyVersion
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.bamTrackpointPolicyVersion))
                {
                    return this.bamTrackpointPolicyVersion;
                }

                try
                {
                    this.bamTrackpointPolicyVersion = ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbBamDefaultTrackpointPolicyVersion].Trim();
                }
                catch
                {
                    this.bamTrackpointPolicyVersion = string.Empty;
                }

                return !string.IsNullOrEmpty(this.bamTrackpointPolicyVersion) ? this.bamTrackpointPolicyVersion : string.Empty;
            }

            set
            {
                var versionValue = value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(versionValue))
                {
                    return;
                }

                // Test to ensure the value is a valid Version specifier.
                Version version;

                this.bamTrackpointPolicyVersion = Version.TryParse(value, out version) ? version.ToString(2) : value;
                this.IncrementPropertySetCount("BamTrackpointPolicyVersion");
                this.SetValidity(this.directiveValidator.ValidateBamTrackpointPolicyVersion());
            }
        }

        #endregion

        #region Category: Retry

        /// <summary>
        /// Gets or sets the number of retries to perform on failure
        /// </summary>
        [DataMember(Name = "RetryCount", Order = 18)]
        [XmlElement("RetryCount", Order = 18)]
        public int RetryCount
        {
            get
            {
                return this.retryCount;
            }

            set
            {
                this.retryCount = value;
                this.IncrementPropertySetCount("RetryCount");
                this.SetValidity(this.directiveValidator.ValidateRetryCount());
            }
        }

        /// <summary>
        /// Gets or sets the number of minutes between each retry.
        /// </summary>
        [DataMember(Name = "RetryInterval", Order = 19)]
        [XmlElement("RetryInterval", Order = 19)]
        public int RetryInterval
        {
            get
            {
                return this.retryInterval;
            }

            set
            {
                this.retryInterval = value;
                this.IncrementPropertySetCount("RetryInterval");
                this.SetValidity(this.directiveValidator.ValidateRetryInterval());
            }
        }

        /// <summary>
        /// Gets or sets the level specifier for a retry.   
        /// </summary>
        /// <remarks>
        /// This may be used in scenarios where the system should perform a series of 
        /// retries at one level, and then 'wrap' these in additional levels of retry 
        /// over successively longer time intervals.  E.g., a policy might state that 
        /// the system should retry three times with an interval of on minute, then 
        /// perform an additional set of retries once an hour for five hours.
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        [DataMember(Name = "RetryLevel", Order = 20)]
        [XmlElement("RetryLevel", Order = 20)]
        public int RetryLevel
        {
            get
            {
                return this.retryLevel;
            }

            set
            {
                this.retryLevel = value;
                this.IncrementPropertySetCount("RetryLevel");
                this.SetValidity(this.directiveValidator.ValidateRetryLevel());
            }
        }

        #endregion

        #region Category: ServiceWindow

        /// <summary>
        /// Gets or sets the time at which service window opens.
        /// </summary>
        [DataMember(Name = "ServiceWindowStartTime", Order = 21)]
        [XmlElement("ServiceWindowStartTime", Order = 21)]
        public DateTime ServiceWindowStartTime
        {
            get
            {
                return this.serviceWindowStartTime;
            }

            set
            {
                this.serviceWindowStartTime = value;
                this.IncrementPropertySetCount("ServiceWindowStartTime");
                this.SetValidity(this.directiveValidator.ValidateServiceWindowStartTime());
            }
        }

        /// <summary>
        /// Gets or sets the time at which service window closes
        /// </summary>
        [DataMember(Name = "ServiceWindowStopTime", Order = 22)]
        [XmlElement("ServiceWindowStopTime", Order = 22)]
        public DateTime ServiceWindowStopTime
        {
            get
            {
                return this.serviceWindowStopTime;
            }

            set
            {
                this.serviceWindowStopTime = value;
                this.IncrementPropertySetCount("ServiceWindowStopTime");
                this.SetValidity(this.directiveValidator.ValidateServiceWindowStopTime());
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Gets or sets the name of the validation policy.
        /// </summary>
        [DataMember(Name = "ValidationPolicyName", Order = 23)]
        [XmlElement("ValidationPolicyName", Order = 23)]
        public string ValidationPolicyName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.validationPolicyName))
                {
                    return this.validationPolicyName;
                }

                try
                {
                    this.validationPolicyName = ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbValidationPolicyName].Trim();
                }
                catch
                {
                    this.validationPolicyName = string.Empty;
                }

                return !string.IsNullOrEmpty(this.validationPolicyName) ? this.validationPolicyName : string.Empty;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                this.validationPolicyName = value;
                this.IncrementPropertySetCount("ValidationPolicyName");
                this.SetValidity(this.directiveValidator.ValidateValidationPolicyName());
            }
        }

        /// <summary>
        /// Gets or sets the version of the validation policy.
        /// </summary>
        [DataMember(Name = "ValidationPolicyVersion", Order = 24)]
        [XmlElement("ValidationPolicyVersion", Order = 24)]
        public string ValidationPolicyVersion
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.validationPolicyVersion))
                {
                    return this.validationPolicyVersion;
                }

                try
                {
                    this.validationPolicyVersion = ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbValidationPolicyVersion].Trim();
                }
                catch
                {
                    this.validationPolicyVersion = string.Empty;
                }

                return !string.IsNullOrEmpty(this.validationPolicyVersion) ? this.validationPolicyVersion : string.Empty;
            }

            set
            {
                var versionValue = value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(versionValue))
                {
                    return;
                }

                // Test to ensure the value is a valid Version specifier.
                Version version;

                this.validationPolicyVersion = Version.TryParse(value, out version) ? version.ToString(2) : value;
                this.IncrementPropertySetCount("ValidationPolicyVersion");
                this.SetValidity(this.directiveValidator.ValidateValidationPolicyVersion());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an error if a validation rule policy indicates invalidity.
        /// </summary>
        [DataMember(Name = "ErrorOnInvalid", Order = 25)]
        [XmlElement("ErrorOnInvalid", Order = 25)]
        public bool ErrorOnInvalid
        {
            get
            {
                // If not already set, get the value from the config file
                if (this.PropertySetCount("ErrorOnInvalid") > 0)
                {
                    return this.errorOnInvalid;
                }

                var errorOnInvalidString = string.Empty;

                try
                {
                    errorOnInvalidString = ConfigurationManager.AppSettings[Facts.Properties.Resources.AppSettingEsbErrorOnInvalid].Trim().ToLower();
                }
                catch
                {
                    this.errorOnInvalid = !(errorOnInvalidString == "false" || errorOnInvalidString == "0" || errorOnInvalidString == "no");
                }

                return this.errorOnInvalid;
            }

            set
            {
                this.errorOnInvalid = value;
                this.IncrementPropertySetCount("ErrorOnInvalid");
                this.SetValidity(this.directiveValidator.ValidateErrorOnInvalid());
            }
        }

        /// <summary>
        /// Gets or sets the collection of general purpose property name-value pairs.
        /// </summary>
        [DataMember(Name = "Properties", Order = 26)]
        [XmlElement("Properties", Order = 26)]
        public Dictionaries.Properties Properties
        {
            get { return this.properties; }
            set { this.properties = value; }
        }

        #endregion

        #region Biztalk Properties

        /// <summary>
        /// Gets or sets the collection of BTS property name-value pairs (with namespaces).
        /// </summary>
        [DataMember(Name = "BtsProperties", Order = 27)]
        [XmlElement("BtsProperties", Order = 27)]
        public Dictionaries.BtsProperties BtsProperties
        {
            get { return this.btsProperties; }
            set { this.btsProperties = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the directive is valid;
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
                return this.isValid ? string.Empty : this.directiveValidator.ValidErrors;
            }
        }

        /// <summary>
        /// Validates the directive.  This method handles any validations that must compare across
        /// multiple directive instructions.
        /// </summary>
        /// <returns>rue if valid; otherwise false.</returns>
        public bool Validate()
        {
            this.SetValidity(this.directiveValidator.ValidateDirective());
            return true;
        }

        /// <summary>
        /// Returns the number of times a given property has been set.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>A count of the number of times the property has been set.</returns>
        internal int PropertySetCount(string propertyName)
        {
            int count;

            return this.propertySetCounts.TryGetValue(propertyName, out count) ? count : 0;
        }

        /// <summary>
        /// Increments the number of times a given property has been set.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        private void IncrementPropertySetCount(string propertyName)
        {
            if (!this.propertySetCounts.ContainsKey(propertyName))
            {
                this.propertySetCounts.Add(propertyName, 0);
            }

            this.propertySetCounts[propertyName] = this.propertySetCounts[propertyName] + 1;
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

        /// <summary>
        /// Structure representing a BTS property name-value pair with namespace.
        /// </summary>
        [Serializable]
        [DataContract(Name = "BtsProperty", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05")]
        [XmlRoot("BtsProperty", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05")]
        public struct BtsProperty
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BtsProperty"/> struct. 
            /// </summary>
            /// <param name="name">
            /// BTS name of property.
            /// </param>
            /// <param name="value">
            /// Value of property.
            /// </param>
            /// <param name="namespace">
            /// XML namespace of property.
            /// </param>
            /// <param name="promoted">
            /// Flag indicating if property should be promoted.
            /// </param>
            public BtsProperty(string name, string value, string @namespace, bool promoted)
                : this()
            {
                this.Name = name;
                this.Value = value;
                this.Namespace = @namespace;
                this.Promoted = promoted;
            }

            /// <summary>
            /// Gets or sets the name of BTS property.
            /// </summary>
            [DataMember(Name = "Name", Order = 0)]
            [XmlElement("Name", Order = 0)]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value of BTS property.
            /// </summary>
            [DataMember(Name = "Value", Order = 1)]
            [XmlElement("Value", Order = 1)]
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets the XML namespace of BTS property.
            /// </summary>
            [DataMember(Name = "Namespace", Order = 2)]
            [XmlElement("Namespace", Order = 2)]
            public string Namespace { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the property should be marked as promoted on BizTalk messages.
            /// </summary>
            [DataMember(Name = "Promoted", Order = 3)]
            [XmlElement("Promoted", Order = 3)]
            public bool Promoted { get; set; }
        }

        #endregion

        /// <summary>
        /// Structure representing a general purpose property name-value pair.
        /// </summary>
        [Serializable]
        [DataContract(Name = "Property", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05")]
        [XmlRoot("Property", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05")]
        public struct Property
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Property"/> struct. 
            /// </summary>
            /// <param name="name">
            /// Name of property.
            /// </param>
            /// <param name="value">
            /// Value of property.
            /// </param>
            public Property(string name, string value)
                : this()
            {
                this.Name = name;
                this.Value = value;
            }

            /// <summary>
            /// Gets or sets the name of property.
            /// </summary>
            [DataMember(Name = "Name", Order = 0)]
            [XmlElement("Name", Order = 0)]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value of property.
            /// </summary>
            [DataMember(Name = "Value", Order = 1)]
            [XmlElement("Value", Order = 1)]
            public string Value { get; set; }
        }

        /// <summary>
        /// Represents a list of BAM step extension names.
        /// </summary>
        [DebuggerStepThrough]
        [CollectionDataContract(Name = "Directive.BamStepExtensionsType", Namespace = "http://solidsoftreply.com/schemas/webservices/esbresolutionservice/2015/05", ItemName = "BamStepExtension")]
        public class BamStepExtensionsType : List<string>
        {
        }
    }
}
