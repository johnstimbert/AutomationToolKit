using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebUiAutomationToolKit.Exceptions;
using static WebUiAutomationToolKit.WebUiAutomationEnums;

namespace WebUiAutomationToolKit.Tests
{
    [TestClass]
    public class LoggerTests
    {
        private static TestContext _testContext;
        public TestContext TestContext { get { return _testContext; } set { _testContext = value; } }

        private static WebUiAutomation _webAutomation;
        LoggerSettings _loggerSettings;

        private const string _loggerTests = "Logger_Tests";

        private const string _logPath = @"c:\logger";
        private const string _baseLogFileName = "logger.txt";
        private string _defaultDateFormatProperty = "MM_dd_yyyy";
        private static readonly string _driverPath = @"C:\Users\j_sti\Source\Repos\BrainStemSolutions\AutomationToolKit\WebUiAutomationToolKit\bin\release";

        private ILogger _logger;
        
        [TestInitialize]
        public void BeforeEachTest()
        {
            if(_webAutomation == null)
                _webAutomation = new WebUiAutomation(_driverPath, 10);

            _loggerSettings = new LoggerSettings()
            {
                LogFileName = _baseLogFileName,
                LogFilePath = _logPath
            };
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            if (Directory.Exists(_logPath))
            {
                var logFiles = Directory.GetFiles(_logPath);
                foreach (var logFile in logFiles)
                {
                    File.Delete(logFile);
                }
            }

            _webAutomation = null;
            Thread.Sleep(2000);
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogIsSavedToTheCorrectLocation()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = false;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            _logger.Log(LogMessageType.TESTINFO, "Test Message");
                        
            Assert.IsTrue(File.Exists(Path.Combine(_logPath, _baseLogFileName)), $"The file {_baseLogFileName} was not found in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogFileNameDoesNotChangeWhenAppendDateConfigIsFalse()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = false;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);
            _logger.Log(LogMessageType.TESTINFO, "Test Message");

            Assert.IsTrue(File.Exists(Path.Combine(_logPath, _baseLogFileName)), $"The file {_baseLogFileName} was not founf in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogFileNameIsCorrectWhenAppendDateConfigIsTrue_DefaultDateTimeFormat()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = true;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);
            _logger.Log(LogMessageType.TESTINFO, "Test Message");

            var fileName = LoggerTestHelpers.AppendDateToLogFile(_baseLogFileName, _defaultDateFormatProperty);

            Assert.IsTrue(File.Exists(Path.Combine(_logPath, fileName)), $"The file {fileName} was not found in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void ErrorThrownWhenDateFormatPropertyIsInvalid()
        {
            string badDateFormat = "12345";
            string expectedErrorText = $"The dateTime format {badDateFormat} is not valid. " +
                $"Please reference the folowing article for valid values https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings";

            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = true;//The default is true

            try
            {
                _loggerSettings.DateFormat = badDateFormat;
                _logger = _webAutomation.GetLogger(_loggerSettings);
            }
            catch (LoggerException ex)
            {
                Assert.AreEqual(expectedErrorText, ex.Message, "Expected exception was not returned");
            }
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogFileRetainsAllLinesForCurrentRun()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = false;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            int i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTPASSED, $"Line{i}");

            var lines = File.ReadAllLines(Path.Combine(_logPath, _logger.GetCurrentLogFileName()));

            Assert.AreEqual(7, lines.Length, $"The log contains {lines.Length} lines, expected {7}");

        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogFileRetainsAllLinesForCurrentRunWhenDateIsAppended()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = true;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            int i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTPASSED, $"Line{i}");

            var lines = File.ReadAllLines(Path.Combine(_logPath, _logger.GetCurrentLogFileName()));

            Assert.AreEqual(7, lines.Length, $"The log contains {lines.Length} lines, expected {7}");

        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LinesForCurrentRunAreWrittenInTheCorrectOrder()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = true;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            int i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTPASSED, $"Line{i}");

            var logName = _logger.GetCurrentLogFileName();

            var lines = File.ReadAllLines(Path.Combine(_logPath, _logger.GetCurrentLogFileName()));

            Assert.AreEqual(7, lines.Length, $"The log contains {lines.Length} lines, expected {7}");

            int x = 0;
            while (x < i + 1)
            {
                var expectedValue = $"Line{x}";
                var result = lines[x].Contains(expectedValue);
                Assert.IsTrue(result, $"Line {x} did not contain the expected value {expectedValue}");
                x++;
            }
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LinesForFailedTestAreWrittenInTheFailureLog()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = false;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            int i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTFAILED, $"Line{i}");

            var lines = File.ReadAllLines(Path.Combine(_logPath, _logger.GetCurrentFailureLogFileName()));

            Assert.AreEqual(8, lines.Length, $"The log contains {lines.Length} lines, expected {8}");

            int x = 0;
            while (x < i + 1)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))//errors are seperated by a blank line
                    continue;

                var expectedValue = $"Line{x}";
                var result = lines[x].Contains(expectedValue);
                Assert.IsTrue(result, $"Line {x} did not contain the expected value {expectedValue}");
                x++;
            }
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LinesForFailedTestAreWrittenInTheFailureLogWithPreceedingSuccess()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = true;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            int i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTPASSED, $"Line{i}");

            i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTFAILED, $"Line{i}");

            var lines = File.ReadAllLines(Path.Combine(_logPath, _logger.GetCurrentFailureLogFileName()));

            Assert.AreEqual(8, lines.Length, $"The log contains {lines.Length} lines, expected {8}");

            int x = 0;
            while (x < i + 1)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))//errors are seperated by a blank line
                    continue;

                var expectedValue = $"Line{x}";
                var result = lines[x].Contains(expectedValue);
                Assert.IsTrue(result, $"Line {x} did not contain the expected value {expectedValue}");
                x++;
            }
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LinesForFailedTestAreWrittenInTheFailureLogWithFollowingingSuccess()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = true;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            int i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTFAILED, $"Line{i}");

            i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTPASSED, $"Line{i}");

            var lines = File.ReadAllLines(Path.Combine(_logPath, _logger.GetCurrentFailureLogFileName()));

            Assert.AreEqual(8, lines.Length, $"The log contains {lines.Length} lines, expected {8}");

            int x = 0;
            while (x < i + 1)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))//errors are seperated by a blank line
                    continue;

                var expectedValue = $"Line{x}";
                var result = lines[x].Contains(expectedValue);
                Assert.IsTrue(result, $"Line {x} did not contain the expected value {expectedValue}");
                x++;
            }
        }
    }
}
