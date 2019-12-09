using OpenQA.Selenium.Chrome;

namespace WebUiAutomationToolKit.DriverFactory
{
    internal static class ChromeDriverManager
    {
        internal static ChromeDriver Create_WebDriver_Instance(string driverPath, ChromeOptions driverOptions = null)
        {
            ChromeDriver driver;

            var defaultOptions = new ChromeOptions();
            defaultOptions.AcceptInsecureCertificates = true;
            defaultOptions.UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify;

            if (driverOptions == null)
                driverOptions = defaultOptions;

            driver = new ChromeDriver(driverPath, driverOptions);

            if(!driverOptions.Arguments.Contains("--headless"))
                driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
