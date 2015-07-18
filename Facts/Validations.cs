// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Validations.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Facts
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a set of validations performed by a validation policy.
    /// </summary>
    [Serializable]
    public class Validations
    {
        /// <summary>
        /// Collection of error validation entries.
        /// </summary>
        private readonly List<ValidationEntry> errors = new List<ValidationEntry>();

        /// <summary>
        /// Collection of warning validation entries.
        /// </summary>
        private readonly List<ValidationEntry> warnings = new List<ValidationEntry>();

        /// <summary>
        /// Collection of information validation entries.
        /// </summary>
        private readonly List<ValidationEntry> information = new List<ValidationEntry>();

        /// <summary>
        /// Gets the count of logged errors.
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return this.errors.Count;
            }
        }

        /// <summary>
        /// Gets the count of logged warnings.
        /// </summary>
        public int WarningCount
        {
            get
            {
                return this.warnings.Count;
            }
        }

        /// <summary>
        /// Gets the count of logged informational entries.
        /// </summary>
        public int InformationCount
        {
            get
            {
                return this.information.Count;
            }
        }

        /// <summary>
        /// Gets the collection of error validation entries
        /// </summary>
        public IEnumerable<ValidationEntry> Errors
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Gets the collection of warning validation entries
        /// </summary>
        public IEnumerable<ValidationEntry> Warnings
        {
            get
            {
                return this.warnings;
            }
        }

        /// <summary>
        /// Gets the collection of information validation entries
        /// </summary>
        public IEnumerable<ValidationEntry> Information
        {
            get
            {
                return this.information;
            }
        }

        /// <summary>
        /// Logs an error entry.
        /// </summary>
        /// <param name="description">Description of the error.</param>
        public void LogError(string description)
        {
            this.LogEntry(ValidationLevel.Error, description);
        }

        /// <summary>
        /// Logs an error entry.
        /// </summary>
        /// <param name="description">Description of the error.</param>
        /// <param name="severity">The error severity.</param>
        public void LogError(string description, int severity)
        {
            this.LogEntry(ValidationLevel.Error, description);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="code">A code representing the entry.</param>
        public void LogError(string description, long code)
        {
            this.LogEntry(ValidationLevel.Error, description, null, code);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="code">A code representing the entry.</param>
        /// <param name="severity">The severity of the entry (error only).</param>
        public void LogError(string description, long code, int severity)
        {
            this.LogEntry(ValidationLevel.Error, description, null, code, severity);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        public void LogError(string description, string type)
        {
            this.LogEntry(ValidationLevel.Error, description, type);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        /// <param name="severity">The severity of the entry (error only).</param>
        public void LogError(string description, string type, int severity)
        {
            this.LogEntry(ValidationLevel.Error, description, type, null, severity);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        /// <param name="code">A code representing the entry.</param>
        public void LogError(string description, string type, long code)
        {
            this.LogEntry(ValidationLevel.Error, description, type, code);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        /// <param name="code">A code representing the entry.</param>
        /// <param name="severity">The severity of the entry (error only).</param>
        public void LogError(string description, string type, long code, int severity)
        {
            this.LogEntry(ValidationLevel.Error, description, type, code, severity);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        public void LogWarning(string description)
        {
            this.LogEntry(ValidationLevel.Warning, description);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="code">A code representing the entry.</param>
        public void LogWarning(string description, long code)
        {
            this.LogEntry(ValidationLevel.Warning, description, null, code);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        public void LogWarning(string description, string type)
        {
            this.LogEntry(ValidationLevel.Warning, description, type);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        /// <param name="code">A code representing the entry.</param>
        public void LogWarning(string description, string type, long code)
        {
            this.LogEntry(ValidationLevel.Warning, description, type, code);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        public void LogInformation(string description)
        {
            this.LogEntry(ValidationLevel.Information, description);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="code">A code representing the entry.</param>
        public void LogInformation(string description, long code)
        {
            this.LogEntry(ValidationLevel.Information, description, null, code);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        public void LogInformation(string description, string type)
        {
            this.LogEntry(ValidationLevel.Information, description, type);
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        /// <param name="code">A code representing the entry.</param>
        public void LogInformation(string description, string type, long code)
        {
            this.LogEntry(ValidationLevel.Information, description, type, code);
        }

        /// <summary>
        /// Returns a string representing all validation entries.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> representing all validation entries.
        /// </returns>
        public override string ToString()
        {
            return this.ToString(ValidationLevel.Error) + this.ToString(ValidationLevel.Warning)
                   + this.ToString(ValidationLevel.Information);
        }

        /// <summary>
        /// Returns a string representing all entries of a given level.
        /// </summary>
        /// <param name="level">
        /// The validation level.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representing all error entries.
        /// </returns>
        public string ToString(ValidationLevel level)
        {
            IEnumerable<ValidationEntry> list = this.errors;

            switch (level)
            {
                case ValidationLevel.Error:
                    if (this.ErrorCount <= 0)
                    {
                        return string.Empty;
                    }

                    break;
                case ValidationLevel.Warning:
                    if (this.WarningCount <= 0)
                    {
                        return string.Empty;
                    }

                    list = this.Warnings;
                    break;
                case ValidationLevel.Information:
                    if (this.InformationCount <= 0)
                    {
                        return string.Empty;
                    }

                    list = this.Information;
                    break;
            }

            var errorsStringBuilder = new StringBuilder();

            foreach (var validationEntry in list)
            {
                errorsStringBuilder.Append(validationEntry);
            }

            return errorsStringBuilder.ToString();
        }

        /// <summary>
        /// Logs an entry.
        /// </summary>
        /// <param name="level">The validation level (error, warning or information).</param>
        /// <param name="description">The entry description.</param>
        /// <param name="type">The type of entry.</param>
        /// <param name="code">A code representing the entry.</param>
        /// <param name="severity">The severity of the entry (error only).</param>
        private void LogEntry(ValidationLevel level, string description, string type = null, long? code = null, int? severity = null)
        {
            var validationEntry = new ValidationEntry(level, description, type, code, severity);

            switch (level)
            {
                case ValidationLevel.Error:
                    this.errors.Add(validationEntry);
                    break;
                case ValidationLevel.Warning:
                    this.warnings.Add(validationEntry);
                    break;
                case ValidationLevel.Information:
                    this.information.Add(validationEntry);
                    break;
            }
        }
    }
}
