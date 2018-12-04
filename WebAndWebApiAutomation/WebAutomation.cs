using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using WebAndWebApiAutomation.DriverFactory;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.Extensions;
using WebAndWebApiAutomation.Helpers;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;
using WebAndWebApiAutomation.Validators;
using static WebAndWebApiAutomation.WebAutomationEnums;
using System.IO;

namespace WebAndWebApiAutomation
{
    /// <summary>
    /// This class conatins the utility and driver instance creation methods for automating the cross browser testing of a web site
    /// </summary>
    public class WebAutomation
    {
        private const string _chromeDriverName = "chromedriver.exe";
        private const string _firefoxDriverName = "geckodriver.exe";
        private const string _ieDriverName = "IEDriverServer.exe";
        private const string _edgeDriverName = "MicrosoftWebDriver.exe";

        private StructureValidator _structureValidator;
        private readonly string _driverPath;
        private readonly int _timeoutForWait;
        WebDriverWait _wait;

        /// <summary>
        /// Creates an instance of the WebAutomation class
        /// </summary>
        /// <param name="driverPath">The path to the driver service executables</param>
        /// <param name="TimeoutForWaitOverride">THis can be set to override the defauslt five (5) second timeout in the webdriver</param>
        /// <exception cref="WebAutomationException"></exception>
        public WebAutomation(string driverPath, int TimeoutForWaitOverride = 5)
        {
            driverPath = ValidateDriverPath(driverPath);

            _timeoutForWait = TimeoutForWaitOverride;
            _driverPath = driverPath;
        }

        private string ValidateDriverPath(string driverPath)
        {
            if (driverPath.LastIndexOf("\\") != driverPath.Length - 1)
                driverPath += @"\";

            if (!Directory.Exists(driverPath))
                throw new WebAutomationException($"The path {driverPath} could not be found");

            return driverPath;
        }

        /// <summary>
        /// Creates and returns and instance of the driver specified by the driverType parameter
        /// NOTE: The INTERNET EXPLORER and EDGE browsers do not currently support headless functionality
        /// </summary>
        /// <param name="driverType">Specifies the driver for the desired browser</param>
        /// <param name="runHeadless">If true the driver will not open a browser. Default is false</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriver GetDriver(DriverType driverType, bool runHeadless = false)
        {
            try
            {
                IWebDriver webDriver = null;

                switch (driverType)
                {
                    case DriverType.Chrome:
                        if (!File.Exists(Path.Combine(_driverPath, _chromeDriverName)))
                            throw new WebAutomationException($"The driver {_chromeDriverName} was not found in the path {_driverPath}");

                        webDriver = ChromeDriverManager.Create_WebDriver_Instance(_driverPath, runHeadless);
                        break;
                    case DriverType.Firefox:
                        if (!File.Exists(Path.Combine(_driverPath, _firefoxDriverName)))
                            throw new WebAutomationException($"The driver {_firefoxDriverName} was not found in the path {_driverPath}");

                        webDriver = FirefoxDriverManager.Create_WebDriver_Instance(_driverPath, runHeadless);
                        break;
                    case DriverType.Ie:
                        if (!File.Exists(Path.Combine(_driverPath, _ieDriverName)))
                            throw new WebAutomationException($"The driver {_ieDriverName} was not found in the path {_driverPath}");

                        webDriver = IEDriverManager.Create_WebDriver_Instance(_driverPath, runHeadless);
                        break;
                    case DriverType.Edge:
                        if (!File.Exists(Path.Combine(_driverPath, _edgeDriverName)))
                            throw new WebAutomationException($"The driver {_edgeDriverName} was not found in the path {_driverPath}");

                        webDriver = EdgeDriverManager.Create_WebDriver_Instance(_driverPath, runHeadless);
                        break;
                }

                _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_timeoutForWait));
                _structureValidator = new StructureValidator(_wait);

