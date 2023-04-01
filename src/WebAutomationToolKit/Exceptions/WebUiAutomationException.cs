namespace WebAutomationToolKit.Exceptions
{
    /// <summary>
    /// Wrapping exception used to clarify exception thrown by IWebDriver.
    /// If an exception was thrown by IWebDriver it will be the inner exception
    /// </summary>
    public sealed class WebAutomationException : Exception
    {
        internal WebAutomationException() { }

        internal WebAutomationException(string message) : base(message) { }
    }
}
