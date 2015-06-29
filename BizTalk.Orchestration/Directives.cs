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

namespace SolidsoftReply.Esb.Libraries.BizTalk.Orchestration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Class containing the result of the resolution
    /// </summary>
    [Serializable]
    public class Directives : IEnumerable<Directive>
    {
        /// <summary>
        /// Directives enumerator.
        /// </summary>
        private readonly IEnumerator<Directive> enumerator;

        /// <summary>
        /// The resolver directive items.
        /// </summary>
        private readonly Resolution.Directives directives;

        /// <summary>
        /// Initializes a new instance of the <see cref="Directives"/> class. 
        /// </summary>
        /// <param name="directives">
        /// The original set of resolution directives.
        /// </param>
        public Directives(Resolution.Directives directives)
        {
            if (directives == null)
            {
                return;
            }

            this.directives = directives;
            this.enumerator = new DirectivesEnumerator(directives);
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
                return this.directives.InAnyServiceWindow;
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
                return this.directives.NextWindowOpen;
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
                return this.directives.CurrentWindowClose;
            }
        }

        /// <summary>
        /// Gets the number of directives.
        /// </summary>
        public int Count
        {
            get
            {
                return this.directives.Count;
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
            return new Directive(this.directives.GetDirective(index));
        }

        /// <summary>
        /// Returns the named directive.
        /// </summary>
        /// <param name="name">The directive name.</param>
        /// <returns>A named resolver directive.</returns>
        public Directive GetDirective(string name)
        {
            return new Directive(this.directives.GetDirective(name));
        }

        /// <summary>
        /// Returns the first directive.  If no directive exists, returns null.
        /// </summary>
        /// <returns>The first directive, or null.</returns>
        public Directive FirstOrDefault()
        {
            return new Directive(this.directives.FirstOrDefault());
        }

        /// <summary>
        /// Performs all BAM actions for configured BAM steps in all the directives.
        /// </summary>
        /// <param name="data">The BAM step data.</param>
        public void OnStep(BamStepData data)
        {
             this.directives.OnStep(data, Resolution.MultiStepControl.AllSteps, false);
        }

        /// <summary>
        /// Performs all BAM actions for configured BAM steps in all the directives.  This method 
        /// can optionally handle step processing after application of a map.
        /// </summary>
        /// <param name="data">The BAM step data.</param>
        /// <param name="afterMap">Indicates if the step is after the application of a map.</param>
        public void OnStep(BamStepData data, bool afterMap)
        {
            this.directives.OnStep(data, Resolution.MultiStepControl.AllSteps, afterMap);
        }

        /// <summary>
        /// Performs all BAM actions for configured BAM steps in either the first or 
        /// all the directives.  Optionally perform BAM actions for all step extensions.
        /// </summary>
        /// <param name="data">The BAM step data.</param>
        /// <param name="depth">
        /// Specify the depth of BAM processing; first or all steps and, optionally, 
        /// each step extension.
        /// </param>
        public void OnStep(BamStepData data, Resolution.MultiStepControl depth)
        {
            this.directives.OnStep(data, depth, false);
        }

        /// <summary>
        /// Performs all BAM actions for configured BAM steps in either the first or all 
        /// the directives.  Optionally perform BAM actions for all step extensions.  
        /// This method can optionally handle step processing after application of a map.
        /// </summary>
        /// <param name="data">The BAM step data.</param>
        /// <param name="depth">
        /// Specify the depth of BAM processing; first or all steps and, optionally, 
        /// each step extension.
        /// </param>
        /// <param name="afterMap">Indicates if the step is after the application of a map.</param>
        public void OnStep(BamStepData data, Resolution.MultiStepControl depth, bool afterMap)
        {
            this.directives.OnStep(data, depth, afterMap);
        }

        /// <summary>
        /// Performs all BAM actions for a BAM steps in a specified directive.  
        /// </summary>
        /// <param name="directiveName">The name of the directive that defines the BAM step.</param>
        /// <param name="data">The BAM step data.</param>
        public void OnStep(string directiveName, BamStepData data)
        {
            this.directives.OnStep(directiveName, data);
        }

        /// <summary>
        /// Performs all BAM actions for a BAM steps in a specified directive.  This method can 
        /// optionally handle step processing after application of a map.
        /// </summary>
        /// <param name="directiveName">The name of the directive that defines the BAM step.</param>
        /// <param name="data">The BAM step data.</param>
        /// <param name="afterMap">Indicates if the step is after the application of a map.</param>
        public void OnStep(string directiveName, BamStepData data, bool afterMap)
        {
            this.directives.OnStep(directiveName, data, afterMap);
        }

        /// <summary>
        /// Return the item enumerator
        /// </summary>
        /// <returns>An IEnumerator interface</returns>
        IEnumerator<Directive> IEnumerable<Directive>.GetEnumerator()
        {
            return this.enumerator;
        }

        /// <summary>
        /// Return the item enumerator
        /// </summary>
        /// <returns>An IEnumerator interface</returns>
        public IEnumerator GetEnumerator()
        {
            return this.enumerator;
        }

        /// <summary>
        /// An enumerator for resolution directives.
        /// </summary>
        [Serializable]
        public class DirectivesEnumerator : IEnumerator<Directive>
        {
            /// <summary>
            /// The collection of resolution directives.
            /// </summary>
            private readonly Resolution.Directives directives;

            /// <summary>
            /// The current index.
            /// </summary>
            private int currentIndex;

            /// <summary>
            /// The current resolution directive.
            /// </summary>
            private Resolution.Directive currentDirective;

            /// <summary>
            /// Initializes a new instance of the <see cref="DirectivesEnumerator"/> class. 
            /// </summary>
            /// <param name="directives">
            /// The resolution directives to be enumerated.
            /// </param>
            public DirectivesEnumerator(Resolution.Directives directives)
            {
                this.directives = directives;
                this.currentIndex = -1;
                this.currentDirective = default(Resolution.Directive);
            }

            /// <summary>
            /// Gets the current directive.
            /// </summary>
            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Gets the current directive.
            /// </summary>
            public Directive Current
            {
                get
                {
                    return new Directive(this.currentDirective);
                }
            }

            /// <summary>
            /// Move to the next directive in the collection.
            /// </summary>
            /// <returns>
            /// True if the next directive exists; otherwise false.
            /// </returns>
            public bool MoveNext()
            {
                // Avoids going beyond the end of the collection. 
                if (++this.currentIndex >= this.directives.Count)
                {
                    return false;
                }

                // Set current box to next item in collection.
                this.currentDirective = this.directives[this.currentIndex];

                return true;
            }

            /// <summary>
            /// Reset the enumerator.
            /// </summary>
            public void Reset()
            {
                this.currentIndex = -1;
            }

            /// <summary>
            /// Dispose the current enumerator.
            /// </summary>
            void IDisposable.Dispose()
            {
            }
        }
    }
}
