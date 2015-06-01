// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="Solidsoft Reply Ltd.">
//   (c) 2013 Solidsoft Reply Ltd.
// </copyright>
// <summary>
//   Summary description for UnitTest1
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Solidsoft.Esb.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit test class.
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        #endregion

        /// <summary>
        /// Tests ESB OnRamp.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            var client = new ESBOnRamp.Solidsoft_BizTalk_ESB_OnRamp_OnRampInterface_OnRampClient();

            object reqResp = "<bb xmlns='aa' />";

            client.Submit(ref reqResp);
        }
    }
}
