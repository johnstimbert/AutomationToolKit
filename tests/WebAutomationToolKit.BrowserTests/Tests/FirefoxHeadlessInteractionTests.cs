using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;
using WebAutomationToolKit;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.BrowserTests
{
    /// <summary>
    /// Firefox headless driver-interaction tests.
    ///
    /// P1 driver-reuse: a single Firefox headless process is created in
    /// <see cref="ClassSetup"/> and destroyed in <see cref="ClassTeardown"/>,
    /// eliminating the ~2–4 s browser spawn/teardown cost that was previously
    /// paid on every test.  <see cref="ResetBrowserState"/> navigates to
    /// about:blank before each test so no test inherits DOM state from its
    /// predecessor.
    /// </summary>
    [TestClass]
    [TestCategory("Browser")]
    [TestCategory("Driver_Interaction_Tests_Headless")]
    public class FirefoxHeadlessInteractionTests
    {
        // ── shared state ────────────────────────────────────────────────────
        private static WebAutomation _webAutomation = null!;
        private static IWebDriverManager _driverManager = null!;

        private readonly string _navUrlGoogle      = "https://www.google.com/";
        private readonly string _interactionTestUrl = "https://www.selenium.dev/";
        private readonly string _screenShotPath    = @"e:\ScreenShots";
        private readonly string _startingUrl       = "https://www.selenium.dev/";
        private readonly string _expectedUrl       = "https://www.selenium.dev/selenium-ide/";

        private readonly SelectorData _clickTarget =
            new SelectorData("ReadMore", HtmlTagType.a, HtmlAttributeType.Href,
                             "https://selenium.dev/selenium-ide/");

        private static readonly SelectorData _divById =
            new SelectorData("divById", HtmlTagType.div, HtmlAttributeType.Id, "main_navbar");

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
        /// Resets browser state before every test.
        /// P0: NavigateWithActiveDriver no longer carries an unconditional 2 s sleep,
        /// so this reset is now essentially free.
        /// </summary>
        [TestInitialize]
        public void ResetBrowserState()
        {
            _driverManager.NavigateWithActiveDriver("about:blank");
        }

        // ── tests ───────────────────────────────────────────────────────────

        #region Driver_Interaction_Tests_Headless

        [TestMethod]
        [Ignore] // TODO: Update Screenshots
        public void TakeScreenShot_Firefox_HeadlessMode()
        {
            string shotName = "FireFoxShot";
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.TakeScreenShot(_screenShotPath, shotName);

            var dirInfo  = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        public void Click_Firefox_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_startingUrl);
            _driverManager.Click(_clickTarget);
            // P1: DoesUrlContain uses WebDriverWait — replaces Thread.Sleep(3000)
            Assert.IsTrue(_driverManager.DoesUrlContain("selenium-ide"),
                $"Expected url was {_expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        public void SendText_Firefox_HeadlessMode()
        {
            var googleSearchInput = new SelectorData("SearchInput",
                HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.SendText(googleSearchInput, "ChromeShot");
        }

        [TestMethod]
        public void SendTextWithDelay_Firefox_HeadlessMode()
        {
            var googleSearchInput = new SelectorData("SearchInput",
                HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.SendTextWithDelay(googleSearchInput, "ChromeShot");
        }

        [TestMethod]
        public void Clear_Firefox_HeadlessMode()
        {
            var googleSearchInput = new SelectorData("SearchInput",
                HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.SendText(googleSearchInput, "ChromeShot");
            _driverManager.ClearText(googleSearchInput);
        }

        [TestMethod]
        public void MoveToElement_Firefox_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_interactionTestUrl);
            _driverManager.MoveToElement(_divById);
            var element = _driverManager.CheckElementExistsReturnIWebElement(_divById);
            Assert.IsTrue(element!.Displayed && element.Enabled,
                "Target element should be on screen");
        }

        [TestMethod]
        public void DoesUrlContain_Firefox_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsTrue(_driverManager.DoesUrlContain("google"));
        }

        [TestMethod]
        public void DoesUrlContainUsingRegex_Firefox_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("google"));
        }

        #endregion
    }
}
