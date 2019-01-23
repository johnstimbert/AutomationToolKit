using System.Collections.Generic;
using OpenQA.Selenium;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;

namespace WebAndWebApiAutomation
{ 
    /// <summary>
    /// This class conatins the utility and driver instance creation methods for automating the cross browser testing of a web site
    /// </summary>
    public interface IWebAutomation
    {
        /// <summary>
        /// Finds and returns the child IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="parentSelectorData">Data to find the parent element with</param>
        /// <param name="childSelectorData">>Data to find the child element with</param>
        /// <param name="webDriverManager"></param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebElement CheckChildElementExistsAndReturnIt(SelectorData parentSelectorData, SelectorData childSelectorData, IWebDriverManager webDriverManager);
        /// <summary>
        /// Finds the IWebElement using the parameters provided and returns the CssSelector based By object, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="webDriverManager"></param>
        /// <returns>By.CssSelector</returns>
        /// <exception cref="WebAutomationException"></exception>
        By CheckElementExistsReturnCssSelector(SelectorData selectorData, IWebDriverManager webDriverManager);
        /// <summary>
        /// Finds and returns the IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="webDriverManager"></param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebElement CheckElementExistsReturnIWebElement(SelectorData selectorData, IWebDriverManager webDriverManager);
        /// <summary>
        /// Builds a CssSelector with the SelectorDataSet provided and uses it to check that an element exists with that data
        /// </summary>
        /// <param name="selectorDataSet">Data to check for in the current DOM instance</param>
        /// <param name="webDriverManager"></param>
        /// <returns>bool</returns>
        /// <exception cref="WebAutomationException"></exception>
        bool CheckElementsExist(SelectorDataSet selectorDataSet, IWebDriverManager webDriverManager);
        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to be cleared</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebDriverManager Clear(IWebDriverManager webDriverManager, SelectorData selectorData);
        /// <summary>
        /// Clicks the provided element after verifying it exists and is clickable
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to click</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebDriverManager Click(IWebDriverManager webDriverManager, SelectorData selectorData);
        /// <summary>
        /// Checks whether or not the current url contains the provided text
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="text">Text to look for in the current Url</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        bool DoesUrlContain(IWebDriverManager webDriverManager, string text);
        /// <summary>
        /// Checks whether or not the current url contains the provided pattern using regex
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="pattern">Text to look for in the current Url</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        bool DoesUrlContainUsingRegex(IWebDriverManager webDriverManager, string pattern);
        /// <summary>
        /// Finds all elements matching the provided selector data and returns a list of xpath by objects for each found elements
        /// /// </summary>
        /// <param name="selectorData">Data to check for in the current DOM instance</param>
        /// <param name="webDriverManager"></param>
        /// <returns></returns>
        List<By> GetAllBysUsingMatchingSelectorData(SelectorData selectorData, IWebDriverManager webDriverManager);
        /// <summary>
        /// Creates and returns and instance of the IWebDriverManager used to create and manage IWebDriver objects
        /// </summary>
        /// <returns>IWebDriverManager</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebDriverManager GetIWebDriverManager();
        /// <summary>
        /// Creates and returns and instance of the ILogger used to write to a common log file
        /// </summary>
        /// <param name="loggerSettings"></param>
        /// <returns>ILogger</returns>
        /// <exception cref="LoggerException"></exception>
        ILogger GetLogger(LoggerSettings loggerSettings);
        /// <summary>
        /// Creates and returns and instance of the ITestExecutor used to execute tests and manage the results
        /// </summary>
        /// <param name="collectTestData"></param>
        /// <param name="resultsPath"></param>
        /// <returns></returns>
        ITestExecutor GetTestExecutor(bool collectTestData, string resultsPath);
        /// <summary>
        /// Creates and returns an IJavaScriptExecutor object using the provided IWebDriver
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns></returns>
        /// <exception cref="WebAutomationException"></exception>
        IJavaScriptExecutor GetJavaScriptExecutor(IWebDriverManager webDriverManager);
        /// <summary>
        /// Modifies the style associated with the selector data object provided to highlight the element on the page
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebAutomationException"></exception>
        void HighlightElement(IWebDriverManager webDriverManager, SelectorData selectorData);
        /// <summary>
        /// Checks whether or not the provided element is selected
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        bool IsElementSelected(IWebDriverManager webDriverManager, SelectorData selectorData);
        /// <summary>
        /// Checks whether or not the provided element is visible
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        bool IsElementVisible(IWebDriverManager webDriverManager, SelectorData selectorData);
        /// <summary>
        /// Clicks the element associated with the selector data object provided using a JavaScript query.
        /// This is useful when the element being clicked may be obstructed
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebAutomationException"></exception>
        void JavaScriptClick(IWebDriverManager webDriverManager, SelectorData selectorData);
        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to move to</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebDriverManager MoveToElement(IWebDriverManager webDriverManager, SelectorData selectorData);
        /// <summary>
        /// This method will remove html data from the innerText of a given element and return only text NOT go beyon the first element. 
        /// This method will not parse the innerText of a child element passed in as part of a larger set
        /// </summary>
        /// <param name="htmlText">Raw htmlText from an IWebElement that contains html to be removed</param>
        /// <returns>string</returns>
        /// <exception cref="WebAutomationException"></exception>
        string ParseInnerText(string htmlText);
        /// <summary>
        /// Enters text into the provided element after verifying it exists and is visible
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebDriverManager SendText(IWebDriverManager webDriverManager, SelectorData selectorData, string textToEnter);
        /// <summary>
        /// Enters text into the provided element one character at a time with a delay between each after verifying it exists and is visible.
        /// Note: Only use this method to send text to elements that have autocomplete and require a delay on each letter typed.
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <param name="delay">Interval before each character is entered</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        IWebDriverManager SendTextWithDelay(IWebDriverManager webDriverManager, SelectorData selectorData, string textToEnter, int delay = 500);
        /// <summary>
        /// Takes a screenshot and saves it in the provided path. THe file name is in the form of "{testMethodName}_{DateTime.Now}.jpeg"
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="sreenShotPath">Path to save the screenshot to</param>
        /// <param name="screenShotName">Name of the screenshot file</param>
        /// <exception cref="WebAutomationException"></exception>
        void TakeScreenShot(IWebDriverManager webDriverManager, string sreenShotPath, string screenShotName);
        /// <summary>
        /// Validates that all the anchor tags <a /> found on the page the driver is currently navigated to.
        /// It returns the XPath By object and NavigationResult for every link found and tested
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns>A list of locators and the corresponding navigation result</returns>
        /// <exception cref="WebAutomationException"></exception>
        List<KeyValuePair<By, WebAutomationEnums.NavigationResult>> TestLinkNavigationForAllAnchorsFoundInPage(IWebDriverManager webDriverManager);
    }
}