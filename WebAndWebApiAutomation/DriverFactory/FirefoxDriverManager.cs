using OpenQA.Selenium.Firefox;

namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class FirefoxDriverManager
    {
        internal static FirefoxDriver Create_WebDriver_Instance(string driverPath, bool rundHeadless)
        {
            FirefoxOptions options = new FirefoxOptions();

            if (rundHeadless)
                options.AddArgument("--headless");

            var driver = new FirefoxDriver(driverPath, options);

            if(!rundHeadless)
                driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
