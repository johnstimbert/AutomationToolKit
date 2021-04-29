using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using WebUiAutomationToolKit.DriverFactory;
using WebUiAutomationToolKit.Exceptions;
using WebUiAutomationToolKit.Extensions;
using WebUiAutomationToolKit.Helpers;
using WebUiAutomationToolKit.Validators;
using WebUiAutomationToolKit.WebAndApiAutomationObjects;
using static WebUiAutomationToolKit.WebUiAutomationEnums;

namespace WebUiAutomationToolKit
{
    /// <summary>
    /// Concrete implementation of IWebDriverManager interface to manage web drivers
    /// </summary>
    internal sealed class WebDriverManager : IWebDriverManager, IJavaScriptExecutor, IWrapsDriver
    {
        #region Constants
        private const string _chromeDriverName = "chromedriver.exe";
        private const string _firefoxDriverName = "geckodriver.exe";
        private const string _ieDriverName = "IEDriverServer.exe";
        private const string _edgeDriverName = "MicrosoftWebDriver.exe";

        #endregion

        private Dictionary<DriverType, IWebDriver> _drivers;
        private WebDriverWait _wait;
        private DriverType _activeDriver = DriverType.None;

        private readonly int _timeoutForWait;
        private string _driverPath;
        private bool _demoMode = false;

        private ChromeOptions _chromeOptions;
        private FirefoxOptions _firefoxOptions;
        private InternetExplorerOptions _internetExplorerOptions;
        private EdgeOptions _edgeOptions;

        private FirefoxProfile _firefoxProfile;

        private StructureValidator _structureValidator;


        #region Constructors
        private WebDriverManager() { }

        internal WebDriverManager(int timeoutForWaitOverride, string driverPath = null)
        {
            if (driverPath != null)
            {
                _driverPath = driverPath;
            }
            else
            {
                _driverPath = $"{Directory.GetCurrentDirectory()}\\";
            }

            _timeoutForWait = timeoutForWaitOverride;
            _structureValidator = new StructureValidator();
        }

        #endregion

        #region IWebDriverManager Methods

        /// <summary>
        /// Enables or disables DemoMode. If enabled each action will be preceded by a 2 second delay
        /// </summary>
        /// <param name="setting">true = on/false = off</param>
        public void EnableDemoMode(bool setting)
        {
            _demoMode = setting;
        }
        /// <summary>
        /// Returns the currently active IWebDriver object
        /// </summary>
        /// <remarks>
        /// This should not be utilized for testing and is provided for troubleshooting any issues found while using this package.
        /// </remarks>
        public IWebDriver WrappedDriver => GetActiveDriver();
        /// <summary>
        /// Sets the options for the associated driver. If the driver is active when this method is called it will be recreated
        /// </summary>
        /// <param name="chromeOptions"></param>
        /// <param name="firefoxOptions"></param>
        /// <param name="internetExplorerOptions"></param>
        /// <param name="edgeOptions"></param>
        public void SetDriverOptions(ChromeOptions chromeOptions = null, FirefoxOptions firefoxOptions = null,
            InternetExplorerOptions internetExplorerOptions = null, EdgeOptions edgeOptions = null)
        {
            _chromeOptions = chromeOptions;
            _firefoxOptions = firefoxOptions;
            _internetExplorerOptions = internetExplorerOptions;
            _edgeOptions = edgeOptions;

            //Check if any of the drivers are active. 
            //If they are and the options objcts are not null, quit the existing instance and create a new one.
            if (HasInstance(DriverType.Chrome) && _chromeOptions != null)
            {
                _drivers[DriverType.Chrome].Quit();
                _drivers[DriverType.Chrome] = GetDriver(DriverType.Chrome);
            }

            if (HasInstance(DriverType.Firefox) && _firefoxOptions != null)
            {
                _drivers[DriverType.Firefox].Quit();
                _drivers[DriverType.Firefox] = GetDriver(DriverType.Firefox);
            }
        }
       
        /// <summary>
        /// Sets the profile for the firefox driver. If the driver is active when this method is called it will be recreated
        /// </summary>
        /// <param name="firefoxProfile"></param>
        public void SetFirefoxProfile(FirefoxProfile firefoxProfile)
        {
            _firefoxProfile = firefoxProfile;

            if (HasInstance(DriverType.Firefox) && _firefoxProfile != null)
            { 
                _drivers[DriverType.Firefox].Quit();
                _drivers[DriverType.Firefox] = GetDriver(DriverType.Firefox);
            }
        }
       
