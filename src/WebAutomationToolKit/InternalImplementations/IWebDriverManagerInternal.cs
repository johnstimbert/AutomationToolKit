using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.InternalImplementations
{
    /// <summary>
    /// Internal interface that exposes driver-level accessors needed by helpers and
    /// internal consumers without requiring a cast to the concrete WebDriverManager type.
    /// </summary>
    internal interface IWebDriverManagerInternal
    {
        IWebDriver GetActiveDriver();
        WebDriverWait GetWait();
        DriverType GetActiveDriverType();
    }
}
