// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteLocationCache.cs" company="Solidsoft Reply Ltd.">
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

namespace SolidsoftReply.Esb.Libraries.Uddi
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Caching;

    using Microsoft.Uddi3;
    using Microsoft.Uddi3.Extensions;

    /// <summary>
    /// A cache of site locations.  This is a wrapper class for MemoryCache.  
    /// </summary>
    internal class SiteLocationCache : IEnumerable<KeyValuePair<string, UddiSiteLocation>>
    {
        /// <summary>
        /// The memory cache for UDDI inquiry services.
        /// </summary>
        private static readonly MemoryCache Cache;

        /// <summary>
        /// Lock object for accessing the memory cache.
        /// </summary>
        private static readonly object Lock;

        /// <summary>
        /// Initializes static members of the <see cref="SiteLocationCache"/> class.
        /// </summary>
        static SiteLocationCache()
        {
            // Precondition
            if (UddiEventLog.DefaultLog == null)
            {
                throw new Exception("The UDDI default event log is not initialised.");
            }

            ////////// Define pre-condition.
            ////////Contract.Assume(UddiEventLog.DefaultLog != null);

            // Write an information record to the event log for start of initialisation of the inquiry services.
            UddiEventLog.DefaultLog.WriteInquiryStartupInfo();

            // Create the lock for guarding cache integrity.
            Lock = new object();

            // Initialise the cache settings.
            var cacheSettings = new NameValueCollection(3)
                                    {
                                        { "cacheMemoryLimitMegabytes", "0" },
                                        { "physicalMemoryPercentage", "0" },
                                        { "pollingInterval", "00:02:00" }
                                    };

            // Create the cache using the given settings.
            Cache = new MemoryCache("UDDIInquiryServices", cacheSettings);

            // The inquiry service address from a config file.
            //       May throw ConfigurationErrors exception.
            var uddiInquiryService = "UDDIInquiryService".GetUriAppSetting();

            // The default service host from a config file.
            //       May throw ConfigurationErrors exception.
            var uddiDefaultServiceHost = "UDDIDefaultServiceHost".GetUriAppSetting();

            // The UDDI inquiry URI from configuration.
            Func<Func<Func<string, Uri>, Uri>, Func<string, Uri>, Uri> inquiryUrlFromConfig = 
                (getInquiryUrlFromAddress, getInquiryUrlUsingBaseServiceHost) => // Create the inquiry URI:
                // If no URL was provided,
                string.IsNullOrWhiteSpace(uddiInquiryService)
                    ? // then create the URI for a Microsoft UDDI directory:
                    getInquiryUrlUsingBaseServiceHost("/uddi/inquire.asmx")
                    : // else use inquiryUrlFromAddress.
                    getInquiryUrlFromAddress(getInquiryUrlUsingBaseServiceHost);
            
            // The UDDI inquiry URL from an address string.
            Func<Func<string, Uri>, Uri> inquiryUrlFromAddress =
                getInquiryUrlUsingBaseServiceHost => // Get the inquiry URL:
                // If the URL is well-formed and absolute
                Uri.IsWellFormedUriString(uddiInquiryService, UriKind.Absolute)
                    ? // then use it:
                    new Uri(uddiInquiryService)
                    : // else create a URI using uddiInquiryService.
                    getInquiryUrlUsingBaseServiceHost(uddiInquiryService);
        
            // The UDDI inquiry URL using a base service host URL.
            Func<string, Uri> inquiryUrlUsingBaseServiceHost = 
                uddiInquiryServiceRelative => // Get the inquiry URL:
                // If no default service host base URI has been provided
                string.IsNullOrWhiteSpace(uddiDefaultServiceHost)
                    ? // then use a default base URI:
                    new Uri(new Uri("http://localhost/"), uddiInquiryServiceRelative)
                    : // else use the default service host base URI as the base for the relative inquiry URL.
                    new Uri(uddiDefaultServiceHost.SetAsBaseUrlOn(uddiInquiryServiceRelative));

            // Remove any existing default UDDI inquire service entry from the cache.
            Remove("DefaultUDDIInquiryService");

            // The UDDI inquiry absolute URI.
            var inquiryUrlAbsoluteUri = inquiryUrlFromConfig(inquiryUrlFromAddress, inquiryUrlUsingBaseServiceHost).AbsoluteUri;

            // Add a new default UDDI inquiry service entry to the cache.
            Add(
                "DefaultUDDIInquiryService",
                new UddiSiteLocation(
                    inquiryUrlAbsoluteUri,
                    string.Empty,
                    string.Empty,
                    "Default UDDI Inquiry Service for Solidsoft Reply ESB UDDI library.",
                    AuthenticationMode.WindowsAuthentication),
                new CacheItemPolicy());

            // Write an information record to the event log for successful initialisation of the inquiry services.
            UddiEventLog.DefaultLog.WriteInquiryInitialisedInfo(inquiryUrlAbsoluteUri);

            // Discover additional UDDI inquiry services from Active Directory.
            DiscoverInquiryServices();
        }

        /// <summary>
        /// Creates an enumerator that can be used to iterate through a collection of cache entries.
        /// </summary>
        /// <returns>
        /// The enumerator object that provides access to the cache entries in the cache.
        /// </returns>
        public IEnumerator<KeyValuePair<string, UddiSiteLocation>> GetEnumerator()
        {
            // Protect the integrity of the cache.
            lock (Lock)
            {
                // Create a collection of cache key/value pairs and return the enumerator.
                return Cache.Select(
                    keyValuePair => // Create a key/value pair entry for the collection:
                    new KeyValuePair<string, UddiSiteLocation>(keyValuePair.Key, (UddiSiteLocation)keyValuePair.Value))
                    .GetEnumerator();
            }
        }

        /// <summary>
        /// Creates an enumerator that can be used to iterate through a collection of cache entries.
        /// </summary>
        /// <returns>The enumerator object that provides access to the items in the cache.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Protect the integrity of the cache.
            lock (Lock)
            {
                // Get the enumerator for the cache.
                return ((IEnumerable)Cache).GetEnumerator();
            }
        }

        /// <summary>
        /// Inserts a cache entry into the Site Location cache, specifying information about
        ///  how the entry will be evicted.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the cache entry.
        /// </param>
        /// <param name="value">
        /// The UDDI site location to insert. 
        /// </param>
        /// <param name="policy">
        /// An object that contains eviction details for the cache entry. 
        /// This object provides more options for eviction than a simple absolute expiration.
        /// </param>
        /// <returns>
        /// true if insertion succeeded, or false if there is an already an entry 
        /// in the cache that has the same key as item.
        /// </returns>
        private static bool Add(string key, UddiSiteLocation value, CacheItemPolicy policy)
        {
            // Protect the integrity of the cache.
            lock (Lock)
            {
                // Add the site location to the cache.
                return Cache.Add(key, value, policy);
            }
        }

        /// <summary>
        /// Determines whether a cache entry exists in the Site Location cache.
        /// </summary>
        /// <param name="key">
        /// A unique identifier for the cache entry to search for.
        /// </param>
        /// <returns>
        /// true if the cache contains a cache entry whose key matches key; otherwise, false.
        /// </returns>
        private static bool Contains(string key)
        {
            // Protect the integrity of the cache.
            lock (Lock)
            {
                // Determine if the cache contains the site location.
                return Cache.Contains(key);
            }
        }

        /// <summary>
        /// Removes a cache entry from the Site Location cache. 
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to remove.</param>
        private static void Remove(string key)
        {
            // Protect the integrity of the cache.
            lock (Lock)
            {
                // Remove the site location from the cache.
                Cache.Remove(key);
            }
        }

        /// <summary>
        /// Discover UDDI inquiry services for UDDI directories registered in Active Directory.
        /// </summary>
        private static void DiscoverInquiryServices()
        {
            // The flag in the config file indicating whether UDDI site discovery from AD should be used.  
            //       May throw ConfigurationErrors exception.
            var uddiDiscoverSites = "ESB.UDDI.DiscoverSites".GetAppSetting();

            // Check configuration to see if site iscovery should be performed.
            if (string.IsNullOrWhiteSpace(uddiDiscoverSites)
                || !(new[] { "true", "yes", "T", "y", "1" }).Contains(uddiDiscoverSites.ToLower()))
            {
                return;
            }

            // Protect the integrity of the cache.
            lock (Lock)
            {
                // Find all UDDI sites registered in Active Directory and log warnings for any invalid sites.
                FindAndCacheRegisteredUddiSites().LogWarningsForInvalidUddiSites();

                // Add a special entry in the cache to control refreshing of inquirey services 
                // in sites registered in Active Directory.
                SetCacheRefreshEntry();
            }
        }

        /// <summary>
        /// Find all UDDI sites registered in Active Directory and log warnings for any invalid sites.
        /// </summary>
        /// <returns>A collection of invalid site locations from Active Directory.</returns>
        private static IEnumerable<UddiSiteLocation> FindAndCacheRegisteredUddiSites()
        {
            // The flag indicating whether a UDDI site location was successfully cached.
            Func<string, UddiSiteLocation, bool> isCached = // Determine if the site location was cached:
                (keyValue, site) =>
                !string.IsNullOrWhiteSpace(site.InquireUrl)
                && Uri.IsWellFormedUriString(site.InquireUrl, UriKind.RelativeOrAbsolute)
                && (Contains(keyValue) || Add(keyValue, site, new CacheItemPolicy()));

            // The unique key value to be used in the cache.
            Func<UddiSiteLocation, string> key = site => // Get the unique cache key value:
                // If the inquiry URL is well-formed and absolute
                Uri.IsWellFormedUriString(site.InquireUrl, UriKind.Absolute)
                    ? // then use it as the key:
                    site.InquireUrl
                    : // else generate a new key as a GUID.
                    Guid.NewGuid().ToString();

            // Clear the cache of all previously discovered sites.
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Cache.Where(item => item.Key != "DefaultUDDIInquiryService").Select(item => Cache.Remove(item.Key)).ToList();

            // Find and cache sites, returning a collection any invalid sites.
            // NB. The Find method doesn't throw errors.  Any exception is silently consumed in the Microsoft.Uddi library.
            // When an error occurs, it is possible for the returned collection to be partially populated with discovered sites.
            // There is no way of telling if Find is broken.
            return
                from site in UddiSiteDiscovery.Find(UddiSiteUrlType.Inquire, AuthenticationMode.WindowsAuthentication)
                where isCached(key(site), site) == false
                select site;
        }

        /// <summary>
        /// Set a special entry in the cache to control refreshing of inquiry services in 
        /// sites registered in Active Directory.
        /// </summary>
        private static void SetCacheRefreshEntry()
        {
            // The configured cache expiry time span (hours).  
            //       May throw ConfigurationErrors exception.
            var uddiExpireDiscoveredSitesAfterHrs = "ESB.UDDI.ExpireDiscoveredSitesAfterHours".GetAppSetting();

            // Add the control entry to the cache.
            Add(
                "ControlCacheRefreshEntry",
                new UddiSiteLocation(string.Empty, string.Empty),
                new CacheItemPolicy
                {
                    AbsoluteExpiration = uddiExpireDiscoveredSitesAfterHrs.ConvertToDecimal(24).ExpiryTime(),
                    RemovedCallback = arguments =>
                    {
                        switch (arguments.RemovedReason)
                        {
                            // Only refresh cache if entry has expired.
                            case CacheEntryRemovedReason.Expired:
                            case CacheEntryRemovedReason.Evicted:
                                // Discover additional UDDI inquiry services from Active Directory.
                                DiscoverInquiryServices();
                                break;
                            case CacheEntryRemovedReason.CacheSpecificEviction:
                                // Ignore. This occurs when assembly is unloaded.
                                break;
                            default:
                                // Write a warning to the event log for an unexpected cache event .
                                arguments.RemovedReason.LogWarningForUnexpectedRemovalOfCacheControlEntry();
                                break;
                        }
                    }
                });
        }
    }
}
