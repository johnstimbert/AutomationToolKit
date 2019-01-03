using OpenQA.Selenium.Chrome;
using System.Linq;

namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class ChromeDriverManager
    {
        internal static ChromeDriver Create_WebDriver_Instance(string driverPath, string[] driverOptions = null)
        {
            ChromeOptions options = new ChromeOptions();

            if(driverOptions != null)
            {
                foreach(string driverOption in driverOptions)
                {
                    options.AddArgument(driverOption);
                }
            }

            var driver = new ChromeDriver(driverPath, options);

            if (driverOptions == null || !driverOptions.ToList().Contains("--headless"))
                driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
