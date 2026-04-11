using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAutomationToolKit;
using WebAutomationToolKit.InternalImplementations;

namespace WebAndApiAutomation.Tests.Tests
{
    [TestClass]
    public class TestExecutorTests
    {
        private static WebAutomation _webAutomation = null!;
        private static IWebDriverManager _driverManager = null!;
        LoggerSettings _loggerSettings = null!;
        ILogger _logger = null!;
        WebAutomationToolKit.ITestExecutor _testExecutor = null!;

        private const string _loggerTests = "Logger_Tests";

        private string _logPath = null!;
        private const string _logFileName = "logger.txt";
        private const string _failedTestLogFileName = "logger.txt_Failures";
        private static readonly string? _driverPath = null;// @"E:\source\AutomationToolKit\WebUiAutomationToolKit\bin\Release";

        [TestInitialize]
        public void BeforeEachTest()
        {
            _logPath = Path.Combine(Path.GetTempPath(), "WatExecutorTests_" + Guid.NewGuid().ToString("N")[..8]);
            Directory.CreateDirectory(_logPath);

           _webAutomation = new WebAutomation(_driverPath, 10);

            _loggerSettings = new LoggerSettings()
            {
                LogFileName = _logFileName,
                LogFilePath = _logPath,
                AppendDateToLogFile = false,
                GenerateFailureLog = false,
                NumberOfLogFilesToPreserve = 0
            };

            _driverManager = _webAutomation.GetIWebDriverManager();
            _logger = _webAutomation.GetLogger(_loggerSettings);
            _testExecutor = _webAutomation.GetTestExecutor(_logPath, false);
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            if (Directory.Exists(_logPath))
                Directory.Delete(_logPath, recursive: true);
        }

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    _testExecutor.Execute(() => {




        //    }, _driverManager);
        //}
    }
}
