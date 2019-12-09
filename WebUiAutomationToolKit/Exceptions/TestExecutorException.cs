using System;

namespace WebUiAutomationToolKit.Exceptions
{
    /// <summary>
    /// Wrapping exception used to clarify exceptions thrown inside the TestExecutor utility.
    /// </summary>
    public sealed class TestExecutorException : Exception
    {
        internal TestExecutorException() { }

        internal TestExecutorException(string message) : base(message) { }

        internal TestExecutorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
