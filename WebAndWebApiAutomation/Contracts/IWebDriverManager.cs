using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation
{
    /// <summary>
    /// Interface that defines methods for WebDriverManager objects
    /// </summary>
    public interface IWebDriverManager
    {
        /// <summary>
        /// Creates an instance of IWebDriver that matches the type provided
        /// </summary>
        /// <param name="driverType">Type of the driver instance to create</param>
        /// <param name="driverOptions">List of options to be set on the driver being created</param>
        void CreateDriverInstance(DriverType driverType, string[] driverOptions = null);
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
    }
}
