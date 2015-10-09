// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UddiTests.cs" company="Solidsoft Reply Ltd.">
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

#define WITHUDDICODECONTRACTS

namespace SolidsoftReply.Esb.Libraries.Uddi.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Fakes;
    using System.Configuration;
    using System.Configuration.Fakes;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Caching;

    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.Uddi3;
    using Microsoft.Uddi3.Businesses.Fakes;
    using Microsoft.Uddi3.Extensions;
    using Microsoft.Uddi3.Extensions.Fakes;
    using Microsoft.Uddi3.Fakes;
    using Microsoft.Uddi3.Services.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SolidsoftReply.Esb.Libraries.Uddi;

    /// <summary>
    /// Unit tests for the UDDI component.
    /// </summary>
    [TestClass]
    public class UddiTests
    {
        /////////////// <summary>
        /////////////// Tests ESB OnRamp.
        /////////////// </summary>
        ////////////[TestMethod]
        ////////////public void TestMethod1()
        ////////////{
        ////////////    var client = new ESBOnRamp.Solidsoft_BizTalk_ESB_OnRamp_OnRampInterface_OnRampClient();

        ////////////    object reqResp = "<bb xmlns='aa' />";

        ////////////    client.Submit(ref reqResp);
        ////////////}
        
        /// <summary>
        /// The URL of the test UDDI inquiry service.
        /// </summary>
        private const string TestUddiInquireUrl = "http://localhost/uddi/inquire.asmx";

        /// <summary>
        /// Find access point for service: service name.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceServiceName()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForService(InquiryServices.CreateServiceName("Service 1")),
                ShimUddiSiteDiscovery);
        }

        /// <summary>
        /// Find access point for service: provider name, service name.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderNameServiceName()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderName("Business A"),
                    InquiryServices.CreateServiceName("Service 1")),
                ShimUddiSiteDiscovery);
        }

        /// <summary>
        /// Find access point for service: service name, endpoint.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceServiceNameEndpoint()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateServiceName("Service 1"),
                    AccessPointUseType.EndPoint),
                ShimUddiSiteDiscovery);
        }

        /// <summary>
        /// Find access point for service: service name, hosting redirector.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceServiceNameHostingRedirector()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateServiceName("Service 1"),
                    AccessPointUseType.HostingRedirector),
                ShimUddiSiteDiscovery);
        }

        /// <summary>
        /// Find access point for service: provider name, service name, endpoint.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderNameServiceNameEndpoint()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderName("Business A"),
                    InquiryServices.CreateServiceName("Service 1"),
                    AccessPointUseType.EndPoint));
        }

        /// <summary>
        /// Find access point for service: provider name, service key, endpoint.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderNameServiceKeyEndpoint()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderName("Business A"),
                    InquiryServices.CreateServiceKey("uddi:service1"),
                    AccessPointUseType.EndPoint));
        }

        /// <summary>
        /// Find access point for service: provider key, service key, endpoint.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyServiceKeyEndpoint()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderKey("uddi:businessa"),
                    InquiryServices.CreateServiceKey("uddi:service1"),
                    AccessPointUseType.EndPoint));
        }

        /// <summary>
        /// Find access point for service: provider name, service name, WSDL deployment.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderNameServiceNameWsdlDeployment()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderName("Business A"),
                    InquiryServices.CreateServiceName("Service 1"),
                    AccessPointUseType.WsdlDeployment));
        }

        /// <summary>
        /// Find access point for service: bad service key, endpoint.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceServiceBadKeyEndpoint()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateServiceKey("Service_1_BAD"),
                    AccessPointUseType.EndPoint));
        }

        /// <summary>
        /// Find access point for service: bad service name, hosting redirector.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceServiceBadNameHostingRedirector()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateServiceName("Service_1_BAD"),
                    AccessPointUseType.HostingRedirector));
        }

        /// <summary>
        /// Find access point for service: bad service name, endpoint.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceServiceBadNameEndpoint()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateServiceName("Service_1_BAD"),
                    AccessPointUseType.EndPoint));
        }

        /// <summary>
        /// Find access point for service: bad provider key, bad service name.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderBadKeyServiceBadName()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderKey("Business_A_BAD"),
                    InquiryServices.CreateServiceName("Service_1_BAD")));
        }

        /// <summary>
        /// Find access point for service: bad provider name, bad service key.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderBadNameServiceBadKey()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderName("Business_A_BAD"),
                    InquiryServices.CreateServiceKey("Service_1_BAD")));
        }

        /// <summary>
        /// Find access point for service: bad provider key, bad service key.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderBadKeyServiceBadKey()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderKey("Business_A_BAD"),
                    InquiryServices.CreateServiceKey("Service_1_BAD")));
        }

        /// <summary>
        /// Find access point for service: bad provider name, bad service name.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderBadNameServiceBadName()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderName("Business_A_BAD"),
                    InquiryServices.CreateServiceName("Service_1_BAD")));
        }

        /// <summary>
        /// Find binding template.
        /// </summary>
        [TestMethod]
        public void FindBindingTemplate()
        {
            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForBinding("uddi:binding1");
                        InquiryServices.FindAccessPointForBinding("uddi:non.existent.binding_BAD");
                    },
                ShimUddiSiteDiscovery);
        }

        /// <summary>
        /// Test the FindAccessPointForBinding method.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForBinding()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "/somelocation/inquire.asmx",
                    InquiryServices.FindAccessPointForBinding("uddi:binding1")));
        }

        /// <summary>
        /// Test the FindAccessPointForBinding method with a null value.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForBindingNull()
        {
            Action<ArgumentException> errorHandler =
                argumentException =>
                    Assert.AreEqual(
                        "Binding key is invalid or missing.\r\nParameter name: bindingKey",
                        argumentException.Message);

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForBinding(null);
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler);
        }

        /// <summary>
        /// Test SetBaseUrl: null, null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlNullNull()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", InquiryServices.SetAsBaseUrlOn(null, null)));
        }

        /// <summary>
        /// Test SetBaseUrl: string.Empty, null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlEmptyStringNull()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", string.Empty.SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test SetBaseUrl: "\0", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlX0Null()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", "\0".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test SetBaseUrl: ":", ":"
        /// </summary>
        [TestMethod]
        public void SetBaseUrlColonColon()
        {
            // This test will fail in certain scenarios.  System.Uri in .NET 4.5 has been changed
            // to improve standards compliance.  It has a quirks mode which can be switched on
            // depending on an interplay with the CLR.  In this mode, System.Uri will incorrectly
            // decide that ":" is a valid relative URI and return 'http://localhost/:'  Specifically,
            // when testing under the ReSharper unit test tools, this problem may occur.
            RunTest(
                () => Assert.AreEqual("http://localhost/", ":".SetAsBaseUrlOn(":")));
        }

        /// <summary>
        /// Test SetBaseUrl: "/", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlFdwslashNull()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", "/".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test SetBaseUrl: ".", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlDotNull()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", ".".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test SetBaseUrl: "\u0100", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlU0100Null()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", "\u0100".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test SetBaseUrl: "\0\0", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlx0X0Null()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", "\0\0".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test SetBaseUrl: "\\\\%", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlBackslashBackslashPercentNull()
        {
            RunTest(() => "\\\\%".SetAsBaseUrlOn(null));
        }

        /// <summary>
        /// Test SetBaseUrl: "\\\\/", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlBackslashBackslashFwdslashNull()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", "\\\\/".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test SetBaseUrl: "~", "~"
        /// </summary>
        [TestMethod]
        public void SetBaseUrlTildeTilde()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/~", "~".SetAsBaseUrlOn("~")));
        }

        /// <summary>
        /// Test SetBaseUrl: "~", "?"
        /// </summary>
        [TestMethod]
        public void SetBaseUrlTildeIterog()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/~?", "~".SetAsBaseUrlOn("?")));
        }

        /// <summary>
        /// Test SetBaseUrl: "?", "?"
        /// </summary>
        [TestMethod]
        public void SetBaseUrlInetrogInterog()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/?", "?".SetAsBaseUrlOn("?")));
        }

        /// <summary>
        /// Test SetBaseUrl: "\u0001\0:", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlU0001X0Null()
        {
            RunTest(
                () => Assert.AreEqual("http://localhost/", "\u0001\0:".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test SetBaseUrl: "x7:", null
        /// </summary>
        [TestMethod]
        public void SetBaseUrlX7Null()
        {
            RunTest(() => Assert.AreEqual("x7:", "x7:".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Test FindAccessPointForBinding: empty string.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForBindingEmptyString()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForBinding(string.Empty);
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                    ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForBinding: x0.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForBindingX0()
        {
            RunTest(() => InquiryServices.FindAccessPointForBinding("\0"));
        }

        /// <summary>
        /// Test FindAccessPointForBinding: u0100.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForBindingU0100()
        {
            RunTest(() => InquiryServices.FindAccessPointForBinding("\u0100"));
        }

        /// <summary>
        /// Test FindAccessPointForBinding: x0 x0.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForBindingX0X0()
        {
            RunTest(() => InquiryServices.FindAccessPointForBinding("\0\0"));
        }

        /// <summary>
        /// Test FindAccessPointForBinding: u0100 u0100.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForBindingU0100U0100()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForBinding("\u0100\u0100"));
        }

        /// <summary>
        /// Test FindAccessPointForService: null, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceNullUnspecified()
        {
            Action<ArgumentNullException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(null, AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentNullException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service key null, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceKeyNullUnspecified()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateServiceKey(null),
                            AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service key empty string, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceKeyEmptyStringUnspecified()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateServiceKey(string.Empty),
                            AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service key x0, unspecified.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceKeyX0Unspecified()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateServiceKey("\0"),
                    AccessPointUseType.Unspecified));
        }

        /// <summary>
        /// Test FindAccessPointForService: service name x0, unspecified.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceNameX0Unspecified()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateServiceName("\0"),
                    AccessPointUseType.Unspecified));
        }

        /// <summary>
        /// Test FindAccessPointForService: service key u0100, unspecified.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceKeyU0100Unspecified()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                        InquiryServices.CreateServiceKey("\u0100"),
                        AccessPointUseType.Unspecified));
        }

        /// <summary>
        /// Test FindAccessPointForService: service key x0 x0, unspecified.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceKeyX0X0Unspecified()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                        InquiryServices.CreateServiceKey("\0\0"),
                        AccessPointUseType.Unspecified));
        }

        /// <summary>
        /// Test FindAccessPointForService: service key u0200 u0200 unspecified.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceKeyU0200U0200Unspecified()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                        InquiryServices.CreateServiceKey("\u0200\u0200"),
                        AccessPointUseType.Unspecified));
        }

        /// <summary>
        /// Test FindAccessPointForService: null, null.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceNullNull()
        {
            Action<ArgumentNullException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(null, null);
                        Assert.Fail("expected an exception of type ArgumentNullException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key null, null.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForProviderKeyNullNull()
        {
            Action<ArgumentNullException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(new ProviderIdentifier(null, true), null);
                        Assert.Fail("expected an exception of type ArgumentNullException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key null, service key null.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyNullServiceKeyNull()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateProviderKey(null),
                            InquiryServices.CreateServiceKey(null));
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key empty string, service key null.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyEmptyStringServiceKeyNull()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateProviderKey(string.Empty),
                            InquiryServices.CreateServiceKey(null));
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key x0, service key null.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyX0ServiceKeyNull()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                {
                    InquiryServices.FindAccessPointForService(
                        InquiryServices.CreateProviderKey("\0"),
                        InquiryServices.CreateServiceKey(null));
                    Assert.Fail("expected an exception of type ArgumentException");
                },
                errorHandler,
                () => ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key u0100, service key null.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyU0100ServiceKeyNull()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                {
                    InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateProviderKey("\u0100"),
                            InquiryServices.CreateServiceKey(null));
                    Assert.Fail("expected an exception of type ArgumentException");
                },
                errorHandler,
                () => ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key u0100, service key u0100.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyU0100ServiceKeyU0100()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                        InquiryServices.CreateProviderKey("\u0100"),
                        InquiryServices.CreateServiceKey("\u0100")));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider name u0100, service key u0100.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderNameU0100ServiceKeyU0100()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                    InquiryServices.CreateProviderName("\u0100"),
                    InquiryServices.CreateServiceKey("\u0100")));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key x0 x0, service key null.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyX0X0ServiceKeyNull()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateProviderKey("\0\0"),
                            InquiryServices.CreateServiceKey(null));
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key null, null.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyNullNull()
        {
            Action<ArgumentNullException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(new ProviderIdentifier(null, true), null);
                        Assert.Fail("expected an exception of type ArgumentNullException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: null, null, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceNullNullUnspecified()
        {
            Action<ArgumentNullException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(null, null, AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentNullException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key null, null, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyNullNullUnspecified()
        {
            Action<ArgumentNullException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            new ProviderIdentifier(null, true),
                            null,
                            AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentNullException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key null, service key null, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyNullServiceKeyNullUnspecified()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateProviderKey(null),
                            InquiryServices.CreateServiceKey(null),
                            AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key empty string, service key null, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyEmptyStringServiceKeyNullUnspecified()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateProviderKey(string.Empty),
                            InquiryServices.CreateServiceKey(null),
                            AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key x0, service key null, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyX0ServiceKeyNullUnspecified()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateProviderKey("\0"),
                            InquiryServices.CreateServiceKey(null),
                            AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key u0400, service key null, unspecified.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// An ApplicationException.
        /// </exception>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyU0400ServiceKeyNullUnspecified()
        {
            Action<ArgumentException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateProviderKey("\u0400"),
                            InquiryServices.CreateServiceKey(null),
                            AccessPointUseType.Unspecified);
                        Assert.Fail("expected an exception of type ArgumentException");
                    },
                errorHandler,
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost"));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider key u0800, service key u0800, unspecified.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderKeyU0800ServiceKeyU0800Unspecified()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                        InquiryServices.CreateProviderKey("\u0800"),
                        InquiryServices.CreateServiceKey("\u0800"),
                        AccessPointUseType.Unspecified));
        }

        /// <summary>
        /// Test FindAccessPointForService: service provider name u0800, service key u0800, unspecified.
        /// </summary>
        [TestMethod]
        public void FindAccessPointForServiceProviderNameU0800ServiceKeyU0800Unspecified()
        {
            RunTest(
                () =>
                InquiryServices.FindAccessPointForService(
                        InquiryServices.CreateProviderName("\u0800"),
                        InquiryServices.CreateServiceKey("\u0800"), 
                        AccessPointUseType.Unspecified));
        }

        /// <summary>
        /// Test the provider identifier default constructor.
        /// </summary>
        [TestMethod]
        public void ProviderIdentifierDefaultConstructor()
        {
            RunTest(
                () =>
                    {
                        var providerIdentifier = new ProviderIdentifier();
                        Assert.IsNotNull(providerIdentifier);
                        Assert.AreEqual(string.Empty, providerIdentifier.Value);
                        Assert.AreEqual(false, providerIdentifier.IsKey);
                    });
        }

        /// <summary>
        /// Test the provider identifier constructor: value, isKey.
        /// </summary>
        [TestMethod]
        public void ProviderIdentifierConstructorValueIsKey()
        {
            RunTest(
                () =>
                    {
                        var providerIdentifier = new ProviderIdentifier(null, false);
                        Assert.IsNotNull(providerIdentifier);
                        Assert.AreEqual(null, providerIdentifier.Value);
                        Assert.AreEqual(false, providerIdentifier.IsKey);
                    });
        }

        /// <summary>
        /// Test the service identifier default constructor.
        /// </summary>
        [TestMethod]
        public void ServiceIdentifierDefaultConstructor()
        {
            RunTest(
                () =>
                    {
                        var serviceIdentifier = new ServiceIdentifier();
                        Assert.IsNotNull(serviceIdentifier);
                        Assert.AreEqual(string.Empty, serviceIdentifier.Value);
                        Assert.AreEqual(false, serviceIdentifier.IsKey);
                    });
        }

        /// <summary>
        /// Test the service identifier constructor: value, isKey.
        /// </summary>
        [TestMethod]
        public void ServiceIdentifierConstructorValueIsKey()
        {
            RunTest(
                () =>
                    {
                        var serviceIdentifier = new ServiceIdentifier(null, false);
                        Assert.IsNotNull(serviceIdentifier);
                        Assert.AreEqual(null, serviceIdentifier.Value);
                        Assert.AreEqual(false, serviceIdentifier.IsKey);
                    });
        }

        /// <summary>
        /// Test the EsbUddiInstaller default constructor.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [TestMethod]
        public void EsbUddiInstallerDefaultConstructor()
        {
            RunTest(
                () =>
                    {
                        var esbUddiInstaller = new EsbUddiInstaller();
                        esbUddiInstaller.Dispose();
                        Assert.IsNotNull(esbUddiInstaller);
                        Assert.IsNull(esbUddiInstaller.Context);
                        Assert.IsNull(esbUddiInstaller.Parent);
                    });
        }

        /// <summary>
        /// Tests for an access point without searching for registered directories in AD.
        /// </summary>
        [TestMethod]
        public void EsbUddiInstallerDispose()
        {
            RunTest(
                () =>
                    {
                        var esbUddiInstaller = new EsbUddiInstaller();
                        var privateObject = new PrivateObject(esbUddiInstaller);
                        privateObject.SetField("components", new DummyInstallerComponents());

                        privateObject.Invoke("Dispose", true);
                        Assert.IsNotNull(esbUddiInstaller);
                        Assert.IsNull(esbUddiInstaller.Context);
                        Assert.IsNull(esbUddiInstaller.Parent);
                    });
        }

        /// <summary>
        /// Tests the UddiEventLog WriteUddiError method with various combinations of parameters.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void UddiEventLog()
        {
            RunTest(
                () =>
                    {
                        var privateObject =
                            new PrivateObject(
                                "SolidsoftReply.Esb.Libraries.Uddi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7bd6faf29a9873a1",
                                "SolidsoftReply.Esb.Libraries.Uddi.UddiEventLog");
                        privateObject.Invoke(
                            "WriteUddiError",
                            string.Empty,
                            null,
                            string.Empty,
                            null,
                            new[] { "aaaa", "bbbb" });
                        privateObject.Invoke(
                            "WriteUddiError",
                            string.Empty,
                            null,
                            string.Empty,
                            null,
                            new[] { string.Empty, null });
                        privateObject.Invoke(
                            "WriteUddiError",
                            string.Empty,
                            null,
                            string.Empty,
                            null,
                            new[] { "aaaa", string.Empty, "bbbb" });
                        privateObject.Invoke("WriteUddiError", string.Empty, null, string.Empty, null, new string[0]);
                        privateObject.Invoke(
                            "WriteUddiError",
                            string.Empty,
                            null,
                            string.Empty,
                            null,
                            new[] { "aaaa", "bbbb" });
                        privateObject.Invoke(
                            "WriteUddiError",
                            "This is a test message",
                            new Exception("This is a test exception"),
                            "test request",
                            (UddiSiteLocation)(new ShimUddiSiteLocation()),
                            new[] { "aaaa", "bbbb" }); 
                        privateObject.Invoke(
                            "WriteUddiError",
                            string.Empty,
                            null,
                            string.Empty,
                            (UddiSiteLocation)(new StubUddiSiteLocation("aaaa", "bbbb", "cccc", "dddd", AuthenticationMode.WindowsAuthentication)),
                            new[] { "aaaa", "bbbb" });
                    });
        }

        /// <summary>
        /// Tests for an access point without searching for registered directories in AD.
        /// </summary>
        [TestMethod]
        public void NoDirectoryDiscovery()
        {
            RunTest(
                () => new PrivateType(typeof(SiteLocationCache)).InvokeStatic("DiscoverInquiryServices"),
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "false", "1", "localhost"));
        }

        /// <summary>
        /// Tests for an access point without searching for registered directories in AD.  No configuration
        /// setting provided.
        /// </summary>
        [TestMethod]
        public void NoDirectoryDiscoveryNoConfigSetting()
        {
            RunTest(
                () => new PrivateType(typeof(SiteLocationCache)).InvokeStatic("DiscoverInquiryServices"),
                () => ShimConfigurationManagerAppSettings(TestUddiInquireUrl, null, "1", "localhost"));
        }

        /// <summary>
        /// Tests the simulation of a cache entry removal for an expired item
        /// </summary>
        [TestMethod]
        public void CacheRemovalExpired()
        {
            RunTestCacheRemoval(CacheEntryRemovedReason.Expired);
        }

        /// <summary>
        /// Tests the simulation of a cache entry removal for an evicted item
        /// </summary>
        [TestMethod]
        public void CacheRemovalEvicted()
        {
            RunTestCacheRemoval(CacheEntryRemovedReason.Evicted);
        }

        /// <summary>
        /// Tests the simulation of a cache entry removal for a removed item
        /// </summary>
        [TestMethod]
        public void CacheRemovalRemoved()
        {
            RunTestCacheRemoval(CacheEntryRemovedReason.Removed);
        }

        /// <summary>
        /// Tests the simulation of a cache entry removal for an cache-specific evicted item
        /// </summary>
        [TestMethod]
        public void CacheRemovalCacheSpecificEviction()
        {
            RunTestCacheRemoval(CacheEntryRemovedReason.CacheSpecificEviction);
        }

        /// <summary>
        /// Tests the simulation of a cache entry removal for change in a monitored resource item
        /// </summary>
        [TestMethod]
        public void CacheRemovalChangeMonitorChanged()
        {
            RunTestCacheRemoval(CacheEntryRemovedReason.ChangeMonitorChanged);
        }

        /// <summary>
        /// Tests the Inquiry Services initializer with a null URL.
        /// </summary>
        [TestMethod]
        public void GetUriAppSettingWithNullUri()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                ShimConfigurationManagerNoAppSettings);
        }

        /// <summary>
        /// Tests the Inquiry Services initializer when a configuration error occurs.
        /// </summary>
        [TestMethod]
        public void GetUriAppSettingWithError()
        {
            Action<Exception> errorHandler = ex =>
                {
                    if (ex.InnerException.GetType() != typeof(ConfigurationErrorsException)
                        && ex.InnerException.InnerException.GetType() != typeof(ConfigurationErrorsException))
                    {
                        Assert.Fail("Expected an exception of type ConfigurationErrorsException.");
                    }
                };

            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                errorHandler,
                () =>
                    {
                        ShimConfigurationManagerAppSettings(
                            TestUddiInquireUrl,
                            "true",
                            "24",
                            "localhost");
                        ShimUddiSiteDiscovery();
                        ShimConfigurationManagerAppSettingsError();
                    });
        }
        
        /// <summary>
        /// Tests the Site Location Cache initializer with a bad default URL.
        /// </summary>
        [TestMethod]
        public void InitialializeSiteLocationCacheWithBadUrl()
        {
            Action<Exception> errorHandler =
                ex => Assert.IsInstanceOfType(ex.InnerException, typeof(ConfigurationErrorsException));

            RunTest(
                () =>
                    {
                        typeof(SiteLocationCache).TypeInitializer.Invoke(null, null);
                        Assert.Fail("expected an exception of type ConfigurationErrorsException");
                    },
                errorHandler,
                () => ShimConfigurationManagerAppSettings("http://localhost/uddi[0]/inquire.asmx", "false", "1", "localhost"));
        }

        /// <summary>
        /// Tests the Site Location Cache initializer with a UDDIExpireDiscoveredSitesAfterHrs setting of string.Empty.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void InitialializeSiteLocationCacheWithExpiresAfterEmpty()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                () =>
                ShimConfigurationManagerAppSettings(
                    TestUddiInquireUrl,
                    "true",
                    string.Empty,
                    "localhost"));
        }

        /// <summary>
        /// Tests the Inquiry Services initializer with a UDDIExpireDiscoveredSitesAfterHrs setting of 'X'.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void InitialializeSiteLocationCacheWithExpiresAfterX()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "X", "localhost"));
        }

        /// <summary>
        /// Tests the Site Location Cache initializer with a UDDIExpireDiscoveredSitesAfterHrs setting that will overflow.
        /// </summary>
        [TestMethod]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public void InitialializeSiteLocationCacheWithExpiresAfterOverflow()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                () =>
                ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "1" + decimal.MaxValue, "localhost"));
        }

        /// <summary>
        /// Tests the Site Location Cache initializer with a bad default URL.
        /// </summary>
        [TestMethod]
        public void InitialializeSiteLocationCacheWithRelativeUrl()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                () => ShimConfigurationManagerAppSettings("/uddi/inquire.asmx", "false", "1", "localhost"));
        }

        /// <summary>
        /// Tests the Inquiry Services initializer with a relative URL and no host.
        /// </summary>
        [TestMethod]
        public void InitialializeSiteLocationCacheWithRelativeUrlNoHost()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                () => ShimConfigurationManagerAppSettings("/uddi/inquire.asmx", "false", "1", string.Empty));
        }

        /// <summary>
        /// Tests the Site Location Cache initializer with a relative URL and absolute host.
        /// </summary>
        [TestMethod]
        public void InitialializeSiteLocationCacheWithRelativeUrlAbsoluteHost()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                () => ShimConfigurationManagerAppSettings("/uddi/inquire.asmx", "false", "1", "http://localhost"));
        }

        /// <summary>
        /// Tests the Inquiry Services initializer with a relative URL and relative host.
        /// </summary>
        [TestMethod]
        public void InitialializeSiteLocationCacheWithRelativeUrlRelativeHost()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                () => ShimConfigurationManagerAppSettings("/uddi/inquire.asmx", "false", "1", "localhost"));
        }
        
        /// <summary>
        /// Tests the Site Location Cache initializer with a bad default URL.
        /// </summary>
        [TestMethod]
        public void InitialializeSiteLocationCacheWithNoDefaultUrl()
        {
            RunTest(
                () => typeof(SiteLocationCache).TypeInitializer.Invoke(null, null),
                () => ShimConfigurationManagerAppSettings(string.Empty, "false", "1", "localhost"));
        }

        /// <summary>
        /// Tests the FindService.Send method with an ArgumentNull exception.
        /// </summary>
        [TestMethod]
        public void FindServiceWithExceptionArgumentNullException()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForService(InquiryServices.CreateServiceName("Service 1")),
                ShimFindServiceSendError<ArgumentNullException>);
        }

        /// <summary>
        /// Tests the FindService.Send method with an Argument exception.
        /// </summary>
        [TestMethod]
        public void FindServiceWithExceptionArgumentException()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForService(
                                    InquiryServices.CreateServiceName("Service 1")),
                ShimFindServiceSendError<ArgumentException>);
        }

        /// <summary>
        /// Tests the FindService.Send method with an InvalidOperation exception.
        /// </summary>
        [TestMethod]
        public void FindServiceWithExceptionInvalidOperationException()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForService(
                                    InquiryServices.CreateServiceName("Service 1")),
                ShimFindServiceSendError<InvalidOperationException>);
        }

        /// <summary>
        /// Tests the FindService.Send method with an InvalidKeyPassed exception.
        /// </summary>
        [TestMethod]
        public void FindServiceWithExceptionInvalidKeyPassedException()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForService(
                                    InquiryServices.CreateServiceName("Service 1")),
                ShimFindServiceSendError<InvalidKeyPassedException>);
        }

        /// <summary>
        /// Tests the FindService.Send method with an UnknownError exception.
        /// </summary>
        [TestMethod]
        public void FindServiceWithExceptionUnknownErrorException()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForService(
                                    InquiryServices.CreateServiceName("Service 1")),
                ShimFindServiceSendError<UnknownErrorException>);
        }

        /// <summary>
        /// Tests the FindService.Send method with a UDDI exception.
        /// </summary>
        [TestMethod]
        public void FindServiceWithExceptionUddiException()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForService(
                                    InquiryServices.CreateServiceName("Service 1")),
                ShimFindServiceSendError<BusyException>);
        }

        /// <summary>
        /// Tests the FindService.Send method with an unexpected exception.
        /// </summary>
        [TestMethod]
        public void FindServiceWithExceptionApplicationException()
        {
            RunTest(
                () => InquiryServices.FindAccessPointForService(
                                    InquiryServices.CreateServiceName("Service 1")),
                ShimFindServiceSendError<ApplicationException>);
        }

        /// <summary>
        /// Log a warning with an ArgumentException.
        /// </summary>
        [TestMethod]
        public void LogWarningWithError()
        {
            RunTest(
                () => "http://localhost[/0]".SetAsBaseUrlOn("/inquiry.asmx"),
                () => ShimLogWarningError<ArgumentException>());
        }

        /// <summary>
        /// Log a warning with an ArgumentException.  Skip an error for coverage purposes
        /// </summary>
        [TestMethod]
        public void LogWarningWithErrorSkipOne()
        {
            RunTest(
                () => "http://localhost[/0]".SetAsBaseUrlOn("/inquiry.asmx"),
                () => ShimLogWarningError<ArgumentException>(1));
        }

        /// <summary>
        /// Log a warning with an ArgumentException.  Skip three for
        /// coverage purposes.
        /// </summary>
        [TestMethod]
        public void LogWarningWithErrorSkipThree()
        {
            RunTest(
                () => "http://localhost[/0]".SetAsBaseUrlOn("/inquiry.asmx"),
                () => ShimLogWarningError<ArgumentException>(3));
        }

        /// <summary>
        /// Log a warning with an ArgumentException for the scenario where the code 
        /// uses an Application source.
        /// </summary>
        [TestMethod]
        public void LogWarningWithErrorForApplicationSource()
        {
            RunTest(
                () => "http://localhost[/0]".SetAsBaseUrlOn("/inquiry.asmx"),
                ShimLogWarningErrorExceptApplicationSource<ArgumentException>);
        }

        /// <summary>
        /// Log a warning with an an unexpected ApplicationException.
        /// </summary>
        [TestMethod]
        public void LogWarningWithUnexpectedError()
        {
            RunTest(
                () => "http://localhost[/0]".SetAsBaseUrlOn("/inquiry.asmx"),
                () => ShimLogWarningError<ApplicationException>());
        }

        /// <summary>
        /// Log a warning with an an unexpected ApplicationException, but log with 'Application' source
        /// </summary>
        [TestMethod]
        public void LogWarningWithUnexpectedErrorAcceptApplicatioSource()
        {
            RunTest(
                () => "http://localhost[/0]".SetAsBaseUrlOn("/inquiry.asmx"),
                ShimLogWarningErrorExceptApplicationSource<ApplicationException>);
        }

        /// <summary>
        /// Log a warning with a 64 bit simulation of Environment.Is64BitOperatingSystem.
        /// </summary>
        [TestMethod]
        public void LogWarningFor64BitOperatingSystemIs64Bit()
        {
            Action<Exception> errorHandler =
                ex => Assert.IsInstanceOfType(ex.InnerException, typeof(ConfigurationErrorsException));

            RunTest(
                () =>
                    {
                        typeof(SiteLocationCache).TypeInitializer.Invoke(null, null);
                        Assert.Fail("expected an exception");
                    },
                errorHandler,
                () =>
                    {
                        ShimEnvironmentIs64BitOperatingSystemGet(true);
                        ShimConfigurationManagerAppSettingsError();
                    });
        }

        /// <summary>
        /// Log a warning with a 32 bit simulation of Environment.Is64BitOperatingSystem.
        /// </summary>
        [TestMethod]
        public void LogWarningFor64BitOperatingSystemIs32Bit()
        {
            Action<Exception> errorHandler =
                ex => Assert.IsInstanceOfType(ex.InnerException, typeof(ConfigurationErrorsException));

            RunTest(
                () =>
                {
                    typeof(SiteLocationCache).TypeInitializer.Invoke(null, null);
                    Assert.Fail("expected an exception");
                },
                errorHandler,
                () =>
                {
                    ShimEnvironmentIs64BitOperatingSystemGet(false);
                    ShimConfigurationManagerAppSettingsError();
                });
        }

        /// <summary>
        /// Log a warning with a 64 bit simulation of Environment.Is64BitProcess.
        /// </summary>
        [TestMethod]
        public void LogWarningFor64BitProcessIs64Bit()
        {
            Action<Exception> errorHandler =
                ex => Assert.IsInstanceOfType(ex.InnerException, typeof(ConfigurationErrorsException));

            RunTest(
                () =>
                {
                    typeof(SiteLocationCache).TypeInitializer.Invoke(null, null);
                    Assert.Fail("expected an exception");
                },
                errorHandler,
                () =>
                {
                    ShimEnvironmentIs64BitProcessGet(true);
                    ShimConfigurationManagerAppSettingsError();
                });
        }

        /// <summary>
        /// Log a warning with a 32 bit simulation of Environment.Is64BitProcess.
        /// </summary>
        [TestMethod]
        public void LogWarningFor64BitProcessIs32Bit()
        {
            Action<Exception> errorHandler =
                ex => Assert.IsInstanceOfType(ex.InnerException, typeof(ConfigurationErrorsException));

            RunTest(
                () =>
                {
                    typeof(SiteLocationCache).TypeInitializer.Invoke(null, null);
                    Assert.Fail("expected an exception");
                },
                errorHandler,
                () =>
                {
                    ShimEnvironmentIs64BitProcessGet(false);
                    ShimConfigurationManagerAppSettingsError();
                });
        }

        /// <summary>
        /// Set the base URL: retrieved access point, absolute base.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",
            Justification = "Reviewed. Suppression is OK here.")]
        [TestMethod]
        public void SetBaseAuthorityOnLookup()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/somelocation/inquire.asmx",
                    "http://acme.com".SetAsBaseUrlOn(
                        InquiryServices.FindAccessPointForService(
                            InquiryServices.CreateServiceName("Service 1")))));
        }

        /// <summary>
        /// Set the base URL: absolute url, absolute base.
        /// </summary>
        [TestMethod]
        public void SetBaseAuthorityOnAuthority()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "http://acme.com".SetAsBaseUrlOn("http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: relative url, absolute base.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here."), TestMethod]
        public void SetBaseAuthorityOnRelative()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "http://acme.com".SetAsBaseUrlOn("/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: relative url with no leading delimiter, absolute base.
        /// </summary>
        [TestMethod]
        public void SetBaseAuthorityOnRelativeNoLeadingDelim()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "http://acme.com".SetAsBaseUrlOn("myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: absolute url, host base.
        /// </summary>
        [TestMethod]
        public void SetBaseHostOnAbsolute()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "acme.com".SetAsBaseUrlOn("http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: relative url, host base.
        /// </summary>
        [TestMethod]
        public void SetBaseHostOnRelative()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "acme.com".SetAsBaseUrlOn("/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: relative url with no leading delimiter, host base
        /// </summary>
        [TestMethod]
        public void SetBaseHostOnRelativeNoLeadingDelim()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "acme.com".SetAsBaseUrlOn("myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: absolute url, null base.
        /// </summary>
        [TestMethod]
        public void SetAbsoluteUrlWithNullBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234",
                    InquiryServices.SetAsBaseUrlOn(null, "http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: absolute url, empty string base.
        /// </summary>
        [TestMethod]
        public void SetAbsoluteUrlWithEmptyStringBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234",
                    string.Empty.SetAsBaseUrlOn("http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: absolute url, whitespace base.
        /// </summary>
        [TestMethod]
        public void SetAbsoluteUrlWithWhitespaceBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "    ".SetAsBaseUrlOn("http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: relative url, null base.
        /// </summary>
        [TestMethod]
        public void SetRelativeUrlWithNullBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234",
                    InquiryServices.SetAsBaseUrlOn(null, "/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: relative url, empty string base.
        /// </summary>
        [TestMethod]
        public void SetRelativeUrlWithEmptyStringBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234",
                    string.Empty.SetAsBaseUrlOn("/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: relative url, whitespace base.
        /// </summary>
        [TestMethod]
        public void SetRelativeUrlWithWhitespaceBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "    ".SetAsBaseUrlOn("/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: null url, null base.
        /// </summary>
        [TestMethod]
        public void SetNullUrlWithNullBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", InquiryServices.SetAsBaseUrlOn(null, null)));
        }

        /// <summary>
        /// Set the base URL: null url, empty string base.
        /// </summary>
        [TestMethod]
        public void SetNullUrlWithEmptyStringBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", string.Empty.SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Set the base URL: null url, whitespace base.
        /// </summary>
        [TestMethod]
        public void SetNullUrlWithWhitespaceBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", "    ".SetAsBaseUrlOn(null)));
        }

        /// <summary>
        /// Set the base URL: empty string url, null base.
        /// </summary>
        [TestMethod]
        public void SetEmptyStringUrlWithNullBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", InquiryServices.SetAsBaseUrlOn(null, string.Empty)));
        }

        /// <summary>
        /// Set the base URL: empty string url, empty string base.
        /// </summary>
        [TestMethod]
        public void SetEmptyStringUrlWithEmptyStringBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", string.Empty.SetAsBaseUrlOn(string.Empty)));
        }

        /// <summary>
        /// Set the base URL: empty string url, whitespace base.
        /// </summary>
        [TestMethod]
        public void SetEmptyStringUrlWithWhitespaceBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", "    ".SetAsBaseUrlOn(string.Empty)));
        }

        /// <summary>
        /// Set the base URL: whitespace url, null base.
        /// </summary>
        [TestMethod]
        public void SetWhitespaceUrlWithNullBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", InquiryServices.SetAsBaseUrlOn(null, "    ")));
        }

        /// <summary>
        /// Set the base URL: whitespace url, empty string base.
        /// </summary>
        [TestMethod]
        public void SetWhitespaceUrlWithEmptyStringBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", string.Empty.SetAsBaseUrlOn("    ")));
        }

        /// <summary>
        /// Set the base URL: whitespace url, whitespace base.
        /// </summary>
        [TestMethod]
        public void SetWhitespaceUrlWithWhitespaceBase()
        {
            RunTest(() => Assert.AreEqual("http://localhost/", "    ".SetAsBaseUrlOn("    ")));
        }

        /// <summary>
        /// Set the base URL: absolute url, bad base.
        /// </summary>
        [TestMethod]
        public void SetAbsoluteUrlWithBadBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234",
                    "http://acme.com[/].uk".SetAsBaseUrlOn("http://localhost/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: bad authority, absolute base.
        /// </summary>
        [TestMethod]
        public void SetBadAuthorityUrlWithAbsoluteBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/",
                    "http://acme.com".SetAsBaseUrlOn("http://localhost[/].uk/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: bad segment, absolute base.
        /// </summary>
        [TestMethod]
        public void SetBadSegmentUrlWithAbsoluteBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://acme.com/",
                    "http://acme.com".SetAsBaseUrlOn("http://localhost/myservice[/].svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: bad authority, bad base.
        /// </summary>
        [TestMethod]
        public void SetBadAuthorityUrlWithBadBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/",
                    "http://acme.com[/].uk".SetAsBaseUrlOn("http://localhost[/].uk/myservice.svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Set the base URL: bad segment, bad base.
        /// </summary>
        [TestMethod]
        public void SetBadSegmentUrlWithBadBase()
        {
            RunTest(
                () =>
                Assert.AreEqual(
                    "http://localhost/",
                    "http://acme.com[/].uk".SetAsBaseUrlOn("http://localhost/myservice[/].svc?gfhdg=657;yeriuyt=35234")));
        }

        /// <summary>
        /// Invokes the GetEnumerator method of the Site Location cache for coverage purposes.
        /// </summary>
        [TestMethod]
        public void GetEnumeratorFroSiteLocationCache()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            RunTest(() => ((IEnumerable)(new SiteLocationCache())).GetEnumerator());
        }

        /// <summary>
        /// Initializes the InquiryService class with default settings.
        /// </summary>
        private static void ShimInitialializeInquireServices()
        {
            ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost");
            ShimUddiSiteDiscovery();
            ShimFindService();
            ShimFindBusiness();
            ShimGetBusinessDetail();
            ShimGetBindingDetail();
            ShimGetServiceDetail();
            typeof(SiteLocationCache).TypeInitializer.Invoke(null, null);
        }

        /// <summary>
        /// Creates a shim for UddiSiteDiscovery.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static void ShimUddiSiteDiscovery()
        {
            Microsoft.Uddi3.Extensions.Fakes.ShimUddiSiteDiscovery.FindUddiSiteUrlTypeAuthenticationMode =
                (uddiSiteUrlType, authenticationMode) =>
                new UddiSiteLocation[]
                        {
                            new StubUddiSiteLocation(
                                "http://someserver/uddi/inquire.asmx",
                                "http//someserver/uddi/publish.asmx",
                                "A fake UDDI server registered in AD"),
                            new StubUddiSiteLocation(
                                "https://anotherserver/servicerirectoryuddi3/uddigetfind.svc",
                                "A second fake UDDI server registered in AD"),
                            new StubUddiSiteLocation(
                                "http://localhost/uddi/inquire.asmx",
                                "http//loclahost/uddi/publish.asmx",
                                "A fake of the local directory, registered in AD"),
                            new StubUddiSiteLocation(
                                "http://localhost/uddi/inquire.asmx",
                                "http//loclahost/uddi/publish.asmx",
                                "A fake of a duplicate local directory, registered in AD"),
                            new StubUddiSiteLocation(
                                "http://localhost[\0]/uddi/inquire.asmx",
                                "http//loclahost/uddi/publish.asmx",
                                "A fake of the local directory, registered in AD"),
                            new StubUddiSiteLocation(
                                "/comegetaservice?inquire=1",
                                @"/comegetaservice?publish=1",
                                "A fake directory with relative URLs"),
                            new StubUddiSiteLocation(
                                string.Empty,
                                string.Empty,
                                "A fake directory with no URLs")
                        };
        }

        /// <summary>
        /// Simulates called Send on FindService.
        /// </summary>
        private static void ShimFindService()
        {
            Microsoft.Uddi3.Fakes.ShimFindService.AllInstances.SendUddiConnection =
                (findService, uddiConnection) =>
                    findService.Names[0].Text.EndsWith("_BAD")
                    ? null
                    : new StubServiceList
                            {
                                ServiceInfos =
                                    new StubServiceInfoCollection
                                        {
                                            new StubServiceInfo("uddi:service1", "uddi:businessa")
                                                {
                                                    Names = new StubNameCollection
                                                                {
                                                                    new StubName("Service 1")
                                                                }
                                                }
                                        }
                            };
        }

        /// <summary>
        /// Simulates called Send on FindBusiness.
        /// </summary>
        private static void ShimFindBusiness()
        {
            Microsoft.Uddi3.Fakes.ShimFindBusiness.AllInstances.SendUddiConnection =
                (findBusiness, uddiConnection) => 
                 findBusiness.Names[0].Text.EndsWith("_BAD")
                    ? null
                    : new StubBusinessList 
                        { 
                            BusinessInfos =
                                new StubBusinessInfoCollection
                                    {
                                        new StubBusinessInfo
                                            {
                                                BusinessKey = "uddi:businessa",
                                                Descriptions = new StubDescriptionCollection(),
                                                Names = new StubNameCollection
                                                            {
                                                                new StubName("Business A"),
                                                            },
                                                ServiceInfos = new StubServiceInfoCollection
                                                                {
                                                                        new StubServiceInfo(
                                                                            "uddi:service1",
                                                                            "uddi:businessa")
                                                                }
                                            }
                                    }
                        };
        }

        /// <summary>
        /// Simulates called Send on GetBusinessDetail.
        /// </summary>
        private static void ShimGetBusinessDetail()
        {
            Microsoft.Uddi3.Fakes.ShimGetBusinessDetail.AllInstances.SendUddiConnection =
                (getBusinessDetail, uddiConnection) =>
                 getBusinessDetail.BusinessKeys[0].EndsWith("_BAD")
                    ? null
                    : new StubBusinessDetail
                        {
                            BusinessEntities = new StubBusinessEntityCollection
                                                   {
                                                       new ShimBusinessEntity
                                                           {
                                                               //////BusinessKeyGet = () => "bussiness1",
                                                               //////BusinessKeySetString = businessKey => { },
                                                               BusinessServicesGet = () => new StubBusinessServiceCollection
                                                                                               {
                                                                                                   CreateShimBusinessService()
                                                                                               },
                                                               //////BusinessServicesSetBusinessServiceCollection = businessServiceCollection => { },
                                                               //////CategoryBagGet = () => new StubCategoryBag(new KeyedReferenceCollection
                                                               //////                                               {
                                                               //////                                                   new StubKeyedReference("uddi:tmodel1", "categoryA", "Category A"),
                                                               //////                                                   new StubKeyedReference("uddi:tmodel2", "categoryB", "Category B"),
                                                               //////                                                   new StubKeyedReference("uddi:tmodel3", "categoryC", "Category C")
                                                               //////                                               }),
                                                               //////CategoryBagSetCategoryBag = categoryBag => { },
                                                               //////ContactsGet = () => new StubContactCollection
                                                               //////                        {
                                                               //////                            new StubContact("John Smith"),
                                                               //////                            new StubContact("Alice Jones")
                                                               //////                        },
                                                               //////ContactsSetContactCollection = contactCollection => { },
                                                               //////DescriptionsGet = () => new StubDescriptionCollection(),
                                                               //////DescriptionsSetDescriptionCollection = descriptionCollection => { },
                                                               //////IdentifierBagGet = () => new StubKeyedReferenceCollection
                                                               //////                             {
                                                               //////                                 new StubKeyedReference("uddi:tmodel1", "categoryA", "Category A"),
                                                               //////                                 new StubKeyedReference("uddi:tmodel2", "categoryB", "Category B"),
                                                               //////                                 new StubKeyedReference("uddi:tmodel3", "categoryC", "Category C")
                                                               //////                             },
                                                               //////IdentifierBagSetKeyedReferenceCollection = keyedReferenceCollection => { },
                                                               //////IsEmpty = () => false,
                                                               //////NamesGet = () => new StubNameCollection
                                                               //////                     {
                                                               //////                         new StubName("Business A"),
                                                               //////                     },
                                                               ////// NamesSetNameCollection = nameCollection => { }                                                  
                                                           }
                                                   }
                        };
        }

        /// <summary>
        /// Simulates called Send on GetBindingDetail.
        /// </summary>
        private static void ShimGetBindingDetail()
        {
            Microsoft.Uddi3.Fakes.ShimGetBindingDetail.AllInstances.SendUddiConnection =
                (getBindingDetail, uddiConnection) =>
                 getBindingDetail.BindingKeys[0].EndsWith("_BAD")
                    ? null
                    : new StubBindingDetail
                            {
                                BindingTemplates = new StubBindingTemplateCollection
                                                       {
                                                           new ShimBindingTemplate
                                                               {
                                                                    AccessPointGet = () => new StubAccessPoint("/somelocation/inquire.asmx", "endPoint"),
                                                                    //////AccessPointSetAccessPoint = accessPoint => { },
                                                                    //////BindingKeyGet = () => "uddi:binding1",
                                                                    //////BindingKeySetString = bindingKey => { },
                                                                    //////CategoryBagGet = () => new StubCategoryBag(new StubKeyedReferenceCollection
                                                                    //////                                                {
                                                                    //////                                                    new StubKeyedReference("uddi:tmodel1", "categoryA", "Category A"),
                                                                    //////                                                    new StubKeyedReference("uddi:tmodel2", "categoryB", "Category B"),
                                                                    //////                                                    new StubKeyedReference("uddi:tmodel3", "categoryC", "Category C")
                                                                    //////                                                }),
                                                                    //////CategoryBagSetCategoryBag = categoryBag => { },
                                                                    //////DescriptionsGet = () => new StubDescriptionCollection(),
                                                                    //////DescriptionsSetDescriptionCollection = descriptionCollection => { },
                                                                    //////HostingRedirectorGet = () => new StubHostingRedirector("uddi:hostingRedirector1"),
                                                                    //////HostingRedirectorSetHostingRedirector = hostingRedirector => { },
                                                                    //////ServiceKeyGet = () => "uddi:service1",
                                                                    //////ServiceKeySetString = serviceKey => { },
                                                                    //////TModelInstanceInfosGet = () => new StubTModelInstanceInfoCollection
                                                                    //////                                    {
                                                                    //////                                        new StubTModelInstanceInfo(
                                                                    //////                                            "uddi:tmodel1",
                                                                    //////                                            new StubInstanceDetails
                                                                    //////                                                {
                                                                    //////                                                    Descriptions = new StubDescriptionCollection(),
                                                                    //////                                                    InstanceParameters = "some parameters",
                                                                    //////                                                    OverviewDocs = new StubOverviewDocCollection
                                                                    //////                                                                        {
                                                                    //////                                                                            new StubOverviewDoc("http://acme.com/mydoc.doc")
                                                                    //////                                                                        }
                                                                    //////                                                })
                                                                    //////                                    }
                                                               }
                                                       }
                            };
        }

        /// <summary>
        /// Simulates called Send on GetBindingDetail.
        /// </summary>
        private static void ShimGetServiceDetail()
        {
            Microsoft.Uddi3.Fakes.ShimGetServiceDetail.AllInstances.SendUddiConnection =
                (getServiceDetail, uddiConnection) =>
                getServiceDetail.ServiceKeys[0].EndsWith("_BAD")
                    ? null
                    : new StubServiceDetail
                          {
                              BusinessServices =
                                  new StubBusinessServiceCollection { CreateShimBusinessService() },
                          };
        }

        /// <summary>
        /// Creates a new shim business service for test purposes.
        /// </summary>
        /// <returns>A shim business service.</returns>
        private static ShimBusinessService CreateShimBusinessService()
        {
            return new ShimBusinessService
                    {
                        BindingTemplatesGet = () => new StubBindingTemplateCollection
                                                        {
                                                            new ShimBindingTemplate
                                                                {
                                                                    AccessPointGet = () => new StubAccessPoint("/somelocation/inquire.asmx", "endPoint"),
                                                                    //////AccessPointSetAccessPoint = accessPoint => { },
                                                                    //////BindingKeyGet = () => "uddi:binding1",
                                                                    //////BindingKeySetString = bindingKey => { },
                                                                    //////CategoryBagGet = () => new StubCategoryBag(new StubKeyedReferenceCollection
                                                                    //////                                                {
                                                                    //////                                                    new StubKeyedReference("uddi:tmodel1", "categoryA", "Category A"),
                                                                    //////                                                    new StubKeyedReference("uddi:tmodel2", "categoryB", "Category B"),
                                                                    //////                                                    new StubKeyedReference("uddi:tmodel3", "categoryC", "Category C")
                                                                    //////                                                }),
                                                                    //////CategoryBagSetCategoryBag = categoryBag => { },
                                                                    //////DescriptionsGet = () => new StubDescriptionCollection(),
                                                                    //////DescriptionsSetDescriptionCollection = descriptionCollection => { },
                                                                    //////HostingRedirectorGet = () => new StubHostingRedirector("uddi:hostingRedirector1"),
                                                                    //////HostingRedirectorSetHostingRedirector = hostingRedirector => { },
                                                                    //////ServiceKeyGet = () => "uddi:service1",
                                                                    //////ServiceKeySetString = serviceKey => { },
                                                                    //////TModelInstanceInfosGet = () => new StubTModelInstanceInfoCollection
                                                                    //////                                    {
                                                                    //////                                        new StubTModelInstanceInfo(
                                                                    //////                                            "uddi:tmodel1",
                                                                    //////                                            new StubInstanceDetails
                                                                    //////                                                {
                                                                    //////                                                    Descriptions = new StubDescriptionCollection(),
                                                                    //////                                                    InstanceParameters = "some parameters",
                                                                    //////                                                    OverviewDocs = new StubOverviewDocCollection
                                                                    //////                                                                        {
                                                                    //////                                                                            new StubOverviewDoc("http://acme.com/mydoc.doc")
                                                                    //////                                                                        }
                                                                    //////                                                })
                                                                    //////                                    }
                                                                    }
                                                        },
                        //////BindingTemplatesSetBindingTemplateCollection = bindingTemplateCollection => { },
                        //////BusinessKeyGet = () => "uddi:businessa",
                        //////BusinessKeySetString = businessKey => { },
                        //////CategoryBagGet = () => new StubCategoryBag(new KeyedReferenceCollection
                        //////                                                {
                        //////                                                    new StubKeyedReference("uddi:tmodel1", "categoryA", "Category A"),
                        //////                                                    new StubKeyedReference("uddi:tmodel2", "categoryB", "Category B"),
                        //////                                                    new StubKeyedReference("uddi:tmodel3", "categoryC", "Category C")
                        //////                                                }),
                        //////CategoryBagSetCategoryBag = categoryBag => { },
                        //////DescriptionsGet = () => new StubDescriptionCollection(),
                        //////DescriptionsSetDescriptionCollection = descriptionCollection => { },
                        NamesGet = () => new StubNameCollection
                                            {
                                                new StubName("Service 1"),
                                            },
                        //////NamesSetNameCollection = nameCollection => { },
                        ServiceKeyGet = () => "uddi:service1",
                        //////ServiceKeySetString = serviceKey => { },
                    };
        }

        /// <summary>
        /// Simulates throwing an exception from the FindService.Send method.
        /// </summary>
        /// <typeparam name="TEx">Type of exception.</typeparam>
        private static void ShimFindServiceSendError<TEx>() where TEx : Exception
        {
            Microsoft.Uddi3.Fakes.ShimFindService.AllInstances.SendUddiConnection =
                (findService, uddiConnection) =>
                {
                    throw (TEx)Activator.CreateInstance(typeof(TEx), "Test call to FindService.Send throws error.");
                };
        }

        /// <summary>
        /// Simulates throwing an exception when logging a warning.
        /// </summary>
        /// <param name="numberOfTimes">
        /// The number of times an error will be thrown in this run.
        /// </param>
        /// <typeparam name="TEx">
        /// Type of exception.
        /// </typeparam>
        private static void ShimLogWarningError<TEx>(int numberOfTimes = -1) where TEx : Exception
        {
            var count = 0;

            System.Diagnostics.Fakes.ShimEventLog.AllInstances.WriteEntryStringEventLogEntryTypeInt32Int16ByteArray =
                (eventLog, message, type, eventId, category, rawData) =>
                {
                    if (numberOfTimes == -1 || numberOfTimes > count++)
                    {
                        throw (TEx)Activator.CreateInstance(typeof(TEx), "Test logging a warning with an error.");
                    }
                };
        }

        /// <summary>
        /// Simulates throwing an exception when logging a warning, except for Application source.
        /// </summary>
        /// <typeparam name="TEx">Type of exception.</typeparam>
        private static void ShimLogWarningErrorExceptApplicationSource<TEx>() where TEx : Exception
        {
            System.Diagnostics.Fakes.ShimEventLog.AllInstances.WriteEntryStringEventLogEntryTypeInt32Int16ByteArray =
                (eventLog, message, type, eventId, category, rawData) =>
                {
                    if (eventLog.Source == "Application")
                    {
                        return;
                    }

                    throw (TEx)Activator.CreateInstance(typeof(TEx), "Test logging a warning with an error.");
                };
        }

        /// <summary>
        /// Simulates getting appSettings from the config file
        /// </summary>
        /// <param name="uddiInquiryService">The InquiryService URL.</param>
        /// <param name="uddiDiscoverSites">A value indicating whether to look for UDDI sites registered in AD.</param>
        /// <param name="uddiExpireDiscoveredSitesAfterHrs">The time after which to expire discovered sites.</param>
        /// <param name="uddiDefaultServiceHost">The default UDDI service host.</param>
        private static void ShimConfigurationManagerAppSettings(
            string uddiInquiryService,
            string uddiDiscoverSites,
            string uddiExpireDiscoveredSitesAfterHrs,
            string uddiDefaultServiceHost)
        {
            ShimConfigurationManager.AppSettingsGet =
                () =>
                {
                    var outValue = new NameValueCollection
                                           {
                                               { "UDDIInquiryService", uddiInquiryService },
                                               { "UDDIDiscoverSites", uddiDiscoverSites },
                                               {
                                                   "UDDIExpireDiscoveredSitesAfterHrs",
                                                   uddiExpireDiscoveredSitesAfterHrs
                                               },
                                               { "UDDIDefaultServiceHost", uddiDefaultServiceHost }
                                           };

                    return outValue;
                };
        }

        /// <summary>
        /// Simulates getting appSettings from the config file for an no configured app setting.
        /// </summary>
        private static void ShimConfigurationManagerNoAppSettings()
        {
            ShimConfigurationManager.AppSettingsGet =
                () => new NameValueCollection();
        }

        /// <summary>
        /// Simulates a ConfigurationErrors exception when an attempt is made to retrieve appSettings from the config file.
        /// </summary>
        private static void ShimConfigurationManagerAppSettingsError()
        {
            ShimConfigurationManager.AppSettingsGet =
                () =>
                {
                    throw new ConfigurationErrorsException("Test: a simulated error has occured when accessing app settings.");
                };
        }

        /// <summary>
        /// Simulates returning a value for System.Environment.Is64BitOperatingSystem
        /// </summary>
        /// <param name="retVal">
        /// The return value.
        /// </param>
        private static void ShimEnvironmentIs64BitOperatingSystemGet(bool retVal)
        {
            System.Fakes.ShimEnvironment.Is64BitOperatingSystemGet = () => retVal;
        }

        /// <summary>
        /// Simulates returning a value for System.Environment.Is64BitProcess
        /// </summary>
        /// <param name="retVal">
        /// The return value.
        /// </param>
        private static void ShimEnvironmentIs64BitProcessGet(bool retVal)
        {
            System.Fakes.ShimEnvironment.Is64BitProcessGet = () => retVal;
        }

        /// <summary>
        /// Simulates the memory cache for UDDI site locations and returns a cache entry removal callback delegate.
        /// </summary>
        /// <param name="setTestCache">
        /// An action which sets the current memory cache to the fake.
        /// </param>
        /// <returns>
        /// A callback delegate for simulating cache entry removal events.
        /// </returns>
        private static CacheEntryRemovedCallback ShimMemoryCacheWithEntryRemoval(Action<MemoryCache> setTestCache)
        {
            var cacheSettings = new NameValueCollection(3)
                        {
                            { "cacheMemoryLimitMegabytes", "0" },
                            { "physicalMemoryPercentage", "0" },
                            { "pollingInterval", "00:02:00" }
                        };

            CacheEntryRemovedCallback callBack = null;

            setTestCache(new System.Runtime.Caching.Fakes.StubMemoryCache("UDDIInquiryServices", cacheSettings)
            {
                AddStringObjectCacheItemPolicyString =
                    (key, value, policy, region) =>
                    {
                        callBack = policy.RemovedCallback;
                        return true;
                    },

                GetEnumerator01 =
                    () =>
                    new Dictionary<string, object>
                                {
                                    {
                                        "TestEntry", 
                                        new UddiSiteLocation(
                                            "http://testEntry/inquire.asmx",
                                            string.Empty,
                                            string.Empty,
                                            "Test UDDI Inquiry Service.",
                                            AuthenticationMode.WindowsAuthentication)
                                    }
                                }.GetEnumerator()
            });

            return callBack;
        }

        /// <summary>
        /// Runs a test simulating the removal of an entry from the UDDI site location cache.
        /// </summary>
        /// <param name="cacheEntryRemovedReason">
        /// The reason the entry was removed.
        /// </param>
        private static void RunTestCacheRemoval(CacheEntryRemovedReason cacheEntryRemovedReason)
        {
            var privateType = new PrivateType(typeof(SiteLocationCache));
            var currentCache = (MemoryCache)privateType.GetStaticField("Cache");
            MemoryCache testCache = null;

            Action<MemoryCache> setTestCache = cache =>
            {
                privateType.SetStaticField("Cache", cache);
                privateType.InvokeStatic("SetCacheRefreshEntry");
                testCache = cache;
            };

            // Add a test inquiry service entry to the cache.
            var testCacheItem = new CacheItem(
                "TestEntry",
                new UddiSiteLocation(
                    "http://testEntry/inquire.asmx",
                    string.Empty,
                    string.Empty,
                    "Test UDDI Inquiry Service.",
                    AuthenticationMode.WindowsAuthentication));

            RunTest(
                () => ShimMemoryCacheWithEntryRemoval(setTestCache)(
                    new CacheEntryRemovedArguments(testCache, cacheEntryRemovedReason, testCacheItem)));

            privateType.SetStaticField("Cache", currentCache);
        }

        /// <summary>
        /// General purpose test running method.
        /// </summary>
        /// <param name="test">An action that performs the test.</param>
        /// <param name="fakeSetup">An action that sets up the fake environment.</param>
        private static void RunTest(Action test, Action fakeSetup = null)
        {
            RunTest<Exception>(test, null, fakeSetup);
        }

        /// <summary>
        /// General purpose test running method.
        /// </summary>
        /// <typeparam name="TEx">The type of exception</typeparam>
        /// <param name="test">An action that performs the test.</param>
        /// <param name="exceptionHandler">An action that handles any exception.</param>
         /// <param name="fakeSetup">An action that sets up the fake environment.</param>
       private static void RunTest<TEx>(Action test, Action<TEx> exceptionHandler, Action fakeSetup = null) where TEx : Exception
        {
            using (ShimsContext.Create())
            {
                (fakeSetup ?? ShimInitialializeInquireServices)();

                try
                {
                    test();
                }
                catch (TEx ex)
                {
                    if (exceptionHandler == null)
                    {
                        throw;
                    }

                    exceptionHandler(ex);
                }
            }
        }

        /// <summary>
        /// A stub IContainer class for testing disposal of the EsbUddiInstaller class.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private class DummyInstallerComponents : IContainer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DummyInstallerComponents"/> class. 
            /// </summary>
            public DummyInstallerComponents()
            {
                try
                {
                    this.Components = new StubComponentCollection(null);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // Do nothing here
                }
            }

            /// <summary>
            /// Gets the components.  This property is stubbed.
            /// </summary>
            public ComponentCollection Components { get; private set; }

            /// <summary>
            /// Stubbed Dispose method.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Stubbed Add method.
            /// </summary>
            /// <param name="component">A component.</param>
            public void Add(IComponent component)
            {
            }

            /// <summary>
            /// A stubbed Add method.
            /// </summary>
            /// <param name="component">A component.</param>
            /// <param name="name">The name of the component.</param>
            public void Add(IComponent component, string name)
            {
            }

            /// <summary>
            /// A stubbed Remove method.
            /// </summary>
            /// <param name="component">A component.</param>
            public void Remove(IComponent component)
            {
            }
        }
    }
}
