using OpenQA.Selenium;
using System;
using System.Configuration;
using WebAndWebApiAutomation.DriverFactory;
using WebAndWebApiAutomation.Exceptions;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation
{
    public class WebAutomation
    {
        /// <summary>
        /// Creates and returns and instance of the driver specified by the driverType parameter
        /// </summary>
        /// <param name="driverType"></param>
        /// <returns></returns>
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
    }
}
