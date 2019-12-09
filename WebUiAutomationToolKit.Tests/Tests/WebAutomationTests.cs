using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using WebUiAutomationToolKit;
using WebUiAutomationToolKit.Exceptions;
using WebUiAutomationToolKit.WebAndApiAutomationObjects;
using static WebUiAutomationToolKit.WebUiAutomationEnums;

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

        private static WebUiAutomation _webAutomation;
        private static IWebDriverManager _driverManager;
        private readonly string _navUrlGoogle = "https://www.google.com/";
        private readonly string _navUrlSeleniumHq = "https://www.seleniumhq.org/";
        private readonly string _screenShotPath = @"C:\ScreenShots";

        private const string _chromeDriverName = "chromedriver.exe";
        private const string _firefoxDriverName = "geckodriver.exe";
        private const string _ieDriverName = "IEDriverServer.exe";
                
        private static readonly string _driverPath = @"C:\Users\j_sti\Source\Repos\BrainStemSolutions\AutomationToolKit\WebUiAutomationToolKit\bin\release";

        private readonly string _driverTypeNoneException = "DriverType Provided was not found. Instantiate the driver before performing actions on or with it";

        private readonly SelectorData copyrightDiv = new SelectorData("CopyrightDiv", HtmlTagType.div, HtmlAttributeType.Class, "copyright");

        private static readonly SelectorData headerById = new SelectorData("headerById", HtmlTagType.div, HtmlAttributeType.Id,
                                                                "header");
        private static readonly SelectorData divByClass = new SelectorData("divByClass", HtmlTagType.div, HtmlAttributeType.Class, "header-description");
        private static readonly SelectorData divTagTypeOnly = new SelectorData("divTagTypeOnly", HtmlTagType.div, HtmlAttributeType.None,
                                                               null);
        private static readonly SelectorData headerTagTypeOnly = new SelectorData("headerTagTypeOnly", "header", HtmlAttributeType.None,
                                                               null);

        private static SelectorDataSet divTags = new SelectorDataSet(HtmlTagType.div);
        
        private static ChromeOptions GetChromeOptions()
        {
            var defaultOptions = new ChromeOptions();
            defaultOptions.AcceptInsecureCertificates = true;
            defaultOptions.UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify;

            return defaultOptions;
        }

        private static FirefoxOptions GetFirefoxOptions()
        {
            var defaultProfile = new FirefoxProfile
            {
                AcceptUntrustedCertificates = true,
                DeleteAfterUse = true,
                AssumeUntrustedCertificateIssuer = true
            };

            var defaultOptions = new FirefoxOptions
            {
                AcceptInsecureCertificates = true,
                UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify,
                UseLegacyImplementation = false
            };

            defaultOptions.Profile = defaultProfile;

            return defaultOptions;
        }

        private static InternetExplorerOptions GetInternetExplorerOptions()
        {
            var defaultOptions = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                EnableNativeEvents = false,
                IgnoreZoomLevel = true,
                UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify
            };

            return defaultOptions;
        }

        private static EdgeOptions GetEdgeOptions()
        {
            var defaultOptions = new EdgeOptions
            {
                AcceptInsecureCertificates = true,
                UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify
            };

            return defaultOptions;
        }

        [TestInitialize]
        public void BeforeEachTest()
        {
            if(_webAutomation == null)
                _webAutomation = new WebUiAutomation(_driverPath, 10);

            if(_driverManager == null)
                _driverManager = _webAutomation.GetIWebDriverManager();

            divTags = new SelectorDataSet(HtmlTagType.div);
            divTags.AddSelectorDataByName(headerById);
            divTags.AddSelectorDataByName(divByClass);
            divTags.AddSelectorDataByName(divTagTypeOnly);
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
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.AreEqual(_navUrlGoogle, _driverManager.GetActiveDriverUrl(), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategoryHeadless)]
        public void GetFirefoxDriver_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
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
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsTrue(_driverManager.GetActiveDriverUrl().Contains(_navUrlGoogle), $"Expected url did not contain {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
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
                var webAutomation = new WebUiAutomation(badPath);
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
                var webAutomation = new WebUiAutomation(badPath);
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
                var webAutomation = new WebUiAutomation(badPath);
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
                var webAutomation = new WebUiAutomation(badPath);
                _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            }
            catch (Exception ex)
            {
                Assert.AreEqual($"The driver {_ieDriverName} was not found in the path {badPath}", ex.Message, $"Expected exception was not returned");
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
            _driverManager.TakeScreenShot(_screenShotPath, shotName);

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
            _driverManager.TakeScreenShot(_screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void TakeScreenShot_IE()
        {
            string shotName = "IEShot";
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.TakeScreenShot(_screenShotPath, shotName);

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
            _driverManager.TakeScreenShot(_screenShotPath, shotName);

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
                _driverManager.TakeScreenShot(_screenShotPath, shotName);
            }
            catch (WebUiAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        private string startingUrl = "https://www.seleniumhq.org/";
        private string expectedUrl = "https://selenium.dev/projects/";
        private SelectorData _clickTarget = new SelectorData("SeleniumProjects", HtmlTagType.a, HtmlAttributeType.InnerText_ExactMatch, "Projects");

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager.Click(_clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager.Click(_clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager.Click(_clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager.Click(_clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_DriverTypeNone()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);
            _driverManager.QuitDriverInstance(DriverType.Edge);

            try
            {
                _driverManager.Click(_clickTarget);
            }
            catch (WebUiAutomationException ex)
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
            _driverManager.SendText(googleSearchInput, text);
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
            _driverManager.SendText(googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.SendText(googleSearchInput, text);
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
            _driverManager.SendText(googleSearchInput, text);
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
                _driverManager.SendText(googleSearchInput, text);
            }
            catch (WebUiAutomationException ex)
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
            _driverManager.SendTextWithDelay(googleSearchInput, text);
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
            _driverManager.SendTextWithDelay(googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.SendTextWithDelay(googleSearchInput, text);
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
            _driverManager.SendTextWithDelay(googleSearchInput, text);
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
                _driverManager.SendTextWithDelay(googleSearchInput, text);
            }
            catch (WebUiAutomationException ex)
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
            _driverManager.SendText(googleSearchInput, text);
            _driverManager.ClearText(googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_Firefox()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null"); ;
            _driverManager.SendText(googleSearchInput, text);
            _driverManager.ClearText(googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null"); ;
            _driverManager.SendText(googleSearchInput, text);
            _driverManager.ClearText(googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_Edge()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null"); ;
            _driverManager.SendText(googleSearchInput, text);
            _driverManager.ClearText(googleSearchInput);
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
                _driverManager.ClearText(googleSearchInput);
            }
            catch (WebUiAutomationException ex) 
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
            _driverManager.MoveToElement(copyrightDiv);
            var element = _driverManager.CheckElementExistsReturnIWebElement(copyrightDiv);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Firefox()
        {

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.MoveToElement(copyrightDiv);
            var element = _driverManager.CheckElementExistsReturnIWebElement(copyrightDiv);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_IE()
        {
            _webAutomation = new WebUiAutomation(_driverPath, 30);

            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.MoveToElement(copyrightDiv);
            var element = _driverManager.CheckElementExistsReturnIWebElement(copyrightDiv);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Edge()
        {

            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.MoveToElement(copyrightDiv);
            var element = _driverManager.CheckElementExistsReturnIWebElement(copyrightDiv);
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
                _driverManager.MoveToElement(copyrightDiv);
            }
            catch (WebUiAutomationException ex)
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
            Assert.IsTrue(_driverManager.DoesUrlContain("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContain("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContain("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContain("intersys"));
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
                Assert.IsTrue(_driverManager.DoesUrlContain("intersys"));
            }
            catch (WebUiAutomationException ex)
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
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("intersys"));
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
                Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("intersys"));
            }
            catch (WebUiAutomationException ex)
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
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.TakeScreenShot(_screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void TakeScreenShot_Firefox_HeadlessMode()
        {
            string shotName = "FireFoxShot";
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.TakeScreenShot(_screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Click_Chrome_HeadlessMode()
        {
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager.Click(_clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Click_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager.Click(_clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driverManager.GetActiveDriverUrl(), $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendText_Chrome_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.SendText(googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendText_Firefox_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.SendText(googleSearchInput, text); ;
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendTextWithDelay_Chrome_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.SendText(googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void SendTextWithDelay_Firefox_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.SendText(googleSearchInput, text);
        }
        
        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Clear_Chrome_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.SendText(googleSearchInput, text);
            _driverManager.ClearText(googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void Clear_Firefox_HeadlessMode()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.SendText(googleSearchInput, text);
            _driverManager.ClearText(googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void MoveToElement_Chrome_HeadlessMode()
        {
            SelectorData copyrightDiv = new SelectorData("CopyrightDiv", HtmlTagType.div, HtmlAttributeType.Class, "copyright");

            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.MoveToElement(copyrightDiv);
            var element = _driverManager.CheckElementExistsReturnIWebElement(copyrightDiv);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void MoveToElement_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.MoveToElement(copyrightDiv);
            var element = _driverManager.CheckElementExistsReturnIWebElement(copyrightDiv);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContain_Chrome_HeadlessMode()
        {
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContain("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContain_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContain("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContainUsingRegex_Chrome_HeadlessMode()
        {
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContainUsingRegex_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("intersys"));
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
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
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
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
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
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
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
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_IE()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_IE()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
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
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
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
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
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
                var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            }
            catch (WebUiAutomationException ex)
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ById_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            Assert.IsNotNull(element, $"An element could not be found with the data defined in SelectorData named {headerById.Name}");
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ById_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var cssBy = _driverManager.CheckElementExistsReturnCssSelector(headerById);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var cssBy = _driverManager.CheckElementExistsReturnCssSelector(headerById);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var cssBy = _driverManager.CheckElementExistsReturnCssSelector(headerById);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var cssBy = _driverManager.CheckElementExistsReturnCssSelector(headerTagTypeOnly);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            Assert.AreEqual(headerTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {headerTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var cssBy = _driverManager.CheckElementExistsReturnCssSelector(headerTagTypeOnly);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            Assert.AreEqual(headerTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {headerTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var cssBy = _driverManager.CheckElementExistsReturnCssSelector(headerTagTypeOnly);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            Assert.AreEqual(headerTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {headerTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByTagTypeOnly_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var cssBy = _driverManager.CheckElementExistsReturnCssSelector(headerTagTypeOnly);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            Assert.AreEqual(headerTagTypeOnly.TagType.ToString().ToLower(), element.TagName.ToString().ToLower(), $"Expected attribute value was {headerTagTypeOnly.TagType}, found {element.TagName}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_IE()
        {
            _driverManager.CreateDriverInstance(DriverType.InternetExplorer);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_Edge()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_DriverTypeNoner()
        {
            _driverManager.CreateDriverInstance(DriverType.Edge);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            _driverManager.QuitDriverInstance(DriverType.Edge);

            try
            {
                Assert.IsTrue(_driverManager.CheckElementsExist(divTags)); ;
            }
            catch (WebUiAutomationException ex)
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

            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Chrome_HeadlessMode()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Firefox_HeadlessMode()
        {
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Firefox_HeadlessMode()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }
        
        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ById_Chrome_HeadlessMode()
        {
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ById_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByClass_Chrome_HeadlessMode()
        {
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            Assert.IsNotNull(element, $"An element could not be found with the data defined in SelectorData named {divByClass.Name}");
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnIWebElement_ByClass_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ById_Chrome_HeadlessMode()
        {
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ById_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(headerById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(headerById.AttributeValue, attributeValue, $"Expected attribute value was {headerById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByClass_Chrome_HeadlessMode()
        {
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            Assert.IsNotNull(element, $"An element could not be found with the data defined in SelectorData named {divByClass.Name}");
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementExistsReturnCssSelector_ByClass_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementsExist_ById_Chrome_HeadlessMode()
        {
            divTags.RemoveSelectorDataByName(divByClass.Name);
            divTags.RemoveSelectorDataByName(divTagTypeOnly.Name);

            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategoryHeadless)]
        public void CheckElementsExist_ById_Firefox_HeadlessMode()
        {
            divTags.RemoveSelectorDataByName(divByClass.Name);
            divTags.RemoveSelectorDataByName(divTagTypeOnly.Name);

            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumHq);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
        }

        #endregion
                
    }
}
