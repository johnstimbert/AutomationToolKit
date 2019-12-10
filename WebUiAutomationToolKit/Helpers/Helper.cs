using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using WebUiAutomationToolKit.Exceptions;

namespace WebUiAutomationToolKit.Helpers
{
    internal static class Helper
    {
        #region INTERNAL ONLY DO NOT EXPOSE
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

        internal static WebDriverManager IsDriverNull(IWebDriverManager webDriverManager)
        {
            WebDriverManager m = (WebDriverManager)webDriverManager;

            if(m.GetActiveDriverType() == WebUiAutomationEnums.DriverType.None)
                throw new WebUiAutomationException("DriverType Provided was not found. Instantiate the driver before performing actions on or with it");

            if (m.GetActiveDriver() == null)
                throw new WebUiAutomationException("Driver Provided was null. Instantiate the driver before performing actions on or with it");

            return m;
        }
                
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
