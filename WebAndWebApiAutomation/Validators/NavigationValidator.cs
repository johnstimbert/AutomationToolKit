using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAndWebApiAutomation.Extensions;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation.Validators
{
    internal class NavigationValidator
    {
        private By _mainElementSelector = By.TagName("Body");
        private List<By> _anchorXPathBys = new List<By>();
        WebDriverWait _wait;

        internal NavigationValidator(WebDriverWait wait)
        {
            _wait = wait;
        }

        internal List<KeyValuePair<By, NavigationResult>> TestLinkNavigationForAllAnchorsFoundInPage(IWebDriver driver)
        {
            List<KeyValuePair<By, NavigationResult>> results = new List<KeyValuePair<By, NavigationResult>>();
            //Get the containing element
            var bodyElement = driver.FindElement(_mainElementSelector);
            //Collect all the anchortags and build xpath selectors for each
            _anchorXPathBys = bodyElement.FindElements(By.TagName("a")).ConvertToXPathBy(driver);
            //Get the url we are starting from as we will need to navigate back to it after each anchor is tested
            var startingUrl = driver.Url;

            for (int i = 0; i < _anchorXPathBys.Count; i++)
            {
                //Set default result values
                NavigationResult navResult = NavigationResult.NavigationNotAttempted;
                //Get the current anchor using xpath and it's href attribute value
                IWebElement currentAnchor = driver.FindElement(_anchorXPathBys.ElementAt(i));
                string hrefValue = currentAnchor.GetAttribute("href");
                //The href value determines our action
                if (hrefValue == null)
                {
                    //Null results in no operation. Not a true success or a failure.
                    navResult = NavigationResult.HrefIsNotPresent;
                }
                else
                {
                    //Get the Url the browser should land on after navigation
                    string expectedUrl = GetExpectedUrl(hrefValue, driver);
                    //Get and return the Url resulting from the navigation
                    string postNavigationUrl = GetPostNavigationUrl(driver);

                    string result = AttemptNavigation(_anchorXPathBys.ElementAt(i), postNavigationUrl, driver);
                    switch (result)
                    {
                        case "True":
                            navResult = NavigationResult.Success;
                            break;
                        case "False":
                            navResult = NavigationResult.Failed;
                            break;
                        default:
                            navResult = NavigationResult.Failed;
                            break;
                    }
                }

                results.Add(new KeyValuePair<By, NavigationResult>(_anchorXPathBys.ElementAt(i), navResult));

                driver.Navigate().GoToUrl(startingUrl);
                driver.WaitForUrlContains(startingUrl, _wait);
            }
            return results;
        }

        private string AttemptNavigation(By currentAnchor, string postNavigationUrl, IWebDriver driver)
        {
            var element = driver.FindElement(currentAnchor);
            //Navigate
            driver.Click(currentAnchor, _wait);

            //Wait for the expected url
            driver.WaitForUrlContains(postNavigationUrl, _wait);

            //Return the result of comparing the expected result to the actual
            return postNavigationUrl.Equals(driver.Url, StringComparison.CurrentCultureIgnoreCase).ToString();
        }

        private string GetPostNavigationUrl(IWebDriver driver)
        {
            //Get all open tabs
            var tabs = driver.WindowHandles;
            if (tabs.Count > 1)//Get the newest tab if there is more than one
                driver.SwitchTo().Window(tabs[tabs.Count - 1]);
            //Return the driver url
            return driver.Url;
        }

        private string GetExpectedUrl(string hrefValue, IWebDriver driver)
        {
            //Close any tabs that are open beyond the current
            var tabs = driver.WindowHandles;
            var currentHandle = driver.CurrentWindowHandle;
            if (tabs.Count > 1)
            {
                for (int i = 1; i < tabs.Count; i++)
                {
                    if (!tabs[i].Equals(currentHandle))
                        driver.SwitchTo().Window(tabs[i]).Close();
                }
                driver.SwitchTo().Window(currentHandle);
            }

            string expectedUrl = string.Empty;
            //If the href contains '#' return the current Url of the driver
            if (hrefValue.Contains('#'))
                return driver.Url;
            //If the href is a fully qualified url return it
            if (hrefValue.Contains("www."))
                return hrefValue;
            //Split the current driver url and href on '/'
            var driverUrl = driver.Url.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var splitHref = hrefValue.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            //Compare the last element of the driver url and the first element of the href
            if (driverUrl[driverUrl.Length].Equals(splitHref[0]))
            {//if they match append the href less the duplicate part of the path
                string modifiedHref = string.Empty;
                for (int i = 1; i < splitHref.Length; i++)
                {
                    modifiedHref += $"{splitHref[i]}/";
                }

                expectedUrl = $"{driver.Url}/{modifiedHref}";
            }
            else
            {//if they do not match append the href to the current Url
                string modifiedHref = string.Empty;
                for (int i = 0; i < splitHref.Length; i++)
                {
                    modifiedHref += $"{splitHref[i]}/";
                }
                expectedUrl = $"{driver.Url}/{modifiedHref}";
            }

            return expectedUrl;
        }

    }
}
