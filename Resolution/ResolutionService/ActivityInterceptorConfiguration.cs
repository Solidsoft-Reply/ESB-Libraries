// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityInterceptorConfiguration.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Resolution.ResolutionService
{
    using System;
    using System.Collections;
    using System.Linq;

    /// <summary>
    /// Represents BAM activity interceptor configuration for track points.  Provides a convenient
    /// array list of track points.
    /// </summary>
    public partial class ActivityInterceptorConfiguration
    {
        /// <summary>
        /// Gets or sets an array list of BAM track points.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public ArrayList TrackPoints
        {
            get
            {
                Func<ArrayList> getTrackPoints = () =>
                    {
                        var outArrayList = new ArrayList();

                        foreach (var trackPoint in this.trackPointField.Select(tp => tp))
                        {
                            outArrayList.Add(trackPoint);
                        }

                        return outArrayList;
                    };

                return this.trackPointField == null ? null : getTrackPoints();
            }

            set
            {
                this.trackPointField = value == null ? null : value.Count == 0 ? new TrackPoint[0] : (TrackPoint[])value.ToArray();
            }
        }
    }
}