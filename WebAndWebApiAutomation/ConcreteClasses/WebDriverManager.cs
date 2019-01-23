using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using WebAndWebApiAutomation.DriverFactory;
using WebAndWebApiAutomation.Exceptions;
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
        /// Creates an instance of IWebDriver that matches the type provided 
        /// </summary>
        /// <param name="driverType">Type of the driver instance to create</param>
        /// <param name="driverOptions">List of options to be set on the driver being created</param>
        /// <exception cref="WebAutomationException"></exception>
        public void CreateDriverInstance(DriverType driverType, string[] driverOptions = null)
        {
            try
            {
                if (_drivers == null)
                    _drivers = new Dictionary<DriverType, IWebDriver>();

                if (HasInstance(driverType))
                    throw new WebAutomationException($"Manager already contains an instance of DriverType {driverType}. " +
                        $"Call QuitDriverInstance then create a new instance or use the existing driver instance");

                _drivers.Add(driverType, GetDriver(driverType, driverOptions));
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
        /// Selects the driver to mark as active. The active driver is used for execution.
        /// </summary>
        /// <param name="driverType"></param>
        /// <exception cref="WebAutomationException"></exception>
        public void SetActiveDriver(DriverType driverType)
        {
            try
            {
                if(!HasInstance(driverType))
                    throw new WebAutomationException($"Manager does not contain an instance of DriverType {driverType}. " +
                        $"Call CreateDriverInstance to create a new driver instance");

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

        public void CloseTabWithActiveDriver(string windowToClose, string windowToBack)
        {
            try
            {
                _drivers[_activeDriver].SwitchTo().Window(windowToClose).Close();
                _drivers[_activeDriver].SwitchTo().Window(windowToBack);
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

        public void SwitchToLastTabWithActiveDriver()
        {
            try
            {
                _drivers[_activeDriver].SwitchTo().Window(_drivers[_activeDriver].WindowHandles.Last());
                if (_drivers[_activeDriver] is OpenQA.Selenium.IE.InternetExplorerDriver)
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
        private IWebDriver GetDriver(DriverType driverType, string[] driverOptions = null)
        {
            try
            {
                IWebDriver webDriver = null;

                switch (driverType)
                {
                    case DriverType.Chrome:
                        if (!File.Exists(Path.Combine(_driverPath, _chromeDriverName)))
                            throw new WebAutomationException($"The driver {_chromeDriverName} was not found in the path {_driverPath}");

                        webDriver = ChromeDriverManager.Create_WebDriver_Instance(_driverPath, driverOptions);
                        break;
                    case DriverType.Firefox:
                        if (!File.Exists(Path.Combine(_driverPath, _firefoxDriverName)))
                            throw new WebAutomationException($"The driver {_firefoxDriverName} was not found in the path {_driverPath}");

                        webDriver = FirefoxDriverManager.Create_WebDriver_Instance(_driverPath, driverOptions);
                        break;
                    case DriverType.InternetExplorer:
                        if (!File.Exists(Path.Combine(_driverPath, _ieDriverName)))
                            throw new WebAutomationException($"The driver {_ieDriverName} was not found in the path {_driverPath}");

                        webDriver = IEDriverManager.Create_WebDriver_Instance(_driverPath, driverOptions);
                        break;
                    case DriverType.Edge:
                        if (!File.Exists(Path.Combine(_driverPath, _edgeDriverName)))
                            throw new WebAutomationException($"The driver {_edgeDriverName} was not found in the path {_driverPath}");

                        webDriver = EdgeDriverManager.Create_WebDriver_Instance(_driverPath, driverOptions);
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
