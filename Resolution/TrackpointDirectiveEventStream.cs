// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackpointDirectiveEventStream.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    using Microsoft.BizTalk.Bam.EventObservation;

    using SolidsoftReply.Esb.Libraries.Resolution.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution.ResolutionService;

    /// <summary>
    /// Represents a BAM event stream that supports configuration via a directive.
    /// </summary>
    [ComVisible(true)]
    [Serializable]
    public class TrackpointDirectiveEventStream : DirectiveEventStream
    {
        /// <summary>
        /// A lock used to control setting the directive.
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        /// The current BAM activity instance.
        /// </summary>
        private readonly IDictionary<string, string> activityInstances = new Dictionary<string, string>();

        /// <summary>
        /// A token representing the current activity instance.  This assumes that a different instance of the
        /// event stream is used for each activity instance.
        /// </summary>
        private readonly string currentActivityInstanceToken = Guid.NewGuid().ToString();

        /// <summary>
        /// The BAM step data currently used by the stream.
        /// </summary>
        private BamStepData bamStepData;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackpointDirectiveEventStream"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        public TrackpointDirectiveEventStream(Directive directive)
            : base(directive)
        {
            this.BamStepData = new BamStepData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackpointDirectiveEventStream"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        /// <param name="data">Additional data for the current step.</param>
        public TrackpointDirectiveEventStream(Directive directive, BamStepData data)
            : base(directive)
        {
            this.BamStepData = data ?? new BamStepData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackpointDirectiveEventStream"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        /// <param name="eventStream">
        /// Inner event stream wrapped by the directive event stream.
        /// </param>DirectiveEventStream
        public TrackpointDirectiveEventStream(Directive directive, EventStream eventStream)
            : base(directive, eventStream)
        {
            this.BamStepData = new BamStepData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackpointDirectiveEventStream"/> class.
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        /// <param name="eventStream">
        /// Inner event stream wrapped by the directive event stream.
        /// </param>
        /// <param name="data">Additional data for the current step.</param>
        public TrackpointDirectiveEventStream(Directive directive, EventStream eventStream, BamStepData data)
            : base(directive, eventStream)
        {
            this.BamStepData = data ?? new BamStepData();
        }

        /// <summary>
        /// Gets or sets the BAM step data currently used by the stream.
        /// </summary>
        public BamStepData BamStepData
        {
            get
            {
                return this.bamStepData;
            }

            set
            {
                lock (this.syncLock)
                {
                    this.bamStepData = value;
                }
            }
        }

        /// <summary>
        /// Provides the current activity instance with references to additional data, as specified in the track points for the current step.
        /// </summary>
        public void AddReferences()
        {
            this.DoAddReferences(false, false, null);
        }

        /// <summary>
        /// Provides the current activity instance with references to additional data, as specified in the track points for the current step.
        /// </summary>
        /// <param name="storeMessage">Indicates whether the XML message content should be stored.</param>
        public void AddReferences(bool storeMessage)
        {
            this.DoAddReferences(storeMessage, false, null);
        }

        /// <summary>
        /// Provides the current activity instance with references to additional data, as specified in the track points for the current step.
        /// </summary>
        /// <param name="storeMessage">Indicates whether the XML message content should be stored.</param>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        public void AddReferences(bool storeMessage, bool afterMap)
        {
            this.DoAddReferences(storeMessage, afterMap, null);
        }

        /// <summary>
        /// Provides the current activity instance with references to additional data, as specified in the track points for the current step.
        /// </summary>
        /// <param name="storeMessage">Indicates whether the XML message content should be stored.</param>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        public void AddReferences(bool storeMessage, bool afterMap, string activityInstance)
        {
            this.DoAddReferences(storeMessage, afterMap, activityInstance);
        }

        /// <summary>
        /// Specifies relationships between the current activity instance and other BAM activity instances, as specified in the track points for the current step.
        /// </summary>
        public void AddRelatedActivities()
        {
            this.DoAddRelatedActivities(false, null);
        }

        /// <summary>
        /// Specifies relationships between the current activity instance and other BAM activity instances, as specified in the track points for the current step.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        public void AddRelatedActivities(bool afterMap)
        {
            this.DoAddRelatedActivities(afterMap, null);
        }

        /// <summary>
        /// Specifies relationships between the current activity instance and other BAM activity instances, as specified in the track points for the current step.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        public void AddRelatedActivities(bool afterMap, string activityInstance)
        {
            this.DoAddRelatedActivities(afterMap, activityInstance);
        }

        /// <summary>
        /// Continues a BAM activity instance under a new continuation token, as specified in the track points for the current step.
        /// </summary>
        public void ContinueActivity()
        {
            this.DoContinueActivity(false, null);
        }

        /// <summary>
        /// Continues a BAM activity instance under a new continuation token, as specified in the track points for the current step.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        public void ContinueActivity(bool afterMap)
        {
            this.DoContinueActivity(afterMap, null);
        }

        /// <summary>
        /// Continues a BAM activity instance under a new continuation token, as specified in the track points for the current step.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        public void ContinueActivity(bool afterMap, string activityInstance)
        {
            this.DoContinueActivity(afterMap, activityInstance);
        }

        /// <summary>
        /// Starts a BAM activity instance using the activity ID specified in the track points for the current step.
        /// </summary>
        public void BeginActivity()
        {
            this.DoBeginActivity(false, null);
        }

        /// <summary>
        /// Starts a BAM activity instance using the activity ID specified in the track points for the current step.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        public void BeginActivity(bool afterMap)
        {
            this.DoBeginActivity(afterMap, null);
        }

        /// <summary>
        /// Starts a BAM activity instance using the activity ID specified in the track points for the current step.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        public void BeginActivity(bool afterMap, string activityInstance)
        {
            this.DoBeginActivity(afterMap, activityInstance);
        }

        /// <summary>
        /// Enables continuations of the current BAM activity instance using the continuation tokens specified in the track points for the current step.
        /// </summary>
        public void EnableContinuations()
        {
            this.DoEnableContinuations(false, null);
        }

        /// <summary>
        /// Enables continuations of the current BAM activity instance using the continuation tokens specified in the track points for the current step.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        public void EnableContinuations(bool afterMap)
        {
            this.DoEnableContinuations(afterMap, null);
        }

        /// <summary>
        /// Enables continuations of the current BAM activity instance using the continuation tokens specified in the track points for the current step.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        public void EnableContinuations(bool afterMap, string activityInstance)
        {
            this.DoEnableContinuations(afterMap, activityInstance);
        }

        /// <summary>
        /// Ends the BAM activity instance specified by the current directive.
        /// </summary>
        public void EndActivity()
        {
            this.DoEndActivity(false, null);
        }

        /// <summary>
        /// Ends a BAM activity instance, as specified by the current directive.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        public void EndActivity(bool afterMap)
        {
            this.DoEndActivity(afterMap, null);
        }

        /// <summary>
        /// Ends a BAM activity instance, as specified by the current directive.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        public void EndActivity(bool afterMap, string activityInstance)
        {
            this.DoEndActivity(afterMap, activityInstance);
        }

        /// <summary>
        /// Inserts or updates milestones and data items for the current activity instance, as specified in the track points for the current step..
        /// </summary>
        public void UpdateActivityAll()
        {
            this.DoUpdateActivity(string.Empty, false, null);
        }

        /// <summary>
        /// Inserts or updates milestones and data items for the current activity instance, as specified in the track points for the current step..
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        public void UpdateActivityAll(bool afterMap)
        {
            this.DoUpdateActivity(string.Empty, afterMap, null);
        }

        /// <summary>
        /// Inserts or updates milestones and data items for the current activity instance, as specified in the track points for the current step..
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        public void UpdateActivityAll(bool afterMap, string activityInstance)
        {
            this.DoUpdateActivity(string.Empty, afterMap, activityInstance);
        }

        /// <summary>
        /// Inserts or updates a named milestone or data item for the current activity instance, as specified in the track points for the current step..
        /// </summary>
        /// <param name="itemName">The name of the data item.</param>
        public void UpdateActivity(string itemName)
        {
            this.DoUpdateActivity(itemName, false, null);
        }

        /// <summary>
        /// Inserts or updates a named milestone or data item for the current activity instance, as specified in the track points for the current step..
        /// </summary>
        /// <param name="itemName">The name of the data item.</param>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        public void UpdateActivity(string itemName, bool afterMap)
        {
            this.DoUpdateActivity(itemName, afterMap, null);
        }

        /// <summary>
        /// Inserts or updates a named milestone or data item for the current activity instance, as specified in the track points for the current step..
        /// </summary>
        /// <param name="itemName">The name of the data item.</param>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        public void UpdateActivity(string itemName, bool afterMap, string activityInstance)
        {
            this.DoUpdateActivity(itemName, afterMap, activityInstance);
        }

        /// <summary>
        /// Gets a registered activity instance.
        /// </summary>
        /// <param name="activityInstanceKey">The activity instance key.</param>
        /// <returns>An activity instance entry.</returns>
        private string GetActivityInstance(string activityInstanceKey)
        {
            try
            {
                return this.activityInstances[activityInstanceKey];
            }
            catch (KeyNotFoundException ex)
            {
                throw new EsbResolutionException(string.Format(Resources.ExceptionActivityNotRegistered, this.Directive.BamActivity, this.Directive.BamStepName));
            }
        }

        /// <summary>
        /// Adds references to the BAM import tables, as specified by the current directive.
        /// </summary>
        /// <param name="storeMessage">Indicates whether the XML message content should be stored.</param>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        private void DoAddReferences(bool storeMessage, bool afterMap, string activityInstance)
        {
            if (this.Directive == null || this.activityInstances == null)
            {
                return;
            }

            var referenceTrackPoints = this.GetTrackPoints(TrackPointType.Reference, afterMap);

            if (referenceTrackPoints == null)
            {
                return;
            }

            var activityInstanceKey = string.Format("{0}#{1}#{2}", this.Directive.BamActivity, this.Directive.BamStepName, activityInstance ?? this.currentActivityInstanceToken);
            var extractor = new XPathDataExtractorWithMacros(this.BamStepData.Properties, this.BamStepData.ValueList);

            foreach (var referenceTrackPoint in referenceTrackPoints)
            {
                var referenceData = extractor.GetValue(referenceTrackPoint.ExtractionInfo, referenceTrackPoint.Location, this.BamStepData.XmlDocument);

                if (referenceData == null)
                {
                    continue;
                }

                var messageContent = storeMessage
                                         ? this.BamStepData.XmlDocument != null ? this.BamStepData.XmlDocument.OuterXml : referenceData.ToString()
                                         : referenceData.ToString();

                this.InnerEventStream.AddReference(
                    this.Directive.BamActivity,
                    this.GetActivityInstance(activityInstanceKey),
                    referenceTrackPoint.ReferenceType,
                    referenceTrackPoint.ItemName,
                    referenceData.ToString(),
                    messageContent);
            }
        }

        /// <summary>
        /// Adds related BAM activities to the BAM import tables, as specified by the current directive.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        private void DoAddRelatedActivities(bool afterMap, string activityInstance)
        {
            if (this.Directive == null || this.activityInstances == null)
            {
                return;
            }

            var relationshipTrackPoints = this.GetTrackPoints(TrackPointType.Relationship, afterMap);

            if (relationshipTrackPoints == null)
            {
                return;
            }

            var activityInstanceKey = string.Format("{0}#{1}#{2}", this.Directive.BamActivity, this.Directive.BamStepName, activityInstance ?? this.currentActivityInstanceToken);
            var extractor = new XPathDataExtractorWithMacros(this.BamStepData.Properties, this.BamStepData.ValueList);

            foreach (var relationshipTrackPoint in relationshipTrackPoints)
            {
                var relatedTraceId = extractor.GetValue(relationshipTrackPoint.ExtractionInfo, relationshipTrackPoint.Location, this.BamStepData.XmlDocument);

                if (relatedTraceId == null)
                {
                    continue;
                }

                this.InnerEventStream.AddRelatedActivity(
                    this.Directive.BamActivity,
                    this.GetActivityInstance(activityInstanceKey),
                    relationshipTrackPoint.ItemName,
                    relatedTraceId.ToString());
            }
        }

        /// <summary>
        /// Starts a BAM activity, as specified by the current directive.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        private void DoBeginActivity(bool afterMap, string activityInstance)
        {
            if (this.Directive == null)
            {
                return;
            }

            var startTrackPoint = this.GetTrackPoints(TrackPointType.Start, afterMap).FirstOrDefault();

            if (startTrackPoint == null)
            {
                return;
            }

            var activityInstanceKey = string.Format("{0}#{1}#{2}", this.Directive.BamActivity, this.Directive.BamStepName, activityInstance ?? this.currentActivityInstanceToken);
            var extractor = new XPathDataExtractorWithMacros(this.BamStepData.Properties, this.BamStepData.ValueList);
            this.activityInstances.Add(activityInstanceKey, Convert.ToString(extractor.GetValue(startTrackPoint.ExtractionInfo, startTrackPoint.Location, this.BamStepData.XmlDocument) ?? string.Empty));
            this.InnerEventStream.BeginActivity(this.Directive.BamActivity, this.GetActivityInstance(activityInstanceKey));
        }

        /// <summary>
        /// Enables continuation of a BAM activity, as specified by the current directive.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        private void DoEnableContinuations(bool afterMap, string activityInstance)
        {
            if (this.Directive == null || this.activityInstances == null)
            {
                return;
            }

            var enableContinuationTrackPoints = this.GetTrackPoints(TrackPointType.EnableContinuation, afterMap);

            if (enableContinuationTrackPoints == null)
            {
                return;
            }

            var activityInstanceKey = string.Format("{0}#{1}#{2}", this.Directive.BamActivity, this.Directive.BamStepName, activityInstance ?? this.currentActivityInstanceToken);
            var extractor = new XPathDataExtractorWithMacros(this.BamStepData.Properties, this.BamStepData.ValueList);

            foreach (var continuationToken in enableContinuationTrackPoints.Select(enableContinuationTrackPoint => extractor.GetValue(enableContinuationTrackPoint.ExtractionInfo, enableContinuationTrackPoint.Location, this.BamStepData.XmlDocument)).Where(continuationToken => continuationToken != null))
            {
                this.InnerEventStream.EnableContinuation(
                    this.Directive.BamActivity,
                    this.GetActivityInstance(activityInstanceKey),
                    continuationToken.ToString());
            }
        }

        /// <summary>
        /// Continues a BAM activity, as specified by the current directive.
        /// </summary>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        private void DoContinueActivity(bool afterMap, string activityInstance)
        {
            if (this.Directive == null)
            {
                return;
            }

            var continueTrackPoint = this.GetTrackPoints(TrackPointType.Continue, afterMap).FirstOrDefault();

            if (continueTrackPoint == null)
            {
                return;
            }

            var activityInstanceKey = string.Format("{0}#{1}#{2}", this.Directive.BamActivity, this.Directive.BamStepName, activityInstance ?? this.currentActivityInstanceToken);
            var extractor = new XPathDataExtractorWithMacros(this.BamStepData.Properties, this.BamStepData.ValueList);
            this.activityInstances.Add(activityInstanceKey, string.Format("{0}{1}", continueTrackPoint.ItemName, Convert.ToString(extractor.GetValue(continueTrackPoint.ExtractionInfo, continueTrackPoint.Location, this.BamStepData.XmlDocument) ?? string.Empty)));
        }

        /// <summary>
        /// Ends a BAM activity, as specified by the current directive.
        /// </summary>continueTrackPoint.ItemName
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        private void DoEndActivity(bool afterMap, string activityInstance)
        {
            if (this.Directive == null)
            {
                return;
            }

            var endTrackPoint = this.GetTrackPoints(TrackPointType.End, afterMap).FirstOrDefault();

            if (endTrackPoint == null)
            {
                return;
            }

            var activityInstanceKey = string.Format("{0}#{1}#{2}", this.Directive.BamActivity, this.Directive.BamStepName, activityInstance ?? this.currentActivityInstanceToken);
            this.InnerEventStream.EndActivity(this.Directive.BamActivity, this.GetActivityInstance(activityInstanceKey));
            this.activityInstances.Clear();
        }

        /// <summary>
        /// Updates data in the import tables for a BAM activity, as specified by the current directive.
        /// </summary>
        /// <param name="itemName">The name of the data item.</param>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <param name="activityInstance">Optional trace instance identifier.</param>
        private void DoUpdateActivity(string itemName, bool afterMap, string activityInstance)
        {
            if (this.Directive == null || this.activityInstances == null)
            {
                return;
            }

            var dataTrackPoints = this.GetTrackPoints(TrackPointType.Data, afterMap);

            if (dataTrackPoints == null)
            {
                return;
            }

            var nameValuePairs = new ArrayList();
            var extractor = new XPathDataExtractorWithMacros(this.BamStepData.Properties, this.BamStepData.ValueList);
            var activityInstanceKey = string.Format("{0}#{1}#{2}", this.Directive.BamActivity, this.Directive.BamStepName, activityInstance ?? this.currentActivityInstanceToken);

            if (string.IsNullOrWhiteSpace(itemName))
            {
                foreach (var dataTrackPoint in dataTrackPoints)
                {
                    nameValuePairs.Add(dataTrackPoint.ItemName);
                    nameValuePairs.Add(
                        extractor.GetValue(dataTrackPoint.ExtractionInfo, dataTrackPoint.Location, this.BamStepData.XmlDocument));
                }
            }
            else
            {
                var dataTrackPoint =
                    (from dtp in dataTrackPoints where dtp.ItemName == itemName select dtp).FirstOrDefault();

                if (dataTrackPoint != null)
                {
                    nameValuePairs.Add(dataTrackPoint.ItemName);
                    nameValuePairs.Add(
                        extractor.GetValue(dataTrackPoint.ExtractionInfo, dataTrackPoint.Location, this.BamStepData.XmlDocument));
                }
            }

            this.InnerEventStream.UpdateActivity(this.Directive.BamActivity, this.GetActivityInstance(activityInstanceKey), nameValuePairs.ToArray());
        }

        /// <summary>
        /// Gets all BAM track points in the current directive of a given type.
        /// </summary>
        /// <param name="type">The type of track points required.</param>
        /// <param name="afterMap">Defines if the after-map step should be used.</param>
        /// <returns>A collection of BAM track points.</returns>
        private IEnumerable<ResolutionService.TrackPoint> GetTrackPoints(TrackPointType type, bool afterMap)
        {
            if (this.Directive == null)
            {
                return null;
            }

            var stepName = afterMap
                                  ? this.Directive.BamAfterMapStepName
                                  : string.IsNullOrWhiteSpace(this.Directive.BamStepName)
                                        ? this.Directive.BamAfterMapStepName
                                        : this.Directive.BamStepName;

            // We can safely assume that the version number is either missing or valid.
            var version = string.IsNullOrWhiteSpace(this.Directive.BamTrackpointPolicyVersion)
                              ? null
                              : new Version(this.Directive.BamTrackpointPolicyVersion);

            // Throw an exception if the directive is blank.
            Func<string> directiveInvalid = () =>
                {
                    throw new EsbResolutionException(Resources.ExceptionInvalidDirective);
                };

            var bamActivityStep = BamStepResolver.GetStep(
                this.Directive.BamActivity,
                string.IsNullOrWhiteSpace(stepName)
                ? directiveInvalid()
                : stepName,
                this.Directive.BamTrackpointPolicyName,
                version);

            // Because of a logic error in Microsoft's code, a separate ActivityInterceptorConfiguration must be used 
            // for each location.  The following code extracts only those track points for a given step name (location).
            return (from ResolutionService.TrackPoint tp in bamActivityStep.TrackPoints
                   where tp.Type == type && (string)tp.Location == stepName
                   select tp).ToList();
        }

        /// <summary>
        /// XPath data extractor for BAM interception.  Supports a simple macro language as an alternative
        /// approach.
        /// </summary>
        [Serializable]
        internal class XPathDataExtractorWithMacros : IBAMDataExtractor
        {
            /// <summary>
            /// A dictionary of macroProperties that can be used by the property macro.
            /// </summary>
            private readonly IDictionary macroProperties = new Hashtable();

            /// <summary>
            /// An array of values that can be used by format strings.
            /// </summary>
            private readonly IList macroValues = new ArrayList();

            /// <summary>
            /// Initializes a new instance of the <see cref="XPathDataExtractorWithMacros"/> class.
            /// </summary>
            /// <param name="properties">A dictionary of macroProperties that can be used by the property macro.</param>
            /// <param name="values">An array of values that can be used by format strings.</param>
            public XPathDataExtractorWithMacros(IDictionary properties, IList values = null)
            {
                this.macroProperties = properties ?? new Hashtable();
                this.macroValues = values ?? new ArrayList();
            }

            /// <summary>
            /// Returns a value from XML data using an XPath.
            /// </summary>
            /// <param name="extractionInfo">
            /// The XPath string.
            /// </param>
            /// <param name="location">
            /// The location.  Not used.
            /// </param>
            /// <param name="data">
            /// The XML data.
            /// </param>
            /// <returns>
            /// The value obtained by the XPath.
            /// </returns>
            /// <exception cref="EsbResolutionException">
            /// Indicates that BAM interception failed.
            /// </exception>
            public object GetValue(object extractionInfo, object location, object data)
            {
                if (!(extractionInfo is string) || string.IsNullOrEmpty((string)extractionInfo))
                {
                    throw new EsbResolutionException(string.Format(Resources.ExceptionInterceptionFailedNotXPath, extractionInfo));
                }

                Func<string, string, Tuple<Type, object>> processMacro = (macro, format) =>
                {
                    switch (macro)
                    {
                        case "now":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(DateTime), DateTime.Now)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.ToString(format, CultureInfo.CurrentCulture));
                        case "date":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(DateTime), DateTime.Now.Date)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Date.ToString(format, CultureInfo.CurrentCulture));
                        case "day":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(int), DateTime.Now.Day)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Day.ToString(format, CultureInfo.CurrentCulture));
                        case "dayofweek":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(string), DateTime.Now.DayOfWeek.ToString())
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.DayOfWeek.ToString(format));
                        case "dayofyear":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(int), DateTime.Now.DayOfYear)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.DayOfYear.ToString(format, CultureInfo.CurrentCulture));
                        case "hour":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(int), DateTime.Now.Hour)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Hour.ToString(format, CultureInfo.CurrentCulture));
                        case "millisecond":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(int), DateTime.Now.Millisecond)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Millisecond.ToString(format, CultureInfo.CurrentCulture));
                        case "minute":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(int), DateTime.Now.Minute)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Minute.ToString(format, CultureInfo.CurrentCulture));
                        case "month":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(int), DateTime.Now.Month)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Month.ToString(format, CultureInfo.CurrentCulture));
                        case "second":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(int), DateTime.Now.Second)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Second.ToString(format, CultureInfo.CurrentCulture));
                        case "ticks":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(long), DateTime.Now.Ticks)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Ticks.ToString(format, CultureInfo.CurrentCulture));
                        case "timeofday":
                            return new Tuple<Type, object>(typeof(string), DateTime.Now.TimeOfDay.ToString(format, CultureInfo.CurrentCulture));
                        case "today":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(DateTime), DateTime.Today)
                                : new Tuple<Type, object>(typeof(string), DateTime.Today.ToString(format, CultureInfo.CurrentCulture));
                        case "utcnow":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(DateTime), DateTime.UtcNow)
                                : new Tuple<Type, object>(typeof(string), DateTime.UtcNow.ToString(format, CultureInfo.CurrentCulture));
                        case "year":
                            return string.IsNullOrWhiteSpace(format)
                                ? new Tuple<Type, object>(typeof(int), DateTime.Now.Year)
                                : new Tuple<Type, object>(typeof(string), DateTime.Now.Year.ToString(format, CultureInfo.CurrentCulture));
                        case "guid":
                            return new Tuple<Type, object>(typeof(string), Guid.NewGuid().ToString(format, CultureInfo.CurrentCulture));
                        case "property":
                            try
                            {
                                var propertyType = this.macroProperties[format].GetType();

                                if (propertyType == typeof(string) ||
                                    propertyType == typeof(DateTime) ||
                                    propertyType == typeof(short) ||
                                    propertyType == typeof(int) ||
                                    propertyType == typeof(long) ||
                                    propertyType == typeof(decimal) ||
                                    propertyType == typeof(float) ||
                                    propertyType == typeof(double))
                                {
                                    return new Tuple<Type, object>(propertyType, this.macroProperties[format]);
                                }

                                return new Tuple<Type, object>(propertyType, this.macroProperties[format].ToString());
                            }
                            catch
                            {
                                throw new EsbResolutionException(String.Format(Resources.ExceptionPropertyNotAvailable, format));
                            }

                        case "if":
                            return new Tuple<Type, object>(typeof(string), string.Empty);
                        case "call":
                            return new Tuple<Type, object>(typeof(string), string.Empty);
                        case "eval":
                            return new Tuple<Type, object>(typeof(string), string.Empty);
                        case "regex":
                            return new Tuple<Type, object>(typeof(string), string.Empty);
                        default:
                            return new Tuple<Type, object>(typeof(string), macro);
                    }
                };

                var infoString = extractionInfo.ToString();
                var macrosExpanded = false;
                var atomicMacro = false;

                if (!string.IsNullOrWhiteSpace(infoString) && infoString.StartsWith("{") && infoString.EndsWith("}"))
                {
                    // Get raw macro string
                    var macroExpression = infoString.ToLower();

                    // Find all macros
                    var macros = Regex.Matches(macroExpression, Resources.MacroExpression);

                    // Expand the macros
                    if (macros.Count > 0)
                    {
                        var infoStringBuilder = new StringBuilder();
                        var startIndex = 0;
                        object rawValue = null;

                        if (macros.Count == 1)
                        {
                            atomicMacro = macros[0].Length == macroExpression.Length;
                        }

                        foreach (Match macro in macros)
                        {
                            if (macro.Groups["invalidParenthesis"].Success)
                            {
                                throw new EsbResolutionException(
                                    string.Format(
                                        Resources.ExceptionInterceptionInvalidParenthesis,
                                        infoString));
                            }

                            var macroGroup = macro.Groups["macro"];
                            var formatGroup = macro.Groups["format"];
                            var formatString = string.Empty;

                            if (macro.Groups["format"].Success)
                            {
                                formatString = infoString.Substring(formatGroup.Index, formatGroup.Length);
                            }

                            var expansion = processMacro(macroGroup.Value, formatString);

                            if (atomicMacro)
                            {
                                rawValue = expansion.Item2;
                                break;
                            }

                            infoStringBuilder.Append(infoString.Substring(startIndex, macroGroup.Index - 1) + expansion.Item2);

                            startIndex = string.IsNullOrWhiteSpace(formatString)
                                             ? macroGroup.Index + macroGroup.Length + 1
                                             : formatGroup.Index + formatGroup.Length + 1;
                        }

                        if (atomicMacro)
                        {
                            return rawValue;
                        }

                        macroExpression = infoStringBuilder.Append(macroExpression.Substring(startIndex)).ToString();
                        macrosExpanded = true;
                    }

                    // Determine if this is a format string
                    if (this.macroValues.Count <= 0)
                    {
                        return macroExpression;
                    }

                    // Get the format string and process it.
                    try
                    {
                        // Convert list of values to a parameter array
                        var array = new object[this.macroValues.Count];

                        for (var index = 0; index < this.macroValues.Count; index++)
                        {
                            array[index] = this.macroValues[index];
                        }

                        // Format the string
                        macroExpression = string.Format(infoString, array);
                    }
                    catch (FormatException ex)
                    {
                        if (macrosExpanded)
                        {
                            return macroExpression;
                        }

                        var matches = Regex.Matches(macroExpression, @"(?<!\{)(?>\{\{)*\{\d(.*?)");
                        var uniqueMatchCount = matches.OfType<Match>().Select(m => m.Value).Distinct().Count();
                        var parameterMatchCount = (uniqueMatchCount == 0)
                                                      ? 0
                                                      : matches.OfType<Match>()
                                                            .Select(m => m.Value)
                                                            .Distinct()
                                                            .Select(m => int.Parse(m.Replace("{", string.Empty)))
                                                            .Max() + 1;
                        throw new EsbResolutionException(
                            string.Format(
                                Resources.ExceptionWrongNumberOfMacroParameters,
                                parameterMatchCount,
                                this.macroValues.Count),
                            ex);
                    }

                    return macroExpression;
                }

                // Process as XPath
                var node = data as XmlNode;

                if (node == null)
                {
                    throw new EsbResolutionException(Resources.ExceptionInterceptionFailedNotXml);
                }

                var selectSingleNode = node.SelectSingleNode((string)extractionInfo);

                if (selectSingleNode == null)
                {
                    throw new EsbResolutionException(string.Format(Resources.ExceptionInterceptionFailedNotXPath, extractionInfo));
                }

                var valueExtracted = selectSingleNode.InnerText;

                return valueExtracted;
            }
        }
    }
}