                return webDriver;
            }
            catch (DriverServiceNotFoundException dsnf)
            {
                throw new WebAutomationException(dsnf.ToString());
            }
        }

        /// <summary>
        /// Takes a screenshot and saves it in the provided path. THe file name is in the form of "{testMethodName}_{DateTime.Now}.jpeg"
        /// </summary>
        /// <param name="driver">IWebDriver object to take the screenshot with</param>
        /// <param name="sreenShotPath">Path to save the screenshot to</param>
        /// <param name="screenShotName">Name of the screenshot file</param>
        /// <exception cref="WebAutomationException"></exception>
        public void TakeScreenShot(IWebDriver driver, string sreenShotPath, string screenShotName)
        {
            Helper.IsDriverNull(driver);
            try
            {
                Helper.TakeScreenShot(driver, sreenShotPath, screenShotName);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Creates and returns an IJavaScriptExecutor object using the provided IWebDriver
        /// </summary>
        /// <param name="driver">IWebDriver object to create the IJavaScriptExecutor with</param>
        /// <returns></returns>
        /// <exception cref="WebAutomationException"></exception>
        public IJavaScriptExecutor GetJavaScriptExecutor(IWebDriver driver)
        {
            Helper.IsDriverNull(driver);
            try
            {
                return Helper.JavaScriptExecutor(driver);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Modifies the style associated with the selector data object provided to highlight the element on the page
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebAutomationException"></exception>
        public void HighlightElement(IWebDriver driver, SelectorData selectorData)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                Helper.HighlightElement(driver, cssBy);
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
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to highlight</param>
        /// <exception cref="WebAutomationException"></exception>
        public void JavaScriptClick(IWebDriver driver, SelectorData selectorData)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                Helper.ClickUsingJavaScript(driver, cssBy);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clicks the provided element after verifying it exists and is clickable
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to click</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriver Click(IWebDriver driver, SelectorData selectorData)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                driver.Click(cssBy, _wait);
                return driver;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Enters text into the provided element after verifying it exists and is visible
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriver SendText(IWebDriver driver, SelectorData selectorData, string textToEnter)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                driver.SendText(cssBy, _wait, textToEnter);
                return driver;
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
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to receive the text</param>
        /// <param name="textToEnter">Text that will be entered</param>
        /// <param name="delay">Interval before each character is entered</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriver SendTextWithDelay(IWebDriver driver, SelectorData selectorData, string textToEnter, int delay = 500)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                driver.SendTextWithDelay(cssBy, _wait, textToEnter, delay);
                return driver;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to be cleared</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriver Clear(IWebDriver driver, SelectorData selectorData)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                driver.Clear(cssBy, _wait);
                return driver;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Clears the text from the provided element after verifiying the element is visible
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to move to</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebDriver MoveToElement(IWebDriver driver, SelectorData selectorData)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                driver.MoveToElement(cssBy, _wait);
                return driver;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is selected
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool IsElementSelected(IWebDriver driver, SelectorData selectorData)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return driver.WaitForElementToBeSelected(cssBy, _wait);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the provided element is visible
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to check</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool IsElementVisible(IWebDriver driver, SelectorData selectorData)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                return driver.WaitForElementToBeVisible(cssBy, _wait);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the current url contains the provided text
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="text">Text to look for in the current Url</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool DoesUrlContain(IWebDriver driver, string text)
        {
            Helper.IsDriverNull(driver);
            try
            {
                return driver.WaitForUrlContains(text, _wait);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Checks whether or not the current url contains the provided pattern using regex
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="pattern">Text to look for in the current Url</param>
        /// <returns>IWebDriver</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool DoesUrlContainUsingRegex(IWebDriver driver, string pattern)
        {
            Helper.IsDriverNull(driver);
            try
            {
                return driver.WaitForUrlRegexContains(pattern, _wait);
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
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebElement CheckElementExistsReturnIWebElement(SelectorData selectorData, IWebDriver driver)
        {
            Helper.IsDriverNull(driver);
            try
            {
                return _structureValidator.CheckElementExistsReturnIWebElement(selectorData, driver);
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
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns>By.CssSelector</returns>
        /// <exception cref="WebAutomationException"></exception>
        public By CheckElementExistsReturnCssSelector(SelectorData selectorData, IWebDriver driver)
        {
            Helper.IsDriverNull(driver);
            try
            {
                return _structureValidator.CheckElementExistsReturnCssSelector(selectorData, driver);
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
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns>bool</returns>
        /// <exception cref="WebAutomationException"></exception>
        public bool CheckElementsExist(SelectorDataSet selectorDataSet, IWebDriver driver)
        {
            Helper.IsDriverNull(driver);
            try
            {
                return _structureValidator.CheckElementsExist(selectorDataSet, driver);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds and returns the child IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="parentSelectorData">Data to find the parent element with</param>
        /// <param name="childSelectorData">>Data to find the child element with</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns>IWebElement</returns>
        /// <exception cref="WebAutomationException"></exception>
        public IWebElement CheckChildElementExistsAndReturnIt(SelectorData parentSelectorData, SelectorData childSelectorData, IWebDriver driver)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var parentElement = CheckElementExistsReturnIWebElement(parentSelectorData, driver);
                if (parentElement == null)
                    return parentElement;

                return _structureValidator.CheckChildElementExists(parentElement, childSelectorData, driver);
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
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns>A list of locators and the corresponding navigation result</returns>
        /// <exception cref="WebAutomationException"></exception>
        public List<KeyValuePair<By, NavigationResult>> TestLinkNavigationForAllAnchorsFoundInPage(IWebDriver driver)
        {
            Helper.IsDriverNull(driver);
            try
            {
                var navValidator = new NavigationValidator(_wait);
                return navValidator.TestLinkNavigationForAllAnchorsFoundInPage(driver);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Finds all elements matching the provided selector data and returns a list of xpath by objects for each found elements
        /// /// </summary>
        /// <param name="selectorData">Data to check for in the current DOM instance</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns></returns>
        public List<By> GetAllBysUsingMatchingSelectorData(SelectorData selectorData, IWebDriver driver)
        {
            Helper.IsDriverNull(driver);
            try
            {
                return _structureValidator.GetAllBysUsingMatchingSelectorData(selectorData, driver);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

    }
}
