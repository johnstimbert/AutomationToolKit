﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WebAndWebApiAutomation.Exceptions;

namespace WebAndWebApiAutomation.Extensions
{
    internal static class DriverExtension
    {
        private const string ElementNotDisplayed = "Target element not displayed";
        private const string ElementNotEnabled = "Target element not enabled";

        private static bool IsDisplayedAndEnabled(IWebDriver driver, By locator, WebDriverWait wait)
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

        //internal static void DoubleClick(this IWebDriver driver, By locator, WebDriverWait wait)
        //{
        //    var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(locator));
        //    new Actions(driver).DoubleClick(element);
        //}

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

        //internal static bool WaitForElementToBeSelected(this IWebDriver _driver, By locator, WebDriverWait wait)
        //{
        //    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
        //    return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeSelected(locator));
        //}

        internal static bool WaitForElementToBeVisible(this IWebDriver driver, By locator, WebDriverWait wait)
        {
            return IsDisplayedAndEnabled(driver, locator, wait);
        }

        internal static bool WaitForUrlContains(this IWebDriver driver, string text, WebDriverWait wait)
        {
            return wait.Until(condition =>
            {
                    return driver.Url.Contains(text);
            });
        }

        //internal static bool WaitForUrlRegexContains(this IWebDriver _driver, string pattern, WebDriverWait wait)
        //{
        //    return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlMatches(pattern));
        //}

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

    }
}
