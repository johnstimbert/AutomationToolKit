using System.Runtime.ExceptionServices;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.InternalImplementations;

namespace WebAutomationToolKit.InternalImplementations
{
    /// <summary>
    /// This class contains the utility and component instance creation methods for automating cross browser testing of a web site
    /// </summary>
    public sealed class WebAutomation
    {
        private readonly string? _driverPath;
        private readonly int _timeoutForWait;
        private WebDriverManager? _webDriverManagerInstance;
        private ILogger? _loggerInstance;
        private TestExecutor? _testExecutorInstance;

        /// <summary>
        /// Creates an instance of the <see cref="WebAutomation"/> class.
        /// </summary>
        /// <param name="driverPath">
        /// Optional path to the directory containing the browser driver executables (e.g., <c>chromedriver.exe</c>).
        /// When <see langword="null"/>, Selenium Manager resolves the driver automatically.
        /// </param>
        /// <param name="timeoutForWaitOverride">
        /// Overrides the default five-second WebDriver wait timeout. Values less than or equal to zero are clamped to five seconds.
        /// </param>
        public WebAutomation(string? driverPath = null, int timeoutForWaitOverride = 5)
        {
            if (driverPath != null)
                driverPath = ValidateDriverPath(driverPath);

            if (timeoutForWaitOverride <= 0)
                timeoutForWaitOverride = 5;

            _timeoutForWait = timeoutForWaitOverride;
            _driverPath = driverPath;
        }

        private static string ValidateDriverPath(string driverPath)
        {
            if (!Path.EndsInDirectorySeparator(driverPath))
                driverPath += Path.DirectorySeparatorChar;

            if (!Directory.Exists(driverPath))
                throw new WebAutomationException($"The path {driverPath} could not be found");

            return driverPath;
        }

        /// <summary>
        /// Returns the <see cref="IWebDriverManager"/> for this instance, creating it on the first call.
        /// The driver path supplied to the <see cref="WebAutomation"/> constructor is forwarded to the manager.
        /// </summary>
        /// <returns>The <see cref="IWebDriverManager"/> instance owned by this <see cref="WebAutomation"/>.</returns>
        public IWebDriverManager GetIWebDriverManager()
            => WrapException(
                () =>
                {
                    _webDriverManagerInstance ??= new WebDriverManager(_timeoutForWait, _driverPath);
                    return _webDriverManagerInstance;
                },
                (msg, ex) => new WebAutomationException(msg, ex));

        /// <summary>
        /// Creates and returns a new <see cref="ILogger"/> configured with the supplied <paramref name="loggerSettings"/>.
        /// Call this method before <see cref="GetTestExecutor"/> if you want the executor to use this logger.
        /// </summary>
        /// <param name="loggerSettings">Settings that control log file location, naming, and retention.</param>
        /// <returns>A new <see cref="ILogger"/> instance.</returns>
        public ILogger GetLogger(LoggerSettings loggerSettings)
            => WrapException(
                () =>
                {
                    _loggerInstance = new Logger(loggerSettings);
                    return _loggerInstance;
                },
                (msg, ex) => new LoggerException(msg, ex));

        /// <summary>
        /// Creates and returns a new <see cref="ITestExecutor"/>.
        /// Always creates a fresh executor on each call.
        /// Call <see cref="GetLogger"/> before this method to associate a logger with the executor.
        /// </summary>
        /// <param name="resultsPath">Directory where test results and screenshots are written.</param>
        /// <param name="collectTestData">When <see langword="true"/>, enables run-summary data collection.</param>
        /// <returns>A new <see cref="ITestExecutor"/> instance.</returns>
        public ITestExecutor GetTestExecutor(string resultsPath, bool collectTestData = false)
            => WrapException(
                () =>
                {
                    _testExecutorInstance = new TestExecutor(resultsPath, _loggerInstance, collectTestData);
                    return _testExecutorInstance;
                },
                (msg, ex) => new TestExecutorException(msg, ex));

        private T WrapException<T>(Func<T> action, Func<string, Exception, Exception> wrapperFactory)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                var candidate = wrapperFactory(ex.Message, ex);
                if (ex.GetType() == candidate.GetType())
                    ExceptionDispatchInfo.Capture(ex).Throw();
                throw candidate;
            }
        }
    }
}
