using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using WebAndWebApiAutomation.Exceptions;

namespace WebAndWebApiAutomation.Helpers
{
    internal static class Helper
    {
        internal static void TakeScreenShot(IWebDriver driver, string screenShotPath, string screenShotName)
        {
            if (!Directory.Exists(screenShotPath))
                Directory.CreateDirectory(screenShotPath);

            var SSName = Path.Combine(screenShotPath, $"{screenShotName}_{DateTime.Now.ToFileTime()}.jpeg");
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();                
            ss.SaveAsFile(SSName, ScreenshotImageFormat.Jpeg);
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

        internal static void IsDriverNull(IWebDriver driver)
        {
            if (driver == null)
                throw new WebAutomationException("Driver Provided was null. Instantiate the driver before performing actions on or with it");
        }

        #region INTERNAL ONLY DO NOT EXPOSE
        /*
         * Both of these methods are used by the structure validator class and should not be exposed outside of the assembly
         */
        internal static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        internal static string GetString<T>(this T value) //where T : struct
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
