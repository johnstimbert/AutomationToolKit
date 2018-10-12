using OpenQA.Selenium.Firefox;

namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class FirefoxDriverManager
    {
        internal static FirefoxDriver Create_WebDriver_Instance(string driverPath)
        {
            var driver = new FirefoxDriver(driverPath);
            driver.Manage().Window.Maximize();
            return driver;
        }
    }
}
