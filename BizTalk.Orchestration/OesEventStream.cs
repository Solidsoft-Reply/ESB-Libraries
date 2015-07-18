// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OesEventStream.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.BizTalk.Orchestration
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    using Microsoft.BizTalk.Bam.EventObservation;

    using BizTalkOrchestrationEventStream = Microsoft.BizTalk.Bam.EventObservation.OrchestrationEventStream;

    /// <summary>
    /// Wrapper around the OrchestrationEventStream provided by Microsoft.BizTalk.Bam.XLANGs.dll 
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [ComVisible(true)]
    [Serializable]
    public class OesEventStream : EventStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OesEventStream"/> class.
        /// </summary>
        public OesEventStream()
        {
        }

        /// <summary>
        /// Adds a reference to an item to the current activity instance.
        /// </summary>
        /// <param name="activityName">The current activity name.  Activity names are limited to 128 characters.</param>
        /// <param name="activityId">The current activity instance ID.  Activity identifiers are limited to 128 characters.</param>
        /// <param name="referenceType">The related item type.  Reference type identifiers are limited to 128 characters.</param>
        /// <param name="referenceName">The related item name.  Reference names are limited to 128 characters.</param>
        /// <param name="referenceData">The related item data.  Limited to 1024 characters of data.</param>
        public override void AddReference(string activityName, string activityId, string referenceType, string referenceName, string referenceData)
        {
            BizTalkOrchestrationEventStream.AddReference(activityName, activityId, referenceType, referenceName, referenceData);
        }

        /// <summary>
        /// Adds a reference to an item containing up to 512 KB of Unicode characters of data that relates to the current activity instance.
        /// </summary>
        /// <param name="activityName">The current activity name.  Activity names are limited to 128 characters.</param>
        /// <param name="activityId">The current activity instance ID.  Activity identifiers are limited to 128 characters.</param>
        /// <param name="referenceType">The related item type.  Reference type identifiers are limited to 128 characters.</param>
        /// <param name="referenceName">The related item name.  Reference names are limited to 128 characters.</param>
        /// <param name="referenceData">The related item data.  Limited to 1024 characters of data.</param>
        /// <param name="longreferenceData">The related item data containing up to 512 KB of Unicode characters of data.</param>
        public override void AddReference(string activityName, string activityId, string referenceType, string referenceName, string referenceData, string longreferenceData)
        {
            BizTalkOrchestrationEventStream.AddReference(activityName, activityId, referenceType, referenceName, referenceData, longreferenceData);
        }

        /// <summary>
        /// Declares another activity instance as related to the current instance.
        /// </summary>
        /// <param name="activityName">The current activity name.</param>
        /// <param name="activityId">The current activity instance ID.</param>
        /// <param name="relatedActivityName">The related activity name.</param>
        /// <param name="relatedTraceId">The related activity instance ID.</param>
        public override void AddRelatedActivity(string activityName, string activityId, string relatedActivityName, string relatedTraceId)
        {
            BizTalkOrchestrationEventStream.AddRelatedActivity(
                activityName,
                activityId,
                relatedActivityName,
                relatedTraceId);
        }

        /// <summary>
        /// Creates a new activity record if data is tracked using the UpdateActivity method.
        /// </summary>
        /// <param name="activityName">The name of the activity.</param>
        /// <param name="activityInstance">The activity instance ID.  The activity Instance ID must be unique.</param>
        public override void BeginActivity(string activityName, string activityInstance)
        {
            BizTalkOrchestrationEventStream.BeginActivity(activityName, activityInstance);
        }

        /// <summary>
        /// Clears the buffered data.
        /// </summary>
        public override void Clear()
        {
            BizTalkOrchestrationEventStream.Clear();
        }

        /// <summary>
        /// Enables data tracked in a different context to contribute to a given activity record.
        /// </summary>
        /// <param name="activityName">The activity name.</param>
        /// <param name="activityInstance">The activity instance ID or continuation token.</param>
        /// <param name="continuationToken">The continuation token used to send additional data to the activity record.</param>
        public override void EnableContinuation(string activityName, string activityInstance, string continuationToken)
        {
            BizTalkOrchestrationEventStream.EnableContinuation(activityName, activityInstance, continuationToken);
        }

        /// <summary>
        /// Indicates that there are no more events expected for the given activity instance or continuation token.
        /// </summary>
        /// <param name="activityName">The activity name.</param>
        /// <param name="activityInstance">The activity instance ID or continuation token.</param>
        public override void EndActivity(string activityName, string activityInstance)
        {
            BizTalkOrchestrationEventStream.EndActivity(activityName, activityInstance);
        }

        /// <summary>
        /// Updates the activity record.
        /// </summary>
        /// <param name="activityName">The activity name.</param>
        /// <param name="activityInstance">The activity instance ID or continuation token.</param>
        /// <param name="data">All data items that must be updated as name-value pairs.</param>
        public override void UpdateActivity(string activityName, string activityInstance, params object[] data)
        {
            BizTalkOrchestrationEventStream.UpdateActivity(activityName, activityInstance, data);
        }

        /// <summary>
        /// Ignored in the contects of the BizTalk OES.  
        /// </summary>
        public override void Flush()
        {
            return;
        }

        /// <summary>
        /// Ignored in the contects of the BizTalk OES.  
        /// </summary>
        /// <param name="connection">A SQL connection.</param>
        public override void Flush(SqlConnection connection)
        {
            return;
        }

        /// <summary>
        /// Ignored in the contects of the BizTalk OES.  
        /// </summary>
        /// <param name="singleEvent">The event to be serialized.</param>
        public override void StoreCustomEvent(IPersistQueryable singleEvent)
        {
            return;
        }
    }
} 