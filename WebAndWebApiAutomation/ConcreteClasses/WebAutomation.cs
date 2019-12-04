using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.Extensions;
using WebAndWebApiAutomation.Helpers;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;
using WebAndWebApiAutomation.Validators;
using static WebAndWebApiAutomation.WebAutomationEnums;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;

namespace WebAndWebApiAutomation
{
    /// <summary>
    /// This class contains the utility and component instance creation methods for automating cross browser testing of a web site
    /// </summary>
    public sealed class WebAutomation : IWebAutomation
    {
        private readonly StructureValidator _structureValidator;
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
        /// <exception cref="WebAutomationException"></exception>
        public WebAutomation(string driverPath, int timeoutForWaitOverride = 5)
        {
            driverPath = ValidateDriverPath(driverPath);

            if (timeoutForWaitOverride <= 0)
                timeoutForWaitOverride = 5;

            _timeoutForWait = timeoutForWaitOverride;
            _driverPath = driverPath;
            _structureValidator = new StructureValidator();
        }

        private WebAutomation() { }

        private string ValidateDriverPath(string driverPath)
        {
            if (driverPath.LastIndexOf("\\") != driverPath.Length - 1)
                driverPath += @"\";

            if (!Directory.Exists(driverPath))
                throw new WebAutomationException($"The path {driverPath} could not be found");

            return driverPath;
        }

