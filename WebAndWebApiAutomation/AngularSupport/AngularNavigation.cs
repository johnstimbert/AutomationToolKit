using OpenQA.Selenium;
using System;

namespace WebAndWebApiAutomation.AngularSupport
{
    /// <summary>
    /// Wraps navigation in an Angular app
    /// </summary>
    public class AngularNavigation : INavigation
    {
        private readonly AngularWebDriver _angularWebDriver;
        private readonly INavigation _navigation;

        /// <summary>
        /// Creates a new instance of <see cref="AngularNavigation"/> by wrapping a <see cref="INavigation"/> instance.
        /// </summary>
        /// <param name="angularWebDriver">The <see cref="AngularWebDriver"/> in use.</param>
        /// <param name="navigation">The existing <see cref="INavigation"/> instance.</param>
        public AngularNavigation(AngularWebDriver angularWebDriver, INavigation navigation)
        {
            _angularWebDriver = angularWebDriver;
            _navigation = navigation;
        }

        /// <summary>
        /// Gets the <see cref="AngularWebDriver"/> instance used to initialize the instance.
        /// </summary>
        public AngularWebDriver AngularWebDriver
        {
            get { return _angularWebDriver; }
        }

        /// <summary>
        /// Gets the wrapped <see cref="INavigation"/> instance.
        /// </summary>
        public INavigation WrappedNavigation
        {
            get { return _navigation; }
        }

        #region INavigation Members

        /// <summary>
        /// Move back a single entry in the browser's history.
        /// </summary>
        public void Back()
        {
            _angularWebDriver.WaitForAngular();
            _navigation.Back();
        }

        /// <summary>
        /// Move a single "item" forward in the browser's history.
        /// </summary>
        public void Forward()
        {
            _angularWebDriver.WaitForAngular();
            _navigation.Forward();
        }

        void INavigation.GoToUrl(Uri url)
        {
            GoToUrl(url, true);
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        public void GoToUrl(Uri url)
        {
            GoToUrl(url, true);
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        /// <param name="ensureAngularApp">Ensure the page is an Angular page by throwing an exception.</param>
        public void GoToUrl(Uri url, bool ensureAngularApp)
        {
            if (ensureAngularApp)
            {
                if (url == null)
                {
                    throw new ArgumentNullException("url", "URL cannot be null.");
                }
                _angularWebDriver.Url = url.ToString();
            }
            else
            {
                _navigation.GoToUrl(url);
            }
        }

        void INavigation.GoToUrl(string url)
        {
            GoToUrl(url, true);
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// </summary>
        /// <param name="url">The URL to load. It is best to use a fully qualified URL</param>
        public void GoToUrl(string url)
        {
            GoToUrl(url, true);
        }

        /// <summary>
        /// Load a new web page in the current browser window.
        /// </summary>
        /// <param name="url">The URL to load. It is best to use a fully qualified URL</param>
        /// <param name="ensureAngularApp">Ensure the page is an Angular page by throwing an exception.</param>
        public void GoToUrl(string url, bool ensureAngularApp)
        {
            if (ensureAngularApp)
            {
                _angularWebDriver.Url = url;
            }
            else
            {
                _navigation.GoToUrl(url);
            }
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        public void Refresh()
        {
            if (_angularWebDriver.IgnoreSynchronization)
            {
                this._navigation.Refresh();
            }
            else
            {
                string url = _angularWebDriver.ExecuteScript("return window.location.href;") as string;
                _angularWebDriver.Url = url;
            }
        }

        #endregion

        /// <summary>
        /// Browse to another page using in-page navigation.
        /// </summary>
        /// <param name="path">The path to load using the same syntax as '$location.url()'.</param>
        public void GoToLocation(string path)
        {
            _angularWebDriver.Location = path;
        }
    }
}
