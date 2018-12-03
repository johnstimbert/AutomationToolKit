using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static WebAndWebApiAutomation.WebAutomationEnums;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;
using WebAndWebApiAutomation.Helpers;
using WebAndWebApiAutomation.Extensions;
using WebAndWebApiAutomation.Exceptions;
using OpenQA.Selenium.Support.UI;

namespace WebAndWebApiAutomation.Validators
{
    internal class StructureValidator
    {
        internal static SelectorData _reCaptchaSelectorData = new SelectorData("reCaptcha", HtmlTagType.iframe, HtmlAttributeType.AttributeText_Contains, "https://www.google.com/recaptcha");
        private WebDriverWait _wait;

        public StructureValidator(WebDriverWait wait)
        {
            _wait = wait;
        }

        internal bool ReCaptchaPresent(IWebDriver driver)
        {
            bool result = false;

            if (CheckElementExistsReturnIWebElement(_reCaptchaSelectorData, driver) != null)
                result = true;

            return result;
        }

        /// <summary>
        /// Finds all elements matching the provided selector data and returns a list of xpath by objects for each found elements
        /// /// </summary>
        /// <param name="selectorData">Data to check for in the current DOM instance</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns></returns>
        internal List<By> GetAllBysUsingMatchingSelectorData(SelectorData selectorData, IWebDriver driver)
        {
            List<By> xPathBys = new List<By>();
            var cssBy = BuildCssSelectorBy(selectorData);

            var elements = driver.FindElements(cssBy);

            foreach (var element in elements)
            {
                xPathBys.Add(ConvertToXPathBy(element, driver));
            }

            return xPathBys;
        }

        /// <summary>
        /// Builds a CssSelector with the SelectorDataSet provided and uses it to check that an element exists with that data
        /// </summary>
        /// <param name="selectorDataSet">Data to check for in the current DOM instance</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns>bool</returns>
        internal bool CheckElementsExist(SelectorDataSet selectorDataSet, IWebDriver driver)
        {
            bool result = true;
            var items = selectorDataSet.SelectorDataItems;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                driver.WaitForElementExists(BuildCssSelectorBy(item), _wait);
            }

            return result;
        }

        /// <summary>
        /// Finds and returns the IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns>IWebElement</returns>
        internal IWebElement CheckElementExistsReturnIWebElement(SelectorData selectorData, IWebDriver driver)
        {
            IWebElement webElement = null;
            try
            {
                if (selectorData.AttributeType == HtmlAttributeType.InnerText_Contains || selectorData.AttributeType == HtmlAttributeType.InnerText_ExactMatch)
                {
                    webElement = CheckElementExistsByTagAndInnerText(selectorData, driver);
                }
                else if(selectorData.AttributeType == HtmlAttributeType.AttributeText_Contains || selectorData.AttributeType == HtmlAttributeType.AttributeText_ExactMatch)
                {
                    webElement = CheckElementExistsByAttributevalue(selectorData, driver);
                }
                else
                {
                    webElement = driver.WaitForElementExists(BuildCssSelectorBy(selectorData), _wait);
                }

                return webElement;
            }
            catch (Exception)
            {
                return webElement;
            }
        }

        /// <summary>
        /// Finds and returns the IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns></returns>
        internal IWebElement CheckElementExistsByTagAndInnerText(SelectorData selectorData, IWebDriver driver)
        {
            IWebElement webElement = null;
            var findSelector = new SelectorData(selectorData.Name, selectorData.TagType, HtmlAttributeType.None, string.Empty);
            var elements = driver.FindElements(BuildCssSelectorBy(findSelector));
            switch (selectorData.AttributeType)
            {
                case HtmlAttributeType.InnerText_Contains:
                    webElement = elements.FirstOrDefault(element => element.Text.Contains(selectorData.AttributeValue));
                    break;
                case HtmlAttributeType.InnerText_ExactMatch:
                    webElement = elements.FirstOrDefault(element => element.Text.Equals(selectorData.AttributeValue));
                    break;
                default:
                    throw new Exception($"{selectorData.AttributeValue} not support by {MethodBase.GetCurrentMethod().Name} use either " +
                                        $"{HtmlAttributeType.InnerText_Contains} or {HtmlAttributeType.InnerText_ExactMatch}");
            }

            return webElement;
        }

