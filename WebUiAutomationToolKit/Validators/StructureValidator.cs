using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static WebUiAutomationToolKit.WebUiAutomationEnums;
using WebUiAutomationToolKit.WebAndApiAutomationObjects;
using WebUiAutomationToolKit.Helpers;
using WebUiAutomationToolKit.Extensions;
using WebUiAutomationToolKit.Exceptions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace WebUiAutomationToolKit.Validators
{
    internal class StructureValidator
    {
        internal static SelectorData _reCaptchaSelectorData = new SelectorData("reCaptcha", HtmlTagType.iframe, HtmlAttributeType.AttributeText_Contains, "https://www.google.com/recaptcha");

        internal StructureValidator()
        {
        }

        internal bool ReCaptchaPresent(IWebDriver driver, WebDriverWait wait)
        {
            bool result = false;

            if (CheckElementExistsReturnIWebElement(_reCaptchaSelectorData, driver, wait) != null)
                result = true;

            return result;
        }

        /// <summary>
        /// Finds and returns all elements matching the provided selector data 
        /// /// </summary>
        /// <param name="selectorData">Data to check for in the current DOM instance</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <returns></returns>
        internal ReadOnlyCollection<IWebElement> GetAllBysUsingMatchingSelectorData(SelectorData selectorData, IWebDriver driver)
        {
            return driver.FindElements(BuildCssSelectorBy(selectorData));
        }

        /// <summary>
        /// Builds a CssSelector with the SelectorDataSet provided and uses it to check that an element exists with that data
        /// </summary>
        /// <param name="selectorDataSet">Data to check for in the current DOM instance</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <param name="wait"></param>
        /// <returns>bool</returns>
        internal bool CheckElementsExist(SelectorDataSet selectorDataSet, IWebDriver driver, WebDriverWait wait)
        {
            bool result = true;
            var items = selectorDataSet.SelectorDataItems;
            for (int i = 0; i < items.Count; i++)
            {
                string itemName = string.Empty;
                try
                {
                    var item = items[i];
                    itemName = item.Name;
                    driver.WaitForElementExists(BuildCssSelectorBy(item), wait);
                }
                catch(WebDriverTimeoutException)
                {
                    throw new WebUiAutomationException($"Timeout locating SelectorData item with the name : {itemName}");
                }
            }

            return result;
        }

        /// <summary>
        /// Finds and returns the Nth IWebElement from a set of identical elements using the parameters provided, if none is found an exception is thrown.
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <param name="nthElement">The Zero based position of the element to be returned</param>
        /// <param name="wait"></param>
        /// <returns></returns>
        internal IWebElement Check_Nth_ElementExistsReturnIWebElement(SelectorData selectorData, int nthElement, IWebDriver driver, WebDriverWait wait)
        {
            IWebElement webElement = null;
            try
            {
                if (selectorData.AttributeType == HtmlAttributeType.InnerText_Contains || selectorData.AttributeType == HtmlAttributeType.InnerText_ExactMatch)
                {
                    webElement = CheckElementExistsByTagAndInnerText(selectorData, driver);
                }
                else if (selectorData.AttributeType == HtmlAttributeType.AttributeText_Contains || selectorData.AttributeType == HtmlAttributeType.AttributeText_ExactMatch)
                {
                    webElement = CheckElementExistsByAttributevalue(selectorData, driver);
                }
                else
                {
                    var elements = driver.FindElements(BuildCssSelectorBy(selectorData));

                    if (nthElement < 0)
                        throw new WebUiAutomationException($"{nthElement} is less than zero, this must be the zero based position of the expected value.");

                    if (elements.Count < nthElement)
                        throw new WebUiAutomationException($"{elements.Count} elements were found with a class attribute of '{selectorData.AttributeValue}'. " +
                            $"An element was not found in the array at the {nthElement} position.");


                    webElement = elements[nthElement];
                }

                return webElement;
            }
            catch (WebUiAutomationException wae)
            {
                throw wae;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                return webElement;
            }
        }

        /// <summary>
        /// Finds and returns the IWebElement using the parameters provided, if none is found null is returned
        /// </summary>
        /// <param name="selectorData">Data to build the CssSelector with</param>
        /// <param name="driver">This must be an initialized IWebDriver object navigated to the page being tested</param>
        /// <param name="wait"></param>
        /// <returns>IWebElement</returns>
        internal IWebElement CheckElementExistsReturnIWebElement(SelectorData selectorData, IWebDriver driver, WebDriverWait wait)
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
                    var elements = driver.FindElements(BuildCssSelectorBy(selectorData));
                    
                    if (elements.Count > 1)
                        throw new WebUiAutomationException($"{elements.Count} elements were found with a class attribute of '{selectorData.AttributeValue}'. " +
                            $"Consider using the method GetAllBysUsingMatchingSelectorData and then filtering it to find the desired element.");

                    if(elements.Count == 1)
                        webElement = driver.WaitForElementExists(BuildCssSelectorBy(selectorData), wait);
                }

                return webElement;
            }
            catch(WebUiAutomationException wae)
            {
                throw wae;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
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
        /// <param name="wait"></param>
        /// <returns></returns>
        internal By CheckElementExistsReturnCssSelector(SelectorData selectorData, IWebDriver driver, WebDriverWait wait)
        {
            try
            {
                IWebElement webElement = null;

                var cssBy = BuildCssSelectorBy(selectorData);
                webElement = driver.WaitForElementExists(cssBy, wait);
                               
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
                throw new WebUiAutomationException($"Could not create the needed IJavaScriptExecutor object using the {Helper.GetDriverBrowserName(driver)}");

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
            var tag = selectorData.TagType.ToLower();
            
            switch (selectorData.AttributeType)
            {
                case HtmlAttributeType.Id:
                    CssSelector = By.CssSelector($"#{selectorData.AttributeValue}");
                    break;
                case HtmlAttributeType.Class:
                case HtmlAttributeType.Name:
                case HtmlAttributeType.Type:
                case HtmlAttributeType.Href:
                case HtmlAttributeType.Src:
                case HtmlAttributeType.Title:
                case HtmlAttributeType.FormControlName:
                case HtmlAttributeType.PlaceHolder:
                    CssSelector = By.CssSelector($"{tag}[{selectorData.AttributeType.ToString().ToLower()}='{selectorData.AttributeValue}']");
                    break;
                case HtmlAttributeType.None:
                    CssSelector = By.CssSelector($"{tag}");
                    break;
                case HtmlAttributeType.AttributeText_ExactMatch:
                case HtmlAttributeType.AttributeText_Contains:
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
