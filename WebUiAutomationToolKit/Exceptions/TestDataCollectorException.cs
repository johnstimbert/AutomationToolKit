using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebUiAutomationToolKit.Exceptions
{
    /// <summary>
    /// Wrapping exception used to clarify exceptions thrown inside the TestDataCollector utility.
    /// </summary>
    public sealed class TestDataCollectorException : Exception
    {
        internal TestDataCollectorException() { }

        internal TestDataCollectorException(string message) : base(message) { }

        internal TestDataCollectorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
