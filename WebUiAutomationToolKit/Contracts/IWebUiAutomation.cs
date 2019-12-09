using System.Collections.Generic;
using OpenQA.Selenium;
using WebUiAutomationToolKit.Exceptions;

namespace WebUiAutomationToolKit
{
    /// <summary>
    /// This class contains the utility and driver instance creation methods for automating the cross browser testing of a web site
    /// </summary>
    public interface IWebUiAutomation
    {
        /// <summary>
        /// Creates and returns and instance of the IWebDriverManager used to create and manage IWebDriver objects
        /// </summary>
        /// <returns>IWebDriverManager</returns>
        /// <exception cref="WebUiAutomationException"></exception>
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
        /// This method will remove html data from the innerText of a given element and return only text NOT go beyon the first element. 
        /// This method will not parse the innerText of a child element passed in as part of a larger set
        /// </summary>
        /// <param name="htmlText">Raw htmlText from an IWebElement that contains html to be removed</param>
        /// <returns>string</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        string ParseInnerText(string htmlText);        
        /// <summary>
        /// Validates that all the anchor tags <a /> found on the page the driver is currently navigated to.
        /// It returns the XPath By object and NavigationResult for every link found and tested
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns>A list of locators and the corresponding navigation result</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        List<KeyValuePair<By, WebUiAutomationEnums.NavigationResult>> TestLinkNavigationForAllAnchorsFoundInPage(IWebDriverManager webDriverManager);
    }
}