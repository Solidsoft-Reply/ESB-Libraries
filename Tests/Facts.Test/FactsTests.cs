// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactsTests.cs" company="Solidsoft Reply Ltd.">
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
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Facts.Test
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration.Fakes;

    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the Facts component.
    /// </summary>
    [TestClass]
    public class FactsTests
    {
        [TestMethod]
        public void SetBamConfigurationDirectiveNull()
        {
            Action<EsbFactsException> errorHandler = ex => { };

            RunTest(
                () =>
                    {
                        (new Interchange()).SetBamConfiguration(null, null, false, 0);
                        Assert.Fail("expected an exception of type EsbFactsException");
                    },
                errorHandler);
        }

        [TestMethod]
        public void SetBamConfigurationDirectiveEmpty()
        {
            Action<EsbFactsException> errorHandler = ex => { };

            RunTest(
                () =>
                {
                    (new Interchange()).SetBamConfiguration(string.Empty, null, false, 0);
                    Assert.Fail("expected an exception of type EsbFactsException");
                },
                errorHandler);
        }

        [TestMethod]
        public void SetBamConfigurationDirective1BamCsNull()
        {
            Action<EsbFactsException> errorHandler = ex => { };

            RunTest(
                () =>
                {
                    (new Interchange()).SetBamConfiguration("1", null, false, 0);
                    Assert.Fail("expected an exception of type EsbFactsException");
                },
                errorHandler);
        }

        [TestMethod]
        public void SetBamConfigurationDirective1BamCsEmpty()
        {
            Action<EsbFactsException> errorHandler = ex => { };

            RunTest(
                () =>
                {
                    (new Interchange()).SetBamConfiguration("1", string.Empty, false, 0);
                    Assert.Fail("expected an exception of type EsbFactsException");
                },
                errorHandler);
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
                (fakeSetup ?? ShimDefaultInitialialize)();

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
        /// Initializes the InquiryService class with default settings.
        /// </summary>
        private static void ShimDefaultInitialialize()
        {
            //ShimConfigurationManagerAppSettings(TestUddiInquireUrl, "true", "24", "localhost");
            //typeof(SiteLocationCache).TypeInitializer.Invoke(null, null);
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
                                               //{ "UDDIInquiryService", uddiInquiryService },
                                               //{ "UDDIDiscoverSites", uddiDiscoverSites },
                                               //{
                                               //    "UDDIExpireDiscoveredSitesAfterHrs",
                                               //    uddiExpireDiscoveredSitesAfterHrs
                                               //},
                                               //{ "UDDIDefaultServiceHost", uddiDefaultServiceHost }
                                           };

                    return outValue;
                };
        }
    }
}
