﻿using OpenQA.Selenium.Firefox;
using System.Linq;

namespace WebAndWebApiAutomation.DriverFactory
{
    internal static class FirefoxDriverManager
    {
        internal static FirefoxDriver Create_WebDriver_Instance(string driverPath, string[] driverOptions = null)
        {
            FirefoxOptions options = new FirefoxOptions();
            FirefoxDriver driver;
            if (driverOptions != null)
            {
                foreach (string driverOption in driverOptions)
                {
                    options.AddArgument(driverOption);
                }
                driver = new FirefoxDriver(driverPath, options);
            }
            else
            {
                driver = new FirefoxDriver(driverPath);
            }

            if (driverOptions == null || !driverOptions.ToList().Contains("--headless"))
                driver.Manage().Window.Maximize();

            return driver;
        }
    }
}
