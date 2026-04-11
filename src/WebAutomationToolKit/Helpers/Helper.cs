using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.InternalImplementations;

namespace WebAutomationToolKit.Helpers
{
    internal static class Helper
    {
        /// <summary>
        /// JavaScript snippet that collects all attributes of a DOM element into a name→value dictionary.
        /// Shared by StructureValidator, WebDriverExtension, and DomNode to avoid duplication (item 14).
        /// </summary>
        internal const string GetAttributesScript =
            "var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) " +
            "{ items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;";

        #region INTERNAL ONLY DO NOT EXPOSE
        internal static void TakeScreenShot(IWebDriver driver, string screenShotPath, string screenShotName)
        {
            if (!Directory.Exists(screenShotPath))
                Directory.CreateDirectory(screenShotPath);

            var SSName = Path.Combine(screenShotPath, $"{screenShotName}_{DateTime.Now.ToFileTime()}.jpeg");
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            ss.SaveAsFile(SSName);
        }

        internal static void TakeElementScreenShot(string screenShotPath, string screenShotName, IWebElement element)
        {
            if (!Directory.Exists(screenShotPath))
                Directory.CreateDirectory(screenShotPath);

            var SSName = Path.Combine(screenShotPath, $"{screenShotName}_{DateTime.Now.ToFileTime()}.jpeg");
            Screenshot ss = ((ITakesScreenshot)element).GetScreenshot();
            ss.SaveAsFile(SSName);
        }

        internal static IJavaScriptExecutor JavaScriptExecutor(IWebDriver driver)
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            return js;
        }

        internal static void HighlightElement(IWebDriver driver, By locator)
        {
            var js = (IJavaScriptExecutor)driver;

            string highlightJavascript = @"arguments[0].style.cssText = ""border-width: 3px; border-style: solid; border-color: red""; ";
            js.ExecuteScript(highlightJavascript, new object[] { driver.FindElement(locator) });
            highlightJavascript = @"arguments[0].style.cssText = ""border-width: 0px"";";
            js.ExecuteScript(highlightJavascript, new object[] { driver.FindElement(locator) });
        }

        internal static void ClickUsingJavaScript(IWebDriver driver, By locator)
        {
            var js = (IJavaScriptExecutor)driver;
            var script = "arguments[0].click();";
            js.ExecuteScript(script, driver.FindElement(locator));
        }

        internal static string GetDriverBrowserName(IWebDriver driver)
        {
            return (driver as RemoteWebDriver).Capabilities.GetCapability("browserName").ToString();
        }

        internal static IWebDriverManagerInternal IsDriverNull(IWebDriverManager webDriverManager)
        {
            var m = webDriverManager as IWebDriverManagerInternal
                ?? throw new WebAutomationException("The provided IWebDriverManager does not implement the required internal interface");

            if (m.GetActiveDriverType() == WebAutomationEnums.DriverType.None)
                throw new WebAutomationException("DriverType Provided was not found. Instantiate the driver before performing actions on or with it");

            if (m.GetActiveDriver() == null)
                throw new WebAutomationException("Driver Provided was null. Instantiate the driver before performing actions on or with it");

            return m;
        }

        internal static string GetString<T>(this T value)
        {
            if (typeof(T).IsEnum)
            {
                FieldInfo field = typeof(T).GetField(value.ToString());
                if (field.IsDefined(typeof(DescriptionAttribute), false))
                {
                    DescriptionAttribute[] xmlEnum = (DescriptionAttribute[])typeof(T).GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
                    return xmlEnum[0].Description;
                }
            }
            return value.ToString();
        }
        #endregion
    }
}