        /// <summary>
        /// Creates an instance of IWebDriver that matches the type provided 
        /// </summary>
        /// <param name="driverType">Type of the driver instance to create</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void CreateDriverInstance(DriverType driverType)
        {
            try
            {
                if (_drivers == null)
                    _drivers = new Dictionary<DriverType, IWebDriver>();

                if (!HasInstance(driverType))
                    _drivers.Add(driverType, GetDriver(driverType));

                _activeDriver = driverType;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Quits the instance of IWebDriver that matches the type provided
        /// </summary>
        /// <param name="driverType">Type of the driver instance to quit</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void QuitDriverInstance(DriverType driverType)
        {
            try
            {
                if(!HasInstance(driverType))
                    throw new WebUiAutomationException($"Manager does not contain an instance of DriverType {driverType}. " +
                        $"Call CreateDriverInstance to create a new driver instance");

                _drivers[driverType].Quit();
                _drivers.Remove(driverType);

                if (_drivers.Count == 0)
                    _activeDriver = DriverType.None;

            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Selects the driver to mark as active. If the DriverType provided does not have an instance already created, one will be created and marked as the active driver. 
        /// </summary>
        /// <param name="driverType"></param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void SetActiveDriver(DriverType driverType)
        {
            try
            {
                if (!HasInstance(driverType))
                    CreateDriverInstance(driverType);

                _activeDriver = driverType;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Returns the DriverType value indicating what driver is currently marked as active. The active driver is used for execution.
        /// </summary>
        /// <returns>DriverType</returns>
        public DriverType GetActiveDriverType()
        {
            try
            {
                return _activeDriver;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        /// <returns></returns>
        public string GetActiveDriverPageSource()
        {
            try
            {
                return _drivers[_activeDriver].PageSource;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Gets the title of the current browser window.
        /// </summary>
        /// <returns></returns>
        public string GetActiveDriverTitle()
        {
            try
            {
                return _drivers[_activeDriver].Title;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the URL the browser is currently displaying.
        /// </summary>
        /// <returns></returns>
        public string GetActiveDriverUrl()
        {
            try
            {
                return _drivers[_activeDriver].Url;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Gets the current window handle, which is an opaque handle to this window that uniquely identifies it within this driver instance.
        /// </summary>
        /// <returns></returns>
        public string GetActiveDriverCurrentWindowHandle()
        {
            try
            {
                return _drivers[_activeDriver].CurrentWindowHandle;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Gets the window handles of open browser windows.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<string> GetActiveDriverWindowHandles()
        {
            try
            {
                return _drivers[_activeDriver].WindowHandles;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Instructs the driver to navigate the browser to another location.
        /// </summary>
        /// <param name="url"></param>
        public void NavigateWithActiveDriver(string url)
        {
            if(_demoMode)
                Thread.Sleep(2000);
            try
            {
                _drivers[_activeDriver].Navigate().GoToUrl(url);
                Thread.Sleep(2000);                
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Closes the farthest tab to the right in the current browser window
        /// </summary>
        /// <param name="mainWindow"></param>
        public void CloseLastTabWithActiveDriver(string mainWindow)
        {
            if (_demoMode)
                Thread.Sleep(2000);
            try
            {
                _drivers[_activeDriver].SwitchTo().Window(_drivers[_activeDriver].WindowHandles.Last()).Close();
                _drivers[_activeDriver].SwitchTo().Window(mainWindow);
                Thread.Sleep(1000);
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Closes the tab designated in the tabToClose parameter and switches the context to the tab designated in the targetTab parameter
        /// </summary>
        /// <param name="tabToClose"></param>
        /// <param name="targetTab"></param>
        public void CloseTabWithActiveDriver(string tabToClose, string targetTab)
        {
            if (_demoMode)
                Thread.Sleep(2000);
            try
            {
                _drivers[_activeDriver].SwitchTo().Window(tabToClose).Close();
                _drivers[_activeDriver].SwitchTo().Window(targetTab);
                Thread.Sleep(1000);
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Switches the context to the farthest tab to the right in the current browser window
        /// </summary>
        public void SwitchToLastTabWithActiveDriver()
        {
            if (_demoMode)
                Thread.Sleep(2000);
            try
            {
                _drivers[_activeDriver].SwitchTo().Window(_drivers[_activeDriver].WindowHandles.Last());
                if (_drivers[_activeDriver] is InternetExplorerDriver)
                    _drivers[_activeDriver].Manage().Window.Maximize();

                Thread.Sleep(1000);
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }            
        }

        /// <summary>
        /// Closes all tabs except for the one currently with focus
        /// </summary>
        public void CloseAllTabsExceptCurrentWithActiveDriver()
        {
            if (_demoMode)
                Thread.Sleep(2000);
            try
            {
                var currentWindow = _drivers[_activeDriver].CurrentWindowHandle;
                var windows = _drivers[_activeDriver].WindowHandles.Where(w => !w.Equals(currentWindow));
                foreach (var win in windows)
                    CloseTabWithActiveDriver(win, currentWindow);

                Thread.Sleep(1000);
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }
        /// <summary>
        /// Switches the driver context to the frame matching the SelectorData provided
        /// </summary>
        /// <param name="selectorData"></param>
        public void SwitchToFrame(SelectorData selectorData)
        {
            try
            {
                var activeDriver = _drivers[_activeDriver];
                var structureValidator = new StructureValidator();
                var frameElement = structureValidator.CheckElementExistsReturnIWebElement(selectorData, activeDriver, _wait);

                activeDriver.SwitchTo().Frame(frameElement);
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }
        /// <summary>
        /// Executes JavaScript in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        /// <remarks>
        /// The OpenQA.Selenium.IJavaScriptExecutor.ExecuteScript(System.String,System.Object[])method
        ///    executes JavaScript in the context of the currently selected frame or window.
        ///    This means that "document" will refer to the current document. If the script
        ///    has a return value, then the following steps will be taken:
        ///    • For an HTML element, this method returns a OpenQA.Selenium.IWebElement
        ///    • For a number, a System.Int64 is returned
        ///    • For a boolean, a System.Boolean is returned
        ///    • For all other cases a System.String is returned.
        ///    • For an array,we check the first element, and attempt to return a System.Collections.Generic.List`1
        ///    of that type, following the rules above. Nested lists are not supported.
        ///    • If the value is null or there is no return value, null is returned.
        ///    Arguments must be a number (which will be converted to a System.Int64), a System.Boolean,
        ///    a System.String or a OpenQA.Selenium.IWebElement. An exception will be thrown
        ///    if the arguments do not meet these criteria. The arguments will be made available
        ///    to the JavaScript via the "arguments" magic variable, as if the function were
        ///    called via "Function.apply"</remarks>
        public object ExecuteScript(string script, params object[] args)
        {
            try
            {
                if (_demoMode)
                    Thread.Sleep(2000);
                return Helper.JavaScriptExecutor(_drivers[_activeDriver]).ExecuteScript(script, args);
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }
        /// <summary>
        /// Executes JavaScript asynchronously in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        public object ExecuteAsyncScript(string script, params object[] args)
        {
            try
            {
                if (_demoMode)
                    Thread.Sleep(2000);
                return Helper.JavaScriptExecutor(_drivers[_activeDriver]).ExecuteAsyncScript(script, args);
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Takes a screenshot and saves it in the provided path. THe file name is in the form of "{testMethodName}_{DateTime.Now}.jpeg"
        /// </summary>
        /// <param name="sreenShotPath">Path to save the screenshot to</param>
        /// <param name="screenShotName">Name of the screenshot file</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void TakeScreenShot(string sreenShotPath, string screenShotName)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                Helper.TakeScreenShot(manager.GetActiveDriver(), sreenShotPath, screenShotName);
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Takes a screenshot of an element in the current browsing context and saves it in the provided path. 
        /// The file name is in the form of "{testMethodName}_{DateTime.Now}.jpeg"
        /// </summary>
        /// <param name="sreenShotPath">Path to save the screenshot to</param>
        /// <param name="screenShotName">Name of the screenshot file</param>
        /// <param name="selectorData">Element selector data to find screenshot target</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void TakeElementScreenShot(string sreenShotPath, string screenShotName, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                var element = CheckElementExistsReturnIWebElement(selectorData);
                Helper.TakeElementScreenShot(sreenShotPath, screenShotName, element);
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Modifies the style associated with the selector data object provided to highlight the element on the page
        /// </summary>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void HighlightElement(SelectorData selectorData)
        {
            if (_demoMode)
                Thread.Sleep(2000);
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                Helper.HighlightElement(manager.GetActiveDriver(), cssBy);
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clicks the element associated with the selector data object provided using a JavaScript query.
        /// This is useful when the element being clicked may be obstructed
        /// </summary>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void JavaScriptClick(SelectorData selectorData)
        {
            if (_demoMode)
                Thread.Sleep(2000);
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                Helper.ClickUsingJavaScript(manager.GetActiveDriver(), cssBy);
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clicks the provided element after verifying it exists and is clickable
        /// </summary>
        /// <param name="selectorData">Object representing the element to click</param>
        /// <remarks>This method will retry the click once after waiting 2 seconds after the first ElementClickInterceptedException is thrown</remarks>
        /// <exception cref="WebUiAutomationException"></exception>
        public void Click(SelectorData selectorData)
        {
            if (_demoMode)
                Thread.Sleep(2000);
            var manager = Helper.IsDriverNull(this);
            try
            {
                try
                {
                    var clickTarget = _structureValidator.CheckElementExistsReturnIWebElement(selectorData, manager.GetActiveDriver(), manager.GetWait());
                    clickTarget.Click();
                }
                catch (ElementClickInterceptedException)
                {
                    Thread.Sleep(2000);
                    var clickTarget = _structureValidator.CheckElementExistsReturnIWebElement(selectorData, manager.GetActiveDriver(), manager.GetWait());
                    clickTarget.Click();
                }
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Enters text into the provided element after verifying it exists and is visible
        /// </summary>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void SendText(SelectorData selectorData, string textToEnter)
        {
            if (_demoMode)
                Thread.Sleep(2000);
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.CheckElementExistsReturnCssSelector(selectorData, manager.GetActiveDriver(), manager.GetWait());
                manager.GetActiveDriver().MoveToElement(cssBy, manager.GetWait());
                WebDriverExtension.IsDisplayedAndEnabled(manager.GetActiveDriver(), cssBy, manager.GetWait());
                manager.GetActiveDriver().SendText(cssBy, manager.GetWait(), textToEnter);
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Enters text into the provided element one character at a time with a delay between each after verifying it exists and is visible.
        /// Note: Only use this method to send text to elements that have autocomplete and require a delay on each letter typed.
        /// </summary>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <param name="delay">Interval before each character is entered</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void SendTextWithDelay(SelectorData selectorData, string textToEnter, int delay = 500)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                manager.GetActiveDriver().SendTextWithDelay(cssBy, manager.GetWait(), textToEnter, delay);
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="selectorData">Object representing the element to be cleared</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public void ClearText(SelectorData selectorData)
        {
            if (_demoMode)
                Thread.Sleep(2000);
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                manager.GetActiveDriver().Clear(cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="selectorData">Object representing the element to move to</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public void MoveToElement(SelectorData selectorData)
        {
            if (_demoMode)
                Thread.Sleep(2000);
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                //Needed as a workaround for a bug in the geckodriver(firefox)
                if (manager.GetActiveDriverType() == DriverType.Firefox)
                {
                    manager.GetActiveDriver().WaitForElementExists(cssBy, manager.GetWait());
                    manager.GetActiveDriver().FindElement(cssBy);
                }
                else
                {
                    IJavaScriptExecutor executor = (IJavaScriptExecutor)manager.GetActiveDriver();
                    executor.ExecuteScript("arguments[0].scrollIntoView()", manager.GetActiveDriver().FindElement(cssBy));
                }
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is visible
        /// </summary>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns></returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public bool IsElementVisible(SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return WebDriverExtension.IsDisplayed(manager.GetActiveDriver(), cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is enabled
        /// </summary>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns></returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public bool IsElementEnabled(SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return WebDriverExtension.IsEnabled(manager.GetActiveDriver(), cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is displayed
        /// </summary>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns></returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public bool IsElementDisplayed(SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return WebDriverExtension.IsEnabled(manager.GetActiveDriver(), cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is displayed and enabled
        /// </summary>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns></returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public bool IsElementDisplayedAndEnabled(SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return WebDriverExtension.IsDisplayedAndEnabled(manager.GetActiveDriver(), cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the current url contains the provided text
        /// </summary>
        /// <param name="text">Text to look for in the current Url</param>
        /// <returns></returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public bool DoesUrlContain(string text)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                return manager.GetActiveDriver().WaitForUrlContains(text, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the current url contains the provided pattern using regex
        /// </summary>
        /// <param name="pattern">Text to look for in the current Url</param>
        /// <returns></returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public bool DoesUrlContainUsingRegex(string pattern)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                return manager.GetActiveDriver().WaitForUrlRegexContains(pattern, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds and returns the IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public IWebElement CheckElementExistsReturnIWebElement(SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                return _structureValidator.CheckElementExistsReturnIWebElement(selectorData, manager.GetActiveDriver(), manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds the IWebElement using the parameters provided and returns the CssSelector based By object, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <returns>By.CssSelector</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public By CheckElementExistsReturnCssSelector(SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                return _structureValidator.CheckElementExistsReturnCssSelector(selectorData, manager.GetActiveDriver(), manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Builds a CssSelector with the SelectorDataSet provided and uses it to check that an element exists with that data
        /// </summary>
        /// <param name="selectorDataSet">Data to check for in the current DOM instance</param>
        /// <returns>bool</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public bool CheckElementsExist(SelectorDataSet selectorDataSet)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                return _structureValidator.CheckElementsExist(selectorDataSet, manager.GetActiveDriver(), manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds and returns the child IWebElement using the parameters provided, if none is found null is returned.
        /// If multiple elements match the SelectorData definition utilize the nthParentElement parameter to select the desired parent
        /// </summary>
        /// <param name="parentSelectorData">Data to find the parent element with</param>
        /// <param name="childSelectorData">>Data to find the child element with</param>
        /// <param name="nthParentElement">The Zero based position of the parent element to search with, this is optional</param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public IWebElement CheckChildElementExistsAndReturnIt(SelectorData parentSelectorData, SelectorData childSelectorData, int nthParentElement = -1)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                IWebElement parentElement = null;
                if (nthParentElement > -1)
                {
                    parentElement = _structureValidator.Check_Nth_ElementExistsReturnIWebElement(parentSelectorData, nthParentElement, manager.GetActiveDriver(), manager.GetWait());
                }
                else
                {
                    parentElement = CheckElementExistsReturnIWebElement(parentSelectorData);
                }

                if (parentElement == null)
                    return parentElement;

                return _structureValidator.CheckChildElementExists(parentElement, childSelectorData, manager.GetActiveDriver());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds and returns all elements matching the provided selector data 
        /// </summary>
        /// <param name="selectorData">Data to check for in the current DOM instance</param>
        /// <returns>ReadOnlyCollectionIWebElement</returns>
        public ReadOnlyCollection<IWebElement> GetAllElementsUsingMatchingSelectorData(SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(this);
            try
            {
                return _structureValidator.GetAllBysUsingMatchingSelectorData(selectorData, manager.GetActiveDriver());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }
        #endregion

        #region Internal Methods
        internal IWebDriver GetActiveDriver()
        {
            try
            {
                return _drivers[_activeDriver];
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        internal WebDriverWait GetWait()
        {
            try
            {
                return _wait;
            }
            catch (WebUiAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        #endregion

        #region Private Methods
        private IWebDriver GetDriver(DriverType driverType)
        {
            try
            {
                IWebDriver webDriver = null;

                switch (driverType)
                {
                    case DriverType.Chrome:
                        if (!File.Exists(Path.Combine(_driverPath, _chromeDriverName)))
                            throw new WebUiAutomationException($"The driver {_chromeDriverName} was not found in the path {_driverPath}");

                        webDriver = ChromeDriverManager.Create_WebDriver_Instance(_driverPath, _chromeOptions);
                        break;
                    case DriverType.Firefox:
                        if (!File.Exists(Path.Combine(_driverPath, _firefoxDriverName)))
                            throw new WebUiAutomationException($"The driver {_firefoxDriverName} was not found in the path {_driverPath}");

                        webDriver = FirefoxDriverManager.Create_WebDriver_Instance(_driverPath, _firefoxOptions, _firefoxProfile);
                        break;
                }

                _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_timeoutForWait));

                return webDriver;
            }
            catch (DriverServiceNotFoundException dsnf)
            {
                throw new WebUiAutomationException(dsnf.ToString());
            }
        }

        private bool HasInstance(DriverType driverType)
        {
            if (_drivers == null || !_drivers.Keys.Contains(driverType))
                return false;

            return true;
        }
        #endregion
    }
}
