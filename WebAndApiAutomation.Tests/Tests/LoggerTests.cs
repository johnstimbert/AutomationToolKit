using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAndWebApiAutomation.Exceptions;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation.Tests
{
    [TestClass]
    public class LoggerTests
    {
        private static TestContext _testContext;
        public TestContext TestContext { get { return _testContext; } set { _testContext = value; } }

        private static WebAutomation _webAutomation;
        private static IWebDriverManager _driverManager;
        LoggerSettings _loggerSettings;

        private const string _loggerTests = "Logger_Tests";

        private const string _logPath = @"c:\logger";
        private const string _logFileName = "logger.txt";
        private const string _failedTestLogFileName = "logger.txt_Failures";
        private string _defaultDateFormatProperty = "MM_dd_yyyy";
        private static readonly string _driverPath = @"C:\Users\j_sti\Source\Repos\BrainStemSolutions\AutomationToolKit\WebAndWebApiAutomation\bin\Debug";

        private ILogger _logger;
        
        [TestInitialize]
        public void BeforeEachTest()
        {
            if(_webAutomation == null)
                _webAutomation = new WebAutomation(_driverPath, 10);

            _loggerSettings = new LoggerSettings()
            {
                LogFileName = _logFileName,
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
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogIsSavedToTheCorrectLocation()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = false;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            _logger.Log(LogMessageType.TESTINFO, "Test Message");
                        
            Assert.IsTrue(File.Exists(Path.Combine(_logPath, _logFileName)), $"The file {_logFileName} was not founf in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogFileNameDoesNotChangeWhenAppendDateConfigIsFalse()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = false;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);
            _logger.Log(LogMessageType.TESTINFO, "Test Message");

            Assert.IsTrue(File.Exists(Path.Combine(_logPath, _logFileName)), $"The file {_logFileName} was not founf in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogFileNameIsCorrectWhenAppendDateConfigIsTrue_DefaultDateTimeFormat()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = true;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);
            _logger.Log(LogMessageType.TESTINFO, "Test Message");

            var fileName = LoggerTestHelpers.AppendDateToLogFile(_logFileName, _defaultDateFormatProperty);

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
            _loggerSettings.AppendDateToLogFile = true;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);

            int i = 0;
            while (i < 6)
            {
                _logger.Log(LogMessageType.TESTINFO, $"Line{i}");
                i++;
            }
            _logger.Log(LogMessageType.TESTPASSED, $"Line{i}");

            var lines = File.ReadAllLines(Path.Combine(_logPath, _logFileName));

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

            var lines = File.ReadAllLines(Path.Combine(_logPath, _logFileName));

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

            var lines = File.ReadAllLines(Path.Combine(_logPath, _failedTestLogFileName));

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

            var lines = File.ReadAllLines(Path.Combine(_logPath, _failedTestLogFileName));

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

            var lines = File.ReadAllLines(Path.Combine(_logPath, _failedTestLogFileName));

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
