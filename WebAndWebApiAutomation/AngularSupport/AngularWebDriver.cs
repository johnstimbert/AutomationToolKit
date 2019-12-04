using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAndWebApiAutomation.AngularSupport.Bys.Base;
using WebAndWebApiAutomation.AngularSupport.Modules;
using WebAndWebApiAutomation.AngularSupport.Scripts;

namespace WebAndWebApiAutomation.AngularSupport
{
    /// <summary>
    /// Wraps IWebDriver in order to auther tests targeting Angular apps
    /// </summary>
    public class AngularWebDriver : IWebDriver, IWrapsDriver, IJavaScriptExecutor
    {
        private const string AngularDeferBootstrap = "NG_DEFER_BOOTSTRAP!";

        private IWebDriver _dwebDiver;
        private IJavaScriptExecutor jsExecutor;
        private string rootElement;
        private IList<Module> mockModules;

        /// <summary>
        /// Creates a new instance of <see cref="NgWebDriver"/> by wrapping a <see cref="IWebDriver"/> instance.
        /// </summary>
        /// <param name="driver">The configured webdriver instance.</param>
        /// <param name="mockModules">
        /// The modules to load before Angular whenever Url setter or Navigate().GoToUrl() is called.
        /// </param>
        public AngularWebDriver(IWebDriver driver, params Module[] mockModules)
            : this(driver, "", mockModules)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="NgWebDriver"/> by wrapping a <see cref="IWebDriver"/> instance.
        /// </summary>
        /// <param name="driver">The configured webdriver instance.</param>
        /// <param name="rootElement">
        /// The CSS selector for an element on which to find Angular. 
        /// <para/>
        /// This is usually 'body' but if your ng-app is on a subsection of the page it may be a subelement.
        /// </param>
        /// <param name="mockModules">
        /// The modules to load before Angular whenever Url setter or Navigate().GoToUrl() is called.
        /// </param>
        public AngularWebDriver(IWebDriver driver, string rootElement, params Module[] mockModules)
        {
            if (!(driver is IJavaScriptExecutor))
            {
                throw new NotSupportedException("The WebDriver instance must implement the IJavaScriptExecutor interface.");
            }
            this._dwebDiver = driver;
            this.jsExecutor = (IJavaScriptExecutor)driver;
            this.rootElement = rootElement;
            this.mockModules = new List<Module>(mockModules);
        }

        #region IWrapsDriver Members

        /// <summary>
        /// Gets the wrapped <see cref="IWebDriver"/> instance.
        /// <para/>
        /// Use this to interact with pages that do not contain Angular (such as a log-in screen).
        /// </summary>
        public IWebDriver WrappedDriver
        {
            get { return this._dwebDiver; }
        }

        #endregion

        /// <summary>
        /// Gets the CSS selector for an element on which to find Angular. 
        /// <para/>
        /// This is usually 'body' but if your ng-app is on a subsection of the page it may be a subelement.
        /// </summary>
        public string RootElement
        {
            get { return this.rootElement; }
        }

        /// <summary>
        /// If true, Protractor will not attempt to synchronize with the page before performing actions. 
        /// This can be harmful because Protractor will not wait until $timeouts and $http calls have been processed, 
        /// which can cause tests to become flaky. 
        /// This should be used only when necessary, such as when a page continuously polls an API using $timeout.
        /// </summary>
        public bool IgnoreSynchronization { get; set; }

        #region IWebDriver Members

        /// <summary>
        /// Gets the current window handle, which is an opaque handle to this 
        /// window that uniquely identifies it within this driver instance.
        /// </summary>
        public string CurrentWindowHandle
        {
            get { return this._dwebDiver.CurrentWindowHandle; }
        }

        /// <summary>
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        public string PageSource
        {
            get
            {
                this.WaitForAngular();
                return this._dwebDiver.PageSource;
            }
        }

        /// <summary>
        /// Gets the title of the current browser window.
        /// </summary>
        public string Title
        {
            get
            {
                this.WaitForAngular();
                return this._dwebDiver.Title;
            }
        }

