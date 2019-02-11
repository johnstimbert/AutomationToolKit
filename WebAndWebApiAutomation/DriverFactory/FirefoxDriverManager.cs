using OpenQA.Selenium.Firefox;

namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class FirefoxDriverManager
    {
        internal static FirefoxDriver Create_WebDriver_Instance(string driverPath, FirefoxOptions driverOptions = null, FirefoxProfile firefoxProfile = null)
        {
            FirefoxDriver driver;

            if (firefoxProfile == null)
            {
                var defaultProfile = new FirefoxProfile
                {
                    AcceptUntrustedCertificates = true,
                    DeleteAfterUse = true,
                    AssumeUntrustedCertificateIssuer = true
                };

                firefoxProfile = defaultProfile;
            }

            if (driverOptions == null)
            {
                var defaultOptions = new FirefoxOptions
                {
                    AcceptInsecureCertificates = true,
                    UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify,
                    UseLegacyImplementation = false
                };

                driverOptions = defaultOptions;
            }

            driverOptions.Profile = firefoxProfile;
            
            driver = new FirefoxDriver(driverPath, driverOptions);

            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
