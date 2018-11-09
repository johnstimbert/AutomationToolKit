using WebAndWebApiAutomation.Api.SwaggerUtilities;

namespace WebAndWebApiAutomation.Api
{
    internal class SwaggerAutomator
    {
        private readonly string _swaggerJsonDefintionString;
        private SwaggerReader _swaggerReader;

        internal SwaggerAutomator(string swaggerJsonDefintionString)
        {
            _swaggerJsonDefintionString = swaggerJsonDefintionString;
            _swaggerReader = new SwaggerReader(_swaggerJsonDefintionString);
            var info = _swaggerReader.GetInfo();
            var paths = _swaggerReader.GetPaths();
        }

        #region Public Properties


        #endregion

        #region Public Methods


        #endregion

        #region Private Methods


        #endregion
    }
}