        /// <summary>
        /// Gets or sets the URL the browser is currently displaying.
        /// </summary>
        public string Url
        {
            get
            {
                this.WaitForAngular();
                return this._dwebDiver.Url;
            }
            set
            {
                // Reset URL
                this._dwebDiver.Url = "about:blank";

                // TODO: test Android
                IHasCapabilities hcDriver = this._dwebDiver as IHasCapabilities;
                string browserName = null;
                if (hcDriver != null && hcDriver.Capabilities.HasCapability("browserName"))
                {
                    browserName = hcDriver.Capabilities.GetCapability("browserName").ToString();
                }
                if (browserName != null &&
                    (browserName == "internet explorer" ||
                     browserName == "MicrosoftEdge" ||
                     browserName == "phantomjs" ||
                     browserName == "firefox" ||
                     browserName.ToLower() == "safari"))
                {
                    this.ExecuteScript("window.name += '" + AngularDeferBootstrap + "';");
                    this._dwebDiver.Url = value;
                }
                else
                {
                    this.ExecuteScript("window.name += '" + AngularDeferBootstrap + "'; window.location.href = '" + value + "';");
                }

                if (!this.IgnoreSynchronization)
                {
                    try
                    {
                        // Make sure the page is an Angular page.
                        long? angularVersion = this.ExecuteAsyncScript(BackingScripts.TestForAngular) as long?;
                        if (angularVersion.HasValue)
                        {
                            if (angularVersion.Value == 1)
                            {
                                // At this point, Angular will pause for us, until angular.resumeBootstrap is called.

                                // Add default module for Angular v1
                                this.mockModules.Add(new Angular1Module());

                                // Register extra modules
                                foreach (Module ngModule in this.mockModules)
                                {
                                    this.ExecuteScript(ngModule.Script);
                                }
                                // Resume Angular bootstrap
                                this.ExecuteScript(BackingScripts.ResumeAngularBootstrap,
                                    String.Join(",", this.mockModules.Select(m => m.Name).ToArray()));
                            }
                            else if (angularVersion.Value == 2)
                            {
                                if (this.mockModules.Count > 0)
                                {
                                    throw new NotSupportedException("Mock modules are not supported in Angular 2");
                                }
                            }
                        }
                    }
                    catch (WebDriverTimeoutException wdte)
                    {
                        throw new InvalidOperationException(
                            String.Format("Angular could not be found on the page '{0}'", value), wdte);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the window handles of open browser windows.
        /// </summary>
        public ReadOnlyCollection<string> WindowHandles
        {
            get { return this._dwebDiver.WindowHandles; }
        }

        /// <summary>
        /// Close the current window, quitting the browser if it is the last window currently open.
        /// </summary>
        public void Close()
        {
            this._dwebDiver.Close();
        }

        /// <summary>
        /// Instructs the driver to change its settings.
        /// </summary>
        /// <returns>
        /// An <see cref="IOptions"/> object allowing the user to change the settings of the driver.
        /// </returns>
        public IOptions Manage()
        {
            return this._dwebDiver.Manage();
        }

        INavigation OpenQA.Selenium.IWebDriver.Navigate()
        {
            return this.Navigate();
        }

        /// <summary>
        /// Instructs the driver to navigate the browser to another location.
        /// </summary>
        /// <returns>
        /// An <see cref="NgNavigation"/> object allowing the user to access 
        /// the browser's history and to navigate to a given URL.
        /// </returns>
        public AngularNavigation Navigate()
        {
            return new AngularNavigation(this, _dwebDiver.Navigate());
        }

        /// <summary>
        /// Quits this driver, closing every associated window.
        /// </summary>
        public void Quit()
        {
            this._dwebDiver.Quit();
        }

        /// <summary>
        /// Instructs the driver to send future commands to a different frame or window.
        /// </summary>
        /// <returns>
        /// An <see cref="ITargetLocator"/> object which can be used to select a frame or window.
        /// </returns>
        public ITargetLocator SwitchTo()
        {
            return this._dwebDiver.SwitchTo();
        }

        /// <summary>
        /// Finds the first <see cref="NgWebElement"/> using the given mechanism. 
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="NgWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public AngularElement FindElement(By by)
        {
            if (by is AngularBaseBy)
            {
                ((AngularBaseBy)by).AdditionalScriptArguments = new object[] { this.RootElement };
            }
            this.WaitForAngular();
            return new AngularElement(this, this._dwebDiver.FindElement(by));
        }

        /// <summary>
        /// Finds all <see cref="NgWebElement"/>s within the current context 
        /// using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>
        /// A <see cref="ReadOnlyCollection{T}"/> of all <see cref="NgWebElement"/>s 
        /// matching the current criteria, or an empty list if nothing matches.
        /// </returns>
        public ReadOnlyCollection<AngularElement> FindElements(By by)
        {
            if (by is AngularBaseBy)
            {
                ((AngularBaseBy)by).AdditionalScriptArguments = new object[] { this.RootElement };
            }
            this.WaitForAngular();
            return new ReadOnlyCollection<AngularElement>(this._dwebDiver.FindElements(by).Select(e => new AngularElement(this, e)).ToList());
        }

        IWebElement ISearchContext.FindElement(By by)
        {
            return this.FindElement(by);
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements(By by)
        {
            if (by is AngularBaseBy)
            {
                ((AngularBaseBy)by).AdditionalScriptArguments = new object[] { this.RootElement };
            }
            this.WaitForAngular();
            return new ReadOnlyCollection<IWebElement>(this._dwebDiver.FindElements(by).Select(e => (IWebElement)new AngularElement(this, e)).ToList());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this._dwebDiver.Dispose();
        }

        #endregion

        /// <summary>
        /// Gets or sets the location for in-page navigation using the same syntax as '$location.url()'.
        /// </summary>
        public string Location
        {
            get
            {
                this.WaitForAngular();
                return this.ExecuteScript(BackingScripts.GetLocation, this.rootElement) as string;
            }
            set
            {
                this.WaitForAngular();
                this.ExecuteScript(BackingScripts.SetLocation, this.rootElement, value);
            }
        }

        /// <summary>
        /// Waits for Angular to finish any ongoing $http, $timeouts, digest cycles etc.
        /// This is used before any action on this driver, except if IgnoreSynchonization flag is set to true.
        /// </summary>
        /// <remarks>
        /// Use NgWebDriver.Manage().Timeouts().AsynchronousJavaScript to specify the amount of time the driver should wait for Angular.
        /// </remarks>
        /// <exception cref="InvalidOperationException">If Angular could not be found.</exception>
        /// <exception cref="WebDriverTimeoutException">If the driver times out while waiting for Angular.</exception>
        public void WaitForAngular()
        {
            if (!this.IgnoreSynchronization)
            {
                this.ExecuteAsyncScript(BackingScripts.WaitForAngular, this.rootElement);
            }
        }

        #region IJavaScriptExecutor Members

        /// <summary>
        /// Executes JavaScript in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        /// <remarks>
        ///   <para>
        ///     The <see cref="M:OpenQA.Selenium.IJavaScriptExecutor.ExecuteScript(System.String,System.Object[])" /> method executes JavaScript in the context of
        ///     the currently selected frame or window. This means that "document" will refer
        ///     to the current document. If the script has a return value, then the following
        ///     steps will be taken:
        ///   </para>
        ///   <para>
        ///     <list type="bullet">
        ///       <item>
        ///         <description>For an HTML element, this method returns a <see cref="T:OpenQA.Selenium.IWebElement" /></description>
        ///       </item>
        ///       <item>
        ///         <description>For a number, a <see cref="T:System.Int64" /> is returned</description>
        ///       </item>
        ///       <item>
        ///         <description>For a boolean, a <see cref="T:System.Boolean" /> is returned</description>
        ///       </item>
        ///       <item>
        ///         <description>For all other cases a <see cref="T:System.String" /> is returned.</description>
        ///       </item>
        ///       <item>
        ///         <description>
        ///           For an array, we check the first element, and attempt to return a
        ///           <see cref="T:System.Collections.Generic.List`1" /> of that type, following the rules above. 
        ///           Nested lists are not supported.
        ///         </description>
        ///       </item>
        ///       <item>
        ///         <description>If the value is null or there is no return value, <see langword="null" /> is returned.</description>
        ///       </item>
        ///     </list>
        ///   </para>
        ///   <para>
        ///     Arguments must be a number (which will be converted to a <see cref="T:System.Int64" />),
        ///     a <see cref="T:System.Boolean" />, a <see cref="T:System.String" /> or a <see cref="T:OpenQA.Selenium.IWebElement" />.
        ///     An exception will be thrown if the arguments do not meet these criteria.
        ///     The arguments will be made available to the JavaScript via the "arguments" magic
        ///     variable, as if the function were called via "Function.apply"
        ///   </para>
        /// </remarks>
        public object ExecuteScript(string script, params object[] args)
        {
            return jsExecutor.ExecuteScript(script, args);
        }

        /// <summary>
        /// Executes JavaScript asynchronously in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        public object ExecuteAsyncScript(string script, params object[] args)
        {
            return jsExecutor.ExecuteAsyncScript(script, args);
        }

        #endregion
    }
}
