using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.Helpers;

namespace WebAutomationToolKit.Extensions
{
    internal static class WebDriverExtension
    {
        private const string ElementNotDisplayed = "Target element not displayed";
        private const string ElementNotEnabled = "Target element not enabled";

        internal static bool IsEnabled(IWebDriver driver, By locator, WebDriverWait wait)
            => WaitForCondition(driver, locator, wait, e => e.Enabled);

       internal static bool IsDisplayed(IWebDriver driver, By locator, WebDriverWait wait)
           => WaitForCondition(driver, locator, wait, e => e.Displayed);

       internal static bool IsDisplayedAndEnabled(IWebDriver driver, By locator, WebDriverWait wait)
           => WaitForCondition(driver, locator, wait, e => e.Displayed && e.Enabled);

       private static bool WaitForCondition(IWebDriver driver, By locator, WebDriverWait wait,
           Func<IWebElement, bool> predicate)
       {
           IWebElement capturedTarget = null;

           var result = wait.Until(condition =>
           {
               try
               {
                   capturedTarget = driver.FindElement(locator);
                   return predicate(capturedTarget);
               }
               catch (StaleElementReferenceException) { return false; }
               catch (NoSuchElementException) { return false; }
           });

           if (!result)
           {
               capturedTarget = driver.FindElement(locator);
               if (!capturedTarget.Displayed)
                   throw new WebAutomationException(ElementNotDisplayed);
               if (!capturedTarget.Enabled)
                   throw new WebAutomationException(ElementNotEnabled);
           }

           return result;
       }

        internal static void Click(this IWebDriver driver, By locator, WebDriverWait wait)
        {
           IsDisplayedAndEnabled(driver, locator, wait);
           driver.FindElement(locator).Click();
        }

        internal static void SendText(this IWebDriver driver, By locator, WebDriverWait wait, string text)
        {
           IsDisplayedAndEnabled(driver, locator, wait);
           driver.FindElement(locator).SendKeys(text);
        }
                
        internal static void SendTextWithDelay(this IWebDriver driver, By locator, WebDriverWait wait, string text, int delay = 500)
        {
           IsDisplayedAndEnabled(driver, locator, wait);
           var target = driver.FindElement(locator);
           foreach (var c in text.ToCharArray())
            {
                System.Threading.Thread.Sleep(delay);
                target.SendKeys(c.ToString());
            }
        }

        internal static void Clear(this IWebDriver driver, By locator, WebDriverWait wait)
        {
           IsDisplayedAndEnabled(driver, locator, wait);
           driver.FindElement(locator).Clear();
        }

        internal static void MoveToElement(this IWebDriver driver, By locator, WebDriverWait wait)
        {
           IsDisplayedAndEnabled(driver, locator, wait);
           new Actions(driver).MoveToElement(driver.FindElement(locator))
                .Build()
                .Perform();
        }

        internal static bool WaitForUrlContains(this IWebDriver driver, string text, WebDriverWait wait)
        {
            return wait.Until(condition =>
            {
                    return driver.Url.Contains(text);
            });
        }

        internal static bool WaitForUrlRegexContains(this IWebDriver driver, string pattern, WebDriverWait wait)
        {
            return wait.Until(condition => {

                return Regex.IsMatch(driver.Url, pattern);
            });
        }

        internal static IWebElement WaitForElementExists(this IWebDriver driver, By locator, WebDriverWait wait)
        {
            IWebElement target = null;

            if (wait.Until(condition =>
            {
                try
                {
                    target = driver.FindElement(locator);
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }))
                return target;

            return target;
        }

        internal static List<By> ConvertToXPathBy(this ReadOnlyCollection<IWebElement> elements, IWebDriver driver)
        {
            List<By> x = new List<By>();

            foreach (var element in elements)
            {
                if (element == null) throw new NullReferenceException();

                var attributes =
                    ((IJavaScriptExecutor)driver).ExecuteScript(
                        Helper.GetAttributesScript,
                        element) as Dictionary<string, object>;
                if (attributes == null) throw new NullReferenceException();

                var selector = "//" + element.TagName;
                selector = attributes.Aggregate(selector, (current, attribute) =>
                     current + "[@" + attribute.Key + "='" + attribute.Value + "']");

                x.Add(By.XPath(selector));
            }

            return x;
        }

        public static By ConvertToBy(this IWebElement element, IWebDriver driver)
        {
            if (element == null) throw new NullReferenceException();

            var attributes =
                ((IJavaScriptExecutor)driver).ExecuteScript(
                    Helper.GetAttributesScript,
                    element) as Dictionary<string, object>;
            if (attributes == null) throw new NullReferenceException();

            var selector = "//" + element.TagName;
            selector = attributes.Aggregate(selector, (current, attribute) =>
                 current + "[@" + attribute.Key + "='" + attribute.Value + "']");

            return By.XPath(selector);
        }

    }
}
