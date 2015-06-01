// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BamActivity.cs" company="Solidsoft Reply Ltd.">
//   (c) 2013 Solidsoft Reply Ltd.
// </copyright>
// <summary>
//   Represents an activity as a fact.  The policy for the named activity
//   will register BAM tracking points to pass back to the resolver.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Solidsoft.Esb.Facts
{
    using System;
    using System.Collections;
    using System.Xml.Serialization;
    using Microsoft.BizTalk.Bam.EventObservation; 
    
    /// <summary>
    /// Represents an activity as a fact.  The policy for the named activity
    /// will register BAM tracking points to pass back to the resolver.
    /// </summary>
    /// <remarks>
    /// This class subclasses the ActivityInterceptorConfiguration and surfaces
    /// the activity name in order to support simple rules that test for the activity name.
    /// </remarks>
    [Serializable]
    [XmlInclude(typeof(TrackPoint))]
    public class BamActivity : ActivityInterceptorConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BamActivity"/> class. 
        /// </summary>
        public BamActivity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BamActivity"/> class. 
        /// Constructor.
        /// </summary>
        /// <param name="activityName">
        /// Name of activity.
        /// </param>
        public BamActivity(string activityName)
            : base(activityName)
        {
            this.ActivityName = activityName;
        }

        /// <summary>
        /// Gets or sets the activity name.
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// Gets the TrackPoints.
        /// </summary>
        [XmlElement("TrackPoint", typeof(TrackPoint))]
        public new ArrayList TrackPoints
        {
            get
            {
                return base.TrackPoints;
            }
        }
    }
}
