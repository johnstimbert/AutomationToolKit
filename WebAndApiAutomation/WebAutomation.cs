using OpenQA.Selenium;
using WebAndApiAutomation.DriverFactory;
using static WebAndApiAutomation.WebAutomationEnums;
using System.Reflection;
using System.IO;

namespace WebAndApiAutomation
{
    public class WebAutomation
    {
        /// <summary>
        /// Creates and returns and instance of the driver specified by the driverType parameter
        /// </summary>
        /// <param name="driverType"></param>
        /// <returns></returns>
        public IWebDriver GetDriver(DriverType driverType, string driverPath)
        {
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
    }
}
