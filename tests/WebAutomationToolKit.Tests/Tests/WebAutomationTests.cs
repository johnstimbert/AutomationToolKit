﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebAutomationToolKit;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.Tests
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
        private readonly string _navUrlSeleniumDev = "https://www.selenium.dev/";
        private readonly string _interactionTestUrl = "https://www.selenium.dev/";
        private readonly string _screenShotPath = @"e:\ScreenShots";

        private static readonly string _driverPath = null;//@"E:\source\AutomationToolKit\WebUiAutomationToolKit\bin\Release";

        private readonly string _driverTypeNoneException = "DriverType Provided was not found. Instantiate the driver before performing actions on or with it";

        private readonly SelectorData donateInput = new SelectorData("DonateInput", HtmlTagType.input, HtmlAttributeType.Name, "PayPal");

        private static readonly SelectorData divById = new SelectorData("divById", HtmlTagType.div, HtmlAttributeType.Id,
                                                                "main_navbar");
        private static readonly SelectorData divByClass = new SelectorData("divByClass", HtmlTagType.div, HtmlAttributeType.Class, "container-fluid td-default td-outer");
        private static readonly SelectorData divTagTypeOnly = new SelectorData("divTagTypeOnly", HtmlTagType.div, HtmlAttributeType.None,
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
                DeleteAfterUse = true
            };

            var defaultOptions = new FirefoxOptions
            {
                AcceptInsecureCertificates = true,
                UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify,
            };

            defaultOptions.Profile = defaultProfile;

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
               _webAutomation = new WebAutomation(_driverPath, 10);

            if(_driverManager == null)
                _driverManager = _webAutomation.GetIWebDriverManager();

            divTags = new SelectorDataSet(HtmlTagType.div);
            divTags.AddSelectorDataByName(divById);
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

        #endregion

        #region Driver_Interaction_Tests
        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        [Ignore]//TODO: Update Screenshots
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
        [Ignore]//TODO: Update Screenshots
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
        [Ignore]//TODO: Update Screenshots
        public void TakeScreenShot_DriverTypeNone()
        {
            string shotName = "nullDriver";

            try
            {
                _driverManager.TakeScreenShot(_screenShotPath, shotName);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        private string startingUrl = "https://www.selenium.dev/";
        private string expectedUrl = "https://www.selenium.dev/selenium-ide/";
        private SelectorData _clickTarget = new SelectorData("ReadMore", HtmlTagType.a, HtmlAttributeType.Href, "https://selenium.dev/selenium-ide/");

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

        //[TestMethod]
        //[TestCategory(_driverInteractionTestCategory)]
        //public void SendTextWithDelay_Edge()
        //{
        //    string text = "ChromeShot";
        //    SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.input, HtmlAttributeType.Type, "text");

        //    _driverManager.CreateDriverInstance(DriverType.Edge);
        //    _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
        //    Assert.IsNotNull(_driverManager, "Driver was null");
        //    _driverManager.SendTextWithDelay(googleSearchInput, text);
        //}
               
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
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_interactionTestUrl);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.MoveToElement(divById);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Firefox()
        {

            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_interactionTestUrl);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.MoveToElement(divById);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_DriverTypeNone()
        {

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_interactionTestUrl);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                _driverManager.MoveToElement(donateInput);
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
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");            
            Assert.IsTrue(_driverManager.DoesUrlContain("google"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContain("google"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_DriverTypeNone()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                Assert.IsTrue(_driverManager.DoesUrlContain("google"));
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
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("google"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("google"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_DriverTypeNone()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("google"));
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
        [Ignore]//TODO: Update Screenshots
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
        [Ignore]//TODO: Update Screenshots
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
        [Ignore]//TODO: Update Screenshots
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
        public void MoveToElement_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_interactionTestUrl);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.MoveToElement(divById);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
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
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContain("google"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContain_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContain("google"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContainUsingRegex_Chrome_HeadlessMode()
        {
            var options = GetChromeOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("google"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategoryHeadless)]
        public void DoesUrlContainUsingRegex_Firefox_HeadlessMode()
        {
            var options = GetFirefoxOptions();
            options.AddArgument("--headless");
            _driverManager.SetDriverOptions(null, options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("google"));
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
        public void CheckElementExistsReturnIWebElement_DriverTypeNone()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);
            
            try
            {
                var element = _driverManager.CheckElementExistsReturnIWebElement(data);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(divById.AttributeValue, attributeValue, $"Expected attribute value was {divById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(divById.AttributeValue, attributeValue, $"Expected attribute value was {divById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_ByClass_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(divById.AttributeValue, attributeValue, $"Expected attribute value was {divById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var cssBy = _driverManager.CheckElementExistsReturnCssSelector(divById);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(divById.AttributeValue, attributeValue, $"Expected attribute value was {divById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnCssSelector_ByClass_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divByClass);
            var attributeValue = element.GetAttribute("class");
            Assert.AreEqual(divByClass.AttributeValue, attributeValue, $"Expected attribute value was {divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_Chrome()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementsExist_ById_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(divById.AttributeValue, attributeValue, $"Expected attribute value was {divById.AttributeValue}, found {attributeValue}");
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(divById.AttributeValue, attributeValue, $"Expected attribute value was {divById.AttributeValue}, found {attributeValue}");
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(divById.AttributeValue, attributeValue, $"Expected attribute value was {divById.AttributeValue}, found {attributeValue}");
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(divById);
            var attributeValue = element.GetAttribute("id");
            Assert.AreEqual(divById.AttributeValue, attributeValue, $"Expected attribute value was {divById.AttributeValue}, found {attributeValue}");
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
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
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            Assert.IsTrue(_driverManager.CheckElementsExist(divTags));
        }

        #endregion
                
    }
}
