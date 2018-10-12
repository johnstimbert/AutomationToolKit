using System.IO;
using OpenQA.Selenium.Chrome;


namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class ChromeDriverManager
    {
        internal static ChromeDriver Create_WebDriver_Instance(string driverPath)
        {
            var driverLocation = Path.Combine(Path.GetDirectoryName(driverPath));
            var driver = new ChromeDriver(driverLocation);
            driver.Manage().Window.Maximize();
            return driver;
        }
    }
}
