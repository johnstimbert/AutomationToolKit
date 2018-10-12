using log4net;
using OpenQA.Selenium;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace WebAndWebApiAutomation.Extensions
{
    internal static class Helper
    {
        internal static readonly ILog Logger =
           LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static void TakeScreenShot(IWebDriver driver, string sreenShotPath, string screenShotName)
        {
            if (!Directory.Exists(sreenShotPath))
                Directory.CreateDirectory(sreenShotPath);

            var SSName = Path.Combine(sreenShotPath, $"{screenShotName}_{DateTime.Now.ToFileTime()}.jpeg");
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
    }
}
