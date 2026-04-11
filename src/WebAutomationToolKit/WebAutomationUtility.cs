using OpenQA.Selenium;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.Helpers;
using WebAutomationToolKit.Validators;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit
{
    /// <summary>
    /// Provides static utility methods for web page analysis that do not require a <see cref="InternalImplementations.WebAutomation"/> instance.
    /// </summary>
    public static class WebAutomationUtility
    {
        private static readonly StructureValidator _structureValidator = new();

        /// <summary>
        /// Parses and returns the inner text of the supplied HTML fragment, stripping all HTML tags.
        /// </summary>
        /// <param name="htmlText">Raw HTML text to parse.</param>
        /// <returns>The inner text with all HTML tags removed.</returns>
        /// <exception cref="WebAutomationException">Thrown when parsing fails.</exception>
        public static string? ParseInnerText(string htmlText)
        {
            try
            {
                return _structureValidator.ParseInnerText(htmlText);
            }
            catch (WebAutomationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Tests navigation for every anchor element found on the current page and returns the results.
        /// </summary>
        /// <param name="webDriverManager">An active <see cref="IWebDriverManager"/> navigated to the page to test.</param>
        /// <returns>A list of navigation results keyed by the anchor's <see cref="By"/> locator.</returns>
        /// <exception cref="WebAutomationException">Thrown when navigation testing fails.</exception>
        public static List<KeyValuePair<By, NavigationResult>> TestLinkNavigationForAllAnchorsFoundInPage(
            IWebDriverManager webDriverManager)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var navValidator = new NavigationValidator(manager.GetWait());
                return navValidator.TestLinkNavigationForAllAnchorsFoundInPage(manager.GetActiveDriver());
            }
            catch (WebAutomationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.Message, ex);
            }
        }
    }
}
