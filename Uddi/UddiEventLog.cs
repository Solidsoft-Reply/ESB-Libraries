// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UddiEventLog.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Uddi
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.Uddi3;
    using Microsoft.Uddi3.Extensions;

    using MessageStrings = Properties.Resources;

    /// <summary>
    /// An event log for the UDDI library.  This code uses the Windows event log rather than ETW
    /// to better align with the BizTalk Server world.
    /// </summary>
    internal class UddiEventLog : EventLog
    {
        /// <summary>
        /// Initializes static members of the <see cref="UddiEventLog"/> class.
        /// </summary>
        static UddiEventLog()
        {
            DefaultLog = new UddiEventLog();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UddiEventLog"/> class. 
        /// </summary>
        public UddiEventLog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the default event log.
        /// </summary>
        public static UddiEventLog DefaultLog { get; set; }

        /// <summary>
        /// Gets an environment information record for logging.
        /// </summary>
        /// <returns>A formatted string containing an environment data record.</returns>
        public static string EnvironmentRecord
        {
            get
            {
                // The current process.
                var currentProcess = Process.GetCurrentProcess();

                // The total time the current process has been running. 
                var totalProcessorTime = currentProcess.TotalProcessorTime;

                // The total time the current process has been running, expressed as a string
                var totalProcessorTimeString = string.Format(
                    "{0} days, {1} hours, {2} minutes and {3}.{4} seconds.",
                    totalProcessorTime.Days,
                    totalProcessorTime.Hours,
                    totalProcessorTime.Minutes,
                    totalProcessorTime.Seconds,
                    totalProcessorTime.Milliseconds);

                // Return the environment information record.
                return string.Format(
                    MessageStrings.EnvironmentInfoTemplate,
                    Environment.MachineName,
                    Environment.ProcessorCount,
                    Environment.UserDomainName,
                    Environment.UserName,
                    Environment.OSVersion,
                    Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit",
                    currentProcess.ProcessName,
                    Environment.Is64BitProcess ? "64 bit" : "32 bit",
                    currentProcess.StartTime,
                    totalProcessorTimeString,
                    currentProcess.Id,
                    Environment.CurrentManagedThreadId,
                    currentProcess.SessionId,
                    Environment.Version,
                    currentProcess.VirtualMemorySize64,
                    currentProcess.WorkingSet64,
                    Environment.StackTrace);
            }
        }
        
        /// <summary>
        /// Log inquiry service startup.
        /// </summary>
        public void WriteInquiryStartupInfo()
        {
            this.LogInformationMessage(MessageStrings.InquiryStartup);
        }

        /// <summary>
        /// Log inquiry service initialization.
        /// </summary>
        /// <param name="inquiryUrl">The UDDI inquiry URL.</param>
        public void WriteInquiryInitialisedInfo(string inquiryUrl)
        {
            this.LogInformationMessage(string.Format(MessageStrings.InquiryInitialised, inquiryUrl));
        }

        /// <summary>
        /// Log the failure to read an app setting from the configuration file.
        /// </summary>
        /// <param name="keyName">The key name of the app setting.</param>
        /// <param name="configEx">The exception for this failure.</param>
        public void WriteAppSettingReadFailedError(string keyName, ConfigurationErrorsException configEx)
        {
            this.LogErrorMessage(string.Format(MessageStrings.AppSettingReadFailed, keyName, configEx.WithInnerExceptions()));
        }

        /// <summary>
        /// Log a warning that a URI is invalid.
        /// </summary>
        /// <param name="invalidUri">The invalid URI</param>
        public void WriteInvalidUriWarning(string invalidUri)
        {
            this.LogWarningMessage(string.Format(MessageStrings.InvalidUri, invalidUri));
        }

        /// <summary>
        /// Log a warning that an invalid URI will be automatically replaced by a valid URI.
        /// </summary>
        /// <param name="invalidUri">The invalid URI</param>
        /// <param name="returnUri">The URI to be returned.</param>
        public void WriteInvalidUriWillBeReplacedWarning(string invalidUri, string returnUri)
        {
            this.LogWarningMessage(string.Format(MessageStrings.InvalidUriWillBeReplaced, invalidUri, returnUri));
        }
        
        /// <summary>
        /// Log a warning that a number of invalid UDDI directory inquiry sites were returned from Active Directory.
        /// </summary>
        /// <param name="invalidSiteList">The list of invalid inquiry URIs.</param>
        public void WriteInvalidUddiSitesFoundInAdWarning(string invalidSiteList)
        {
            this.LogWarningMessage(string.Format(MessageStrings.InvalidUddiSitesFoundInAd, invalidSiteList));
        }

        /// <summary>
        /// Log a warning that a UDDI site directory cache entry has been unexpectedly removed.
        /// </summary>
        /// <param name="reason">The given reason.</param>
        public void WriteUddiSiteUnexpectedlyRemovedFromCacheWarning(string reason)
        {
            this.LogWarningMessage(string.Format(MessageStrings.UddiSiteUnexpectedlyRemovedFromCache, reason));
        }

        /// <summary>
        /// Log a warning that conversion of a value to a decimal failed and that a default value will be returned.
        /// </summary>
        /// <param name="value">The value that could not be converted.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="frmEx">The format exception.</param>
        public void WriteConversionToDecimalFailedWarning(string value, decimal defaultValue, FormatException frmEx)
        {
            this.LogWarningMessage(string.Format(MessageStrings.ConversionToDecimalFailedInvalidFormat, value, defaultValue, frmEx.WithInnerExceptions()));
        }

        /// <summary>
        /// Log a warning that conversion of a value to a decimal failed due to an overflow error and that a 
        /// default value will be returned.
        /// </summary>
        /// <param name="value">The value that could not be converted.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="ovrEx">The overflow exception.</param>
        public void WriteConversionToDecimalFailedWarning(string value, decimal defaultValue, OverflowException ovrEx)
        {
            this.LogWarningMessage(string.Format(MessageStrings.ConversionToDecimalFailedOverflow, value, defaultValue, ovrEx.WithInnerExceptions()));
        }

        /// <summary>
        /// Log an error for a null UDDI connection passed when trying to invoke a UDDI service.
        /// </summary>
        /// <param name="argEx">
        /// The null argument exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        public void WriteUddiConnectionIsNullError(ArgumentNullException argEx, string requestType, UddiSiteLocation siteLocation, params string[] loggingData)
        {
            this.WriteUddiError(MessageStrings.UddiConnectionIsNull, argEx, requestType, siteLocation, loggingData);
        }

        /// <summary>
        /// Log an error for an unknown message type passed when trying to invoke a UDDI service.
        /// </summary>
        /// <param name="argEx">
        /// The argument exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        public void WriteUddiUnknownMessageTypeError(ArgumentException argEx, string requestType, UddiSiteLocation siteLocation, params string[] loggingData)
        {
            this.WriteUddiError(MessageStrings.UddiUnknownMessageType, argEx, requestType, siteLocation, loggingData);
        }

        /// <summary>
        /// Log an error for a null site location or Inquiry URL passed when trying to invoke a UDDI service.
        /// </summary>
        /// <param name="operationEx">
        /// The invalid operation exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        public void WriteUddiInvalidSiteError(InvalidOperationException operationEx, string requestType, UddiSiteLocation siteLocation, params string[] loggingData)
        {
            this.WriteUddiError(MessageStrings.UddiInvalidSiteError, operationEx, requestType, siteLocation, loggingData);
        }

        /// <summary>
        /// Log an error for an invalid key value passed when trying to invoke a UDDI service.
        /// </summary>
        /// <param name="keyEx">
        /// The invalid key exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        public void WriteUddiInvalidKeyError(InvalidKeyPassedException keyEx, string requestType, UddiSiteLocation siteLocation, params string[] loggingData)
        {
            this.WriteUddiError(MessageStrings.UddiInvalid, keyEx, requestType, siteLocation, loggingData);
        }

        /// <summary>
        /// Log an unexpected error when invoking a UDDI service.
        /// </summary>
        /// <param name="unkEx">
        /// The unknown error exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        public void WriteUddiUnknownError(UnknownErrorException unkEx, string requestType, UddiSiteLocation siteLocation, params string[] loggingData)
        {
            this.WriteUddiError(MessageStrings.UddiUnknown, unkEx, requestType, siteLocation, loggingData);
        }

        /// <summary>
        /// Log an error for a SOAP fault when invoking a UDDI service.
        /// </summary>
        /// <param name="uddiEx">
        /// The UDDI exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        public void WriteUddiError(UddiException uddiEx, string requestType, UddiSiteLocation siteLocation, string[] loggingData)
        {
            this.WriteUddiError(MessageStrings.Uddi, uddiEx, requestType, siteLocation, loggingData);
        }

        /// <summary>
        /// Log an unexpected error when invoking a UDDI service.
        /// </summary>
        /// <param name="ex">
        /// The exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        public void WriteUddiUnexpectedError(Exception ex, string requestType, UddiSiteLocation siteLocation, params string[] loggingData)
        {
            this.WriteUddiError(MessageStrings.UddiUnexpected, ex, requestType, siteLocation, loggingData);
        }

        /// <summary>
        /// Initialize the component.
        /// </summary>
        private void InitializeComponent()
        {
            this.BeginInit();

            this.Log = "Application";
            this.Source = Properties.Resources.SolidsoftEsbUddiSource;

            this.EndInit();
        }

        /// <summary>
        /// Log an error for a UDDI service.
        /// </summary>
        /// <param name="messageString">
        /// The error message string
        /// </param>
        /// <param name="ex">
        /// The exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "LINQ expression is complex enough to brak across multiple lines.")]
        private void WriteUddiError(string messageString, Exception ex, string requestType, UddiSiteLocation siteLocation, string[] loggingData)
        {
            Func<string> flattenLoggingData =
                () =>
                string.Format(
                    "{0}{1}",
                    string.IsNullOrWhiteSpace(loggingData.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)))
                        ? string.Empty
                        : "\r\n",
                    loggingData.Length > 0
                        ? loggingData.Aggregate(
                            (stringOut, stringItem) =>
                            string.IsNullOrWhiteSpace(stringItem)
                                ? stringOut
                                : string.Format("{0}\r\n{1}", stringOut, stringItem))
                        : string.Empty);

            this.LogErrorMessage(
                string.Format(
                    MessageStrings.UddiErrorTemplate,
                        ex == null ? (object)"<unknown exception>" : ex.WithInnerExceptions(),
                        string.IsNullOrWhiteSpace(messageString) ? (object)"<unknown message>" : messageString,
                        flattenLoggingData(),
                        string.IsNullOrWhiteSpace(requestType) ? (object)"<unknown>" : requestType,
                        siteLocation != null ? siteLocation.AuthenticationMode : (object)"<unknown>",
                        siteLocation != null && !string.IsNullOrWhiteSpace(siteLocation.Description) ? siteLocation.Description : (object)"<unknown>",
                        siteLocation != null && !string.IsNullOrWhiteSpace(siteLocation.InquireUrl) ? siteLocation.InquireUrl : (object)"<unknown>",
                        siteLocation != null && !string.IsNullOrWhiteSpace(siteLocation.PublishUrl) ? siteLocation.PublishUrl : (object)"<unknown>",
                        siteLocation != null && !string.IsNullOrWhiteSpace(siteLocation.ExtensionsUrl) ? siteLocation.ExtensionsUrl : (object)"<unknown>"));
        }

        /// <summary>
        /// Log the message as information.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        private void LogInformationMessage(string message)
        {
            this.LogMessage(message, EventLogEntryType.Information);
        }

        /// <summary>
        /// Log the message as a warning.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        private void LogWarningMessage(string message)
        {
            this.LogMessage(message, EventLogEntryType.Warning);
        }

        /// <summary>
        /// Log the message as an error.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        private void LogErrorMessage(string message)
        {
            this.LogMessage(message, EventLogEntryType.Error);
        }

        /// <summary>
        /// Log the message.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="entryType">The event log entry type.</param>
        private void LogMessage(string message, EventLogEntryType entryType)
        {
            try
            {
                this.WriteEntry(message.RemoveEscapedCharacters(), entryType);
            }
            catch (ArgumentException argEx)
            {
                // The most likely reason for this error is that the source is not registered and cannot be
                // registered.  However, it could be that the message string or registry key is too long. A
                // simple way to check is to try to log the exception.
                try
                {
                    this.WriteEntry(argEx.Message, EventLogEntryType.Error);
                }
                catch (ArgumentException argEx2)
                {
                    try
                    {
                        // If this exception occurs then we can be fairly certain that this is a problem 
                        // with the source, probably due to lack of privileges.  We will attempt to
                        // log the message to the 'Application' source which will generally pre-exist.
                        this.Source = "Application";

                        this.WriteEntry(message, entryType);
                        this.WriteEntry(
                            string.Format(
                                MessageStrings.ArgumentExceptionLoggedToApplication,
                                Properties.Resources.SolidsoftEsbUddiSource,
                                argEx2),
                            EventLogEntryType.Error);
                    }
                    catch (ArgumentException argEx3)
                    {
                        // If this exception occurs, it could be that we have a combination of
                        // issue that make it impossible to log the warning.  We will attempt
                        // to log the current exception in the Application log.
                        try
                        {
                            this.WriteEntry(argEx3.Message, entryType);
                        }
                        // ReSharper disable once EmptyGeneralCatchClause
                        catch
                        {
                            // An error thrown here will be ignored.  We don't want an error 
                            // concerning event logging to propogate back to the site where we are
                            // logging.  If we allowed this, we would run the risk of masking the errors
                            // we are attempting to log.  Simialrly, for warnings and information, we don't 
                            // want a problem with error logging to affect execution.
                        }
                    }
                    finally
                    {
                        this.Source = Properties.Resources.SolidsoftEsbUddiSource;
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // An error thrown by LogMessage will ultimately be ignored.  We don't want an
                // error concerning event logging to propogate back to the site where we are
                // logging.  If we allowed this, we would run the risk of masking the errors
                // we are attempting to log.  Simialrly, for warnings and information, we don't 
                // want a problem with error logging to affect execution.
            }
        }
    }
}
