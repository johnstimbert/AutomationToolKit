using OpenQA.Selenium.Edge;

namespace WebAutomationToolKit.DriverManagers
{
    internal static class EdgeDriverManager
    {
        internal static EdgeDriver Create_WebDriver_Instance(string driverPath, EdgeOptions? driverOptions = null)
        {
           driverOptions = BuildOptions(driverOptions);
            var driver = new EdgeDriver(driverPath, driverOptions);
            driver.Manage().Window.Maximize();
            return driver;
        }

        internal static EdgeDriver Create_WebDriver_Instance(EdgeOptions? driverOptions = null)
        {
           driverOptions = BuildOptions(driverOptions);
            var driver = new EdgeDriver(driverOptions);
            driver.Manage().Window.Maximize();
            return driver;
        }

       private static EdgeOptions BuildOptions(EdgeOptions? driverOptions)
       {
           if (driverOptions != null)
               return driverOptions;

           var defaultOptions = new EdgeOptions();
           defaultOptions.AcceptInsecureCertificates = true;
           defaultOptions.UnhandledPromptBehavior = OpenQA.Selenium.UnhandledPromptBehavior.DismissAndNotify;
           return defaultOptions;
       }
    }
}
