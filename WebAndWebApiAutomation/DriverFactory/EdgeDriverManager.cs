using OpenQA.Selenium.Edge;
using System.IO;

namespace WebAndWebApiAutomation.DriverFactory
{
    /// Notes : Take care when testing with Edge to avoid problems you should use the correct driver for the OS Build that you have, nowadays we have OS Build 16299 in case the server o machines has 
    ///         a different one, you should go to https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/ and look up for the driver that match with your OS Build
    internal static class EdgeDriverManager
    {

        internal static EdgeDriver Create_WebDriver_Instance(string driverPath, bool rundHeadless)
        {
            //var driverLocation = Path.Combine(Path.GetDirectoryName(driverPath));
            var driver = new EdgeDriver(driverPath);

            driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
