using System;

namespace WebUiAutomationToolKit.Exceptions
{
    /// <summary>
    /// Wrapping exception used to clarify exceptions thrown inside the Logger utility.
    /// </summary>
    public sealed class LoggerException : Exception
    {
        internal LoggerException() { }

        internal LoggerException(string message) : base(message) { }

        internal LoggerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
