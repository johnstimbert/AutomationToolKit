using OpenQA.Selenium;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.Helpers;
using WebAutomationToolKit.Validators;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit
{
    /// <summary>
    /// This class contains the utility and component instance creation methods for automating cross browser testing of a web site
    /// </summary>
    public sealed class WebAutomation
    {
        private readonly string? _driverPath;
        private readonly int _timeoutForWait;
        private static WebDriverManager _webDriverManagerInstance;
        private static Logger _loggerInstance;
        private static TestExecutor _testExecutorInstance;

        /// <summary>
        /// Creates an instance of the WebAutomation class
        /// </summary>
        /// <param name="driverPath">The path to the driver service executables</param>
        /// <param name="timeoutForWaitOverride">THis can be set to override the defauslt five (5) second timeout in the webdriver</param>
        public WebAutomation(string driverPath, int timeoutForWaitOverride = 5)
        {
            if(driverPath != null)
                driverPath = ValidateDriverPath(driverPath);

            if (timeoutForWaitOverride <= 0)
                timeoutForWaitOverride = 5;

            _timeoutForWait = timeoutForWaitOverride;
            _driverPath = driverPath;
        }


        private WebAutomation() { }

        private string ValidateDriverPath(string driverPath)
        {
            if (driverPath.LastIndexOf("\\") != driverPath.Length - 1)
                driverPath += @"\";

            if (!Directory.Exists(driverPath))
                throw new WebAutomationException($"The path {driverPath} could not be found");

            return driverPath;
        }

        public IWebDriverManager GetIWebDriverManager()
        {
            try
            {
                if(_webDriverManagerInstance == null)
                    _webDriverManagerInstance = new WebDriverManager(_timeoutForWait);

                return _webDriverManagerInstance;
            }
            catch (WebAutomationException wea)
            {
                throw wea;
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        public ILogger GetLogger(LoggerSettings loggerSettings)
        {
            try
            {
                if (_loggerInstance == null)
                    _loggerInstance = new Logger(loggerSettings);

                return _loggerInstance;
            }
            catch (LoggerException le)
            {
                throw le;
            }
            catch (Exception ex)
            {
                throw new LoggerException(ex.ToString());
            }
        }

        public ITestExecutor GetTestExecutor(string resultsPath, bool collectTestData = false)
        {
            try
            {                
                if (_testExecutorInstance == null)
                    _testExecutorInstance = new TestExecutor(resultsPath, _loggerInstance, collectTestData);

                return _testExecutorInstance;
            }
            catch (TestExecutorException te)
            {
                throw te;
            }
            catch (Exception ex)
            {
                throw new LoggerException(ex.ToString());
            }
        }
        
        public List<KeyValuePair<By, NavigationResult>> TestLinkNavigationForAllAnchorsFoundInPage(IWebDriverManager webDriverManager)
        {
            var manager = Helper.IsDriverNull(webDriverManager);
            try
            {
                var navValidator = new NavigationValidator(manager.GetWait());
                return navValidator.TestLinkNavigationForAllAnchorsFoundInPage(manager.GetActiveDriver());
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

        public string ParseInnerText(string htmlText)
        {
            try
            {
                StructureValidator structureValidator = new StructureValidator();
                return structureValidator.ParseInnerText(htmlText);
            }
            catch (Exception ex)
            {
                throw new WebAutomationException(ex.ToString());
            }
        }

    }
}
