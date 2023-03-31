using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using WebAutomationToolKit.Exceptions;

namespace WebAutomationToolKit.Extensions
{
    internal static class WebDriverExtension
    {
        private const string ElementNotDisplayed = "Target element not displayed";
        private const string ElementNotEnabled = "Target element not enabled";

        internal static bool IsEnabled(IWebDriver driver, By locator, WebDriverWait wait)
        {
            IWebElement target = null;

            var result = wait.Until(condition =>
            {
                try
                {
                    target = driver.FindElement(locator);

                    return target.Enabled;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });

            if (!result)
            {
                target = driver.FindElement(locator);

                if (!target.Displayed)
                    throw new WebAutomationException(ElementNotDisplayed);

                if (!target.Enabled)
                    throw new WebAutomationException(ElementNotEnabled);
            }

            return result;
        }

        internal static bool IsDisplayed(IWebDriver driver, By locator, WebDriverWait wait)
        {
            IWebElement target = null;

            var result = wait.Until(condition =>
            {
                try
                {
                    target = driver.FindElement(locator);

                    return target.Displayed;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });

            if (!result)
            {
                target = driver.FindElement(locator);

                if (!target.Displayed)
                    throw new WebAutomationException(ElementNotDisplayed);

                if (!target.Enabled)
                    throw new WebAutomationException(ElementNotEnabled);
            }

            return result;
        }

        internal static bool IsDisplayedAndEnabled(IWebDriver driver, By locator, WebDriverWait wait)
        {
            IWebElement target = null;

            var result = wait.Until(condition =>
            {
                try
                {
                    target = driver.FindElement(locator);

                    return target.Displayed && target.Enabled;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });

            if (!result)
            {
                target = driver.FindElement(locator);

                if (!target.Displayed)
                    throw new WebAutomationException(ElementNotDisplayed);

                if (!target.Enabled)
                    throw new WebAutomationException(ElementNotEnabled);
            }

            return result;
        }

        internal static void Click(this IWebDriver driver, By locator, WebDriverWait wait)
        {
            IWebElement target = null;

            if (IsDisplayedAndEnabled(driver, locator, wait))
                target = driver.FindElement(locator);

            target.Click();
        }

        internal static void SendText(this IWebDriver driver, By locator, WebDriverWait wait, string text)
        {
            IWebElement target = null;

            if (IsDisplayedAndEnabled(driver, locator, wait))
                target = driver.FindElement(locator);

            target.SendKeys(text);
        }
                
        internal static void SendTextWithDelay(this IWebDriver driver, By locator, WebDriverWait wait, string text, int delay = 500)
        {
            IWebElement target = null;

            if (IsDisplayedAndEnabled(driver, locator, wait))
                target = driver.FindElement(locator);
                       
            foreach (var c in text.ToCharArray())
            {
                System.Threading.Thread.Sleep(delay);
                target.SendKeys(c.ToString());
            }
        }

        internal static void Clear(this IWebDriver driver, By locator, WebDriverWait wait)
        {
            IWebElement target = null;

            if (IsDisplayedAndEnabled(driver, locator, wait))
                target = driver.FindElement(locator);

            target.Clear();
        }

        internal static void MoveToElement(this IWebDriver driver, By locator, WebDriverWait wait)
        {
            IWebElement target = null;

            if (IsDisplayedAndEnabled(driver, locator, wait))
                target = driver.FindElement(locator);
            
            new Actions(driver).MoveToElement(target)
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
                        "var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) { items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;",
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
                    "var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) { items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;",
                    element) as Dictionary<string, object>;
            if (attributes == null) throw new NullReferenceException();

            var selector = "//" + element.TagName;
            selector = attributes.Aggregate(selector, (current, attribute) =>
                 current + "[@" + attribute.Key + "='" + attribute.Value + "']");

            return By.XPath(selector);
        }

    }
}
