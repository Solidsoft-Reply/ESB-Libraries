// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InquiryServices.cs" company="Solidsoft Reply Ltd.">
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
    using System.Linq;

    using Microsoft.Uddi3;
    using Microsoft.Uddi3.Businesses;
    using Microsoft.Uddi3.Extensions;
    using Microsoft.Uddi3.Services;

    /// <summary>
    ///   Represents enquiry services provided by one or more UDDI directories.  The
    ///   class can be configured to search Active Directory for multiple directories.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InquiryServices
    {
        /// <summary>
        /// The app setting name for the default UDDI inquiry service URL.
        /// </summary>
        private const string DefaultUddiInquiryService = "DefaultUDDIInquiryService";

        /// <summary>
        /// The key name for the special cache entry for controlling cache refresh.
        /// </summary>
        private const string ControlCacheRefreshEntry = "ControlCacheRefreshEntry";

        /// <summary>
        /// The cache for UDDI inquiry services.
        /// </summary>
        private static readonly SiteLocationCache Cache;

        /// <summary>
        /// Initializes static members of the <see cref="InquiryServices"/> class. 
        /// </summary>
        static InquiryServices()
        {
            // Create the site location new cache.
            Cache = new SiteLocationCache();
        }

        /// <summary>
        /// Returns a new Provider Identifier for a key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <returns>A Provider Identifier.</returns>
        public static ProviderIdentifier CreateProviderKey(string key)
        {
            // Precondition
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Provider key is invalid or missing.", "key");
            }

            ////////// Define pre-condition.
            ////////Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(key));

            // Return a new service provider identifier for the given key.
            return new ProviderIdentifier(key, true);
        }

        /// <summary>
        /// Returns a new Provider Identifier for a key.
        /// </summary>
        /// <param name="name">The provider name.</param>
        /// <returns>A Provider Identifier.</returns>
        public static ProviderIdentifier CreateProviderName(string name)
        {
            // Precondition
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Provider name is invalid or missing.", "name");
            }

            ////////// Define pre-condition.
            ////////Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(name));

            // Return a new service provider identifier for the given name.
            return new ProviderIdentifier(name, false);
        }

        /// <summary>
        /// Returns a new Service Identifier for a key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <returns>A Service Identifier.</returns>
        public static ServiceIdentifier CreateServiceKey(string key)
        {
            // Precondition
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Service key is invalid or missing.", "key");
            }

            //////// Define pre-condition.
            //////Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(key));

            // Return a new service identifier for the given key.
            return new ServiceIdentifier(key, true);
        }

        /// <summary>
        /// Returns a new Service Identifier for a key.
        /// </summary>
        /// <param name="name">The service name.</param>
        /// <returns>A Service Identifier.</returns>
        public static ServiceIdentifier CreateServiceName(string name)
        {
            // Precondition
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Service name is invalid or missing.", "name");
            }

            //////// Define pre-condition.
            //////Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(name));

            // Return a new service identifier for the given name.
            return new ServiceIdentifier(name, false);
        }

        /// <summary>
        /// Find an access point for a given binding key.
        /// </summary>
        /// <param name="bindingKey">A registered binding key.</param>
        /// <returns>As access point.</returns>
        public static string FindAccessPointForBinding(string bindingKey)
        {
            // Precondition
            if (string.IsNullOrWhiteSpace(bindingKey))
            {
                throw new ArgumentException("Binding key is invalid or missing.", "bindingKey");
            }

            //////////// Define pre-condition.
            //////////Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(bindingKey));

            // The binding template for the given binding key.
            var bindingTemplate = GetBindingTemplate(bindingKey);

            // Return the access point value for the given binding template:
            // If no binding template was found
            return bindingTemplate == default(BindingTemplate)
                       ? // then indicate this
                       string.Empty
                       : // else return the access point value.
                       bindingTemplate.AccessPoint.Text;
        }

        /// <summary>
        /// Find an endpoint access point for a given service.
        /// </summary>
        /// <param name="service">The service key or name.</param>
        /// <returns>As access point.</returns>
        public static string FindAccessPointForService(ServiceIdentifier service)
        {
            // Precondition.
            if (service == null)
            {
                throw new ArgumentNullException("service", "Service identifier is invalid or missing.");
            }

            ////////// Define pre-condition.
            ////////Contract.Requires<ArgumentNullException>(service != null);

            // Return the access point value for the given service.
            return FindAccessPointForService(service, AccessPointUseType.EndPoint);
        }

        /// <summary>
        /// Find an access point for a given service .
        /// </summary>
        /// <param name="service">The service key or name.</param>
        /// <param name="useType">The use type of the access point</param>
        /// <returns>As access point.</returns>
        public static string FindAccessPointForService(ServiceIdentifier service, AccessPointUseType useType)
        {
            // Precondition.
            if (service == null)
            {
                throw new ArgumentNullException("service", "Service identifier is invalid or missing.");
            }

            // Precondition.
            if (string.IsNullOrWhiteSpace(service.Value))
            {
                throw new ArgumentException("Service identifier value is invalid or missing.", "service");
            }

            ////////// Define pre-conditions.
            ////////Contract.Requires<ArgumentNullException>(service != null);
            ////////Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(service.Value));

            // The business service for the given service key or name. 
            var businessService = GetBusinessService(service);

            // The binding template for the access point of the given use type for the given business service.
            var bindingTemplate = // Search for the binding template:
                // If no business service was found
                businessService == null
                    ? // then indicate that no binding template is available:
                    default(BindingTemplate)
                    : // else search the business service for the first binding template for an access point of the required use type.
                    (from template in businessService.BindingTemplates
                     where
                         string.Compare(
                             template.AccessPoint.UseType,
                             useType.ToString(),
                             StringComparison.OrdinalIgnoreCase) == 0
                     select template).FirstOrDefault();

            // Return the access point value for the given binding template:
            // If no binding template was found
            return bindingTemplate == default(BindingTemplate)
                       ? // then indicate this
                       string.Empty
                       : // else return the access point value.
                       bindingTemplate.AccessPoint.Text;
        }

        /// <summary>
        /// Find an endpoint access point for a given service provided by a given business entity.
        /// </summary>
        /// <param name="provider">The provider key or name.</param>
        /// <param name="service">The service key or name.</param>
        /// <returns>As access point.</returns>
        public static string FindAccessPointForService(ProviderIdentifier provider, ServiceIdentifier service)
        {
            // Precondition.
            if (provider == null)
            {
                throw new ArgumentNullException("provider", "Provider identifier is invalid or missing.");
            }

            // Precondition.
            if (service == null)
            {
                throw new ArgumentNullException("service", "Service identifier is invalid or missing.");
            }

            ////////// Define pre-conditions.
            ////////Contract.Requires<ArgumentNullException>(provider != null);
            ////////Contract.Requires<ArgumentNullException>(service != null);

            // Return the endpoint access point value for the given service provided by a given business entity.
            return FindAccessPointForService(provider, service, AccessPointUseType.EndPoint);
        }

        /// <summary>
        /// Find an access point for a given service provided by a given business entity and of a give use type.
        /// </summary>
        /// <param name="provider">The provider key or name.</param>
        /// <param name="service">The service key or name.</param>
        /// <param name="useType">The use type of the access point</param>
        /// <returns>As access point.</returns>
        public static string FindAccessPointForService(ProviderIdentifier provider, ServiceIdentifier service, AccessPointUseType useType)
        {
            // Precondition.
            if (provider == null)
            {
                throw new ArgumentNullException("provider", "Provider identifier is invalid or missing.");
            }

            // Precondition.
            if (service == null)
            {
                throw new ArgumentNullException("service", "Service identifier is invalid or missing.");
            }

            // Precondition.
            if (string.IsNullOrWhiteSpace(provider.Value))
            {
                throw new ArgumentException("Provider identifier value is invalid or missing.", "provider");
            }

            // Precondition.
            if (string.IsNullOrWhiteSpace(service.Value))
            {
                throw new ArgumentException("Service identifier value is invalid or missing.", "service");
            }
            
            ////////// Define pre-conditions
            ////////Contract.Requires<ArgumentNullException>(provider != null);
            ////////Contract.Requires<ArgumentNullException>(service != null);
            ////////Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(provider.Value));
            ////////Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(service.Value));

            // The business entity for the given provider key or name. 
            var businessEntity = GetBusinessEntity(provider);

            // The business service of the given name for the given provider.
            var businessService = // Search for the business service:
                // If no business entity was provided
                businessEntity == default(BusinessEntity)
                    ? // then indicate that no business service is available:
                    default(BusinessService)
                    : // else treat the service value as a name and search for the first business service that has that name...
                    (from bs in businessEntity.BusinessServices
                     where
                         bs.Names.SingleOrDefault(name => string.CompareOrdinal(name.Text, service.Value) == 0) != null
                     select bs).FirstOrDefault()
                    ?? // ...and if nothing is found, treat the service value as a key and search for 
                    //       the first business service that has that key.
                    (from bs in businessEntity.BusinessServices
                     where string.CompareOrdinal(bs.ServiceKey, service.Value) == 0
                     select bs).FirstOrDefault();

            // The binding template for the access point of the given use type for the given business service.
            var bindingTemplate = // Get the binding template:
                // If no business service was found
                businessService == default(BusinessService)
                    ? // then indicate that no binding template is available:
                    default(BindingTemplate)
                    : // else search the business service for the first binding template for an access point of the required use type.
                    (from template in businessService.BindingTemplates
                     where
                         string.Compare(
                             template.AccessPoint.UseType,
                             useType.ToString(),
                             StringComparison.OrdinalIgnoreCase) == 0
                     select template).FirstOrDefault();

            // Return the access point value for the given binding template:
            // If no binding template was found
            return bindingTemplate == default(BindingTemplate)
                       ? // then indicate this
                       string.Empty 
                       : // else return the access point value.
                       bindingTemplate.AccessPoint.Text;
        }

        /// <summary>
        /// Sets the base URL for the given access point.  If the access point
        /// is an absolute URL, the schema and domain are overridden.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="accessPoint">The access Point.</param>
        /// <returns>The access point with the given base URL.</returns>
        public static string SetAsBaseUrlOn(string baseUrl, string accessPoint)
        {
            // Return absolute URI fpor the access point using the base URL.
            return baseUrl.SetAsBaseUrlOn(accessPoint);
        }

        /// <summary>
        /// Get a business entity for the given provider key or name.
        /// </summary>
        /// <param name="provider">The business key or a name of a registered business.</param>
        /// <returns>A business entity.  Returns null if no entity is found.</returns>
        private static BusinessEntity GetBusinessEntity(IIdentifier provider)
        {
            // Precondition.
            if (provider == null) 
            {
                throw new ArgumentNullException("provider", "Provider identifier is invalid or missing.");
            }

            //////// Define pre-condition.
            //////Contract.Requires(provider != null);

            // The default UDDI inquiry service entry in the configuration file.
            var defaultDirectoryEntry = Cache.SingleOrDefault(entry => entry.Key == DefaultUddiInquiryService);

            // The business entity for a given provider key
            Func<BusinessInfo, BusinessEntity> uddiBusinessEntityForProviderKey =
                businessInfo => // Search for and return the business entity for the provider key:
                // If business information was found
                businessInfo != default(BusinessInfo)
                    ? // then use the key to search the default directory:
                    businessInfo.BusinessKey.UddiBusinessEntityByKey(defaultDirectoryEntry.Value)
                    : // else indicate that no business entity was found.
                    default(BusinessEntity);

            // The business entity for a given provider name.
            Func<UddiSiteLocation, Func<UddiSiteLocation, BusinessInfo>, BusinessInfo> uddiBusinessEntityByName =
                (siteLocation, getFirstBusinessInfoByName) =>
                    getFirstBusinessInfoByName.SafeInvoke(string.Format("Provider name: {0}", provider))(siteLocation);

            // The first business information found for a given provider name.
            Func<UddiSiteLocation, BusinessInfo> firstBusinessInfoByName =
                siteLocation =>
                (new FindBusiness(provider.Value)).Send(new UddiConnection(siteLocation)).BusinessInfos.FirstOrDefault();

            // The business entity.
            Func<UddiSiteLocation, BusinessEntity> uddiBusinessEntity =
                siteLocation =>
                uddiBusinessEntityForProviderKey(uddiBusinessEntityByName(siteLocation, firstBusinessInfoByName));

            // Search for and return the business entity:
            // If the provider value is a business key
            return provider.IsKey
                       ? // then search for the key in the default directory...
                       provider.Value.UddiBusinessEntityByKey(defaultDirectoryEntry.Value)
                       ?? // ...and if nothing is found, search through all discovered directories:
                       (from entry in Cache
                        where entry.Key != ControlCacheRefreshEntry && entry.Key != DefaultUddiInquiryService
                        select provider.Value.UddiBusinessEntityByKey(entry.Value)).FirstOrDefault(
                            businessEntity => businessEntity != null)
                       : // else treat the provider value as a business name and search for it in the default directory...
                       uddiBusinessEntity(defaultDirectoryEntry.Value)
                       ?? // ...and if nothing is found, search through all discovered directories.
                       (from entry in Cache
                        where entry.Key != ControlCacheRefreshEntry && entry.Key != DefaultUddiInquiryService
                        select uddiBusinessEntity(entry.Value)).FirstOrDefault(
                            businessEntity => businessEntity != null);
        }

        /// <summary>
        /// Get a business service for the given service key or name.
        /// </summary>
        /// <param name="service">The registered service key or a name.</param>
        /// <returns>A business service.  Returns null if no service is found.</returns>
        private static BusinessService GetBusinessService(IIdentifier service)
        {
            // Precondition.
            if (service == null)
            {
                throw new ArgumentNullException("service", "Service identifier is invalid or missing.");
            }

            //////// Define pre-condition.
            //////Contract.Requires(service != null);

            // The default UDDI inquiry service entry in the configuration file.
            var defaultDirectoryEntry = Cache.SingleOrDefault(entry => entry.Key == DefaultUddiInquiryService);

            // The business service for a given service key
            Func<ServiceInfo, BusinessService> uddiBusinessServiceForServiceKey =
                serviceInfo => // Search for and return the business service for the service key:
                // If service information was found
                serviceInfo != default(ServiceInfo)
                    ? // then use the key to search the default directory:
                    serviceInfo.ServiceKey.UddiBusinessServiceByKey(defaultDirectoryEntry.Value)
                    : // else indicate that no business service was found.
                    default(BusinessService);

            // The business service for a given service name.
            Func<UddiSiteLocation, Func<UddiSiteLocation, ServiceInfo>, ServiceInfo> uddiBusinessServiceByName =
                (siteLocation, getFirstServiceInfoByName) =>
                    getFirstServiceInfoByName.SafeInvoke(string.Format("Service name: {0}", service))(siteLocation);

            // The first service found for a given service name.
            Func<UddiSiteLocation, ServiceInfo> firstServiceInfoByName =
                siteLocation =>
                (new FindService(service.Value)).Send(new UddiConnection(siteLocation)).ServiceInfos.FirstOrDefault();

            // The business service.
            Func<UddiSiteLocation, BusinessService> uddiBusinessService =
                siteLocation =>
                uddiBusinessServiceForServiceKey(uddiBusinessServiceByName(siteLocation, firstServiceInfoByName));

            // Search for and return the business service:
            // If the value of the service is a key value
            return service.IsKey
                       ? // then search for the key in the default directory...
                       service.Value.UddiBusinessServiceByKey(defaultDirectoryEntry.Value)
                       ?? // ...and if nothing is found, search through all discovered directories:
                       (from entry in Cache
                        where entry.Key != ControlCacheRefreshEntry && entry.Key != DefaultUddiInquiryService
                        select service.Value.UddiBusinessServiceByKey(entry.Value)).FirstOrDefault(
                            businessService => businessService != null)
                       : // else treat the service value as a business name and search for it in the default directory...
                       uddiBusinessService(defaultDirectoryEntry.Value)
                       ?? // ...and if nothing is found, search through all discovered directories.
                       (from entry in Cache
                        where entry.Key != ControlCacheRefreshEntry && entry.Key != DefaultUddiInquiryService
                        select uddiBusinessService(entry.Value)).FirstOrDefault(
                            businessService => businessService != null);
        }

        /// <summary>
        /// Get a binding template for the given binding key.
        /// </summary>
        /// <param name="bindingKey">The registered binding key.</param>
        /// <returns>A binding template.  Returns null if no template is found.</returns>
        private static BindingTemplate GetBindingTemplate(string bindingKey)
        {
            // The default UDDI inquiry service entry in the configuration file.
            var defaultDirectoryEntry = Cache.SingleOrDefault(entry => entry.Key == DefaultUddiInquiryService);

            // The first binding template, if found.
            Func<UddiSiteLocation, BindingTemplate> getBindingTemplate = // Search a site location for a binding template:
                siteLocation =>
                new GetBindingDetail(bindingKey).Send(new UddiConnection(siteLocation))
                    .BindingTemplates.FirstOrDefault();

            // Search for and return the business service:
            return // Search the default directory for the binding template...
                getBindingTemplate.SafeInvoke(string.Format("Binding key: {0}", bindingKey))(
                    defaultDirectoryEntry.Value)
                ?? // ...and if nothing is found, search through all discovered directories.
                (from entry in Cache
                 where entry.Key != ControlCacheRefreshEntry && entry.Key != DefaultUddiInquiryService
                 select getBindingTemplate.SafeInvoke(string.Format("Binding key: {0}", bindingKey))(entry.Value))
                    .FirstOrDefault(bindingTemplate => bindingTemplate != null);
        }
    }
}