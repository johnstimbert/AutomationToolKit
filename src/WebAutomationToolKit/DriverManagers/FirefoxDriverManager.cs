using OpenQA.Selenium.Firefox;

namespace WebAutomationToolKit.DriverManagers
{
    internal static class FirefoxDriverManager
    {
        internal static FirefoxDriver Create_WebDriver_Instance(string driverPath, FirefoxOptions driverOptions = null, FirefoxProfile firefoxProfile = null)
        {
            driverOptions = BuildOptions(driverOptions, firefoxProfile);
            var driver = new FirefoxDriver(driverPath, driverOptions);
            driver.Manage().Window.Maximize();
            return driver;
        }

        internal static FirefoxDriver Create_WebDriver_Instance(FirefoxOptions driverOptions = null, FirefoxProfile firefoxProfile = null)
        {
            driverOptions = BuildOptions(driverOptions, firefoxProfile);
            var driver = new FirefoxDriver(driverOptions);
            driver.Manage().Window.Maximize();
            return driver;
        }

        private static FirefoxOptions BuildOptions(FirefoxOptions driverOptions, FirefoxProfile firefoxProfile)
        {
            if (driverOptions == null)
                driverOptions = new FirefoxOptions();

            if (firefoxProfile == null)
                firefoxProfile = new FirefoxProfile();

            driverOptions.Profile = firefoxProfile;
            return driverOptions;
        }
    }
}
