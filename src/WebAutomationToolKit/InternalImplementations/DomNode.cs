using OpenQA.Selenium;
using WebAutomationToolKit.Extensions;
using System;
using System.Collections.Generic;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.Helpers;
using WebAutomationToolKit.Contracts;

namespace WebAutomationToolKit.InternalImplementations
{
    internal sealed class DomNode : IDomNode
    {
        private const string _xpathForRetrievingChildren = @".//*";
        private By[] _children;


        private int _lastChildPosition = -1;

        private bool _hasAttributes = false;


        private DomNode() {}

        internal DomNode(By parentNodeSelector, By nodeSelector, WebDriverManager webDriverManager)
        {
            ParentNodeSelector = parentNodeSelector;
            NodeSelector = nodeSelector;

            FindChildren(webDriverManager);
            FindAttributes(webDriverManager);
        }

        internal DomNode(By nodeSelector, WebDriverManager webDriverManager)
        {    
            NodeSelector = nodeSelector;

            FindChildren(webDriverManager);
            FindAttributes(webDriverManager);
        }

        internal By ParentNodeSelector { get; }

        internal By NodeSelector { get; }

        internal By[] GetChildren(IWebDriverManager webDriverManager)
        {
            FindChildren(webDriverManager);

            return _children;
        }

        /// <summary>
        /// Indicates if this node has child nodes or not
        /// </summary>
        public bool HasChildren(IWebDriverManager webDriverManager)
        {
            FindChildren(webDriverManager);

            return _lastChildPosition > -1;
        }

        /// <summary>
        /// Indicates if this node has attribnutes or not
        /// </summary>
        public bool HasAttributes(IWebDriverManager webDriverManager)
        {
            FindAttributes(webDriverManager);

            return _hasAttributes;
        }

        /// <summary>
        /// Returns any attributes found;
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetAttributes(IWebDriverManager webDriverManager)
        {
            return FindAttributes(webDriverManager);
        }

        /// <summary>
        /// Gets the innerText of this node, without any leading or trailing whitespace, and with other whitespace collapsed.
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns></returns>
        public string GetText(IWebDriverManager webDriverManager)
        {
            return GetElementReference(webDriverManager).Text;
        }

        /// <summary>
        /// Gets the tag name of this element.
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns></returns>
        public string GetTagName(IWebDriverManager webDriverManager)
        {
            return GetElementReference(webDriverManager).TagName;
        }

        private Dictionary<string, object> FindAttributes(IWebDriverManager webDriverManager)
        {
            try
            {
                WebDriverManager manager = (WebDriverManager)webDriverManager;
                //Get fresh refrence to the element this node 
                var thisNode = GetElementReference(webDriverManager);

                var executor = Helper.JavaScriptExecutor(manager.GetActiveDriver());
                var atts = executor.ExecuteScript("var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) " +
                    "{ items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;", thisNode) as Dictionary<string, object>;

                _hasAttributes = atts.Count > 0;

                return atts;
            }
            catch (NoSuchElementException)
            {
                throw new WebAutomationException($"Could not locate any element using the XPath {NodeSelector}");
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.Message);
            }
        }

        private void FindChildren(IWebDriverManager webDriverManager)
        {
            try
            {
                WebDriverManager manager = (WebDriverManager)webDriverManager;
                //Get fresh refrence to the element this node 
                var thisNode = GetElementReference(webDriverManager);
                //Find anything that could be child with the Xpath locator ".//*"
                var children = thisNode.FindElements(By.XPath(_xpathForRetrievingChildren));
                _children = children.ConvertToXPathBy(manager.GetActiveDriver()).ToArray();

                _lastChildPosition = children.Count -1;

            }
            catch (NoSuchElementException)
            {
                throw new WebAutomationException($"Could not locate any element using the XPath {NodeSelector}");
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.Message);
            }
        }

        private IWebElement GetElementReference(IWebDriverManager webDriverManager)
        {
            try
            {                
                WebDriverManager manager = (WebDriverManager)webDriverManager;
                return manager.GetActiveDriver().FindElement(NodeSelector);
            }
            catch (NoSuchElementException)
            {
                throw new WebAutomationException($"Could not locate any element using the locator {NodeSelector}");
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.Message);
            }
        }
    }
}
