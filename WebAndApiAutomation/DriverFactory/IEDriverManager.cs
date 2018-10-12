using OpenQA.Selenium.IE;

namespace WebAndApiAutomation.DriverFactory
{
    internal static class IEDriverManager
    {
        internal static InternetExplorerDriver Create_WebDriver_Instance(string driverPath)
        {
            var optionsIE = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                EnableNativeEvents = false,
                IgnoreZoomLevel = true
            };
            var driver = new InternetExplorerDriver(driverPath, optionsIE);
            driver.Manage().Window.Maximize();
            return driver;
        }
    }
}