        /// <summary>
        /// Returns an instance of the IWebDriverManager used to create and manage IWebDriver objects. This is a Singleton.
        /// </summary>
        /// <returns>IWebDriverManager</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriverManager GetIWebDriverManager()
        {
            try
            {
                if(_webDriverManagerInstance == null)
                    _webDriverManagerInstance = new WebDriverManager(_driverPath, _timeoutForWait);

                return _webDriverManagerInstance;
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
        /// <param name="collectTestData"></param>
        /// <param name="resultsPath"></param>
        /// <returns></returns>
        public ITestExecutor GetTestExecutor(bool collectTestData, string resultsPath)
        {
            try
            {                
                if (_testExecutorInstance == null)
                    _testExecutorInstance = new TestExecutor(_loggerInstance, collectTestData, resultsPath);

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
        /// Takes a screenshot and saves it in the provided path. THe file name is in the form of "{testMethodName}_{DateTime.Now}.jpeg"
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="sreenShotPath">Path to save the screenshot to</param>
        /// <param name="screenShotName">Name of the screenshot file</param>
        /// <exception cref="WebAutomationException"></exception>
        public void TakeScreenShot(IWebDriverManager webDriverManager, string sreenShotPath, string screenShotName)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                Helper.TakeScreenShot(manager.GetActiveDriver(), sreenShotPath, screenShotName);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Creates and returns an IJavaScriptExecutor object using the provided IWebDriver
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns></returns>
        /// <exception cref="WebAutomationException"></exception>
        public IJavaScriptExecutor GetJavaScriptExecutor(IWebDriverManager webDriverManager)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                return Helper.JavaScriptExecutor(manager.GetActiveDriver());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Modifies the style associated with the selector data object provided to highlight the element on the page
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebAutomationException"></exception>
        public void HighlightElement(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                Helper.HighlightElement(manager.GetActiveDriver(), cssBy);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clicks the element associated with the selector data object provided using a JavaScript query.
        /// This is useful when the element being clicked may be obstructed
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriverManager JavaScriptClick(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                Helper.ClickUsingJavaScript(manager.GetActiveDriver(), cssBy);
                return webDriverManager;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clicks the provided element after verifying it exists and is clickable
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to click</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriverManager Click(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var clickTarget = _structureValidator.CheckElementExistsReturnIWebElement(selectorData, manager.GetActiveDriver(), manager.GetWait());
                clickTarget.Click();
                Thread.Sleep(_timeoutForWait * 1000);
                return webDriverManager;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Enters text into the provided element after verifying it exists and is visible
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriverManager SendText(IWebDriverManager webDriverManager, SelectorData selectorData, string textToEnter)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.CheckElementExistsReturnCssSelector(selectorData, manager.GetActiveDriver(), manager.GetWait());
                manager.GetActiveDriver().MoveToElement(cssBy, manager.GetWait());
                WebDriverExtension.IsDisplayedAndEnabled(manager.GetActiveDriver(), cssBy, manager.GetWait());
                manager.GetActiveDriver().SendText(cssBy, manager.GetWait(), textToEnter);
                return manager;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

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
        public IWebDriverManager SendTextWithDelay(IWebDriverManager webDriverManager, SelectorData selectorData, string textToEnter, int delay = 500)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                manager.GetActiveDriver().SendTextWithDelay(cssBy, manager.GetWait(), textToEnter, delay);
                return manager;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to be cleared</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriverManager Clear(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                manager.GetActiveDriver().Clear(cssBy, manager.GetWait());
                return manager;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to move to</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriverManager MoveToElement(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
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

                return manager;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is visible
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool IsElementVisible(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return WebDriverExtension.IsDisplayed(manager.GetActiveDriver(), cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is enabled
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool IsElementEnabled(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return WebDriverExtension.IsEnabled(manager.GetActiveDriver(), cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is displayed
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool IsElementDisplayed(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return WebDriverExtension.IsEnabled(manager.GetActiveDriver(), cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is displayed and enabled
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool IsElementDisplayedAndEnabled(IWebDriverManager webDriverManager, SelectorData selectorData)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return WebDriverExtension.IsDisplayedAndEnabled(manager.GetActiveDriver(), cssBy, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the current url contains the provided text
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="text">Text to look for in the current Url</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool DoesUrlContain(IWebDriverManager webDriverManager, string text)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                return manager.GetActiveDriver().WaitForUrlContains(text, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the current url contains the provided pattern using regex
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="pattern">Text to look for in the current Url</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool DoesUrlContainUsingRegex(IWebDriverManager webDriverManager, string pattern)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                return manager.GetActiveDriver().WaitForUrlRegexContains(pattern, manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds and returns the IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="webDriverManager"></param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebElement CheckElementExistsReturnIWebElement(SelectorData selectorData, IWebDriverManager webDriverManager)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                return _structureValidator.CheckElementExistsReturnIWebElement(selectorData, manager.GetActiveDriver(), manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds the IWebElement using the parameters provided and returns the CssSelector based By object, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="webDriverManager"></param>
        /// <returns>By.CssSelector</returns>
        /// <exception cref="WebAutomationException"></exception>
        public By CheckElementExistsReturnCssSelector(SelectorData selectorData, IWebDriverManager webDriverManager)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                return _structureValidator.CheckElementExistsReturnCssSelector(selectorData, manager.GetActiveDriver(), manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Builds a CssSelector with the SelectorDataSet provided and uses it to check that an element exists with that data
        /// </summary>
        /// <param name="selectorDataSet">Data to check for in the current DOM instance</param>
        /// <param name="webDriverManager"></param>
        /// <returns>bool</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool CheckElementsExist(SelectorDataSet selectorDataSet, IWebDriverManager webDriverManager)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                return _structureValidator.CheckElementsExist(selectorDataSet, manager.GetActiveDriver(), manager.GetWait());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds and returns the child IWebElement using the parameters provided, if none is found null is returned.
        /// If multiple elements match the SelectorData definition utilize the nthParentElement parameter to select the desired parent
        /// </summary>
        /// <param name="parentSelectorData">Data to find the parent element with</param>
        /// <param name="childSelectorData">>Data to find the child element with</param>
        /// <param name="webDriverManager"></param>
        /// <param name="nthParentElement">The Zero based position of the parent element to search with, this is optional</param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebElement CheckChildElementExistsAndReturnIt(SelectorData parentSelectorData, SelectorData childSelectorData, IWebDriverManager webDriverManager, int nthParentElement = -1)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                IWebElement parentElement = null;
                if (nthParentElement > -1)
                {
                    parentElement = _structureValidator.Check_Nth_ElementExistsReturnIWebElement(parentSelectorData, nthParentElement, manager.GetActiveDriver(), manager.GetWait());
                }
                else
                {
                    parentElement = CheckElementExistsReturnIWebElement(parentSelectorData, webDriverManager);
                }

                if (parentElement == null)
                    return parentElement;

                return _structureValidator.CheckChildElementExists(parentElement, childSelectorData, manager.GetActiveDriver());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// This method will remove html data from the innerText of a given element and return only text NOT go beyon the first element. 
        /// This method will not parse the innerText of a child element passed in as part of a larger set
        /// </summary>
        /// <param name="htmlText">Raw htmlText from an IWebElement that contains html to be removed</param>
        /// <returns>string</returns>
        /// <exception cref="WebAutomationException"></exception>
        public string ParseInnerText(string htmlText)
        {
            try
            {
                return _structureValidator.ParseInnerText(htmlText);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Validates that all the anchor tags <a /> found on the page the driver is currently navigated to.
        /// It returns the XPath By object and NavigationResult for every link found and tested
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns>A list of locators and the corresponding navigation result</returns>
        /// <exception cref="WebAutomationException"></exception>
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
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds and returns all elements matching the provided selector data 
        /// </summary>
        /// <param name="selectorData">Data to check for in the current DOM instance</param>
        /// <param name="webDriverManager">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns>ReadOnlyCollectionIWebElement</returns>
        public ReadOnlyCollection<IWebElement> GetAllElementsUsingMatchingSelectorData(SelectorData selectorData, IWebDriverManager webDriverManager)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                return _structureValidator.GetAllBysUsingMatchingSelectorData(selectorData, manager.GetActiveDriver());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }
    }
}
