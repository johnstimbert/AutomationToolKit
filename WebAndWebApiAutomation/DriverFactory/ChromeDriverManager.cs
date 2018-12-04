using OpenQA.Selenium.Chrome;


namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class ChromeDriverManager
    {
        internal static ChromeDriver Create_WebDriver_Instance(string driverPath, bool rundHeadless)
        {
            ChromeOptions options = new ChromeOptions();

            if(rundHeadless)
                options.AddArgument("headless");

            var driver = new ChromeDriver(driverPath);

            if (!rundHeadless)
                driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
