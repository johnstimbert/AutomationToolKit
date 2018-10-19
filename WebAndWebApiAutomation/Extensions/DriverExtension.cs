using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;

namespace WebAndWebApiAutomation.Extensions
{
    internal static class DriverExtension
    {
        internal static void Click(this IWebDriver driver, By locator, WebDriverWait wait)
        {
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(locator));
            element.Click();
        }

        internal static void DoubleClick(this IWebDriver driver, By locator, WebDriverWait wait)
        {
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(locator));
            new Actions(driver).DoubleClick(element);
        }

        internal static void SendText(this IWebDriver driver, By locator, WebDriverWait wait, string text)
        {
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
            element.SendKeys(text);
        }

        /// <summary>
        /// Note: Only use this method to send text to elements that have autocomplete and require a delay on each letter typed.
        /// </summary>
        internal static void SendTextWithDelay(this IWebDriver driver, By locator, WebDriverWait wait, string text, int delay = 500)
        {
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
            foreach (var c in text.ToCharArray())
            {
                System.Threading.Thread.Sleep(delay);
                element.SendKeys(c.ToString());
            }
        }

        internal static void Clear(this IWebDriver driver, By locator, WebDriverWait wait)
        {
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
            element.Clear();
        }
        
        internal static void MoveToElement(this IWebDriver _driver, By locator, WebDriverWait wait)
        {
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
            new Actions(_driver).MoveToElement(element)
                .Build()
                .Perform();
        }

        internal static bool WaitForElementToBeSelected(this IWebDriver _driver, By locator, WebDriverWait wait)
        {
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeSelected(locator));            
        }

        internal static bool WaitForElementToBeVisible(this IWebDriver _driver, By locator, WebDriverWait wait)
        {
            var e = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
            if (e != null)
                return false;

            return true;
        }

        internal static bool WaitForUrlContains(this IWebDriver _driver, string text, WebDriverWait wait)
        {
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(text));            
        }

        internal static bool WaitForUrlRegexContains(this IWebDriver _driver, string pattern, WebDriverWait wait)
        {
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlMatches(pattern));
        }

        internal static IWebElement WaitForElementExists(this IWebDriver _driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(locator));
        }

    }
}
