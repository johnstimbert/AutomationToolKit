using System;

namespace WebAndWebApiAutomation.Exceptions
{
    public sealed class SwaggerAutomatorException : Exception
    {
        internal SwaggerAutomatorException() { }
        internal SwaggerAutomatorException(string message) : base(message) { }
    }
}
