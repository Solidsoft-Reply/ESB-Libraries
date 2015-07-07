// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Directive.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.BizTalk.Orchestration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Xml;

    using Microsoft.BizTalk.Bam.EventObservation;
    using Microsoft.BizTalk.XLANGs.BTXEngine;
    using Microsoft.RuleEngine;
    using Microsoft.Win32;
    using Microsoft.XLANGs.BaseTypes;

    using SolidsoftReply.Esb.Libraries.BizTalk.Orchestration.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution.Dictionaries;
    using SolidsoftReply.Esb.Libraries.Resolution.ResolutionService;

    /// <summary>
    /// Class representing the item on the output list
    /// </summary>
    [ComVisible(true)]
    [Serializable]
    public class Directive : Resolution.Directive
    {
        #region Static Fields

        /// <summary>
        /// Dictionary of rule engine configuration values.
        /// </summary>
        private static readonly IDictionary ConfigValues;

        #endregion

        #region Fields

        /// <summary>
        /// A directive returned from the resolver
        /// </summary>
        private readonly Resolution.Directive directive;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Orchestration.Directive" /> class.
        /// </summary>
        static Directive()
        {
            ConfigValues = ConfigurationManager.GetSection("Microsoft.RuleEngine") as IDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Orchestration.Directive"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        public Directive(Resolution.Directive directive)
        {
            if (directive != null)
            {
                this.directive = directive;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Orchestration.Directive" /> class.
        /// </summary>
        ~Directive()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the BAM activity to which this directive applies.
        /// </summary>
        public override string BamActivity
        {
            get
            {
                return this.directive.BamActivity;
            }
        }

        /// <summary>
        /// Gets the name of the step within the BAM activity to which this directive applies.
        /// </summary>
        public override string BamAfterMapStepName
        {
            get
            {
                return this.directive.BamAfterMapStepName;
            }
        }

        /// <summary>
        /// Gets the connection string for BAM.
        /// </summary>
        public override string BamConnectionString
        {
            get
            {
                return this.directive.BamConnectionString;
            }
        }

        /// <summary>
        /// Gets a value that determines under what conditions the buffered
        /// data will be sent to the tracking database.
        /// </summary>
        /// <remarks>
        /// &lt;= 0 This value is not allowed.   If set to 0, the eventStream
        /// would never flush automatically and the application would have to
        /// call the Flush method explicitly.   There is no obvious way to do
        /// this in most common resolution scenarios
        /// 1       Each event will be immediately persisted in the BAM database.
        /// &gt; 1  The eventStream will accumulate the events in memory until the
        /// event count equals or exceeds this threshold; at this point, the Flush
        /// method will be called internally.
        /// </remarks>
        public override int BamFlushThreshold
        {
            get
            {
                return this.directive.BamFlushThreshold;
            }
        }

        /// <summary>
        /// Gets a value indicating whether BAM will use a buffered event stream.
        /// </summary>
        public override bool BamIsBuffered
        {
            get
            {
                return this.directive.BamIsBuffered;
            }
        }

        /// <summary>
        /// Gets the name of the step within the BAM activity to which this directive applies.
        /// </summary>
        public override string BamStepName
        {
            get
            {
                return this.directive.BamStepName;
            }
        }

        /// <summary>
        /// Gets a list of steps that extend the step specified in the StepName property.
        /// </summary>
        public override IList<string> BamStepExtensions
        {
            // NB. The type is List<string> rather than IList<string> in order to be serialisable.
            get
            {
                return this.directive.BamStepExtensions;
            }
        }

        /// <summary>
        /// Gets the BAM Trackpoint policy name.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        public override string BamTrackpointPolicyName
        {
            get
            {
                return this.directive.BamTrackpointPolicyName;
            }
        }

        /// <summary>
        /// Gets the BAM Trackpoint policy bamTrackpointVersion.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        public override string BamTrackpointPolicyVersion
        {
            get
            {
                return this.directive.BamTrackpointPolicyVersion;
            }
        }

        /// <summary>
        /// Gets the BizTalk Server property values.
        /// </summary>
        public override BtsProperties BtsProperties
        {
            get
            {
                return this.directive.BtsProperties;
            }
        }

        /// <summary>
        /// Gets the message endpoint.
        /// </summary>
        public override string EndPoint
        {
            get
            {
                return this.directive.EndPoint;
            }
        }

        /// <summary>
        /// Gets the end point configuration token.
        /// </summary>
        public override string EndPointConfiguration
        {
            get
            {
                return this.directive.EndPointConfiguration;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to throw an error if a validation rule policy indicates invalidity.
        /// </summary>
        public override bool ErrorOnInvalid
        {
            get
            {
                return this.directive.ErrorOnInvalid;
            }
        }

        /// <summary>
        /// Gets the BAM event stream.
        /// </summary>
        public override EventStream EventStream
        {
            get
            {
                return this.directive.EventStream;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current time is within the specified time service window.
        /// </summary>
        public override bool InServiceWindow
        {
            get
            {
                return this.directive.InServiceWindow;
            }
        }

        /// <summary>
        /// Gets the full name of a map.
        /// </summary>
        public override string MapFullName
        {
            get
            {
                return this.directive.MapFullName;
            }
        }

        /// <summary>
        /// Gets a collection of strong names for the map target schemas.
        /// </summary>
        /// <remarks>This property is only set after a map has been executed.</remarks>
        public override IEnumerable<string> MapTargetSchemaStrongNames
        {
            get
            {
                return this.directive.MapTargetSchemaStrongNames;
            }
        }

        /// <summary>
        /// Gets the type of the map to apply.
        /// </summary>
        public override Type MapType
        {
            get
            {
                return this.directive.MapType;
            }
        }

        /// <summary>
        /// Gets the name of directive used as a key in the policy.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.directive.Name;
            }
        }

        /// <summary>
        /// Gets the BizTalk Server property values.
        /// </summary>
        public override Resolution.Dictionaries.Properties Properties
        {
            get
            {
                return this.directive.Properties;
            }
        }

        /// <summary>
        /// Gets the number of retries for the current level.
        /// </summary>
        public override int RetryCount
        {
            get
            {
                return this.directive.RetryCount;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Retry Count has been specified
        /// </summary>
        public override bool RetryCountSpecified
        {
            get
            {
                return this.directive.RetryCountSpecified;
            }
        }

        /// <summary>
        /// Gets the interval between retries.
        /// </summary>
        /// <remarks>
        /// Resolver clients are free
        /// to interpret this using any unit of time, but minutes is recommended
        /// (in keeping with BizTalk Send Ports).
        /// </remarks>
        public override int RetryInterval
        {
            get
            {
                return this.directive.RetryInterval;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Retry Interval has been set
        /// </summary>
        public override bool RetryIntervalSpecified
        {
            get
            {
                return this.directive.RetryIntervalSpecified;
            }
        }

        /// <summary>
        /// Gets the a level indicator for the retry
        /// </summary>
        /// <remarks>
        /// Retries must sometimes be carried out at different levels.   For example, we may want to
        /// carry out 3 retries at a minute interval at level 0, and then wrap this in an outer
        /// loop that retries 5 times each hour (level 1).
        /// </remarks>
        public override int RetryLevel
        {
            get
            {
                return this.directive.RetryLevel;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Retry Level has been specified
        /// </summary>
        public override bool RetryLevelSpecified
        {
            get
            {
                return this.directive.RetryLevelSpecified;
            }
        }

        /// <summary>
        /// Gets the time at which service window opens.
        /// </summary>
        public override DateTime ServiceWindowStartTime
        {
            get
            {
                return this.directive.ServiceWindowStartTime;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Service Window start time has been specified
        /// </summary>
        public override bool ServiceWindowStartTimeSpecified
        {
            get
            {
                return this.directive.ServiceWindowStartTimeSpecified;
            }
        }

        /// <summary>
        /// Gets the time at which service window closes.
        /// </summary>
        public override DateTime ServiceWindowStopTime
        {
            get
            {
                return this.directive.ServiceWindowStopTime;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Service Window stop time has been specified.
        /// </summary>
        public override bool ServiceWindowStopTimeSpecified
        {
            get
            {
                return this.directive.ServiceWindowStopTimeSpecified;
            }
        }

        /// <summary>
        /// Gets a URI indicating the intent of the SOAP operation.
        /// </summary>
        public override string SoapAction
        {
            get
            {
                return this.directive.SoapAction;
            }
        }

        /// <summary>
        /// Gets the transport type.
        /// </summary>
        public override string TransportType
        {
            get
            {
                return this.directive.TransportType;
            }
        }

        /// <summary>
        /// Gets the validation policy name.
        /// </summary>
        public override string ValidationPolicyName
        {
            get
            {
                return this.directive.ValidationPolicyName;
            }
        }

        /// <summary>
        /// Gets the validation policy version.
        /// </summary>
        public override string ValidationPolicyVersion
        {
            get
            {
                return this.directive.ValidationPolicyVersion;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        public new void Dispose()
        {
            // dispose of resources
            this.Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a BTS property
        /// </summary>
        /// <param name="name">
        /// Name of BTS property (e.g., BTS.MessageType)
        /// </param>
        /// <returns>
        /// An object that provides a full description of a BTS property
        /// </returns>
        public override DirectivesDictionaryItemValueDirectiveItemValue1 GetBtsProperty(string name)
        {
            return this.directive == null ? null : this.directive.GetBtsProperty(name);
        }

        /// <summary>
        /// Returns the element name for a BTS property
        /// </summary>
        /// <param name="name">
        /// Name of BTS property (e.g., BTS.MessageType)
        /// </param>
        /// <returns>
        /// The element name of the BTS property.
        /// </returns>
        public override string GetBtsPropertyName(string name)
        {
            return this.directive == null ? null : this.directive.GetBtsPropertyName(name);
        }

        /// <summary>
        /// Returns the namespace for a BTS property
        /// </summary>
        /// <param name="name">
        /// Name of BTS property (e.g., BTS.MessageType)
        /// </param>
        /// <returns>
        /// The namespace of the BTS property.
        /// </returns>
        public override string GetBtsPropertyNamespace(string name)
        {
            return this.directive == null ? null : this.directive.GetBtsPropertyNamespace(name);
        }

        /// <summary>
        /// Returns the value of a BTS property
        /// </summary>
        /// <param name="name">
        /// Name of BTS property (e.g., BTS.MessageType)
        /// </param>
        /// <returns>
        /// The value of the BTS property
        /// </returns>
        public override string GetBtsPropertyValue(string name)
        {
            return this.directive == null ? null : this.directive.GetBtsPropertyValue(name);
        }

        /// <summary>
        /// Returns a general property.
        /// </summary>
        /// <param name="name">
        /// Name of the property
        /// </param>
        /// <returns>
        /// An object that provides full details of the property.
        /// </returns>
        public override DirectivesDictionaryItemValueDirectiveItemValue GetProperty(string name)
        {
            return this.directive == null ? null : this.directive.GetProperty(name);
        }

        /// <summary>
        /// Returns the value of a general property.
        /// </summary>
        /// <param name="name">
        /// The property name.
        /// </param>
        /// <returns>
        /// The value of the named property.
        /// </returns>
        public override string GetPropertyValue(string name)
        {
            return this.directive == null ? null : this.directive.GetPropertyValue(name);
        }

        /// <summary>
        /// Returns the 'promoted' flag for a BTS property
        /// </summary>
        /// <param name="name">
        /// Name of BTS property (e.g., BTS.MessageType)
        /// </param>
        /// <returns>
        /// True, if marked as promoted
        /// </returns>
        public override bool IsPromotedBtsProperty(string name)
        {
            return this.directive != null && this.directive.IsPromotedBtsProperty(name);
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity. Call this method on every
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="data">
        /// The BAM step data.
        /// </param>
        public void OnStep(BamStepData data)
        {
            if (this.directive == null)
            {
                return;
            }

            this.directive.OnStep(data);
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity. Call this method on every
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="data">
        /// The BAM step data.
        /// </param>
        public override void OnStep(Resolution.BamStepData data)
        {
            if (this.directive == null)
            {
                return;
            }

            this.directive.OnStep(data);
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity. Call this method on every
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="data">
        /// The BAM step data.
        /// </param>
        /// <param name="afterMap">
        /// Indicates if the step is after the application of a map.
        /// </param>
        public void OnStep(BamStepData data, bool afterMap)
        {
            if (this.directive == null)
            {
                return;
            }

            this.directive.OnStep(data, afterMap);
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity. Call this method on every
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="data">
        /// The BAM step data.
        /// </param>
        /// <param name="afterMap">
        /// Indicates if the step is after the application of a map.
        /// </param>
        public override void OnStep(Resolution.BamStepData data, bool afterMap)
        {
            if (this.directive == null)
            {
                return;
            }

            this.directive.OnStep(data, afterMap);
        }

        /// <summary>
        /// Resets the event stream.  If the event stream has previously been set, it will
        /// revert to an event stream specified by the directive.
        /// </summary>
        public override void ResetEventStream()
        {
            if (this.directive == null)
            {
                return;
            }

            this.directive.ResetEventStream();
        }

        /// <summary>
        /// Transform the message by applying a map.
        /// </summary>
        /// <param name="messageIn">
        /// The inbound message.
        /// </param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        public override XmlDocument Transform(XmlDocument messageIn)
        {
            return this.directive == null ? null : this.directive.Transform(messageIn);
        }

        /// <summary>
        /// Transforms the aggregation of two XML messages by applying a map.
        /// </summary>
        /// <param name="messageIn1">
        /// The first inbound message.
        /// </param>
        /// <param name="messageIn2">
        /// The second inbound message.
        /// </param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        public override XmlDocument Transform(XmlDocument messageIn1, XmlDocument messageIn2)
        {
            return this.directive == null ? null : this.directive.Transform(messageIn1, messageIn2);
        }

        /// <summary>
        /// Transform the message by applying a map.   Invoke BAM interception as required.
        /// </summary>
        /// <param name="messageIn">
        /// The inbound message.
        /// </param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        public override XmlDocument TransformWithInterception(XmlDocument messageIn)
        {
            return this.directive == null ? null : this.directive.TransformWithInterception(messageIn);
        }

        /// <summary>
        /// Transform the message by applying a map.   Invoke BAM interception as required.
        /// </summary>
        /// <param name="messageIn">
        /// The inbound message.
        /// </param>
        /// <param name="messageProperties">
        /// A dictionary of message properties.
        /// </param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        public override XmlDocument TransformWithInterception(XmlDocument messageIn, IDictionary messageProperties)
        {
            return this.directive == null
                       ? null
                       : this.directive.TransformWithInterception(messageIn, messageProperties);
        }

        /// <summary>
        /// Validate an XML document using a BRE policy.
        /// </summary>
        /// <param name="xmlDocument">
        /// The XML message document to be validated.
        /// </param>
        /// <param name="documentType">
        /// The document type of the XML document, as used in the validation rules.
        /// </param>
        /// <returns>
        /// A <see cref="Validations"/> object containing the results of all validations.
        /// </returns>
        public Validations ValidateDocument(XmlNode xmlDocument, string documentType)
        {
            var validations = new Validations();

            if (this.directive == null)
            {
                return validations;
            }

            if (string.IsNullOrWhiteSpace(this.directive.ValidationPolicyName))
            {
                return validations;
            }

            var trace = Convert.ToBoolean(ConfigurationManager.AppSettings[Resources.AppSettingsEsbBreTrace]);
            var tempFileName = string.Empty;

            if (trace)
            {
                var traceFileFolder = ConfigurationManager.AppSettings[Resources.AppSettingsEsbBreTraceFileLocation];

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
                    @"{0}\ValidationPolicyTrace_{1}_{2}.txt",
                    traceFileFolder,
                    DateTime.Now.ToString("yyyyMMdd"),
                    Guid.NewGuid());
            }

            var breValidations = new Libraries.Facts.Validations();

            // Determine if static support is being used by rule engine and create the array of short-term facts 
            var shortTermFacts = IsStaticSupport()
                                          ? new object[]
                                                {
                                                   new TypedXmlDocument(documentType, xmlDocument), breValidations 
                                                }
                                          : new object[]
                                                {
                                                    new TypedXmlDocument(documentType, xmlDocument), breValidations, 
                                                    new XmlHelper()
                                                };

            // Assert the XML messsage and a validation object.
            var policyName = this.directive.ValidationPolicyName;
            Version version;
            Version.TryParse(this.directive.ValidationPolicyVersion, out version);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings[Resources.AppSettingsEsbBrePolicyTester]))
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
                        throw new RuleSetNotFoundException(
                            string.Format(Resources.ExceptionRsNotInStore, policyName, maj, min));
                    }

                    policyTester = new PolicyTester(ruleSet);

                    if (trace)
                    {
                        // Create the debug tracking object
                        var dti = new DebugTrackingInterceptor(tempFileName);

                        // Execute the policy with trace
                        policyTester.Execute(shortTermFacts, dti);

                        Trace.Write(
                            "[Esb.BizTalk.Orchestration.Directive] ValidateMessage Trace: " + dti.GetTraceOutput());
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

                        Trace.Write(
                            "[Esb.BizTalk.Orchestration.Directive] ValidateMessage Trace: " + dti.GetTraceOutput());
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

            // Throw an exception if dictated by the policy
            if (this.directive.ErrorOnInvalid && breValidations.ErrorCount > 0)
            {
                throw new ValidationException(string.Format("\r\nValidation Errors:\r\n{0}", breValidations.ToString(Libraries.Facts.ValidationLevel.Error)));
            }

            return validations.InitialiseFromBreValidations(breValidations);
        }

        /// <summary>
        /// Validate an XLANG message using a BRE policy.
        /// </summary>
        /// <param name="bizTalkMessage">
        /// The XLANG message.
        /// </param>
        /// <returns>
        /// A <see cref="Validations"/> object containing the results of all validations.
        /// </returns>
        public Validations ValidateMessage(XLANGMessage bizTalkMessage)
        {
            var validations = new Validations();

            if (this.directive == null)
            {
                return validations;
            }

            if (string.IsNullOrWhiteSpace(this.directive.ValidationPolicyName))
            {
                return validations;
            }

            // Invoke private method to unwrap message.
            var unwrappedBizTalkMessage =
                (BTXMessage)
                bizTalkMessage.GetType()
                    .GetMethod("Unwrap", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(bizTalkMessage, null);

            // Get the XML content (if any) of the body part
            var part = unwrappedBizTalkMessage.BodyPart ?? unwrappedBizTalkMessage[0];

            if (part == null)
            {
                return validations;
            }

            XmlDocument xmlDocument;

            try
            {
                xmlDocument = (XmlDocument)part.RetrieveAs(typeof(XmlDocument));
            }
            catch
            {
                return validations;
            }

            return this.ValidateDocument(xmlDocument, part.GetPartType().FullName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Dispose the current object.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the current object has been disposed.
        /// </param>
        protected new void Dispose(bool disposing)
        {
            // Ensure that buffers are always flushed 
            this.directive.Dispose();
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
                regKey = Registry.LocalMachine.OpenSubKey(Resources.RegKey);

                if (regKey == null)
                {
                    regKey = Registry.LocalMachine.OpenSubKey(Resources.RegKeyWow6432);

                    if (regKey == null)
                    {
                        throw new RuleStoreConnectionParametersException(Resources.ExceptionNoConnectionParameters);
                    }
                }

                dataBaseName = regKey.GetValue(Resources.RegKeyDatabaseName);
                dataBaseServer = regKey.GetValue(Resources.RegKeyDatabaseServer);

                if (dataBaseName == null || dataBaseServer == null)
                {
                    throw new RuleStoreConnectionParametersException(Resources.ExceptionNoConnectionParameters);
                }
            }
            finally
            {
                if (regKey != null)
                {
                    regKey.Close();
                }
            }

            return string.Format(Resources.ConnectionString, dataBaseServer, dataBaseName);
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
            var registryKey =
                Registry.LocalMachine.OpenSubKey(
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

        #endregion
    }
}