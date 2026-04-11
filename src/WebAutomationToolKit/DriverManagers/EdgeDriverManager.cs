using OpenQA.Selenium.Edge;

namespace WebAutomationToolKit.DriverManagers
{
    internal static class EdgeDriverManager
    {
        internal static EdgeDriver Create_WebDriver_Instance(string driverPath, EdgeOptions driverOptions = null)
        {
            driverOptions ??= new EdgeOptions();
            var driver = new EdgeDriver(driverPath, driverOptions);
            driver.Manage().Window.Maximize();
            return driver;
        }

        internal static EdgeDriver Create_WebDriver_Instance(EdgeOptions driverOptions = null)
        {
            driverOptions ??= new EdgeOptions();
            var driver = new EdgeDriver(driverOptions);
            driver.Manage().Window.Maximize();
            return driver;
        }
    }
}