        /// <summary>
        /// Finds the IWebElement using the parameters provided and returns the CssSelector based By object, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns></returns>
        internal By CheckElementExistsReturnCssSelector(SelectorData selectorData, IWebDriver driver)
        {
            try
            {
                IWebElement webElement = null;

                var cssBy = BuildCssSelectorBy(selectorData);
                webElement = driver.WaitForElementExists(cssBy, _wait);
                               
                return cssBy;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds and returns the child IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="parentElement">The element to find the child element with</param>
        /// <param name="selectorData">>Data to build the CssSelector with</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns></returns>
        internal IWebElement CheckChildElementExists(IWebElement parentElement, SelectorData selectorData, IWebDriver driver)
        {
            try
            {
                IWebElement webElement = null;

                By childCssSelector = BuildCssSelectorBy(selectorData);

                webElement = parentElement.FindElement(childCssSelector);

                return webElement;
            }
            catch (Exception)
            {
                return null;
            }
        }
               
        /// <summary>
        /// Creates and returns an XPath By object for the element provided
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        internal By ConvertToXPathBy(IWebElement element, IWebDriver driver)
        {
            if (element == null) throw new NullReferenceException();

            if (!(((IJavaScriptExecutor)driver).ExecuteScript(
                    "var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) { items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;",
                    element) is Dictionary<string, object> attributes)) throw new NullReferenceException();

            var selector = "//" + element.TagName;
            selector = attributes.Aggregate(selector, (current, attribute) =>
                 current + "[@" + attribute.Key + "='" + attribute.Value + "']");

            return By.XPath(selector);
        }

        /// <summary>
        /// This method will remove html data from the innerText of a given element and return only text NOT contained in a child element. This method will not parse the innerText of a child element.
        /// </summary>
        /// <param name="innerText">Raw innerText from an IWebElement that contains html to be removed</param>
        /// <returns></returns>
        internal string ParseInnerText(string innerText)
        {
            if (string.IsNullOrEmpty(innerText))
                return null;

            string parsedInnertext = innerText;

            char[] charArray = innerText.ToCharArray();
            int i = 0;
            int length = charArray.Length;
            do
            {
                if (charArray[i].Equals("<"))
                {
                    //If the character in question looks like the opening of an html tag "<"
                    //we need to step through the following steps to try and remove the entire tag
                    // 1. Check to see if the text following the "<" matches a known html tag type
                    var potentialTagType = AttemptToMatchToTagType(0, parsedInnertext.Substring(i));
                    if(potentialTagType != null)
                    {
                        //Find the end of the opening tag by seeking the next instance of ">",
                        //This will handle any empty tags by default as well as self closing tags, ie. "/>"
                        var endOFOpeningTag = innerText.IndexOf(">", i, StringComparison.CurrentCultureIgnoreCase);
                        if(endOFOpeningTag > 0)
                        {
                            //Get the text to remove and replace it with an empty string
                            var textToRemove = parsedInnertext.Substring(i, endOFOpeningTag);
                            parsedInnertext.Replace(textToRemove, string.Empty);
                            //Reset the reference values
                            i = 0;
                            charArray = parsedInnertext.ToCharArray();
                            length = charArray.Length;
                        }
                        //Find the end of the closing tag by seeking the next instance of either "</tagName>". 
                        //This covers any non-empty and self-closing tags(ie. "<br> or "/>") by default.
                        var closingTagEndingIndex = parsedInnertext.IndexOf($"</{potentialTagType}>", i, StringComparison.CurrentCultureIgnoreCase);
                        if (closingTagEndingIndex > -1)
                        {
                            //Update the index to the position after the end of the tag location
                            closingTagEndingIndex = $"</{potentialTagType}>".Length;
                            //Get the text to remove and replace it with an empty string
                            var textToRemove = parsedInnertext.Substring(i, closingTagEndingIndex);
                            parsedInnertext.Replace(textToRemove, string.Empty);
                            //Reset the reference values
                            i = 0;
                            charArray = parsedInnertext.ToCharArray();
                            length = charArray.Length;
                        }                        
                    }
                }
                i++;
            } while (i < length);

            return parsedInnertext;
        }

        /// <summary>
        /// Finds and returns the IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns></returns>
        internal IWebElement CheckElementExistsByAttributevalue(SelectorData selectorData, IWebDriver driver)
        {
            IWebElement webElement = null;
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            IJavaScriptExecutor javaScriptExecutor = Helper.JavaScriptExecutor(driver);
            if(javaScriptExecutor == null)
                throw new WebAutomationException($"Could not create the needed IJavaScriptExecutor object using the {Helper.GetDriverBrowserName(driver)}");

            var findSelector = new SelectorData(selectorData.Name, selectorData.TagType, HtmlAttributeType.None, string.Empty);
            var elements = driver.FindElements(BuildCssSelectorBy(findSelector));
            switch(selectorData.AttributeType)
            {
                case HtmlAttributeType.AttributeText_Contains:
                    foreach(var element in elements)
                    {
                        //Get the attributes of the element using a javascript query
                        var attsObject = javaScriptExecutor.ExecuteScript("var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) { items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;", element);
                        //Cast them to a dictionary object
                        attributes = (Dictionary<string, object>)attsObject;
                        foreach(var attribute in attributes)
                        {
                            if (attribute.Value.ToString().ToLower().Contains(selectorData.AttributeValue.ToLower()))
                                return element;
                        }
                    }
                    break;
                case HtmlAttributeType.AttributeText_ExactMatch:
                    foreach (var element in elements)
                    {
                        //Get the attributes of the element using a javascript query
                        var attsObject = javaScriptExecutor.ExecuteScript("var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) { items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;", element);
                        //Cast them to a dictionary object
                        attributes = (Dictionary<string, object>)attsObject;
                        foreach (var attribute in attributes)
                        {
                            if (attribute.Value.ToString().ToLower().Equals(selectorData.AttributeValue.ToLower()))
                                return element;
                        }
                    }
                    break;
                default:
                    throw new Exception($"{selectorData.AttributeValue} not support by {MethodBase.GetCurrentMethod().Name} use either " +
                                        $"{HtmlAttributeType.InnerText_Contains} or {HtmlAttributeType.InnerText_ExactMatch}");
            }

            return webElement;
        }

        internal By BuildCssSelectorBy(SelectorData selectorData)
        {
            By CssSelector = null;
            var tag = selectorData.TagType.GetString().ToLower();
            
            switch (selectorData.AttributeType)
            {
                case HtmlAttributeType.Id:
                    CssSelector = By.CssSelector($"#{selectorData.AttributeValue}");
                    break;
                case HtmlAttributeType.Class:
                    CssSelector = By.CssSelector($"{tag}[{HtmlAttributeType.Class.ToString().ToLower()}='{selectorData.AttributeValue}']");
                    break;
                case HtmlAttributeType.Name:
                    CssSelector = By.CssSelector($"{tag}[{HtmlAttributeType.Name.ToString().ToLower()}='{selectorData.AttributeValue}']");
                    break;
                case HtmlAttributeType.Type:
                    CssSelector = By.CssSelector($"{tag}[{HtmlAttributeType.Type.ToString().ToLower()}='{selectorData.AttributeValue}']");
                    break;
                case HtmlAttributeType.Href:
                    CssSelector = By.CssSelector($"{tag}[{HtmlAttributeType.Href.ToString().ToLower()}='{selectorData.AttributeValue}']");
                    break;
                case HtmlAttributeType.Src:
                    CssSelector = By.CssSelector($"{tag}[{HtmlAttributeType.Src.ToString().ToLower()}='{selectorData.AttributeValue}']");
                    break;
                case HtmlAttributeType.Title:
                    CssSelector = By.CssSelector($"{tag}[{HtmlAttributeType.Title.ToString().ToLower()}='{selectorData.AttributeValue}']");
                    break;
                case HtmlAttributeType.FormControlName:
                    CssSelector = By.CssSelector($"{tag}[{HtmlAttributeType.FormControlName.ToString().ToLower()}='{selectorData.AttributeValue}']");
                    break;
                case HtmlAttributeType.None:
                    CssSelector = By.CssSelector($"{tag}");
                    break;
                case HtmlAttributeType.InnerText_ExactMatch:
                case HtmlAttributeType.InnerText_Contains:
                    CssSelector = By.CssSelector($"{tag}");
                    break;
                default:
                    //Helper.Logger.Info($"[Structure Test] ==== Attribute type {selectorData.AttributeType} is not supported ====");
                    break;
            }
            
            return CssSelector;
        }

        internal string AttemptToMatchToTagType(int startingIndex, string value)
        {
            string tagName = null;
            int potentialTagEnd = value.IndexOf(" ", startingIndex);
            string potentialTag = value.Substring(startingIndex + 1, value.Length - (potentialTagEnd - 1));
            if (potentialTagEnd < -1 && Enum.IsDefined(typeof(HtmlTagType), potentialTag))
            {
                tagName = potentialTag;
            }
                
            return tagName;
        }
        
    }
}
