using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAndWebApiAutomation;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndApiAutomation.Tests
{
    [TestClass]
    public class WebAutomationTests
    {
        private static TestContext _testContext;
        public TestContext TestContext { get { return _testContext; } set { _testContext = value; } }

        private const string _driverTestCategory = "Driver_Instance_Creation_Tests";
        private const string _driverInteractionTestCategory = "Driver_Interaction_Tests";
        private const string _elementLocationMethodsTestCategory = "Element_Location_Method_Tests";

        private const string _driverTestCategoryHeadless = "Driver_Instance_Creation_Tests_Headless";
        private const string _driverInteractionTestCategoryHeadless = "Driver_Interaction_Tests_Headless";
        private const string _elementLocationMethodsTestCategoryHeadless = "Element_Location_Method_Tests_Headless";

        private static WebAutomation _webAutomation;
        private static IWebDriverManager _driverManager;
        private readonly string _navUrlGoogle = "https://www.google.com/";
        private readonly string _navUrlIntersys = "https://www.intersysconsulting.com/";
        private readonly string _screenShotPath = @"C:\ScreenShots";

        private const string _chromeDriverName = "chromedriver.exe";
        private const string _firefoxDriverName = "geckodriver.exe";
        private const string _ieDriverName = "IEDriverServer.exe";
        private const string _edgeDriverName = "MicrosoftWebDriver.exe";

        private static readonly string _driverPath = @"C:\Users\j_sti\source\repos\AutomationToolKit\WebAndWebApiAutomation\bin\Release";
        //private static readonly string _driverPath = @"C:\Users\j_sti\source\repos\AutomationToolKit\WebAndWebApiAutomation\bin\Debug";

        private readonly string _driverTypeNoneException = "DriverType Provided was not found. Instantiate the driver before performing actions on or with it";

        private readonly SelectorData copyrightDiv = new SelectorData("CopyrightDiv", HtmlTagType.div, HtmlAttributeType.Class, "copyright");

        private static readonly SelectorData navById = new SelectorData("navById", HtmlTagType.nav, HtmlAttributeType.Id,
                                                                "mainNav");
        private static readonly SelectorData navByClass = new SelectorData("navByClass", HtmlTagType.nav, HtmlAttributeType.Class,
                                                                    "main-nav mm-menu mm-border-offset mm-theme-white mm-multiline mm-offcanvas mm-right mm-front mm-columns");
        private static readonly SelectorData navTagTypeOnly = new SelectorData("navTagTypeOnly", HtmlTagType.nav, HtmlAttributeType.None,
                                                               null);

        private static SelectorDataSet navTags = new SelectorDataSet(HtmlTagType.nav);

        [AssemblyInitialize()]
        public static void TestRunSetup(TestContext testContex)
        {
            _testContext = testContex;
            _webAutomation = new WebAutomation(_driverPath, 10);
            _driverManager = _webAutomation.GetIWebDriverManager();
        }

        [TestInitialize]
        public void BeforeEachTest()
        {
            if(_webAutomation == null)
                _webAutomation = new WebAutomation(_driverPath, 10);

            if(_driverManager == null)
                _driverManager = _webAutomation.GetIWebDriverManager();

            navTags = new SelectorDataSet(HtmlTagType.nav);
            navTags.AddSelectorDataByName(navById);
            navTags.AddSelectorDataByName(navByClass);
            navTags.AddSelectorDataByName(navTagTypeOnly);
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            if (_driverManager.GetActiveDriverType() != DriverType.None)
            {
                var activeDriverType = _driverManager.GetActiveDriverType();
                _driverManager.QuitDriverInstance(activeDriverType);
            }
            if (Directory.Exists(_screenShotPath))
            {
                var dirInfo = new DirectoryInfo(_screenShotPath);
                dirInfo.Delete(true);
            }

        }

        #region Driver_Tests_Headless
        [TestMethod]
        [TestCategory(_driverTestCategoryHeadless)]
        public void GetChromeDriver_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.AreEqual(_navUrlGoogle, _driverManager.GetActiveDriverUrl(), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategoryHeadless)]
        public void GetFirefoxDriver_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.AreEqual(_navUrlGoogle, _driverManager.GetActiveDriverUrl(), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        #endregion

        #region Driver_Tests
        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetChromeDriver()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.AreEqual(_navUrlGoogle, _driverManager.GetActiveDriverUrl(), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetFirefoxDriver()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.AreEqual(_navUrlGoogle, _driverManager.GetActiveDriverUrl(), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetIEDriver()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.AreEqual(_navUrlGoogle, _driverManager.GetActiveDriverUrl(), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetEdgeDriver()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.AreEqual(_navUrlGoogle, _driverManager.GetActiveDriverUrl(), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetDriver_BadPath()
        {
            string badPath = @"C:\xyz";
            try
            {
                var webAutomation = new WebAutomation(badPath);
            }
            catch(Exception ex)
            {
                Assert.AreEqual($"The path {badPath}\\ could not be found", ex.Message, $"Expected exception was not returned");
            }
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetChromeDriver_GoodPath_NoDriverexe()
        {
            string badPath = _driverPath.Substring(0, _driverPath.IndexOf("bin"));
            try
            {
                var webAutomation = new WebAutomation(badPath);
                _driverManager.CreateDriverInstance(DriverType.Chrome);
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"The driver {_chromeDriverName} was not found in the path {badPath}", ex.Message, $"Expected exception was not returned");
            }
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetFirefoxDriver_GoodPath_NoDriverexe()
        {
            string badPath = _driverPath.Substring(0, _driverPath.IndexOf("bin"));
            try
            {
                var webAutomation = new WebAutomation(badPath);
                _driverManager.CreateDriverInstance(DriverType.Firefox);
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"The driver {_firefoxDriverName} was not found in the path {badPath}", ex.Message, $"Expected exception was not returned");
            }
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetIEDriver_GoodPath_NoDriverexe()
        {
            string badPath = _driverPath.Substring(0, _driverPath.IndexOf("bin"));
            try
            {
                var webAutomation = new WebAutomation(badPath);
                _driverManager.CreateDriverInstance(DriverType.Edge);
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"The driver {_ieDriverName} was not found in the path {badPath}", ex.Message, $"Expected exception was not returned");
            }
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetEdgeDriver_GoodPath_NoDriverexe()
        {
            string badPath = _driverPath.Substring(0, _driverPath.IndexOf("bin"));
            try
            {
                var webAutomation = new WebAutomation(badPath);
                _driverManager.CreateDriverInstance(DriverType.Edge);
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"The driver {_edgeDriverName} was not found in the path {badPath}", ex.Message, $"Expected exception was not returned");
            }
        }


        #endregion

        #region Driver_Interaction_Tests
        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void TakeScreenShot_Chrome()
        {
            string shotName = "ChromeShot";
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void TakeScreenShot_Firefox()
        {
            string shotName = "FireFoxShot";
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void TakeScreenShot_IE()
        {
            string shotName = "IEShot";
            _driverManager.CreateDriverInstance(DriverType.Ie);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void TakeScreenShot_Edge()
        {
            string shotName = "EdgeShot";
            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void TakeScreenShot_DriverTypeNone()
        {
            string shotName = "nullDriver";

            try
            {
                _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            var executor = _webAutomation.GetJavaScriptExecutor(_driverManager);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            var executor = _webAutomation.GetJavaScriptExecutor(_driverManager);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            var executor = _webAutomation.GetJavaScriptExecutor(_driverManager);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            var executor = _webAutomation.GetJavaScriptExecutor(_driverManager);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_DriverTypeNone()
        {
            try
            {
                _webAutomation.GetJavaScriptExecutor(_driverManager);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Chrome()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Firefox()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_IE()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Edge()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_DriverTypeNone()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);
            _driverManager.QuitDriverInstance(DriverType.Edge);

            try
            {
                _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }            
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_Chrome()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_Firefox()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Ie);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_Edge()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_DriverTypeNone()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                _webAutomation.SendText(_driverManager, googleSearchInput, text);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_Chrome()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_Firefox()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Ie);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_Edge()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
        }
               
        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_DriverTypeNone()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_Chrome()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
            _webAutomation.Clear(_driverManager, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_Firefox()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
            _webAutomation.Clear(_driverManager, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Ie);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
            _webAutomation.Clear(_driverManager, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_Edge()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
            _webAutomation.Clear(_driverManager, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_DriverTypeNone()
        {
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                _webAutomation.Clear(_driverManager, googleSearchInput);
            }
            catch (WebAutomationException ex) 
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Chrome()
        {
            SelectorData copyrightDiv = new SelectorData("CopyrightDiv", HtmlTagType.div, HtmlAttributeType.Class, "copyright");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager = _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driverManager);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Firefox()
        {

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager = _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driverManager);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_IE()
        {
            _webAutomation = new WebAutomation(_driverPath, 30);

            _driverManager.CreateDriverInstance(DriverType.Ie);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager = _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driverManager);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Edge()
        {

            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager = _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driverManager);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_DriverTypeNone()
        {

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");            
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_DriverTypeNone()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_DriverTypeNone()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        #endregion

        #region Driver_Interaction_Tests_Headless
        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void TakeScreenShot_Chrome_HeadlessMode()
        {
            string shotName = "ChromeShot";
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void TakeScreenShot_Firefox_HeadlessMode()
        {
            string shotName = "FireFoxShot";
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void TakeScreenShot_IE_HeadlessMode()
        {
            string shotName = "IEShot";
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void TakeScreenShot_Edge_HeadlessMode()
        {
            string shotName = "EdgeShot";
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _webAutomation.TakeScreenShot(_driverManager, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void GetJavaScriptExecutor_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driverManager);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void GetJavaScriptExecutor_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driverManager);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void GetJavaScriptExecutor_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driverManager);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void GetJavaScriptExecutor_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driverManager);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Click_Chrome_HeadlessMode()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Click_Firefox_HeadlessMode()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Click_IE_HeadlessMode()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Click_Edge_HeadlessMode()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.Title, "Selenium Projects");

            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager = _webAutomation.Click(_driverManager, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendText_Chrome_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendText_Firefox_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendText_IE_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendText_Edge_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendTextWithDelay_Chrome_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendTextWithDelay_Firefox_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendTextWithDelay_IE_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendTextWithDelay_Edge_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendTextWithDelay(_driverManager, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Clear_Chrome_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
            _webAutomation.Clear(_driverManager, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Clear_Firefox_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
            _webAutomation.Clear(_driverManager, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Clear_IE_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
            _webAutomation.Clear(_driverManager, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Clear_Edge_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _webAutomation.SendText(_driverManager, googleSearchInput, text);
            _webAutomation.Clear(_driverManager, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void MoveToElement_Chrome_HeadlessMode()
        {
            SelectorData copyrightDiv = new SelectorData("CopyrightDiv", HtmlTagType.div, HtmlAttributeType.Class, "copyright");

            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager = _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driverManager);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void MoveToElement_Firefox_HeadlessMode()
        {

            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager = _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driverManager);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void MoveToElement_IE_HeadlessMode()
        {
            _webAutomation = new WebAutomation(_driverPath, 30);

            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager = _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driverManager);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void MoveToElement_Edge_HeadlessMode()
        {

            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager = _webAutomation.MoveToElement(_driverManager, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driverManager);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContain_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContain_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContain_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContain_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driverManager, "intersys"));
        }
        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContainUsingRegex_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContainUsingRegex_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContainUsingRegex_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContainUsingRegex_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driverManager, "intersys"));
        }

        #endregion

        #region Element_Location_Method_Tests

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Chrome()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Chrome()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Firefox()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Firefox()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_IE()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_IE()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Edge()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Edge()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }
        
        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_DriverTypeNone()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);
            
            try
            {
                var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ById_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ById_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            Assert.IsNotNull(element, $"An element could not be found with the data defined in SelectorData named {navById.Name}");
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ById_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navByClass, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navByClass, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navByClass, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navByClass, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByTagTypeOnly_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByTagTypeOnly_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByTagTypeOnly_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByTagTypeOnly_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navById, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navById, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navById, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navById, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navByClass, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navByClass, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navByClass, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navByClass, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navTagTypeOnly, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navTagTypeOnly, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navTagTypeOnly, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navTagTypeOnly, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_DriverTypeNoner()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            _driverManager.QuitDriverInstance(DriverType.Edge);

            try
            {
                Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }

        }

        #endregion

        #region Element_Location_Method_Tests_Headless
        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Chrome_HeadlessMode()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Chrome_HeadlessMode()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Firefox_HeadlessMode()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Firefox_HeadlessMode()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_IE_HeadlessMode()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_IE_HeadlessMode()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Edge_HeadlessMode()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Edge_HeadlessMode()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driverManager);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }
        
        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ById_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ById_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ById_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ById_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByClass_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navByClass, _driverManager);
            Assert.IsNotNull(element, $"An element could not be found with the data defined in SelectorData named {navByClass.Name}");
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByClass_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navByClass, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByClass_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navByClass, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByClass_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navByClass, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByTagTypeOnly_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByTagTypeOnly_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByTagTypeOnly_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByTagTypeOnly_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ById_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navById, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ById_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navById, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ById_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navById, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ById_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navById, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(navById.AttributeValue, attributeValue, $"Expected attribute value was {navById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByClass_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navByClass, _driverManager);
            Assert.IsNotNull(cssBy, $"An element could not be found with the data defined in SelectorData named {navByClass.Name}");
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navById, _driverManager);
            Assert.IsNotNull(element, $"An element could not be found with the data defined in SelectorData named {navById.Name}");
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByClass_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navByClass, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByClass_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navByClass, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByClass_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navByClass, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(navByClass.AttributeValue, attributeValue, $"Expected attribute value was {navByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Chrome_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navTagTypeOnly, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Firefox_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navTagTypeOnly, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_IE_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navTagTypeOnly, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Edge_HeadlessMode()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            var cssBy = _webAutomation.CheckElementExistsReturnCssSelector(navTagTypeOnly, _driverManager);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(navTagTypeOnly, _driverManager);
            Assert.AreEqual(navTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {navTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementsExist_ById_Chrome_HeadlessMode()
        {
            navTags.RemoveSelectorDataByName(navByClass.Name);
            navTags.RemoveSelectorDataByName(navTagTypeOnly.Name);

            _driverManager.CreateDriverInstance(DriverType.Chrome, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementsExist_ById_Firefox_HeadlessMode()
        {
            navTags.RemoveSelectorDataByName(navByClass.Name);
            navTags.RemoveSelectorDataByName(navTagTypeOnly.Name);

            _driverManager.CreateDriverInstance(DriverType.Firefox, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementsExist_ById_IE_HeadlessMode()
        {
            navTags.RemoveSelectorDataByName(navByClass.Name);
            navTags.RemoveSelectorDataByName(navTagTypeOnly.Name);

            _driverManager.CreateDriverInstance(DriverType.Ie, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementsExist_ById_Edge_HeadlessMode()
        {
            navTags.RemoveSelectorDataByName(navByClass.Name);
            navTags.RemoveSelectorDataByName(navTagTypeOnly.Name);

            _driverManager.CreateDriverInstance(DriverType.Edge, new string[] { "--headless" });
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlIntersys);
            Assert.IsTrue(_webAutomation.CheckElementsExist(navTags, _driverManager));
        }
        
        #endregion

    }
}
