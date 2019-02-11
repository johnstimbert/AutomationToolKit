using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using static WebAndWebApiAutomation.WebAutomationEnums;
using OpenQA.Selenium;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;

namespace WebAndWebApiAutomation
{
    /// <summary>
    /// Interface that defines methods for WebDriverManager objects
    /// </summary>
    public interface IWebDriverManager
    {
        /// <summary>
        /// Finds an element using the FindElement Mthod on the active driver. Available for troubleshooting in the Alpha and will be deprecated in the released version
        /// </summary>
        /// <param name="selectorData"></param>
        /// <returns></returns>
        IWebElement FindElementWithActiveDriver(SelectorData selectorData);
        /// <summary>
        /// Sets the options for the associated driver. If the driver is active when this method is called it will be recreated
        /// </summary>
        /// <param name="chromeOptions"></param>
        /// <param name="firefoxOptions"></param>
        /// <param name="internetExplorerOptions"></param>
        /// <param name="edgeOptions"></param>
        void SetDriverOptions(ChromeOptions chromeOptions = null, FirefoxOptions firefoxOptions = null, 
            InternetExplorerOptions internetExplorerOptions = null, EdgeOptions edgeOptions = null);
        /// <summary>
        /// Sets the profile for the firefox driver. If the driver is active when this method is called it will be recreated
        /// </summary>
        /// <param name="firefoxProfile"></param>
        void SetFirefoxProfile(FirefoxProfile firefoxProfile);
        /// <summary>
        /// Creates an instance of IWebDriver that matches the type provided
        /// </summary>
        /// <param name="driverType">Type of the driver instance to create</param>
        void CreateDriverInstance(DriverType driverType);
        /// <summary>
        /// Quits an instance of IWebDriver that matches the type provided
        /// </summary>
        /// <param name="driverType">Type of the driver instance to quit</param>
        void QuitDriverInstance(DriverType driverType);
        /// <summary>
        /// Selects the driver to mark as active. The active driver is used for execution.
        /// </summary>
        /// <param name="driverType"></param>
        void SetActiveDriver(DriverType driverType);
        /// <summary>
        /// Returns the DriverType value indicating what driver is currently marked as active. The active driver is used for execution.
        /// </summary>
        /// <returns>DriverType</returns>
        DriverType GetActiveDriverType();
        /// <summary>
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        /// <returns></returns>
        string GetActiveDriverPageSource();
        /// <summary>
        /// Gets the title of the current browser window.
        /// </summary>
        /// <returns></returns>
        string GetActiveDriverTitle();
        /// <summary>
        /// Gets or sets the URL the browser is currently displaying.
        /// </summary>
        /// <returns></returns>
        string GetActiveDriverUrl();
        /// <summary>
        /// Gets the current window handle, which is an opaque handle to this window that uniquely identifies it within this driver instance.
        /// </summary>
        /// <returns></returns>
        string GetActiveDriverCurrentWindowHandle();
        /// <summary>
        /// Gets the window handles of open browser windows.
        /// </summary>
        /// <returns></returns>
        ReadOnlyCollection<string> GetActiveDriverWindowHandles();
        /// <summary>
        /// Instructs the driver to navigate the browser to another location.
        /// </summary>
        /// <param name="url"></param>
        void NavigateWithActiveDriver(string url);
        /// <summary>
        /// Closes the farthest tab to the right in the current browser window
        /// </summary>
        /// <param name="mainWindow"></param>
        void CloseLastTabWithActiveDriver(string mainWindow);
        /// <summary>
        /// Closes the tab designated in the tabToClose parameter and switches the context to the tab designated in the targetTab parameter
        /// </summary>
        /// <param name="tabToClose"></param>
        /// <param name="targetTab"></param>
        void CloseTabWithActiveDriver(string tabToClose, string targetTab);
        /// <summary>
        /// Switches the context to the farthest tab to the right in the current browser window
        /// </summary>
        void SwitchToLastTabWithActiveDriver();
        /// <summary>
        /// Closes all tabs except for the one currently with focus
        /// </summary>
        void CloseAllTabsExceptCurrentWithActiveDriver();
    }
}
