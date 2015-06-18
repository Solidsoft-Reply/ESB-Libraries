// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLog.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.ResolutionService
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class EventLog : System.Diagnostics.EventLog
    {
        /// <summary>
        /// Writes an information type entry, with the given message text, to the event log.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        public new void WriteEntry(string message)
        {
            this.WriteEntry(message, EventLogEntryType.Information);
        }

        /// <summary>
        ///    Writes an error, warning, information, success audit, or failure audit entry
        ///    with the given message text to the event log.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        public new void WriteEntry(string message, EventLogEntryType type)
        {
            try
            {
                base.WriteEntry(message, type);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch 
            {
            }
        }

        /// <summary>
        ///    Writes an information type entry with the given message text to the event
        ///    log, using the specified registered event source.
        /// </summary>
        /// <param name="source">The source by which the application is registered on the specified computer.</param>
        /// <param name="message">The string to write to the event log.</param>
        public new static void WriteEntry(string source, string message)
        {
            WriteEntry(source, message, EventLogEntryType.Information);
        }

        /// <summary>
        ///    Writes an entry with the given message text and application-defined event
        ///    identifier to the event log.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        public new void WriteEntry(string message, EventLogEntryType type, int eventId)
        {
            try
            {
                base.WriteEntry(message, type, eventId);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an error, warning, information, success audit, or failure audit entry
        ///    with the given message text to the event log, using the specified registered
        ///    event source.
        /// </summary>
        /// <param name="source">The source by which the application is registered on the specified computer.</param>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        public new static void WriteEntry(string source, string message, EventLogEntryType type)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEntry(source, message, type);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an entry with the given message text, application-defined event identifier,
        ///    and application-defined category to the event log.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        /// <param name="category">The application-specific subcategory associated with the message.</param>
        public new void WriteEntry(string message, EventLogEntryType type, int eventId, short category)
        {
            try
            {
                base.WriteEntry(message, type, eventId, category);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an entry with the given message text and application-defined event
        ///    identifier to the event log, using the specified registered event source.
        /// </summary>
        /// <param name="source">The source by which the application is registered on the specified computer.</param>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        public new static void WriteEntry(string source, string message, EventLogEntryType type, int eventId)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEntry(source, message, type, eventId);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an entry with the given message text, application-defined event identifier,
        ///    and application-defined category to the event log, and appends binary data
        ///    to the message.
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        /// <param name="category">The application-specific subcategory associated with the message.</param>
        /// <param name="rawData">An array of bytes that holds the binary data associated with the entry.</param>
        public new void WriteEntry(string message, EventLogEntryType type, int eventId, short category, byte[] rawData)
        {
            try
            {
                base.WriteEntry(message, type, eventId, category, rawData);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an entry with the given message text, application-defined event identifier,
        ///    and application-defined category to the event log, using the specified registered
        ///    event source. The category can be used by the Event Viewer to filter events
        ///    in the log.
        /// </summary>
        /// <param name="source">The source by which the application is registered on the specified computer.</param>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        /// <param name="category">The application-specific subcategory associated with the message.</param>
        public new static void WriteEntry(
            string source,
            string message,
            EventLogEntryType type,
            int eventId,
            short category)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEntry(
                    source,
                    message,
                    type,
                    eventId,
                    category);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an entry with the given message text, application-defined event identifier,
        ///    and application-defined category to the event log (using the specified registered
        ///    event source) and appends binary data to the message.        /// </summary>
        /// <param name="source">The source by which the application is registered on the specified computer.</param>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the System.Diagnostics.EventLogEntryType values.</param>
        /// <param name="eventId">The application-specific identifier for the event.</param>
        /// <param name="category">The application-specific subcategory associated with the message.</param>
        /// <param name="rawData">An array of bytes that holds the binary data associated with the entry.</param>
        public new static void WriteEntry(
            string source,
            string message,
            EventLogEntryType type,
            int eventId,
            short category,
            byte[] rawData)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEntry(source, message, type, eventId, category, rawData);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes a localized entry to the event log.
        /// </summary>
        /// <param name="instance">
        ///    An System.Diagnostics.EventInstance instance that represents a localized
        ///    event log entry.
        /// </param>
        /// <param name="values">An array of strings to merge into the message text of the event log entry.</param>
        [ComVisible(false)]
        public new void WriteEvent(EventInstance instance, params object[] values)
        {
            try
            {
                base.WriteEvent(instance, values);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an event log entry with the given event data, message replacement
        ///    strings, and associated binary data.
        /// </summary>
        /// <param name="instance">
        ///    An System.Diagnostics.EventInstance instance that represents a localized
        ///    event log entry.
        /// </param>
        /// <param name="data">An array of bytes that holds the binary data associated with the entry.</param>
        /// <param name="values">An array of strings to merge into the message text of the event log entry.</param>
        [ComVisible(false)]
        public new void WriteEvent(EventInstance instance, byte[] data, params object[] values)
        {
            try
            {
                base.WriteEvent(instance, data, values);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an event log entry with the given event data and message replacement
        ///    strings, using the specified registered event source.
        /// </summary>
        /// <param name="source">
        ///    The name of the event source registered for the application on the specified
        ///    computer.
        /// </param>
        /// <param name="instance">
        ///    An System.Diagnostics.EventInstance instance that represents a localized
        ///    event log entry.
        /// </param>
        /// <param name="values">An array of strings to merge into the message text of the event log entry.</param>
        public new static void WriteEvent(string source, EventInstance instance, params object[] values)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEvent(source, instance, values);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }

        /// <summary>
        ///    Writes an event log entry with the given event data, message replacement
        ///    strings, and associated binary data, and using the specified registered event
        ///    source.
        /// </summary>
        /// <param name="source">
        ///    The name of the event source registered for the application on the specified
        ///    computer.
        /// </param>
        /// <param name="instance">
        ///    An System.Diagnostics.EventInstance instance that represents a localized
        ///    event log entry.
        /// </param>
        /// <param name="data">An array of bytes that holds the binary data associated with the entry.</param>
        /// <param name="values">An array of strings to merge into the message text of the event log entry.</param>
        public new static void WriteEvent(string source, EventInstance instance, byte[] data, params object[] values)
        {
            try
            {
                System.Diagnostics.EventLog.WriteEvent(source, instance, data, values);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }
        }
    }
}