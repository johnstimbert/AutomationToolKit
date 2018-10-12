using System;

namespace WebAndApiAutomation.Api.SwaggerUtilities.Exceptions
{
    internal class SwaggerAutomatorException : Exception
    {
        internal SwaggerAutomatorException(string message) : base(message) { }
    }
}
