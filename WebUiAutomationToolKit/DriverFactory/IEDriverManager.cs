using OpenQA.Selenium.IE;

namespace WebUiAutomationToolKit.DriverFactory
{
    internal static class IEDriverManager
    {
        internal static InternetExplorerDriver Create_WebDriver_Instance(string driverPath, InternetExplorerOptions driverOptions = null)
        {
            var defaultOptions = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                EnableNativeEvents = false,
                IgnoreZoomLevel = true,
                UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify
            };

            if (driverOptions == null)
                driverOptions = defaultOptions;

            var driver = new InternetExplorerDriver(driverPath, driverOptions);

            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
