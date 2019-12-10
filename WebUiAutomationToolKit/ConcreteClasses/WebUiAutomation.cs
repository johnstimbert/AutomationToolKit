using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebUiAutomationToolKit.Exceptions;
using WebUiAutomationToolKit.Helpers;
using WebUiAutomationToolKit.Validators;
using static WebUiAutomationToolKit.WebUiAutomationEnums;
using System.IO;

namespace WebUiAutomationToolKit
{
    /// <summary>
    /// This class contains the utility and component instance creation methods for automating cross browser testing of a web site
    /// </summary>
    public sealed class WebUiAutomation
    {
        private readonly string _driverPath;
        private readonly int _timeoutForWait;
        private static WebDriverManager _webDriverManagerInstance;
        private static Logger _loggerInstance;
        private static TestExecutor _testExecutorInstance;

        /// <summary>
        /// Creates an instance of the WebAutomation class
        /// </summary>
        /// <param name="driverPath">The path to the driver service executables</param>
        /// <param name="timeoutForWaitOverride">THis can be set to override the defauslt five (5) second timeout in the webdriver</param>
        /// <exception cref="WebUiAutomationException"></exception>
        public WebUiAutomation(string driverPath, int timeoutForWaitOverride = 5)
        {
            driverPath = ValidateDriverPath(driverPath);

            if (timeoutForWaitOverride <= 0)
                timeoutForWaitOverride = 5;

            _timeoutForWait = timeoutForWaitOverride;
            _driverPath = driverPath;
        }

        private WebUiAutomation() { }

        private string ValidateDriverPath(string driverPath)
        {
            if (driverPath.LastIndexOf("\\") != driverPath.Length - 1)
                driverPath += @"\";

            if (!Directory.Exists(driverPath))
                throw new WebUiAutomationException($"The path {driverPath} could not be found");

            return driverPath;
        }

        /// <summary>
        /// Returns an instance of the IWebDriverManager used to create and manage IWebDriver objects. This is a Singleton.
        /// </summary>
        /// <returns>IWebDriverManager</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public IWebDriverManager GetIWebDriverManager()
        {
            try
            {
                if(_webDriverManagerInstance == null)
                    _webDriverManagerInstance = new WebDriverManager(_driverPath, _timeoutForWait);

                return _webDriverManagerInstance;
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
        /// Creates and returns and instance of the ILogger used to write to a common log file
        /// </summary>
        /// <param name="loggerSettings"></param>
        /// <returns>ILogger</returns>
        /// <exception cref="LoggerException"></exception>
        public ILogger GetLogger(LoggerSettings loggerSettings)
        {
            try
            {
                if (_loggerInstance == null)
                    _loggerInstance = new Logger(loggerSettings);

                return _loggerInstance;
            }
            catch (LoggerException le)
            {
                throw le;
            }
            catch (Exception ex)
            {
                throw new LoggerException(ex.ToString());
            }
        }

        /// <summary>
        /// Creates and returns and instance of the ITestExecutor used to execute tests and manage the results
        /// </summary>
        /// <param name="collectTestData">Enables the detailed collection of test data</param>
        /// <param name="resultsPath">This is where screenshots taken during failures will be saved</param>
        /// <returns></returns>
        public ITestExecutor GetTestExecutor(string resultsPath, bool collectTestData = false)
        {
            try
            {                
                if (_testExecutorInstance == null)
                    _testExecutorInstance = new TestExecutor(resultsPath, _loggerInstance, collectTestData);

                return _testExecutorInstance;
            }
            catch (TestExecutorException te)
            {
                throw te;
            }
            catch (Exception ex)
            {
                throw new LoggerException(ex.ToString());
            }
        }
        
        /// <summary>
        /// Validates that all the anchor tags <a /> found on the page the driver is currently navigated to.
        /// It returns the XPath By object and NavigationResult for every link found and tested
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns>A list of locators and the corresponding navigation result</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public List<KeyValuePair<By, NavigationResult>> TestLinkNavigationForAllAnchorsFoundInPage(IWebDriverManager webDriverManager)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var navValidator = new NavigationValidator(manager.GetWait());
                return navValidator.TestLinkNavigationForAllAnchorsFoundInPage(manager.GetActiveDriver());
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// This method will remove html data from the innerText of a given element and return only text NOT go beyon the first element. 
        /// This method will not parse the innerText of a child element passed in as part of a larger set
        /// </summary>
        /// <param name="htmlText">Raw htmlText from an IWebElement that contains html to be removed</param>
        /// <returns>string</returns>
        /// <exception cref="WebUiAutomationException"></exception>
        public string ParseInnerText(string htmlText)
        {
            try
            {
                StructureValidator structureValidator = new StructureValidator();
                return structureValidator.ParseInnerText(htmlText);
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.ToString());
            }
        }

    }
}
