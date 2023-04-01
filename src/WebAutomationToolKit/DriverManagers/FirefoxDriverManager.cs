using OpenQA.Selenium.Firefox;

namespace WebAutomationToolKit.DriverManagers
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
                    //TODO: Is this Needed?
                };

                firefoxProfile = defaultProfile;
            }

            if (driverOptions == null)
            {
                var defaultOptions = new FirefoxOptions
                {
                    //TODO: Is this Needed?
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
