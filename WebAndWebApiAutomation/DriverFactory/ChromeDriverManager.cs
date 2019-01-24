using OpenQA.Selenium.Chrome;
using System.Linq;

namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class ChromeDriverManager
    {
        internal static ChromeDriver Create_WebDriver_Instance(string driverPath, string[] driverOptions = null)
        {
            ChromeOptions options = new ChromeOptions();
            ChromeDriver driver;
            if(driverOptions != null)
            {
                foreach(string driverOption in driverOptions)
                {
                    options.AddArgument(driverOption);
                }

                driver = new ChromeDriver(driverPath, options);
            }
            else
            {
                driver = new ChromeDriver(driverPath);
            }

            

            if (driverOptions == null || !driverOptions.ToList().Contains("--headless"))
                driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
