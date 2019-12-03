using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using static WebAndWebApiAutomation.WebAutomationEnums;
using OpenQA.Selenium;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;
using WebAndWebApiAutomation.Exceptions;

namespace WebAndWebApiAutomation
{
    /// <summary>
    /// Interface that defines methods for WebDriverManager objects
    /// </summary>
    public interface IWebDriverManager
    {
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
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        /// <remarks>
        ///     If the page has been modified after loading (for example, by JavaScript) there
        ///     is no guarantee that the returned text is that of the modified page. Please consult
        ///     the documentation of the particular driver being used to determine whether the
        ///     returned text reflects the current state of the page or the text last sent by
        ///     the web server. The page source returned is a representation of the underlying
        ///     DOM: do not expect it to be formatted or escaped in the same way as the response
        ///     sent from the web server.</remarks>
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
        /// <summary>
        /// Switches the driver context to the frame matching the SelectorData provided
        /// </summary>
        /// <param name="selectorData"></param>
        void SwitchToFrame(SelectorData selectorData);
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
        object ExecuteAsyncScript(string script, params object[] args);
        /// <summary>
        /// Executes JavaScript asynchronously in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        object ExecuteScript(string script, params object[] args);
        /// <summary>
        /// Finds and returns the child IWebElement using the parameters provided, if none is found null is returned.
        /// If multiple elements match the SelectorData definition utilize the nthParentElement parameter to select the desired parent
        /// </summary>
        /// <param name="parentSelectorData">Data to find the parent element with</param>
        /// <param name="childSelectorData">>Data to find the child element with</param>
        /// <param name="nthParentElement">The Zero based position of the parent element to search with, this is optional</param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebElement CheckChildElementExistsAndReturnIt(SelectorData parentSelectorData, SelectorData childSelectorData, int nthParentElement = -1);
        /// <summary>
        /// Finds the IWebElement using the parameters provided and returns the CssSelector based By object, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <returns>By.CssSelector</returns>
        /// <exception cref="WebAutomationException"></exception>
        By CheckElementExistsReturnCssSelector(SelectorData selectorData);
        /// <summary>
        /// Finds and returns the IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebElement CheckElementExistsReturnIWebElement(SelectorData selectorData);
        /// <summary>
        /// Builds a CssSelector with the SelectorDataSet provided and uses it to check that an element exists with that data
        /// </summary>
        /// <param name="selectorDataSet">Data to check for in the current DOM instance</param>
        /// <returns>bool</returns>
        /// <exception cref="WebAutomationException"></exception>
        bool CheckElementsExist(SelectorDataSet selectorDataSet);
        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="selectorData">Object representing the element to be cleared</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        void ClearText(SelectorData selectorData);
        /// <summary>
        /// Clicks the provided element after verifying it exists and is clickable
        /// </summary>
        /// <param name="selectorData">Object representing the element to click</param>
        /// <exception cref="WebAutomationException"></exception>
        void Click(SelectorData selectorData);
        /// <summary>
        /// Checks whether or not the current url contains the provided text
        /// </summary>
        /// <param name="text">Text to look for in the current Url</param>
        /// <exception cref="WebAutomationException"></exception>
        bool DoesUrlContain(string text);
        /// <summary>
        /// Checks whether or not the current url contains the provided pattern using regex
        /// </summary>
        /// <param name="pattern">Text to look for in the current Url</param>
        /// <exception cref="WebAutomationException"></exception>
        bool DoesUrlContainUsingRegex(string pattern);
        /// <summary>
        /// Finds and returns all elements matching the provided selector data 
        /// </summary>
        /// <param name="selectorData">Data to check for in the current DOM instance</param>
        /// <returns>ReadOnlyCollection<IWebElement></IWebElement></returns>
        ReadOnlyCollection<IWebElement> GetAllElementsUsingMatchingSelectorData(SelectorData selectorData);
        /// <summary>
        /// Modifies the style associated with the selector data object provided to highlight the element on the page
        /// </summary>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebAutomationException"></exception>
        void HighlightElement(SelectorData selectorData);
        /// <summary>
        /// Checks whether or not the provided element is displayed
        /// </summary>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <exception cref="WebAutomationException"></exception>
        bool IsElementDisplayed(SelectorData selectorData);
        /// <summary>
        /// Checks whether or not the provided element is enabled
        /// </summary>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <exception cref="WebAutomationException"></exception>
        bool IsElementEnabled(SelectorData selectorData);
        /// <summary>
        /// Checks whether or not the provided element is displayed and enabled
        /// </summary>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <exception cref="WebAutomationException"></exception>
        bool IsElementDisplayedAndEnabled(SelectorData selectorData);
        /// <summary>
        /// Clicks the element associated with the selector data object provided using a JavaScript query.
        /// This is useful when the element being clicked may be obstructed
        /// </summary>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebAutomationException"></exception>
        void JavaScriptClick(SelectorData selectorData);
        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="selectorData">Object representing the element to move to</param>
        /// /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        void MoveToElement(SelectorData selectorData);
        /// <summary>
        /// Enters text into the provided element after verifying it exists and is visible
        /// </summary>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <exception cref="WebAutomationException"></exception>
        void SendText(SelectorData selectorData, string textToEnter);
        /// <summary>
        /// Enters text into the provided element one character at a time with a delay between each after verifying it exists and is visible.
        /// Note: Only use this method to send text to elements that have autocomplete and require a delay on each letter typed.
        /// </summary>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <param name="delay">Interval before each character is entered</param>
        /// <exception cref="WebAutomationException"></exception>
        void SendTextWithDelay(SelectorData selectorData, string textToEnter, int delay = 500);
        /// <summary>
        /// Takes a screenshot and saves it in the provided path. THe file name is in the form of "{testMethodName}_{DateTime.Now}.jpeg"
        /// </summary>
        /// <param name="sreenShotPath">Path to save the screenshot to</param>
        /// <param name="screenShotName">Name of the screenshot file</param>
        /// <exception cref="WebAutomationException"></exception>
        void TakeScreenShot(string sreenShotPath, string screenShotName);
    }
}
