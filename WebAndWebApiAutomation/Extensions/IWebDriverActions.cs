using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace WebAndWebApiAutomation.Extensions
{
    public static class IWebDriverActions
    {
        internal static readonly int TimeoutForWait = 5;
        internal static readonly int TimeourForImplicitWait = 5;

        public static void Click(this IWebDriver _driver, By locator)
        {
            var element = _driver.FindElement(locator);
            element.Click();
        }

        public static void DoubleClick(this IWebDriver _driver, By locator)
        {
            var element = _driver.FindElement(locator);
            new Actions(_driver).DoubleClick(element);
        }

        public static void SendText(this IWebDriver _driver, By locator, string text)
        {
            var element = _driver.FindElement(locator);
            element.SendKeys(text);
        }

        /// <summary>
        /// Note: Only use this method to send text to elements that have autocomplete and require a delay on each letter typed.
        /// </summary>
        public static void SendTextWithDelay(this IWebDriver _driver, By locator, string text)
        {
            var element = _driver.FindElement(locator);
            foreach(var c in text.ToCharArray())
            {
                System.Threading.Thread.Sleep(500);
                element.SendKeys(c.ToString());
            }
        }

        public static void Clear(this IWebDriver _driver, By locator)
        {
            var element = _driver.FindElement(locator);
            element.Clear();
        }

        public static void SelectDropdownOption(this IWebDriver _driver, By locator, string id)
        {
            var element = _driver.FindElement(locator);
            var options = element.FindElements(By.CssSelector("ul > li > a"));
            options.FirstOrDefault(o => o.Text == id)?.Click();
        }

        public static void MouseOverOnElement(this IWebDriver _driver, By locator)
            => new Actions(_driver).MoveToElement(_driver.WaitForElementToBeVisible(locator)).Build().Perform();

        public static void MouseOverOnElement(this IWebDriver _driver, IWebElement element)
            => new Actions(_driver).MoveToElement(element).Build().Perform();

        
        
        #region Waits
        public static IWebElement WaitForElementToBeClickable(this IWebDriver _driver, By locator)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(locator));
            }
            catch (WebDriverTimeoutException)
            {
                Helper.Logger.Error($"Error Waiting for Element To Be Clickable : {locator}");
                return null;
            }
        }

        public static IWebElement WaitForElementToBeVisible(this IWebDriver _driver, By locator)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
            }
            catch (WebDriverTimeoutException)
            {
                Helper.Logger.Error($"Error Waiting for Element To Be Visible : {locator}");
                return null;
            }
        }

        public static bool WaitForElementToBeSelected(this IWebDriver _driver, By locator)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeSelected(locator));
            }
            catch (WebDriverTimeoutException)
            {
                Helper.Logger.Error($"Error Waiting for Element To Be Selected : {locator}");
                return false;
            }
        }

        public static IWebElement WaitForElementExists(this IWebDriver _driver, By locator)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(locator));
            }
            catch (WebDriverTimeoutException)
            {
                Helper.Logger.Error($"Error Waiting for Element Exists : {locator}");
                return null;
            }
        }

        public static bool WaitForUrlContains(this IWebDriver _driver, string locator)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains(locator));
            }
            catch (WebDriverTimeoutException)
            {
                Helper.Logger.Error($"Error waiting for Url contains: {locator} | actual: {_driver.Url}");
                return false;
            }
        }

        public static bool WaitForUrlRegexContains(this IWebDriver _driver, string pattern)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlMatches(pattern));
            }
            catch (WebDriverTimeoutException ex)
            {
                Helper.Logger.Error($"Error waiting for Url Pattern contains: {pattern}" + ex.ToString());
                return false;
            }
        }

        public static bool WaitForInvisibilityOfElement(this IWebDriver _driver, By locator)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(locator));
                WaitForXTime(_driver);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static void WaitForXTime(this IWebDriver _driver)
        {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TimeourForImplicitWait);
        }

        public static bool WaitForElementContentText(this IWebDriver _driver, IWebElement element, string textToContent)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(element, textToContent));

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static bool WaitForTextToBePresentInElementValue(this IWebDriver _driver, By locator, string text)
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementLocated(locator, text));
            }
            catch(WebDriverTimeoutException ex)
            {
                Helper.Logger.Error($"Error waiting for text present in element: {Environment.NewLine}Locator: {locator} {Environment.NewLine}Text: {text} {Environment.NewLine}{ex.ToString()}");
                return false;
            }
        }

        public static string WaitForAttributeChangedOnAction(this IWebDriver _driver, By locator, string attribute, Action<IWebDriver, By> action)
        {
            try
            {
                // Get Initial value
                var initValue = _driver.FindElement(locator).GetAttribute(attribute);

                // Execute Action
                action(_driver, locator);

                // Wait for attribute changed
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(d =>
                {
                    var value = d.FindElement(locator).GetAttribute(attribute);
                    if (initValue != value) return value;

                    return null;
                });
            }
            catch(Exception ex)
            {
                Helper.Logger.Error($"WaitForAttributeChangedOnAction | locator: {locator} | attribute: {attribute} {Environment.NewLine}{ex}");
                return null;
            }
        }

        public static string WaitForCssValueChangedOnAction(this IWebDriver _driver, By locator, string attribute, Action<IWebDriver, By> action)
        {
            try
            {
                // Get Initial value
                var initValue = _driver.FindElement(locator).GetCssValue(attribute);

                // Execute Action
                action(_driver, locator);

                // Wait for CssAttribute changed
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutForWait));
                return wait.Until(d =>
                {
                    var value = d.FindElement(locator).GetCssValue(attribute);
                    if (initValue != value) return value;

                    return null;
                });
            }
            catch (Exception ex)
            {
                Helper.Logger.Error($"WaitForCssValueChangedOnAction | locator: {locator} | CssAttribute: {attribute} {Environment.NewLine}{ex}");
                return null;
            }
        }
        
        public static bool VerifyElementDisplayed(this IWebDriver _driver, By locator)
        {
            try
            {
                if (!_driver.FindElement(locator).Displayed)
                {
                    Helper.Logger.Error($"Error finding element: {locator}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Helper.Logger.Error($"Error finding element: {locator}{Environment.NewLine}{ex}");
                return false;
            }
        }
        
        public static void MoveToSomePlace(this IWebDriver _driver, By locator)
        {
            new Actions(_driver).MoveToElement(_driver.FindElement(locator))
                .Build()
                .Perform();
        }

        #endregion
    }
}
