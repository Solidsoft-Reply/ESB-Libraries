// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectiveEventStream.cs" company="Solidsoft Reply Ltd.">
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
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    using Microsoft.BizTalk.Bam.EventObservation;

    using SolidsoftReply.Esb.Libraries.Resolution.Properties;

    /// <summary>
    /// Represents a BAM event stream that supports configuration via a directive.
    /// </summary>
    /// <remarks>
    /// This class may be used directly in code in scenarios where the ESB Libraries'
    /// support for BAM interception is not being used. In this case, the directive
    /// over which the event stream is created will only be used to specify the 
    /// connection configuration for BAM for a notional step.
    /// </remarks>
    [ComVisible(true)]
    [Serializable]
    public class DirectiveEventStream : EventStream
    {
        /// <summary>
        /// A lock used to control setting the directive.
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        /// A resolver directive used by the event stream.
        /// </summary>
        private Directive directive;

        /// <summary>
        /// A BAM event stream.
        /// </summary>
        private volatile EventStream eventStream;

        /// <summary>
        /// Indicates whether an attempt has been made to read the app settings for BAM configuration
        /// </summary>
        private bool appSettingsRead;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectiveEventStream"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        public DirectiveEventStream(Directive directive)
        {
            this.Directive = directive;
            this.UpdateDirectiveBamData(directive);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectiveEventStream"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        /// <param name="eventStream">
        /// Inner event stream wrapped by the directive event stream.
        /// </param>
        public DirectiveEventStream(Directive directive, EventStream eventStream)
        {
            this.Directive = directive;
            this.eventStream = eventStream;
        }

        /// <summary>
        /// Gets or sets a resolver directive used by the event stream.
        /// </summary>
        public Directive Directive
        {
            get
            {
                return this.directive;
            }

            protected set
            {
                this.directive = value;
            }
        }

        /// <summary>
        /// Gets the inner BAM event stream.
        /// </summary>
        public EventStream InnerEventStream
        {
            get
            {
                return this.eventStream;
            }
        }

        /// <summary>
        /// Gets or sets the BAM Trackpoint policy bamTrackpointVersion.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public virtual string BamTrackpointPolicyVersion { get; set; }

        /// <summary>
        /// Gets or sets the connection string for BAM.
        /// </summary>
        internal string BamConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether BAM will use a buffered event stream.
        /// </summary>
        internal bool BamIsBuffered { get; set; }

        /// <summary>
        /// Gets or sets a value that determines under what conditions the buffered 
        /// data will be sent to the tracking database.
        /// </summary>
        /// <remarks>
        /// &lt;= 0 This value is not allowed.   If set to 0, the eventStream 
        ///         would never flush automatically and the application would have to
        ///         call the Flush method explicitly.   There is no obvious way to do
        ///         this in most common resolution scenarios
        /// 1       Each event will be immediately persisted in the BAM database. 
        /// &gt; 1  The eventStream will accumulate the events in memory until the 
        ///         event count equals or exceeds this threshold; at this point, the Flush 
        ///         method will be called internally. 
        /// </remarks>
        internal int BamFlushThreshold { get; set; }

        /// <summary>
        /// Gets or sets the BAM Trackpoint policy name.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        internal string BamTrackpointPolicyName { get; set; }

        /// <summary>
        /// Provides the current activity instance with a reference to additional data.
        /// </summary>
        /// <param name="activityName">The current activity name.  Activity names are limited to 128 characters.</param>
        /// <param name="activityId">The current activity instance ID.  Activity identifiers are limited to 128 characters.</param>
        /// <param name="referenceType">The related item type.  Reference type identifiers are limited to 128 characters.</param>
        /// <param name="referenceName">The related item name.  Reference names are limited to 128 characters.</param>
        /// <param name="referenceData">The related item data.  Limited to 1024 characters of data.</param>
        public override void AddReference(string activityName, string activityId, string referenceType, string referenceName, string referenceData)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.AddReference(activityName, activityId, referenceType, referenceName, referenceData);
        }

        /// <summary>
        /// Provides the current activity instance with a reference to an item containing up to 512 KB of 
        /// Unicode characters of data.
        /// </summary>
        /// <param name="activityName">The current activity name.  Activity names are limited to 128 characters.</param>
        /// <param name="activityId">The current activity instance ID.  Activity identifiers are limited to 128 characters.</param>
        /// <param name="referenceType">The related item type.  Reference type identifiers are limited to 128 characters.</param>
        /// <param name="referenceName">The related item name.  Reference names are limited to 128 characters.</param>
        /// <param name="referenceData">The related item data.  Limited to 1024 characters of data.</param>
        /// <param name="longreferenceData">The related item data containing up to 512 KB of Unicode characters of data.</param>
        public override void AddReference(string activityName, string activityId, string referenceType, string referenceName, string referenceData, string longreferenceData)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.AddReference(activityName, activityId, referenceType, referenceName, referenceData, longreferenceData);
        }

        /// <summary>
        /// Specifies a relationship between the current activity instance and another BAM activity instance.
        /// </summary>
        /// <param name="activityName">The current activity name.</param>
        /// <param name="activityId">The current activity instance ID.</param>
        /// <param name="relatedActivityName">The related activity name.</param>
        /// <param name="relatedTraceId">The related activity instance ID.</param>
        public override void AddRelatedActivity(string activityName, string activityId, string relatedActivityName, string relatedTraceId)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.AddRelatedActivity(
                activityName,
                activityId,
                relatedActivityName,
                relatedTraceId);
        }

        /// <summary>
        /// Starts a BAM activity.  A new activity record will be created if data is tracked using the 
        /// UpdateActivity method.
        /// </summary>
        /// <param name="activityName">The name of the activity.</param>
        /// <param name="activityInstance">The activity instance ID.  The activity Instance ID must be unique.</param>
        public override void BeginActivity(string activityName, string activityInstance)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.BeginActivity(activityName, activityInstance);
        }

        /// <summary>
        /// Clears the buffered data.
        /// </summary>
        public override void Clear()
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.Clear();
        }

        /// <summary>
        /// Enables the continuation of the current BAM activity instance using a continuation token.  
        /// Data tracked in a different context can contribute to the current activity record.
        /// </summary>
        /// <param name="activityName">The activity name.</param>
        /// <param name="activityInstance">The activity instance ID or continuation token.</param>
        /// <param name="continuationToken">The continuation token used to send additional data to the activity record.</param>
        public override void EnableContinuation(string activityName, string activityInstance, string continuationToken)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.EnableContinuation(activityName, activityInstance, continuationToken);
        }

        /// <summary>
        /// Ends the current activity instance.  Indicates that there are no more events expected for the given activity instance or continuation token.
        /// </summary>
        /// <param name="activityName">The activity name.</param>
        /// <param name="activityInstance">The activity instance ID or continuation token.</param>
        public override void EndActivity(string activityName, string activityInstance)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.EndActivity(activityName, activityInstance);
        }

        /// <summary>
        /// Updates or inserts an activity record for a named milestone or data item.
        /// </summary>
        /// <param name="activityName">The activity name.</param>
        /// <param name="activityInstance">The activity instance ID or continuation token.</param>
        /// <param name="data">All data items that must be updated as name-value pairs.</param>
        public override void UpdateActivity(string activityName, string activityInstance, params object[] data)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.UpdateActivity(activityName, activityInstance, data);
        }

        /// <summary>
        /// Flushes the event stream.
        /// </summary>
        public override void Flush()
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.Flush();
        }

        /// <summary>
        /// Flushes the event stream for a given SQL connection.
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        public override void Flush(SqlConnection connection)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.Flush(connection);
        }

        /// <summary>
        /// Stores a custom serialized event.
        /// </summary>
        /// <param name="singleEvent">The event to be serialized.</param>
        public override void StoreCustomEvent(IPersistQueryable singleEvent)
        {
            if (this.eventStream == null)
            {
                return;
            }

            this.eventStream.StoreCustomEvent(singleEvent);
        }

        /// <summary>
        /// Updates the event stream with BAM data from the current directive.
        /// </summary>
        /// <param name="directive">The directive.</param>
        // ReSharper disable once ParameterHidesMember
        internal void UpdateDirectiveBamData(Directive directive)
        {
            lock (this.syncLock)
            {
                // Copy directive information as required
                if (this.Directive != null)
                {
                    // We are only changing the directive, but not the event stream, so
                    // we will update the directive to conform to the current event stream.
                    this.BamConnectionString = this.Directive.BamConnectionString;
                    this.BamIsBuffered = this.Directive.BamIsBuffered;
                    this.BamFlushThreshold = this.Directive.BamFlushThreshold;

                    if (string.IsNullOrWhiteSpace(directive.BamTrackpointPolicyName))
                    {
                        this.BamTrackpointPolicyName = this.Directive.BamTrackpointPolicyName;
                    }

                    if (string.IsNullOrWhiteSpace(directive.BamTrackpointPolicyVersion))
                    {
                        this.BamTrackpointPolicyVersion = this.Directive.BamTrackpointPolicyVersion;
                    }
                }

                this.InitializeEventStreamFromDirective();
            }
        }

        /// <summary>
        /// Initializes the event stream from the directive.
        /// </summary>
        private void InitializeEventStreamFromDirective()
        {
            try
            {
                this.eventStream = this.BamIsBuffered
                                       ? (EventStream)
                                         new BufferedEventStream(
                                             this.BamConnectionString,
                                             this.BamFlushThreshold)
                                       : new DirectEventStream(
                                             this.BamConnectionString,
                                             this.BamFlushThreshold);
            }
            catch
            {
                // TODO: Log a warning here.  The failure to initialize an event stream
                // suggests that the developer has tried to create a directive over an invalid
                // directive name or has failed to set the BAM connection data in the directive.
                return;
            }

            this.ReadBamAppSettings();
        }

        /// <summary>
        /// Reads the BAM application settings from config file, if they exist.
        /// </summary>
        private void ReadBamAppSettings()
        {
            if (this.appSettingsRead)
            {
                return;
            }

            // If the BamIsBuffered app setting is set correctly in the local config file, then this will
            // override any setting returned via the policy.
            try
            {
                var appSettingBamIsBuffered = ConfigurationManager.AppSettings[Resources.AppSettingEsbBamIsBuffered];

                if (!string.IsNullOrEmpty(appSettingBamIsBuffered))
                {
                    switch (appSettingBamIsBuffered.Trim().ToLower())
                    {
                        case "yes":
                        case "1":
                        case "true":
                            this.BamIsBuffered = true;
                            break;
                        case "no":
                        case "0":
                        case "false":
                            this.BamIsBuffered = false;
                            break;
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // TODO: Configuration error.  Log this
            }

            // If the BamFlushThreshold app setting is set correctly in the local config file, then this will
            // override any setting returned via the policy.
            try
            {
                var appSettingBamFlushThreshold =
                    ConfigurationManager.AppSettings[Resources.AppSettingEsbBamFlushThreshold];
                if (!string.IsNullOrWhiteSpace(appSettingBamFlushThreshold))
                {
                    this.BamFlushThreshold = Convert.ToInt32(appSettingBamFlushThreshold.Trim());
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // TODO: Configuration error or non-numeric.  Log this
            }

            // Check the local application settings.   If set, these will override any connection string 
            // assigned in the policy.
            try
            {
                var appSettingBamBufferedConnectionString =
                    ConfigurationManager.AppSettings[Resources.AppSettingEsbBamBufferedConnectionString];
                var appSettingBamDirectConnectionString =
                    ConfigurationManager.AppSettings[Resources.AppSettingEsbBamDirectConnectionString];

                if (!string.IsNullOrWhiteSpace(appSettingBamBufferedConnectionString)
                    && !string.IsNullOrWhiteSpace(appSettingBamDirectConnectionString))
                {
                    this.BamConnectionString = this.Directive.BamIsBuffered
                                                    ? appSettingBamBufferedConnectionString.Trim()
                                                    : appSettingBamDirectConnectionString.Trim();
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // TODO: Configuration error.  Log this
            }

            this.appSettingsRead = true;
        }
    }
}
