using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using WebAndWebApiAutomation.AngularSupport.Scripts;
using WebAndWebApiAutomation.AngularSupport.Bys.Base;

namespace WebAndWebApiAutomation.AngularSupport
{
    /// <summary>
    /// Wraps IWebElement to allow it to be used with Angular Apps.
    /// </summary>
    public class AngularElement : IWebElement, IWrapsElement
    {
        private AngularWebDriver _angularWebDriver;
        private IWebElement _element;

        /// <summary>
        /// Creates a new instance of <see cref="AngularElement"/> by wrapping a <see cref="IWebElement"/> instance.
        /// </summary>
        /// <param name="angularDriver">The <see cref="AngularWebDriver"/> in use.</param>
        /// <param name="element">The existing <see cref="IWebElement"/> instance.</param>
        public AngularElement(AngularWebDriver angularDriver, IWebElement element)
        {
            _angularWebDriver = angularDriver;
            _element = element;
        }

        /// <summary>
        /// Gets the <see cref="AngularWebDriver"/> instance used to initialize the element.
        /// </summary>
        public AngularWebDriver AngularDriver
        {
            get { return _angularWebDriver; }
        }

        #region IWrapsElement Members

        /// <summary>
        /// Gets the wrapped <see cref="IWebElement"/> instance.
        /// </summary>
        public IWebElement WrappedElement
        {
            get { return _element; }
        }

        #endregion

        #region IWebElement Members

        /// <summary>
        /// Gets a value indicating whether or not this element is displayed.
        /// </summary>
        public bool Displayed
        {
            get
            {
                _angularWebDriver.WaitForAngular();
                return _element.Displayed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                _angularWebDriver.WaitForAngular();
                return _element.Enabled;
            }
        }

        /// <summary>
        /// Gets a <see cref="Point"/> object containing the coordinates of the upper-left corner
        /// of this element relative to the upper-left corner of the page.
        /// </summary>
        public Point Location
        {
            get
            {
                _angularWebDriver.WaitForAngular();
                return _element.Location;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is selected.
        /// </summary>
        public bool Selected
        {
            get
            {
                _angularWebDriver.WaitForAngular();
                return _element.Selected;
            }
        }

        /// <summary>
        /// Gets a <see cref="Size"/> object containing the height and width of this element.
        /// </summary>
        public Size Size
        {
            get
            {
                _angularWebDriver.WaitForAngular();
                return _element.Size;
            }
        }

        /// <summary>
        /// Gets the tag name of this element.
        /// </summary>
        public string TagName
        {
            get
            {
                _angularWebDriver.WaitForAngular();
                return _element.TagName;
            }
        }

        /// <summary>
        /// Gets the innerText of this element, without any leading or trailing whitespace,
        /// and with other whitespace collapsed.
        /// </summary>
        public string Text
        {
            get
            {
                _angularWebDriver.WaitForAngular();
                return _element.Text;
            }
        }

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        public void Clear()
        {
            _angularWebDriver.WaitForAngular();
            _element.Clear();
        }

        /// <summary>
        /// Clicks this element. 
        /// </summary>
        public void Click()
        {
            _angularWebDriver.WaitForAngular();
            _element.Click();
        }

        /// <summary>
        /// Gets the value of the specified attribute for this element.
        /// </summary>
        public string GetAttribute(string attributeName)
        {
            _angularWebDriver.WaitForAngular();
            return _element.GetAttribute(attributeName);
        }

        /// <summary>
        /// Gets the value of the specified property for this element.
        /// </summary>
        public string GetProperty(string propertyName)
        {
            _angularWebDriver.WaitForAngular();
            return _element.GetProperty(propertyName);
        }

        /// <summary>
        /// Gets the value of a CSS property of this element.
        /// </summary>
        public string GetCssValue(string propertyName)
        {
            _angularWebDriver.WaitForAngular();
            return _element.GetCssValue(propertyName);
        }

        /// <summary>
        /// Simulates typing text into the element.
        /// </summary>
        public void SendKeys(string text)
        {
            _angularWebDriver.WaitForAngular();
            _element.SendKeys(text);
        }

        /// <summary>
        /// Submits this element to the web server.
        /// </summary>
        public void Submit()
        {
            _angularWebDriver.WaitForAngular();
            _element.Submit();
        }

        /// <summary>
        /// Finds the first <see cref="AngularElement"/> using the given mechanism. 
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="AngularElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public AngularElement FindElement(By by)
        {
            if (by is AngularBaseBy)
            {
                ((AngularBaseBy)by).AdditionalScriptArguments = new object[] { _angularWebDriver.RootElement, _element };
            }
            _angularWebDriver.WaitForAngular();
            return new AngularElement(_angularWebDriver, _element.FindElement(by));
        }

        /// <summary>
        /// Finds all <see cref="AngularElement"/>s within the current context 
        /// using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>
        /// A <see cref="ReadOnlyCollection{T}"/> of all <see cref="AngularElement"/>s 
        /// matching the current criteria, or an empty list if nothing matches.
        /// </returns>
        public ReadOnlyCollection<AngularElement> FindElements(By by)
        {
            if (by is AngularBaseBy)
            {
                ((AngularBaseBy)by).AdditionalScriptArguments = new object[] { _angularWebDriver.RootElement, _element };
            }
            _angularWebDriver.WaitForAngular();
            return new ReadOnlyCollection<AngularElement>(_element.FindElements(by).Select(e => new AngularElement(_angularWebDriver, e)).ToList());
        }

        IWebElement ISearchContext.FindElement(By by)
        {
            return FindElement(by);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements(By by)
        {
            if (by is AngularBaseBy)
            {
                ((AngularBaseBy)by).AdditionalScriptArguments = new object[] { _angularWebDriver.RootElement, _element };
            }
            _angularWebDriver.WaitForAngular();
            return new ReadOnlyCollection<IWebElement>(_element.FindElements(by).Select(e => (IWebElement)new AngularElement(_angularWebDriver, e)).ToList());
        }

        #endregion

        /// <summary>
        /// Evaluates the expression as if it were on the scope of the current element.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The expression evaluated by Angular.</returns>
        public object Evaluate(string expression)
        {
            _angularWebDriver.WaitForAngular();
            return _angularWebDriver.ExecuteScript(BackingScripts.Evaluate, _element, expression);
        }
    }
}
