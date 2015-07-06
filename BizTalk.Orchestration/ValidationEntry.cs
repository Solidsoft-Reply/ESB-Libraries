// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationEntry.cs" company="Solidsoft Reply Ltd.">
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

    /// <summary>
    /// Validation entry.
    /// </summary>
    [Serializable]
    public struct ValidationEntry
    {
        /// <summary>
        /// The validation level (error, warning or information).
        /// </summary>
        private readonly ValidationLevel level;

        /// <summary>
        /// The entry description.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// The type of entry.
        /// </summary>
        private readonly string type;

        /// <summary>
        /// A code representing the entry.
        /// </summary>
        private readonly long? code;

        /// <summary>
        /// The severity of the entry (error only).
        /// </summary>
        private readonly int? severity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEntry"/> struct.
        /// </summary>
        /// <param name="level">The validation level (error, warning or information).</param>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        /// <param name="code">A code representing the entry.</param>
        /// <param name="severity">The severity of the entry (error only).</param>
        public ValidationEntry(ValidationLevel level, string description, string type = null, long? code = null, int? severity = null)
        {
            this.level = level;
            this.description = description;
            this.type = type;
            this.code = code;
            this.severity = severity;
        }

        /// <summary>
        /// Gets the validation level (error, warning or information).
        /// </summary>
        public ValidationLevel Level
        {
            get
            {
                return this.level;
            }
        }
            
        /// <summary>
        /// Gets the entry description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }
            
        /// <summary>
        /// Gets the type of entry.
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
        }
            
        /// <summary>
        /// Gets a code representing the entry.
        /// </summary>
        public long? Code
        {
            get
            {
                return this.code;
            }
        }
            
        /// <summary>
        /// Gets the severity of the entry (error only).
        /// </summary>
        public int? Severity
        {
            get
            {
                return this.severity;
            }
        }

        /// <summary>
        /// Returns a string representation of the validation entry.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> representing the validation entry.
        /// </returns>
        public override string ToString()
        {
            var entryString = string.Empty;

            switch (this.Level)
            {
                case ValidationLevel.Error:
                    entryString = this.Severity == null
                        ? "[ERROR] "
                        : string.Format("[ERROR severity={0}] ", this.Severity);
                    break;
                case ValidationLevel.Warning:
                    entryString = "[WARNING] ";
                    break;
                case ValidationLevel.Information:
                    entryString = "[INFORMATION] ";
                    break;
            }

            var codeText = Convert.ToString(this.code);
            entryString += codeText;

            if (!string.IsNullOrWhiteSpace(codeText)
                && (!string.IsNullOrWhiteSpace(this.type) || !string.IsNullOrWhiteSpace(this.description)))
            {
                entryString += ", ";
            }

            entryString += string.IsNullOrWhiteSpace(this.type) ? string.Empty : "<" + this.type;

            if (!string.IsNullOrWhiteSpace(this.type))
            {
                entryString += ">";
            }

            if (!string.IsNullOrWhiteSpace(this.description))
            {
                entryString += " ";
            }

            entryString += this.description + "\r\n";

            return entryString;
        }
    }
}
