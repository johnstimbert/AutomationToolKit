using WebAndApiAutomation.Api.SwaggerUtilities.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebAndApiAutomation.Api.SwaggerUtilities.Enums;

namespace WebAndApiAutomation.Api.SwaggerUtilities
{
    internal class SwaggerReader
    {
        private readonly JObject _swaggerDefinitionJObject;
        private OpenApiVersion _openApiVersion;
        private Info _info = null;
        private Paths _paths = null;

        internal SwaggerReader(string swaggerJsonDefintion)
        {
            _swaggerDefinitionJObject = JObject.Parse(swaggerJsonDefintion);
            _openApiVersion = DetermineApiVersion(_swaggerDefinitionJObject);
        }

        #region Private Methods
        private OpenApiVersion DetermineApiVersion(JObject swaggerDefinitionJObject)
        {
            string version = (string)swaggerDefinitionJObject["swagger"];
            if(!string.IsNullOrEmpty(version))
            {
                return OpenApiVersion.Two;
            }

            version = (string)swaggerDefinitionJObject["openapi"];
            if (!string.IsNullOrEmpty(version))
            {
                return OpenApiVersion.Three;
            }

            throw new Exception("No OpenApi version was found make sure it contains either swagger: <version> or openapi: <version>");
        }

        private Info ParseInfo()
        {
            return new Info(_swaggerDefinitionJObject, _openApiVersion);
        }

        private Paths ParsePaths()
        {
            return new Paths(_swaggerDefinitionJObject, _openApiVersion);
        }

        #endregion

        #region Internal Methods
        internal Info GetInfo()
        {
            if (_info == null)
                _info = ParseInfo();
            return _info;
        }

        internal Paths GetPaths()
        {
            if (_paths == null)
                _paths = ParsePaths();
            return _paths;
        }

        #endregion

    }
}
