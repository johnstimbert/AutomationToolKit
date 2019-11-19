using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WebAndWebApiAutomation.AngularSupport.Bys.Base
{
    internal class AngularBaseBy : By
    {
        private string _script;
        private object[] _args;
        internal string _description;

        /// <summary>
        /// Creates a new instance of <see cref="AngularBaseBy"/>.
        /// </summary>
        /// <param name="script">
        /// The JavaScript code to execute.
        /// </param>
        /// <param name="args">
        /// The arguments to the script.
        /// </param>
        internal AngularBaseBy(string script, params object[] args)
        {
            _script = script;
            _args = args;
            _description = "AngularBaseBy";
        }

        /// <summary>
        /// Gets or sets any additional arguments to the script.
        /// </summary>
        internal object[] AdditionalScriptArguments { get; set; }

        /// <summary>
        /// Finds the first element matching the criteria.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ISearchContext"/> object to use to search for the elements.
        /// </param>
        /// <returns>
        /// The first matching <see cref="IWebElement"/> on the current context.
        /// </returns>
        internal IWebElement FindElement(ISearchContext context)
        {
            ReadOnlyCollection<IWebElement> elements = this.FindElements(context);
            if (elements.Count == 0)
            {
                throw new NoSuchElementException(String.Format("Unable to locate element: {{ {0} }}.", _description));
            }
            return elements[0];
        }

        /// <summary>
        /// Finds all elements matching the criteria.
        /// </summary>
        /// <param name="context">
        /// An <see cref="ISearchContext"/> object to use to search for the elements.
        /// </param>
        /// <returns>
        /// A collection of all <see cref="OpenQA.Selenium.IWebElement"/> matching the current criteria, 
        /// or an empty list if nothing matches.
        /// </returns>
        internal ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            // Create script arguments
            object[] scriptArgs = _args;
            if (this.AdditionalScriptArguments != null && this.AdditionalScriptArguments.Length > 0)
            {
                // Add additionnal script arguments
                scriptArgs = new object[_args.Length + this.AdditionalScriptArguments.Length];
                _args.CopyTo(scriptArgs, 0);
                AdditionalScriptArguments.CopyTo(scriptArgs, _args.Length);
            }

            // Get JS executor
            IJavaScriptExecutor jsExecutor = context as IJavaScriptExecutor;
            if (jsExecutor == null)
            {
                IWrapsDriver wrapsDriver = context as IWrapsDriver;
                if (wrapsDriver != null)
                {
                    jsExecutor = wrapsDriver.WrappedDriver as IJavaScriptExecutor;
                }
            }
            if (jsExecutor == null)
            {
                throw new NotSupportedException("Could not get an IJavaScriptExecutor instance from the context.");
            }

            ReadOnlyCollection<IWebElement> elements = jsExecutor.ExecuteScript(_script, scriptArgs) as ReadOnlyCollection<IWebElement>;
            if (elements == null)
            {
                elements = new ReadOnlyCollection<IWebElement>(new List<IWebElement>(0));
            }
            return elements;
        }
    }
}
