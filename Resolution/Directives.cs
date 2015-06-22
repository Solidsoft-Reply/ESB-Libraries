// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Directives.cs" company="Solidsoft Reply Ltd.">
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
    using System.Linq;

    using SolidsoftReply.Esb.Libraries.Resolution.ResolutionService;

    /// <summary>
    /// Class containing the result of the resolution
    /// </summary>
    [Serializable]
    public class Directives : IEnumerable<Directive>
    {
        /// <summary>
        /// The resolver directive items.
        /// </summary>
        private readonly List<Directive> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="Directives"/> class. 
        /// </summary>
        public Directives()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Directives"/> class. 
        /// </summary>
        /// <param name="interchange">
        /// The resolution service interchange object.
        /// </param>
        public Directives(Interchange interchange)
        {
            if (interchange == null)
            {
                return;
            }

            this.items = new List<Directive>();

            if (interchange.Directives == null)
            {
                return;
            }

            foreach (var rd in interchange.Directives.Select(di => new Directive(di.Value.Directive)))
            {
                this.items.Add(rd);
            }
        }

        /// <summary>
        /// Gets a value indicating whether any of the directives define a service windows.
        /// Returns true if the current time is in any of those service windows.   Otherwise
        /// returns false.   If no service windows are defined, always returns true.
        /// </summary>
        public bool InAnyServiceWindow
        {
            get
            {
                // return 'true' by default for scenarios where no
                // service window is defined by any directive.
                var result = true;

                if (this.items == null)
                {
                    return true;
                }

                var anyInWindow = false;
                var anyOutsideWindow = false;

                foreach (var directive in this.items)
                {
                    if (!directive.ServiceWindowStartTimeSpecified || !directive.ServiceWindowStopTimeSpecified)
                    {
                        continue;
                    }

                    // Take a snapshot of the current time
                    var timeNow = DateTime.Now.TimeOfDay;

                    // This directive has a defined service window
                    if (directive.ServiceWindowStartTime.TimeOfDay > timeNow
                        && directive.ServiceWindowStopTime.TimeOfDay <= timeNow)
                    {
                        anyOutsideWindow = true;
                    }
                    else
                    {
                        anyInWindow = true;
                    }
                }

                if (anyOutsideWindow && !anyInWindow)
                {
                    result = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a TimeSpan for the time of day at which the next service window opens.
        /// If no service window later than now is defined, returns TimeSpan.MaxValue.
        /// </summary>
        public TimeSpan NextWindowOpen
        {
            get
            {
                // Take a snapshot of the current time
                var timeNow = DateTime.Now.TimeOfDay;

                TimeSpan[] earliestNext = { TimeSpan.MaxValue };

                if (this.items == null)
                {
                    return earliestNext[0];
                }

                foreach (var directive in this.items.Where(directive => directive.ServiceWindowStartTimeSpecified && directive.ServiceWindowStopTimeSpecified).Where(directive => directive.ServiceWindowStartTime.TimeOfDay > timeNow
                                                                                                                                                                                  && directive.ServiceWindowStartTime.TimeOfDay < earliestNext[0]))
                {
                    earliestNext[0] = directive.ServiceWindowStartTime.TimeOfDay;
                }

                return earliestNext[0];
            }
        }

        /// <summary>
        /// Gets a TimeSpan for the time of day at which the current service window closes.
        /// If no service window later than now is defined, returns TimeSpan.MaxValue.
        /// </summary>
        public TimeSpan CurrentWindowClose
        {
            get
            {
                // Take a snapshot of the current time
                var timeNow = DateTime.Now.TimeOfDay;

                TimeSpan[] earliestNext = { TimeSpan.MaxValue };

                if (this.items == null)
                {
                    return earliestNext[0];
                }

                foreach (var directive in this.items.Where(directive => directive.ServiceWindowStartTimeSpecified && directive.ServiceWindowStopTimeSpecified).Where(directive => directive.ServiceWindowStopTime.TimeOfDay > timeNow
                                                                                                                                                                                  && directive.ServiceWindowStopTime.TimeOfDay < earliestNext[0]))
                {
                    earliestNext[0] = directive.ServiceWindowStopTime.TimeOfDay;
                }

                return earliestNext[0];
            }
        }

        /// <summary>
        /// Gets the number of directives.
        /// </summary>
        public int Count
        {
            get
            {
                //////if (this.items == null)
                //////    throw new EsbResolutionException("The object is not constructed properly.");

                return this.items.Count;
            }
        }

        /// <summary>
        /// Gets a directive by index.
        /// </summary>
        /// <param name="index">The directive index.</param>
        /// <returns>The indexed resolver directive.</returns>
        public Directive this[int index]
        {
            get
            {
                return this.GetDirective(index);
            }
        }

        /// <summary>
        /// Gets a directive by name.
        /// </summary>
        /// <param name="name">The directive name.</param>
        /// <returns>The named resolver directive.</returns>
        public Directive this[string name]
        {
            get
            {
                return this.GetDirective(name);
            }
        }

        /// <summary>
        /// Returns the indexed directive.
        /// </summary>
        /// <param name="index">The directive index.</param>
        /// <returns>A named resolver directive.</returns>
        public Directive GetDirective(int index)
        {
            if (this.items == null)
            {
                throw new EsbResolutionException(Properties.Resources.ExceptionObjectMisconstructed);
            }

            if (index > this.items.Count)
            {
                throw new EsbResolutionException(Properties.Resources.ExceptionElementNotOnList);
            }

            return this.items[index];
        }

        /// <summary>
        /// Returns the named directive.
        /// </summary>
        /// <param name="name">The directive name.</param>
        /// <returns>A named resolver directive.</returns>
        public Directive GetDirective(string name)
        {
            if (this.items == null)
            {
                throw new EsbResolutionException(Properties.Resources.ExceptionObjectMisconstructed);
            }

            return this.items.FirstOrDefault(directive => directive.Name == name) ?? new Directive();
        }

        /// <summary>
        /// Returns the first directive.  If no directive exists, returns null.
        /// </summary>
        /// <returns>The first directive, or null.</returns>
        public Directive FirstOrDefault()
        {
            if (this.items == null || this.items.Count == 0)
            {
                return default(Directive);
            }

            return this.items[0];
        }

        /// <summary>
        /// Retrieves data for a step of a BAM activity for the first directive found that defines a
        /// step. Call this method on every step in which some data may be needed for BAM 
        /// - e.g., at the point a service is called, or at the point of resolution.
        /// </summary>
        /// <param name="data">The BAM step data.</param>
        public void OnFirstStep(BamStepData data)
        {
            var firstBamDirective = this.items.FirstOrDefault(item => string.IsNullOrWhiteSpace(item.BamStepName))
                                    ?? this.items.FirstOrDefault(item => string.IsNullOrWhiteSpace(item.BamAfterMapStepName));

            if (firstBamDirective != null)
            {
                firstBamDirective.OnStep(data);
            }
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity for the first directive found that 
        /// defines a step.    Call this method on every step in which some data may be needed for BAM 
        /// - e.g., at the point a service is called, or at the point of resolution.
        /// </summary>
        /// <param name="data">The BAM step data.</param>
        /// <param name="afterMap">Indicates if the step is after the application of a map.</param>
        public void OnFirstStep(BamStepData data, bool afterMap)
        {
            var bamDirective = this.items.FirstOrDefault(item => string.IsNullOrWhiteSpace(item.BamStepName))
                                    ?? this.items.FirstOrDefault(item => string.IsNullOrWhiteSpace(item.BamAfterMapStepName));

            if (bamDirective != null)
            {
                bamDirective.OnStep(data, afterMap);
            }
        }

        /// <summary>
        /// Retrieves data for a step of a BAM activity for a named directive. Call this method on a 
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="directiveName">The name of the directive that defines the BAM step.</param>
        /// <param name="data">The BAM step data.</param>
        public void OnStep(string directiveName, BamStepData data)
        {
            var bamDirective = this.items.FirstOrDefault(directive => directive.Name == directiveName);

            if (bamDirective != null)
            {
                bamDirective.OnStep(data);
            }
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity for a named directive. Call this method on a 
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="directiveName">The name of the directive that defines the BAM step.</param>
        /// <param name="data">The BAM step data.</param>
        /// <param name="afterMap">Indicates if the step is after the application of a map.</param>
        public void OnStep(string directiveName, BamStepData data, bool afterMap)
        {
            var firstBamDirective = this.items.FirstOrDefault(directive => directive.Name == directiveName);

            if (firstBamDirective != null)
            {
                firstBamDirective.OnStep(data, afterMap);
            }
        }

        /// <summary>
        /// Return the item enumerator
        /// </summary>
        /// <returns>An IEnumerator interface</returns>
        IEnumerator<Directive> IEnumerable<Directive>.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Return the item enumerator
        /// </summary>
        /// <returns>An IEnumerator interface</returns>
        public IEnumerator GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    }
}
