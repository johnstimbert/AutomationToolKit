using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;
using WebAutomationToolKit;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.BrowserTests
{
    /// <summary>
    /// Firefox headless element-location tests.
    ///
    /// P1 driver-reuse: a single Firefox headless process is created in
    /// <see cref="ClassSetup"/> and destroyed in <see cref="ClassTeardown"/>.
    /// <see cref="ResetBrowserState"/> rebuilds the shared <see cref="_divTags"/>
    /// selector set and navigates to about:blank before each test, preventing
    /// both DOM-state and selector-state leakage between tests.
    /// </summary>
    [TestClass]
    [TestCategory("Browser")]
    [TestCategory("Element_Location_Method_Tests_Headless")]
    public class FirefoxHeadlessElementLocationTests
    {
        // ── shared state ────────────────────────────────────────────────────
        private static WebAutomation _webAutomation = null!;
        private static IWebDriverManager _driverManager = null!;

        private readonly string _navUrlGoogle      = "https://www.google.com/";
        private readonly string _navUrlSeleniumDev = "https://www.selenium.dev/";

        private static readonly SelectorData _divById =
            new SelectorData("divById", HtmlTagType.div,
                             HtmlAttributeType.Id, "main_navbar");

        private static readonly SelectorData _divByClass =
            new SelectorData("divByClass", HtmlTagType.div,
                             HtmlAttributeType.Class, "container-fluid td-default td-outer");

        private static readonly SelectorData _divTagTypeOnly =
            new SelectorData("divTagTypeOnly", HtmlTagType.div,
                             HtmlAttributeType.None, null!);

        // Reset per-test in ResetBrowserState so mutations in one test don't bleed into the next
        private SelectorDataSet _divTags = null!;

        // ── lifecycle ───────────────────────────────────────────────────────

        /// <summary>
        /// P1: One Firefox headless driver for the whole class.
        /// Saves ~2–4 s × (test count − 1) compared with per-test creation.
        /// </summary>
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            _webAutomation = new WebAutomation(null, 10);
            _driverManager = _webAutomation.GetIWebDriverManager();

            var profile = new FirefoxProfile { DeleteAfterUse = true };
            var options = new FirefoxOptions
            {
                AcceptInsecureCertificates = true,
                UnhandledPromptBehavior =
                    OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify,
            };
            options.Profile = profile;
            options.AddArgument("--headless");

            _driverManager.SetDriverOptions(firefoxOptions: options);
            _driverManager.CreateDriverInstance(DriverType.Firefox);
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            if (_driverManager.GetActiveDriverType() != DriverType.None)
                _driverManager.QuitDriverInstance(DriverType.Firefox);
        }

        /// <summary>
        /// Rebuilds <see cref="_divTags"/> and resets browser state before every test.
        /// Rebuilding guarantees that tests which call RemoveSelectorDataByName do not
        /// affect subsequent tests in the class.
        /// P0: NavigateWithActiveDriver no longer carries an unconditional 2 s sleep,
        /// so this reset is now essentially free.
        /// </summary>
        [TestInitialize]
        public void ResetBrowserState()
        {
            _divTags = new SelectorDataSet(HtmlTagType.div);
            _divTags.AddSelectorDataByName(_divById);
            _divTags.AddSelectorDataByName(_divByClass);
            _divTags.AddSelectorDataByName(_divTagTypeOnly);

            _driverManager.NavigateWithActiveDriver("about:blank");
        }

        // ── tests ───────────────────────────────────────────────────────────

        #region Element_Location_Method_Tests_Headless

        [TestMethod]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Firefox_HeadlessMode()
        {
            SelectorData data = new SelectorData("GoogleForm",
                HtmlTagType.form, HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            var attributeValue = element!.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue,
                $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Firefox_HeadlessMode()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm",
                HtmlTagType.form, HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            var element = _driverManager.CheckElementExistsReturnIWebElement(data);
            var attributeValue = element!.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue,
                $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        public void CheckElementExistsReturnIWebElement_ById_Firefox_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(_divById);
            var attributeValue = element!.GetAttribute("id");
            Assert.AreEqual(_divById.AttributeValue, attributeValue,
                $"Expected attribute value was {_divById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        public void CheckElementExistsReturnIWebElement_ByClass_Firefox_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(_divByClass);
            var attributeValue = element!.GetAttribute("class");
            Assert.AreEqual(_divByClass.AttributeValue, attributeValue,
                $"Expected attribute value was {_divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        public void CheckElementExistsReturnCssSelector_ById_Firefox_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(_divById);
            var attributeValue = element!.GetAttribute("id");
            Assert.AreEqual(_divById.AttributeValue, attributeValue,
                $"Expected attribute value was {_divById.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        public void CheckElementExistsReturnCssSelector_ByClass_Firefox_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            var element = _driverManager.CheckElementExistsReturnIWebElement(_divByClass);
            var attributeValue = element!.GetAttribute("class");
            Assert.AreEqual(_divByClass.AttributeValue, attributeValue,
                $"Expected attribute value was {_divByClass.AttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        public void CheckElementsExist_ById_Firefox_HeadlessMode()
        {
            // Remove selectors that are not present on selenium.dev to avoid false negatives
            _divTags.RemoveSelectorDataByName(_divByClass.Name);
            _divTags.RemoveSelectorDataByName(_divTagTypeOnly.Name);

            _driverManager.NavigateWithActiveDriver(_navUrlSeleniumDev);
            Assert.IsTrue(_driverManager.CheckElementsExist(_divTags));
        }

        #endregion
    }
}
