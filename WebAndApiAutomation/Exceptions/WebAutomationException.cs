using System;

namespace WebAndApiAutomation.Exceptions
{
    public sealed class WebAutomationException : Exception
    {
        internal WebAutomationException() { }

        internal WebAutomationException(string message) : base(message) { }
    }
}
