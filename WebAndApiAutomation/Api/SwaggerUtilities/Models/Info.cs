using WebAndApiAutomation.Api.SwaggerUtilities.Exceptions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using static WebAndApiAutomation.Api.SwaggerUtilities.Enums;

namespace WebAndApiAutomation.Api.SwaggerUtilities.Models
{
    internal class Info : ModelsBase
    {
        #region OpenApi3 Attribute Definitions
        private enum OpenApi3InfoObjectRequiredAttributes
        {
            title,
            version
        }

        private enum OpenApi3InfoObjectOptionalAttributes
        {
            description,
            termsOfService,
            contact,
            license
        }

        private enum OpenApi3ServerObjectRequiredAttributes
        {
            url
        }

        private enum OpenApi3ServerObjectOptionalAttributes
        {
            description,
            variables
        }

        #endregion

        #region SwaggerApi2 Attribute Definitions
        private enum SwaggerApi2OptionalAttributes
        {
            title,
            description,
            version,
            host,
            basePath,
            schemes
        }

        #endregion

        private OpenApiVersion _version;

        internal Info(JObject swaggerDefinitionJobject, OpenApiVersion version)
        {
            BaseUrls = new List<string>();

            _version = version;
            ParseInfo(swaggerDefinitionJobject);
        }

        private void ParseInfo(JObject swaggerDefinitionJobject)
        {
            JObject infoJObject = JObject.Parse(swaggerDefinitionJobject["info"].ToString());

            switch(_version)
            {
                case OpenApiVersion.Two:
                    //Get values specific to the Info object definition
                    Title = (string)infoJObject.Property(SwaggerApi2OptionalAttributes.title.ToString()) ?? _attributeNotDefined;
                    InfoDescription = (string)infoJObject.Property(SwaggerApi2OptionalAttributes.title.ToString()) ?? _attributeNotDefined;
                    ApiVersion = (string)infoJObject.Property(SwaggerApi2OptionalAttributes.version.ToString()) ?? _attributeNotDefined;

                    //Get values not defined with a specific json object
                    var host = (string)swaggerDefinitionJobject.Property(SwaggerApi2OptionalAttributes.host.ToString()) ?? _attributeNotDefined;
                    var basePath = (string)swaggerDefinitionJobject.Property(SwaggerApi2OptionalAttributes.basePath.ToString()) ?? _attributeNotDefined;
                    var schemes = (string)swaggerDefinitionJobject.Property(SwaggerApi2OptionalAttributes.schemes.ToString()) ?? _attributeNotDefined;

                    if(host.Equals(_attributeNotDefined) || basePath.Equals(_attributeNotDefined) || schemes.Equals(_attributeNotDefined))
                    {
                        BaseUrls.Add($"Base URLs require {SwaggerApi2OptionalAttributes.host}, {SwaggerApi2OptionalAttributes.basePath} and " +
                                     $"{SwaggerApi2OptionalAttributes.schemes} to be defined");
                    }
                    else
                    {
                        var allSchemes = swaggerDefinitionJobject[SwaggerApi2OptionalAttributes.schemes].Children();
                        foreach(var child in allSchemes.Children())
                        {
                            BaseUrls.Add($"{child}://{host}{basePath}");
                        }
                    }

                    break;
                case OpenApiVersion.Three:
                    //Check for required values
                    if (infoJObject[OpenApi3InfoObjectRequiredAttributes.title] == null)
                        throw new SwaggerAutomatorException($"{_requiredAttributeNotFound}: {OpenApi3InfoObjectRequiredAttributes.title}");

                    if (infoJObject[OpenApi3InfoObjectRequiredAttributes.version] == null)
                        throw new SwaggerAutomatorException($"{_requiredAttributeNotFound}: {OpenApi3InfoObjectRequiredAttributes.version}");

                    if (infoJObject[OpenApi3ServerObjectRequiredAttributes.url] == null)
                        throw new SwaggerAutomatorException($"{_requiredAttributeNotFound}: {OpenApi3ServerObjectRequiredAttributes.url}");

                    //Get required values
                    Title = (string)infoJObject[OpenApi3InfoObjectRequiredAttributes.title];
                    ApiVersion = (string)infoJObject[OpenApi3InfoObjectRequiredAttributes.version];

                    var servers = (JArray)swaggerDefinitionJobject[OpenApi3ServerObjectRequiredAttributes.url];
                    for (int i = 0; i < servers.Count; i++)
                    {
                        BaseUrls.Add((string)servers[OpenApi3ServerObjectRequiredAttributes.url]);
                    }

                    //Get optional values
                    InfoDescription = (string)infoJObject[OpenApi3InfoObjectOptionalAttributes.description] ?? _attributeNotDefined;

                    


                    break;
                default:
                    throw new SwaggerAutomatorException(_apiVersionNotSupported);
            }
        }

        internal string Title { get; set; }
        internal string InfoDescription { get; set; }
        internal string ServerDescription { get; set; }
        internal List<string> BaseUrls { get; set; }
        internal string ApiVersion { get; set; }
    }
}
