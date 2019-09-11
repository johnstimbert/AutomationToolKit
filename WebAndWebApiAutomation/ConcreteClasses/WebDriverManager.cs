using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using WebAndWebApiAutomation.DriverFactory;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.Validators;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation
{
    /// <summary>
    /// Concrete implementation of IWebDriverManager interface to manage web drivers
    /// </summary>
    internal sealed class WebDriverManager : IWebDriverManager
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

        private ChromeOptions _chromeOptions;
        private FirefoxOptions _firefoxOptions;
        private InternetExplorerOptions _internetExplorerOptions;
        private EdgeOptions _edgeOptions;

        private FirefoxProfile _firefoxProfile;


        #region Constructors
        private WebDriverManager() { }

        internal WebDriverManager(string driverPath, int timeoutForWaitOverride)
        {
            _driverPath = driverPath;
            _timeoutForWait = timeoutForWaitOverride;
        }

        #endregion

        #region IWebDriverManager Methods
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

            if (HasInstance(DriverType.Edge) && _edgeOptions != null)
            {
                _drivers[DriverType.Edge].Quit();
                _drivers[DriverType.Edge] = GetDriver(DriverType.Edge);
            }

            if (HasInstance(DriverType.InternetExplorer) && _internetExplorerOptions != null)
            {
                _drivers[DriverType.InternetExplorer].Quit();
                _drivers[DriverType.InternetExplorer] = GetDriver(DriverType.InternetExplorer);
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
        /// <exception cref="WebAutomationException"></exception>
        public void CreateDriverInstance(DriverType driverType)
        {
            try
            {
                if (_drivers == null)
                    _drivers = new Dictionary<DriverType, IWebDriver>();

                if (HasInstance(driverType))
                    throw new WebAutomationException($"Manager already contains an instance of DriverType {driverType}. " +
                        $"Call QuitDriverInstance then create a new instance or use the existing driver instance");

                _drivers.Add(driverType, GetDriver(driverType));
                _activeDriver = driverType;
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Quits the instance of IWebDriver that matches the type provided
        /// </summary>
        /// <param name="driverType">Type of the driver instance to quit</param>
        /// <exception cref="WebAutomationException"></exception>
        public void QuitDriverInstance(DriverType driverType)
        {
            try
            {
                if(!HasInstance(driverType))
                    throw new WebAutomationException($"Manager does not contain an instance of DriverType {driverType}. " +
                        $"Call CreateDriverInstance to create a new driver instance");

                _drivers[driverType].Quit();
                _drivers.Remove(driverType);

                if (_drivers.Count == 0)
                    _activeDriver = DriverType.None;

            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Selects the driver to mark as active. If the DriverType provided does not have an instance already created, one will be created and marked as the active driver. 
        /// </summary>
        /// <param name="driverType"></param>
        /// <exception cref="WebAutomationException"></exception>
        public void SetActiveDriver(DriverType driverType)
        {
            try
            {
                if (!HasInstance(driverType))
                    CreateDriverInstance(driverType);

                _activeDriver = driverType;
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Instructs the driver to navigate the browser to another location.
        /// </summary>
        /// <param name="url"></param>
        public void NavigateWithActiveDriver(string url)
        {
            try
            {
                _drivers[_activeDriver].Navigate().GoToUrl(url);
                Thread.Sleep(2000);                
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Closes the farthest tab to the right in the current browser window
        /// </summary>
        /// <param name="mainWindow"></param>
        public void CloseLastTabWithActiveDriver(string mainWindow)
        {
            try
            {
                _drivers[_activeDriver].SwitchTo().Window(_drivers[_activeDriver].WindowHandles.Last()).Close();
                _drivers[_activeDriver].SwitchTo().Window(mainWindow);
                Thread.Sleep(1000);
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Closes the tab designated in the tabToClose parameter and switches the context to the tab designated in the targetTab parameter
        /// </summary>
        /// <param name="tabToClose"></param>
        /// <param name="targetTab"></param>
        public void CloseTabWithActiveDriver(string tabToClose, string targetTab)
        {
            try
            {
                _drivers[_activeDriver].SwitchTo().Window(tabToClose).Close();
                _drivers[_activeDriver].SwitchTo().Window(targetTab);
                Thread.Sleep(1000);
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Switches the context to the farthest tab to the right in the current browser window
        /// </summary>
        public void SwitchToLastTabWithActiveDriver()
        {
            try
            {
                _drivers[_activeDriver].SwitchTo().Window(_drivers[_activeDriver].WindowHandles.Last());
                if (_drivers[_activeDriver] is InternetExplorerDriver)
                    _drivers[_activeDriver].Manage().Window.Maximize();

                Thread.Sleep(1000);
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }            
        }

        /// <summary>
        /// Closes all tabs except for the one currently with focus
        /// </summary>
        public void CloseAllTabsExceptCurrentWithActiveDriver()
        {
            try
            {
                var currentWindow = _drivers[_activeDriver].CurrentWindowHandle;
                var windows = _drivers[_activeDriver].WindowHandles.Where(w => !w.Equals(currentWindow));
                foreach (var win in windows)
                    CloseTabWithActiveDriver(win, currentWindow);

                Thread.Sleep(1000);
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        internal WebDriverWait GetWait()
        {
            try
            {
                return _wait;
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
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
                            throw new WebAutomationException($"The driver {_chromeDriverName} was not found in the path {_driverPath}");

                        webDriver = ChromeDriverManager.Create_WebDriver_Instance(_driverPath, _chromeOptions);
                        break;
                    case DriverType.Firefox:
                        if (!File.Exists(Path.Combine(_driverPath, _firefoxDriverName)))
                            throw new WebAutomationException($"The driver {_firefoxDriverName} was not found in the path {_driverPath}");

                        webDriver = FirefoxDriverManager.Create_WebDriver_Instance(_driverPath, _firefoxOptions, _firefoxProfile);
                        break;
                    case DriverType.InternetExplorer:
                        if (!File.Exists(Path.Combine(_driverPath, _ieDriverName)))
                            throw new WebAutomationException($"The driver {_ieDriverName} was not found in the path {_driverPath}");

                        webDriver = IEDriverManager.Create_WebDriver_Instance(_driverPath, _internetExplorerOptions);
                        break;
                    case DriverType.Edge:
                        if (!File.Exists(Path.Combine(_driverPath, _edgeDriverName)))
                            throw new WebAutomationException($"The driver {_edgeDriverName} was not found in the path {_driverPath}");

                        webDriver = EdgeDriverManager.Create_WebDriver_Instance(_driverPath, _edgeOptions);
                        break;
                }

                _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_timeoutForWait));

                return webDriver;
            }
            catch (DriverServiceNotFoundException dsnf)
            {
                throw new WebAutomationException(dsnf.ToString());
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
