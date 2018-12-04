using OpenQA.Selenium.IE;

namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class IEDriverManager
    {
        internal static InternetExplorerDriver Create_WebDriver_Instance(string driverPath, bool rundHeadless)
        {
            var optionsIE = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                EnableNativeEvents = false,
                IgnoreZoomLevel = true
            };

            var driver = new InternetExplorerDriver(driverPath, optionsIE);

            if(!rundHeadless)
                driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
