using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using WebAutomationToolKit;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.BrowserTests
{
    /// <summary>
    /// Chrome headless driver-interaction tests.
    ///
    /// P1 driver-reuse: a single Chrome headless process is created in
    /// <see cref="ClassSetup"/> and destroyed in <see cref="ClassTeardown"/>,
    /// eliminating the ~0.7–1 s browser spawn/teardown cost that was previously
    /// paid on every test.  <see cref="ResetBrowserState"/> navigates to
    /// about:blank before each test so no test inherits DOM state from its
    /// predecessor.
    /// </summary>
    [TestClass]
    [TestCategory("Browser")]
    [TestCategory("Driver_Interaction_Tests_Headless")]
    public class ChromeHeadlessInteractionTests
    {
        // ── shared state ────────────────────────────────────────────────────
        private static WebAutomation _webAutomation = null!;
        private static IWebDriverManager _driverManager = null!;

        private readonly string _navUrlGoogle     = "https://www.google.com/";
        private readonly string _screenShotPath   = @"e:\ScreenShots";
        private readonly string _startingUrl      = "https://www.selenium.dev/";
        private readonly string _expectedUrl      = "https://www.selenium.dev/selenium-ide/";

        private readonly SelectorData _clickTarget =
            new SelectorData("ReadMore", HtmlTagType.a, HtmlAttributeType.Href,
                             "https://selenium.dev/selenium-ide/");

        // ── lifecycle ───────────────────────────────────────────────────────

        /// <summary>
        /// P1: One Chrome headless driver for the whole class.
        /// Saves ~0.7–1 s × (test count − 1) compared with per-test creation.
        /// </summary>
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            _webAutomation = new WebAutomation(null, 10);
            _driverManager = _webAutomation.GetIWebDriverManager();

            var options = new ChromeOptions();
            options.AcceptInsecureCertificates = true;
            options.UnhandledPromptBehavior =
                OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify;
            options.AddArgument("--headless");

            _driverManager.SetDriverOptions(chromeOptions: options);
            _driverManager.CreateDriverInstance(DriverType.Chrome);
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            if (_driverManager.GetActiveDriverType() != DriverType.None)
                _driverManager.QuitDriverInstance(DriverType.Chrome);
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
        public void TakeScreenShot_Chrome_HeadlessMode()
        {
            string shotName = "ChromeShot";
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.TakeScreenShot(_screenShotPath, shotName);

            var dirInfo  = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [Ignore] // TODO: Update Screenshots
        public void Click_Chrome_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_startingUrl);
            _driverManager.Click(_clickTarget);
            // P1: DoesUrlContain uses WebDriverWait — replaces Thread.Sleep(3000)
            Assert.IsTrue(_driverManager.DoesUrlContain("selenium-ide"),
                $"Expected url was {_expectedUrl}, found {_driverManager.GetActiveDriverUrl()}");
        }

        [TestMethod]
        public void SendText_Chrome_HeadlessMode()
        {
            var googleSearchInput = new SelectorData("SearchInput",
                HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.SendText(googleSearchInput, "ChromeShot");
        }

        [TestMethod]
        public void SendTextWithDelay_Chrome_HeadlessMode()
        {
            var googleSearchInput = new SelectorData("SearchInput",
                HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.SendTextWithDelay(googleSearchInput, "ChromeShot");
        }

        [TestMethod]
        public void Clear_Chrome_HeadlessMode()
        {
            var googleSearchInput = new SelectorData("SearchInput",
                HtmlTagType.textarea, HtmlAttributeType.Name, "q");

            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            _driverManager.SendText(googleSearchInput, "ChromeShot");
            _driverManager.ClearText(googleSearchInput);
        }

        [TestMethod]
        public void DoesUrlContain_Chrome_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsTrue(_driverManager.DoesUrlContain("google"));
        }

        [TestMethod]
        public void DoesUrlContainUsingRegex_Chrome_HeadlessMode()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle);
            Assert.IsTrue(_driverManager.DoesUrlContainUsingRegex("google"));
        }

        /// <summary>
        /// P0 smoke-test: verifies the new <c>postNavigationWaitMs</c> parameter
        /// completes without error and lands on the expected URL.
        /// An explicit 500 ms post-navigation delay is passed to exercise the opt-in path.
        /// </summary>
        [TestMethod]
        public void NavigateWithActiveDriver_PostWaitMs_NavigatesSuccessfully()
        {
            _driverManager.NavigateWithActiveDriver(_navUrlGoogle, 500);
            Assert.IsTrue(
                _driverManager.GetActiveDriverUrl().StartsWith(_navUrlGoogle),
                $"Expected url starting with {_navUrlGoogle}, " +
                $"found {_driverManager.GetActiveDriverUrl()}");
        }

        #endregion
    }
}
