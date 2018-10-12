using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace WebAndWebApiAutomation.WebExtensions
{
    public static class Helper
    {
        public static readonly log4net.ILog Logger =
           log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void TakeScreenShot(IWebDriver _driver, string SectionName, string sreenShotPath, string testMethodName)
        {
            if (!Directory.Exists(sreenShotPath))
                Directory.CreateDirectory(sreenShotPath);

            var SSName = Path.Combine(sreenShotPath, $"{testMethodName}_{SectionName}_{DateTime.Now.ToFileTime()}.jpeg");
            Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();                
            ss.SaveAsFile(SSName, ScreenshotImageFormat.Jpeg);
        }
               
        public static IJavaScriptExecutor Scripts(this IWebDriver _driver)
        {
            IJavaScriptExecutor js = _driver as IJavaScriptExecutor;
            return js;
        }

        public static void HighlightElement(IWebDriver _driver, By locator)
        {
            var js = (IJavaScriptExecutor)_driver;
            
            string highlightJavascript = @"arguments[0].style.cssText = ""border-width: 3px; border-style: solid; border-color: red""; ";
            js.ExecuteScript(highlightJavascript, new object[] { _driver.FindElement(locator) });
            highlightJavascript = @"arguments[0].style.cssText = ""border-width: 0px"";";
            js.ExecuteScript(highlightJavascript, new object[] { _driver.FindElement(locator) });
        }

        public static void ClickingWithJavaScript(IWebDriver _driver, By locator)
        {
            var js = (IJavaScriptExecutor)_driver;
            var script = "arguments[0].click();";
            js.ExecuteScript(script, _driver.FindElement(locator));
        }

        public static bool IsOrdered<T>(this IList<T> list, IComparer<T> comparer = null)
        {
            if (comparer == null) comparer = Comparer<T>.Default;

            if (list.Count > 1)
                for (int i = 1; i < list.Count; i++)
                    if (comparer.Compare(list[i - 1], list[i]) > 0)
                        return false;

            return true;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static bool IsPhoneNumberLink(this IWebDriver _driver, By locator) 
            => _driver.FindElement(locator).GetAttribute("href").StartsWith("tel:");

        public static string GetString<T>(this T value) //where T : struct
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
