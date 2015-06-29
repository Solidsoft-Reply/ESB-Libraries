// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StepTrackPointsByType.cs" company="Solidsoft Reply Ltd.">
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
    using System.Collections.Generic;
    using System.Linq;

    using ResolutionService;

    /// <summary>
    /// Represents a collection of track points of a specified type for a given BAM step
    /// </summary>
    public class StepTrackPointsByType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolidsoftReply.Esb.Libraries.Resolution.StepTrackPointsByType"/> class.
        /// </summary>
        /// <param name="type">The track point type.</param>
        /// <param name="bamActivityStep">The BAM activity step.</param>
        public StepTrackPointsByType(TrackPointType type, BamActivityStep bamActivityStep)
        {
            this.ActivityName = bamActivityStep.ActivityName;
            this.StepName = bamActivityStep.StepName;
            this.ExtendedStepName = bamActivityStep.ExtendedStepName;
            this.TrackPointType = type;
            this.TrackPoints = (from TrackPoint tp in bamActivityStep.TrackPoints
                                where tp.Type == type
                                select tp).ToList();
        }

        /// <summary>
        /// Gets the activity name.
        /// </summary>
        public string ActivityName { get; private set; }

        /// <summary>
        /// Gets the BAM step name.
        /// </summary>
        public string StepName { get; private set; }

        /// <summary>
        /// Gets the BAM extended step name.
        /// </summary>
        public string ExtendedStepName { get; private set; }

        /// <summary>
        /// Gets the type of the track points.
        /// </summary>
        public TrackPointType TrackPointType { get; private set; }

        /// <summary>
        /// Gets the list of track points of a specific type.
        /// </summary>
        public IEnumerable<TrackPoint> TrackPoints { get; private set; }
    }
}
