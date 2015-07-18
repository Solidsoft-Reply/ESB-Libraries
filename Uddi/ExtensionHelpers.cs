// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionHelpers.cs" company="Solidsoft Reply Ltd.">
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
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Text;

    using Microsoft.Uddi3;
    using Microsoft.Uddi3.Businesses;
    using Microsoft.Uddi3.Extensions;
    using Microsoft.Uddi3.Services;

    using SolidsoftReply.Esb.Libraries.Uddi.Properties;

    /// <summary>
    /// A library of helper extension methods.
    /// </summary>
    internal static class ExtensionHelpers
    {
        /// <summary>
        /// Remove leading path delimiters from a relative URI string.
        /// </summary>
        /// <param name="uriText">The relative URI string.</param>
        /// <returns>The URI string without leading path delimiters.</returns>
        public static string RemoveLeadingPathDelimiters(this string uriText)
        {
            while (!string.IsNullOrWhiteSpace(uriText) && (uriText.StartsWith("/") || uriText.StartsWith(@"\")))
            {
                uriText = uriText.Substring(1);
            }

            return uriText;
        }

        /// <summary>
        /// Get a URI app setting from the configuration file.
        /// </summary>
        /// <param name="keyName">The App Setting key.</param>
        /// <returns>The URI value of an app setting in the configuration file.</returns>
        /// <exception cref="ConfigurationErrorsException">The configuration file contains an incorrect entry.</exception>
        public static string GetUriAppSetting(this string keyName)
        {
            // Get the app setting for the given key.
            var uriText = GetAppSetting(keyName);

            if (uriText != null && !Uri.IsWellFormedUriString(uriText, UriKind.RelativeOrAbsolute))
            {
                // Throw the configuration error
                throw new ConfigurationErrorsException(
                    string.Format(
                        "The {0} appSetting in the configuration file contains an invalid URI: {1}",
                        keyName,
                        uriText));
            }

            return uriText;
        }

        /// <summary>
        /// Get an app setting from the configuration file.
        /// </summary>
        /// <param name="keyName">The App Setting key.</param>
        /// <returns>The URI value of an app setting in the configuration file.</returns>
        /// <exception cref="ConfigurationErrorsException">The configuration file contains an incorrect entry.</exception>
        public static string GetAppSetting(this string keyName)
        {
            try
            {
                // Attempt to get the inquiry service address from a config file
                return ConfigurationManager.AppSettings[keyName];
            }
            catch (ConfigurationErrorsException configEx)
            {
                // Trace and re-throw the configuration warning
                UddiEventLog.DefaultLog.WriteAppSettingReadFailedError(keyName, configEx);
                throw;
            }
        }

        /// <summary>
        /// Log warning for an invalid URI string.
        /// </summary>
        /// <param name="invalidUri">The invalid URI.</param>
        /// <param name="returnUri">Optional return URI string.  Empty string by default.  
        /// Otherwise, must be a valid URI string.</param>
        /// <returns>An empty string or a given URI return string.</returns>
        public static string LogWarningForInvalidUri(this string invalidUri, string returnUri = "")
        {
            // Precondition
            if (UddiEventLog.DefaultLog == null)
            {
                throw new Exception(Resources.ExceptionUDDIDefaultEventLogNotInitialised_);
            }

            ////// Define pre-condition.
            ////Contract.Assume(UddiEventLog.DefaultLog != null);

            // Write warning to event log.
            UddiEventLog.DefaultLog.WriteInvalidUriWarning(invalidUri);

            if (string.IsNullOrWhiteSpace(returnUri))
            {
                return string.Empty;
            }
            
            // Write warning to event log.
            UddiEventLog.DefaultLog.WriteInvalidUriWillBeReplacedWarning(invalidUri, returnUri);
            return returnUri;
        }

        /// <summary>
        /// Log warnings for a collection of invalid UDDI site locations.
        /// </summary>
        /// <param name="invalidSites">The collection of invalid sites.</param>
        public static void LogWarningsForInvalidUddiSites(this IEnumerable<UddiSiteLocation> invalidSites)
        {
            // Precondition
            if (invalidSites == null)
            {
                throw new ArgumentException(Resources.ExceptionUDDIDefaultEventLogNotInitialised_, "invalidSites");
            }

            ////////// Define pre-condition.
            ////////Contract.Requires(invalidSites != null);

            // Textual list of invalid site error reports
            Func<string> invalidSiteList = () =>
            {
                var stringBuilder = new StringBuilder();

                foreach (var site in invalidSites)
                {
                    stringBuilder.Append(site.InquireUrl);
                    stringBuilder.Append(" : ");
                    stringBuilder.AppendLine(site.Description);
                }

                return stringBuilder.ToString();
            };

            // Log any invalid sites
            UddiEventLog.DefaultLog.WriteInvalidUddiSitesFoundInAdWarning(invalidSiteList());
        }

        /// <summary>
        /// Log warnings for unexpected removal of a cache control entry.
        /// </summary>
        /// <param name="reason">
        /// The reason for removal from the cache.
        /// </param>
        public static void LogWarningForUnexpectedRemovalOfCacheControlEntry(this CacheEntryRemovedReason reason)
        {
            // Precondition
            if (UddiEventLog.DefaultLog == null)
            {
                throw new Exception(Resources.ExceptionUDDIDefaultEventLogNotInitialised_);
            }

            //////// Define pre-condition.
            //////Contract.Assume(UddiEventLog.DefaultLog != null);

            UddiEventLog.DefaultLog.WriteUddiSiteUnexpectedlyRemovedFromCacheWarning(reason.ToString());
        }

        /// <summary>
        /// Convert a string value to a decimal value.  Return a default value if the 
        /// conversion fails.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="defaultValue">The default value to be returned on failure.</param>
        /// <returns>The decimal value.</returns>
        public static decimal ConvertToDecimal(this string value, decimal defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            try
            {
                return decimal.Parse(value);
            }
            catch (FormatException frmEx)
            {
                UddiEventLog.DefaultLog.WriteConversionToDecimalFailedWarning(value, defaultValue, frmEx);
            }
            catch (OverflowException ovrEx)
            {
                UddiEventLog.DefaultLog.WriteConversionToDecimalFailedWarning(value, defaultValue, ovrEx);
            }

            return defaultValue;
        }

        /// <summary>
        /// Convert a decimal value representing the configured cache expiry time span in hours.
        /// The value can represent fractions of hours.
        /// </summary>
        /// <param name="spanHours">The cache expiry time span (hours).</param>
        /// <returns>A value representing the expiry time relative to UTC.</returns>
        public static DateTimeOffset ExpiryTime(this decimal spanHours)
        {
            // Covert to timespan, allowing for fractions of an hour
            var hrs = (int)(spanHours - (spanHours % 1));
            var minsDecimal = (spanHours % 1) * 60;
            var mins = (int)(minsDecimal - (minsDecimal % 1));
            var secsDecimal = (minsDecimal % 1) * 60;
            var secs = (int)(secsDecimal - (secsDecimal % 1));

            return DateTimeOffset.Now.Add(new TimeSpan(hrs, mins, secs));
        }

        /// <summary>
        /// Returns a UDDI business entity for the given site location, treating the
        /// string as a business key. 
        /// </summary>
        /// <param name="businessKey">The business entity key.</param>
        /// <param name="siteLocation">The UDDI site location.</param>
        /// <returns>The UDDI business entity.</returns>
        public static BusinessEntity UddiBusinessEntityByKey(this string businessKey, UddiSiteLocation siteLocation)
        {
            // The UDDI business entity for the given key.
            Func<BusinessEntity> uddiBusinessEntityByKey = 
                () =>
                     new GetBusinessDetail(businessKey)
                            .Send(new UddiConnection(siteLocation))
                            .BusinessEntities 
                            .FirstOrDefault();

            return uddiBusinessEntityByKey.SafeInvoke(
                siteLocation, 
                string.Format("UDDI business key: {0}", businessKey))();
        }

        /// <summary>
        /// Returns a UDDI business service for the given site location, treating the
        /// string as a service key. 
        /// </summary>
        /// <param name="serviceKey">The service key.</param>
        /// <param name="siteLocation">The UDDI site location.</param>
        /// <returns>The UDDI business service.</returns>
        public static BusinessService UddiBusinessServiceByKey(this string serviceKey, UddiSiteLocation siteLocation)
        {
            // The UDDI business service for the given key.
            Func<BusinessService> uddiBusinessServiceByKey =
                () =>
                     new GetServiceDetail(serviceKey)
                            .Send(new UddiConnection(siteLocation))
                            .BusinessServices
                            .FirstOrDefault();

            return uddiBusinessServiceByKey.SafeInvoke(
                siteLocation, 
                string.Format("UDDI service key: {0}", serviceKey))();
        }

        /// <summary>
        /// Safely invoke a function that performs a UDDI SOAP call for business info. 
        /// </summary>
        /// <param name="businessInfoSend">The business information instance.</param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        /// <returns>A wrapped version of the function with additional error handling.</returns>
        public static Func<UddiSiteLocation, BusinessInfo> SafeInvoke(
            this Func<UddiSiteLocation, BusinessInfo> businessInfoSend,
            params string[] loggingData)
        {
            return siteLocation =>
                {
                    try
                    {
                        return businessInfoSend(siteLocation);
                    }
                    catch (Exception ex)
                    {
                        HandleSafeInvokeException(ex, "find business info", siteLocation, loggingData);
                    }

                    return null;
                };
        }

        /// <summary>
        /// Safely invoke a function that performs a UDDI SOAP call for service info. 
        /// </summary>
        /// <param name="serviceInfoSend">The service information instance.</param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        /// <returns>A wrapped version of the function with additional error handling.</returns>
        public static Func<UddiSiteLocation, ServiceInfo> SafeInvoke(
            this Func<UddiSiteLocation, ServiceInfo> serviceInfoSend,
            params string[] loggingData)
        {
            return siteLocation =>
                {
                    try
                    {
                        return serviceInfoSend(siteLocation);
                    }
                    catch (Exception ex)
                    {
                        HandleSafeInvokeException(ex, "find service info", siteLocation, loggingData);
                    }

                    return null;
                };
        }

        /// <summary>
        /// Safely invoke a function that performs a UDDI SOAP call for a binding template. 
        /// </summary>
        /// <param name="bindingTemplate">The binding template instance.</param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        /// <returns>A wrapped version of the function with additional error handling.</returns>
        public static Func<UddiSiteLocation, BindingTemplate> SafeInvoke(
            this Func<UddiSiteLocation, BindingTemplate> bindingTemplate,
            params string[] loggingData)
        {
            return siteLocation =>
                {
                    try
                    {
                        return bindingTemplate(siteLocation);
                    }
                    catch (Exception ex)
                    {
                        HandleSafeInvokeException(ex, "find binding template", siteLocation, loggingData);
                    }

                    return null;
                };
        }

        /// <summary>
        /// Safely invoke a function that performs a UDDI SOAP call for a business entity. 
        /// </summary>
        /// <param name="businessEntity">The business entity instance.</param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        /// <returns>A wrapped version of the function with additional error handling.</returns>
        public static Func<BusinessEntity> SafeInvoke(
            this Func<BusinessEntity> businessEntity,
            UddiSiteLocation siteLocation,
            params string[] loggingData)
        { 
            return () =>
                {
                    try
                    {
                        return businessEntity();
                    }
                    catch (Exception ex)
                    {
                        HandleSafeInvokeException(ex, "find business entity", siteLocation, loggingData);
                    }

                    return null;
                };
        }

        /// <summary>
        /// Safely invoke a function that performs a UDDI SOAP call for a business service. 
        /// </summary>
        /// <param name="businessService">The business service instance.</param>
        /// <param name="siteLocation">
        /// The site location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        /// <returns>A wrapped version of the function with additional error handling.</returns>
        public static Func<BusinessService> SafeInvoke(
            this Func<BusinessService> businessService,
            UddiSiteLocation siteLocation,
            params string[] loggingData)
        {
            return () =>
            {
                try
                {
                    return businessService();
                }
                catch (Exception ex)
                {
                    HandleSafeInvokeException(ex, "find business service", siteLocation, loggingData);
                }

                return null;
            };
        }

        /// <summary>
        /// Returns a concatenation of the error information and information for all inner exceptions.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        /// <param name="stringBuilder">A string builder.  If none is passed, a new one is created</param>
        /// <returns>The concatenated string the error information and information for all inner exceptions.</returns>
        public static string WithInnerExceptions(this Exception ex, StringBuilder stringBuilder = null)
        {
            Action<StringBuilder> appendDivider =
                stringBuilderOut => stringBuilderOut.AppendLine().AppendLine(new string('-', 72)).AppendLine();

            while (true)
            {
                if (stringBuilder == null)
                {
                    stringBuilder = new StringBuilder();
                }

                if (ex == null)
                {
                    appendDivider(stringBuilder);
                    stringBuilder.AppendLine(
                        UddiEventLog.EnvironmentRecord);
                    return stringBuilder.ToString();
                }

                appendDivider(stringBuilder);
                stringBuilder.AppendLine(ex.ToString());

                if (ex.InnerException != null)
                {
                    appendDivider(stringBuilder);
                    stringBuilder.AppendLine(
                        UddiEventLog.EnvironmentRecord);
                    return stringBuilder.ToString();
                }

                ex = ex.InnerException;
            }
        }

        /// <summary>
        /// Strips the escape characters from a string.
        /// </summary>
        /// <param name="inString">The string to be stripped.</param>
        /// <returns>A string without escape characters.</returns>
        public static string RemoveEscapedCharacters(this string inString)
        {
            // A buffer to hold the acceptable characters.
            var buffer = new char[inString.Length];

            // Index into the buffer.
            var index = 0;

            foreach (var nextChar in inString)
            {
                var nextCharWord = Convert.ToInt16(nextChar);

                // Reject some of the chacaters below 0x0020.
                if (nextCharWord >= 0x0020 || nextCharWord == 0x0009 || nextCharWord == 0x000A || nextCharWord == 0x00D)
                {
                    buffer[index++] = nextChar;
                    continue;
                }

                buffer[index++] = '?';
            }

            return new string(buffer, 0, index);
        }
        
        /// <summary>
        /// Sets the base URL for the given access point.  If the access point
        /// is an absolute URL, the schema and domain are overridden.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="accessPoint">The access Point.</param>
        /// <returns>The access point with the given base URL.</returns>
        public static string SetAsBaseUrlOn(this string baseUrl, string accessPoint)
        {
            // The default base URI.
            const string DefaultBaseUri = "http://localhost/";

            // The base URI for the given base URL parameter
            Func<Func<string, Uri>, Func<Uri>> baseUriForBaseUrl =
                getBaseUriForNonAbsoluteUriString => // Create the base URI:
                // If no base URL was provided
                string.IsNullOrWhiteSpace(baseUrl)
                    ? // then use DefaultBaseUri
                    () => new Uri(DefaultBaseUri)
                    : // else IF the URL is well formed (either relative or absolute) 
                    Uri.IsWellFormedUriString(baseUrl, UriKind.RelativeOrAbsolute)
                        ? // then IF the base URL is absolute
                        Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute)
                            ? // then use the base URL
                            () => new Uri(baseUrl)
                            : // else remove any leading path delimiters and use baseUriForNonAbsoluteUriString
                            (Func<Uri>)(() => getBaseUriForNonAbsoluteUriString(baseUrl.RemoveLeadingPathDelimiters()))
                        : // else write a warning to the event log and use DefaultBaseUri
                        () => new Uri(baseUrl.LogWarningForInvalidUri(DefaultBaseUri));

            // The base URL for URI string that is non-absolute.
            Func<string, Uri> baseUriForNonAbsoluteUriString =
                uriStringWithoutLeadingPathDelimiters => // Create the base URI:
                // If the processed URL, combined with the HTTP scheme, can be interpreted as an absolute URL
                Uri.IsWellFormedUriString("http://" + uriStringWithoutLeadingPathDelimiters, UriKind.Absolute)
                    ? // then use this combination:
                    new Uri("http://" + uriStringWithoutLeadingPathDelimiters)
                    : // else combine the URL with DefaultBaseUri as the base URI.
                    new Uri(new Uri(DefaultBaseUri), baseUrl);

            // The access point URI string for the given accessPoint parameter
            Func<Func<Uri, string>, Func<string>> accessPointForAccessPointUrl =
                getRelativeUriFromAccessPoint => // Create the access point URI:
                // If no access point was provided
                string.IsNullOrWhiteSpace(accessPoint)
                    ? // then write a warning to the event log
                    () => "<nullOrWhitespace>".LogWarningForInvalidUri()
                    : // else IF the access point is a well formed URI (absolute or relative)
                    Uri.IsWellFormedUriString(accessPoint, UriKind.RelativeOrAbsolute)
                        ? // then IF the access point is an absolute URI
                        Uri.IsWellFormedUriString(accessPoint, UriKind.Absolute)
                            ? // then convert it into a relative URI:
                            (Func<string>)(() => getRelativeUriFromAccessPoint(new Uri(accessPoint)))
                            : // else use it as a URI.
                            () => accessPoint
                        : // else write a warning to the event log.
                        () => accessPoint.LogWarningForInvalidUri();

            // Convert the access point to a relative URI by removing the authority.
            Func<Uri, string> convertAccessPointToRelativeUri =
                accessPointUri => // Create the relative access point URI:
                new Uri(accessPointUri.GetLeftPart(UriPartial.Authority)).MakeRelativeUri(accessPointUri).ToString();

            // The base URI.
            var baseUri = baseUriForBaseUrl(baseUriForNonAbsoluteUriString);

            // The access point URI string.
            var accessPointUriText = accessPointForAccessPointUrl(convertAccessPointToRelativeUri);

            // Return an access point URI combining accessPointUriText with baseUri as the base URI.
            return new Uri(baseUri(), accessPointUriText()).AbsoluteUri;
        }

        /// <summary>
        /// Handles an exception returned from the overloaded SafeInvoke methods.
        /// </summary>
        /// <param name="ex">
        /// The exception.
        /// </param>
        /// <param name="requestType">
        /// The UDDI request type.
        /// </param>
        /// <param name="siteLocation">
        /// The site Location.
        /// </param>
        /// <param name="loggingData">
        /// Additional strings to be logged.
        /// </param>
        private static void HandleSafeInvokeException(Exception ex, string requestType, UddiSiteLocation siteLocation, string[] loggingData)
        {
            // Precondition
            if (UddiEventLog.DefaultLog == null)
            {
                throw new Exception(Resources.ExceptionUDDIDefaultEventLogNotInitialised_);
            }

            ////////// Define pre-condition.
            ////////Contract.Assume(UddiEventLog.DefaultLog != null);

            var exception = ex as ArgumentNullException;

            if (exception != null)
            {
                // A null UDDI connection was passed when trying to invoke a UDDI service.
                UddiEventLog.DefaultLog.WriteUddiConnectionIsNullError(exception, requestType, siteLocation, loggingData);
                return;
            }

            var argEx = ex as ArgumentException;

            if (argEx != null)
            {
                // An unknown message type was passed when trying to invoke a UDDI service.
                UddiEventLog.DefaultLog.WriteUddiUnknownMessageTypeError(argEx, requestType, siteLocation, loggingData);
                return;
            }

            var operationEx = ex as InvalidOperationException;

            if (operationEx != null)
            {
                // A null site location or Inquiry URL was passed when trying to invoke a UDDI service.
                UddiEventLog.DefaultLog.WriteUddiInvalidSiteError(operationEx, requestType, siteLocation, loggingData);
                return;
            }

            var keyEx = ex as InvalidKeyPassedException;

            if (keyEx != null)
            {
                // An invalid key value was passed when trying to invoke a UDDI service.
                UddiEventLog.DefaultLog.WriteUddiInvalidKeyError(keyEx, requestType, siteLocation, loggingData);
                return;
            }

            var unkEx = ex as UnknownErrorException;

            if (unkEx != null)
            {
                // An unexpected error occurred when invoking a UDDI service.
                UddiEventLog.DefaultLog.WriteUddiUnknownError(unkEx, requestType, siteLocation, loggingData);
                return;
            }

            var uddiEx = ex as UddiException;

            if (uddiEx != null)
            {
                // A SOAP fault occurred when invoking a UDDI service.
                UddiEventLog.DefaultLog.WriteUddiError(uddiEx, requestType, siteLocation, loggingData);
                return;
            }

            // An unexpected error occurred when invoking a UDDI service.
            UddiEventLog.DefaultLog.WriteUddiUnexpectedError(ex, requestType, siteLocation, loggingData);
        }
    }
}
