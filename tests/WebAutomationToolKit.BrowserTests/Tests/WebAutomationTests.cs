using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebAutomationToolKit;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.BrowserTests
{
    [TestClass]
    [TestCategory("Browser")]
    public class WebAutomationTests
    {
        public TestContext TestContext { get; set; } = null!;

        private const string _driverTestCategory = "Driver_Instance_Creation_Tests";
        private const string _driverInteractionTestCategory = "Driver_Interaction_Tests";
        private const string _elementLocationMethodsTestCategory = "Element_Location_Method_Tests";

        private const string _driverTestCategoryHeadless = "Driver_Instance_Creation_Tests_Headless";
        private const string _driverInteractionTestCategoryHeadless = "Driver_Interaction_Tests_Headless";
        private const string _elementLocationMethodsTestCategoryHeadless = "Element_Location_Method_Tests_Headless";

        private static WebAutomation _webAutomation = null!;
        private static IWebDriverManager _driverManager = null!;
        private readonly string _navUrlGoogle = "https://www.google.com/";
        private readonly string _navUrlSeleniumDev = "https://www.selenium.dev/";
        private readonly string _interactionTestUrl = "https://www.selenium.dev/";
        private readonly string _screenShotPath = @"e:\ScreenShots";

        private static readonly string? _driverPath = null;//@"E:\source\AutomationToolKit\WebUiAutomationToolKit\bin\Release";

        private readonly string _driverTypeNoneException = "DriverType Provided was not found. Instantiate the driver before performing actions on or with it";

        private readonly SelectorData donateInput = new SelectorData("DonateInput", HtmlTagType.input, HtmlAttributeType.Name, "PayPal");

        private static readonly SelectorData divById = new SelectorData("divById", HtmlTagType.div, HtmlAttributeType.Id,
                                                                "main_navbar");
        private static readonly SelectorData divByClass = new SelectorData("divByClass", HtmlTagType.div, HtmlAttributeType.Class, "container-fluid td-default td-outer");
        private static readonly SelectorData divTagTypeOnly = new SelectorData("divTagTypeOnly", HtmlTagType.div, HtmlAttributeType.None,
                                                               null!);

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
           _webAutomation = new WebAutomation(_driverPath, 10);
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
            Assert.IsTrue(_driverManager.GetActiveDriverUrl().StartsWith(_navUrlGoogle), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
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
            Assert.IsTrue(_driverManager.GetActiveDriverUrl().StartsWith(_navUrlGoogle), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        #endregion

        #region Driver_Tests
        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetChromeDriver()
        {
            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsTrue(_driverManager.GetActiveDriverUrl().StartsWith(_navUrlGoogle), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetFirefoxDriver()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsTrue(_driverManager.GetActiveDriverUrl().StartsWith(_navUrlGoogle), $"Expected url was {_navUrlGoogle}, Url found was {_driverManager.GetActiveDriverUrl()}");
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
            // P1: DoesUrlContain uses WebDriverWait internally — replaces Thread.Sleep(3000)
            Assert.IsTrue(_driverManager.DoesUrlContain("selenium-ide"),
                $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");

        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Firefox()
        {
            _driverManager.CreateDriverInstance(DriverType.Firefox);
            Assert.IsNotNull(_driverManager, "Driver was null");
            _driverManager.NavigateWithActiveDriver(startingUrl);

            _driverManager.Click(_clickTarget);
            // P1: DoesUrlContain uses WebDriverWait internally — replaces Thread.Sleep(3000)
            Assert.IsTrue(_driverManager.DoesUrlContain("selenium-ide"),
                $"Expected url was {expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_Chrome()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

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
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

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
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                _driverManager.SendText(googleSearchInput, text);
                Assert.Fail("Expected WebAutomationException was not thrown");
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
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

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
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

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
        //    SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

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
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                _driverManager.SendTextWithDelay(googleSearchInput, text);
                Assert.Fail("Expected WebAutomationException was not thrown");
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
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

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
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

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
            SelectorData googleSearchInput = new SelectorData("SearchInput", HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.CreateDriverInstance(DriverType.Chrome);
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsNotNull(_driverManager, "Driver was null");

            _driverManager.QuitDriverInstance(DriverType.Chrome);

            try
            {
                _driverManager.ClearText(googleSearchInput);
                Assert.Fail("Expected WebAutomationException was not thrown");
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
            Assert.IsTrue((element!.Displayed && element!.Enabled), "Target element should be on screen");
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
            Assert.IsTrue((element!.Displayed && element!.Enabled), "Target element should be on screen");
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
                Assert.Fail("Expected WebAutomationException was not thrown");
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
                Assert.Fail("Expected WebAutomationException was not thrown");
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
                Assert.Fail("Expected WebAutomationException was not thrown");
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_driverTypeNoneException, message, "The expected null driver exception was not found");
            }
        }

        #endregion

        // ── Driver_Interaction_Tests_Headless REMOVED ─────────────────────────────────
        // Moved to ChromeHeadlessInteractionTests and FirefoxHeadlessInteractionTests.
        // Each class uses [ClassInitialize]/[ClassCleanup] to share a single browser
        // instance across all tests in the category (P1 driver-reuse optimisation).

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
            var attributeValue = element!.GetAttribute("method");
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
            var attributeValue = element!.GetAttribute("method");
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
            var attributeValue = element!.GetAttribute("method");
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
            var attributeValue = element!.GetAttribute("method");
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
            var attributeValue = element!.GetAttribute("id");
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
            var attributeValue = element!.GetAttribute("id");
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
            var attributeValue = element!.GetAttribute("class");
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
            var attributeValue = element!.GetAttribute("class");
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
            var attributeValue = element!.GetAttribute("id");
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
            var attributeValue = element!.GetAttribute("id");
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
            var attributeValue = element!.GetAttribute("class");
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
            var attributeValue = element!.GetAttribute("class");
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

        // ── Element_Location_Method_Tests_Headless REMOVED ───────────────────────────
        // Moved to ChromeHeadlessElementLocationTests and FirefoxHeadlessElementLocationTests.
        // Each class uses [ClassInitialize]/[ClassCleanup] to share a single browser
        // instance across all tests in the category (P1 driver-reuse optimisation).
    }
}
