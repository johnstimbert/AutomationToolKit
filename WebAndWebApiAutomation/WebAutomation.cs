using log4net;
using OpenQA.Selenium;
using System;
using System.Configuration;
using WebAndWebApiAutomation.DriverFactory;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.Extensions;
using WebAndWebApiAutomation.SelectorDataObjects;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation
{
    public class WebAutomation
    {
        private readonly StructureValidator _structureValidator;

        public WebAutomation()
        {
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
    }
}
