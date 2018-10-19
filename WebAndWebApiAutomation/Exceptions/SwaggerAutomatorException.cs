using System;

namespace WebAndWebApiAutomation.Exceptions
{
    internal sealed class SwaggerAutomatorException : Exception
    {
        internal SwaggerAutomatorException() { }
        internal SwaggerAutomatorException(string message) : base(message) { }
    }
}
