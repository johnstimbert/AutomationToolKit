using System;

namespace WebUiAutomationToolKit.Exceptions
{
    /// <summary>
    /// Wrapping exception used to clarify exception thrown by IWebDriver.
    /// If an exception was thrown by IWebDriver it will be the inner exception
    /// </summary>
    public sealed class WebUiAutomationException : Exception
    {
        internal WebUiAutomationException() { }

        internal WebUiAutomationException(string message) : base(message) { }
    }
}
