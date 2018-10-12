using System;

namespace WebAndApiAutomation.Exceptions
{
    public sealed class SwaggerAutomatorException : Exception
    {
        internal SwaggerAutomatorException() { }
        internal SwaggerAutomatorException(string message) : base(message) { }
    }
}
