using OpenQA.Selenium.Chrome;

namespace WebAutomationToolKit.DriverManagers
{
    internal static class ChromeDriverManager
    {
        internal static ChromeDriver Create_WebDriver_Instance(string driverPath, ChromeOptions? driverOptions = null)
        {
            driverOptions = BuildOptions(driverOptions);
            var driver = new ChromeDriver(driverPath, driverOptions);
            if (!driverOptions.Arguments.Contains("--headless"))
                driver.Manage().Window.Maximize();
            return driver;
        }

        internal static ChromeDriver Create_WebDriver_Instance(ChromeOptions? driverOptions = null)
        {
            driverOptions = BuildOptions(driverOptions);
            var driver = new ChromeDriver(driverOptions);
            if (!driverOptions.Arguments.Contains("--headless"))
                driver.Manage().Window.Maximize();
            return driver;
        }

        private static ChromeOptions BuildOptions(ChromeOptions? driverOptions)
        {
            if (driverOptions != null)
                return driverOptions;

            var defaultOptions = new ChromeOptions();
            defaultOptions.AcceptInsecureCertificates = true;
            defaultOptions.UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify;
            return defaultOptions;
        }
    }
}
