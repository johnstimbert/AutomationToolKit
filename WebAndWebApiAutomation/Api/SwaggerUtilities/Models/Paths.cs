using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static WebAndWebApiAutomation.Api.SwaggerUtilities.Enums;

namespace WebAndWebApiAutomation.Api.SwaggerUtilities.Models
{
    internal class Paths : ModelsBase
    {
        private OpenApiVersion _version;
        private List<SwaggerPath> _paths;

        internal Paths(JObject swaggerDefinitionJobject, OpenApiVersion version)
        {
            _paths = new List<SwaggerPath>();

            _version = version;
            ParsePaths(swaggerDefinitionJobject);
        }

        internal List<SwaggerPath> GetPaths() => _paths;

        private void ParsePaths(JObject swaggerDefinitionJobject)
        {
            string paths = swaggerDefinitionJobject["paths"].ToString();

            if (string.IsNullOrEmpty(paths))
                throw new Exception("No 'paths' key defined in the provided swagger definition");

            var allPaths = swaggerDefinitionJobject["paths"].Children();
            foreach (var path in allPaths)
            {
                //_paths.Add(BuildPathObject(path));
            };
        }

        //private SwaggerPath BuildPathObject(JToken path)
        //{
        //    foreach(JProperty property in path.)
        //    var swaggerPath = new SwaggerPath()
        //    {
        //        //Url = path.Name;
        //    };

        //    return swaggerPath;
        //}


    }    
}
