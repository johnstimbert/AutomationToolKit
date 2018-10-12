using System.Collections.Generic;
using static WebAndWebApiAutomation.Api.SwaggerUtilities.Enums;

namespace WebAndWebApiAutomation.Api.SwaggerUtilities.Models
{
    internal class SwaggerPath
    {
        internal SwaggerPath()
        {
            Responses = new List<SwaggerPathResponse>();
            Consumes = new List<string>();
            Produces = new List<string>();
        }

        internal string Url { get; set; }
        internal Operations Operation { get; set; }
        internal string Summary { get; set; }
        internal string OperationId { get; set; }
        internal List<string> Consumes { get; set; }
        internal List<string> Produces { get; set; }
        internal SwaggerPathParameters Parameters { get; set; }
        internal List<SwaggerPathResponse> Responses { get; set; }
    }
}
