using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Configuration;
using WebAndWebApiAutomation.DriverFactory;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.SelectorDataObjects;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation
{
    public class WebAutomation
    {
        private readonly StructureValidator _structureValidator;
        private readonly int _timeoutForWait = 5;
        WebDriverWait _wait;

        public WebAutomation(int TimeoutForWait)
        {
            _timeoutForWait = TimeoutForWait;
            _structureValidator = new StructureValidator();
        }

        /// <summary>
        /// Creates and returns and instance of the driver specified by the driverType parameter
        /// </summary>
        /// <param name="driverType">Specifies the driver for the desired browser</param>
        /// <returns>IWebDriver</returns>
        public IWebDriver GetDriver(DriverType driverType)
        {
            try
            {
                string driverPath = ConfigurationManager.AppSettings["DriverPath"];

                if (string.IsNullOrEmpty(driverPath))
                    throw new WebAutomationException("DriverPath not defined in the app.config. Please add '<add key=\"DriverPath\" value=\"PathToDriverExecutables\"/>' to the app.config ");

                IWebDriver webDriver = null;

                switch (driverType)
                {
                    case DriverType.Chrome:
                        webDriver = ChromeDriverManager.Create_WebDriver_Instance(driverPath);
                        break;
                    case DriverType.Firefox:
                        webDriver = FirefoxDriverManager.Create_WebDriver_Instance(driverPath);
                        break;
                    case DriverType.Ie:
                        webDriver = IEDriverManager.Create_WebDriver_Instance(driverPath);
                        break;
                    case DriverType.Edge:
                        webDriver = EdgeDriverManager.Create_WebDriver_Instance(driverPath);
                        break;
                }

                _wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(_timeoutForWait));

                return webDriver;
            }
            catch(DriverServiceNotFoundException dsnf)
            {
                throw new WebAutomationException(dsnf.ToString());
            }
            catch(Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Returns an instance of the log4net logger to standardize log output
        /// </summary>
        /// <returns>ILog</returns>
        public ILog GetLogger()
        {
            try
            {
                return Helper.Logger;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        /// <summary>
        /// Takes a screenshot and saves it in the provided path. THe file name is in the form of "{testMethodName}_{DateTime.Now}.jpeg"
        /// </summary>
        /// <param name="driver">IWebDriver object to take the screenshot with</param>
        /// <param name="sreenShotPath">Path to save the screenshot to</param>
        /// <param name="screenShotName">Name of the screenshot file</param>
        public void TakeScreenShot(IWebDriver driver, string sreenShotPath, string screenShotName)
        {
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
        public IJavaScriptExecutor GetJavaScriptExecutor(IWebDriver driver)
        {
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
        public void HighlightElement(IWebDriver driver, SelectorData selectorData)
        {
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
        public void JavaScriptClick(IWebDriver driver, SelectorData selectorData)
        {
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
        public IWebDriver Click(IWebDriver driver, SelectorData selectorData)
        {
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
        /// Double clicks the provided element after verifying it exists and is clickable
        /// </summary>
        /// <param name="driver">IWebDriver object</param>
        /// <param name="selectorData">Object representing the element to double click</param>
        /// <returns>IWebDriver</returns>
        public IWebDriver DoubleClick(IWebDriver driver, SelectorData selectorData)
        {
            try
            {
                var cssBy = _structureValidator.BuildCssSelectorBy(selectorData);
                driver.DoubleClick(cssBy, _wait);
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
        public IWebDriver SendText(IWebDriver driver, SelectorData selectorData, string textToEnter)
        {
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
        public IWebDriver SendTextWithDelay(IWebDriver driver, SelectorData selectorData, string textToEnter, int delay = 500)
        {
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
        public IWebDriver Clear(IWebDriver driver, SelectorData selectorData)
        {
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
        public IWebDriver MoveToElement(IWebDriver driver, SelectorData selectorData)
        {
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
        public bool IsElementSelected(IWebDriver driver, SelectorData selectorData)
        {
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
        public bool IsElementVisible(IWebDriver driver, SelectorData selectorData)
        {
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
        /// <param name="text">Text to loof for in the current Url</param>
        /// <returns>IWebDriver</returns>
        public bool DoesUrlContain(IWebDriver driver, string text)
        {
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
        /// <param name="pattern">Text to loof for in the current Url</param>
        /// <returns>IWebDriver</returns>
        public bool DoesUrlContainUseingRegex(IWebDriver driver, string pattern)
        {
            try
            {
                return driver.WaitForUrlRegexContains(pattern, _wait);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }
    }
}
