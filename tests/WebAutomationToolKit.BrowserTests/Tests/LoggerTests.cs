using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.InternalImplementations;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.BrowserTests
{
    [TestClass]
    public class LoggerTests
    {
        public TestContext TestContext { get; set; } = null!;

        private static WebAutomation _webAutomation = null!;
        LoggerSettings _loggerSettings = null!;

        private const string _loggerTests = "Logger_Tests";

        private string _logPath = null!;
        private const string _baseLogFileName = "logger.txt";
        private const string _defaultDateFormatProperty = "MM_dd_yyyy";
        private static readonly string? _driverPath = null;//@"E:\source\AutomationToolKit\WebUiAutomationToolKit\bin\Release";

        private ILogger _logger = null!;
        
        [TestInitialize]
        public void BeforeEachTest()
        {
            _logPath = Path.Combine(Path.GetTempPath(), "WatLoggerTests_" + Guid.NewGuid().ToString("N")[..8]);
            Directory.CreateDirectory(_logPath);

           _webAutomation = new WebAutomation(_driverPath, 10);

            _loggerSettings = new LoggerSettings()
            {
                LogFileName = _baseLogFileName,
                LogFilePath = _logPath
            };
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            // P3: Flush buffered data, null both references so the Logger and its
            // StreamWriters become unreachable, then force GC + finalizers.
            // StreamWriter's finalizer closes the file handle, allowing
            // Directory.Delete to succeed on Windows.
            _logger?.Flush();
            _logger = null!;
            _webAutomation = null!;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (Directory.Exists(_logPath))
                Directory.Delete(_logPath, recursive: true);
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        [Ignore]
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
        [Ignore]
        public void LogFileNameDoesNotChangeWhenAppendDateConfigIsFalse()
        {
            _loggerSettings.NumberOfLogFilesToPreserve = 0;//Clear the directory
            _loggerSettings.AppendDateToLogFile = false;//The default is true

            _logger = _webAutomation.GetLogger(_loggerSettings);
            _logger.Log(LogMessageType.TESTINFO, "Test Message");

            Assert.IsTrue(File.Exists(Path.Combine(_logPath, _baseLogFileName)), $"The file {_baseLogFileName} was not found in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        [Ignore]
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
        [Ignore]
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

            var logFileName = _logger.GetCurrentLogFileName();
            _logger.Flush(); // ensure buffered data is on disk before reading
            var lines = ReadLogFile(Path.Combine(_logPath, logFileName));

            Assert.HasCount(7, lines, $"The log contains {lines.Length} lines, expected {7}");

        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        [Ignore]
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

            var logFileName = _logger.GetCurrentLogFileName();
            _logger.Flush(); // ensure buffered data is on disk before reading
            var lines = ReadLogFile(Path.Combine(_logPath, logFileName));

            Assert.HasCount(7, lines, $"The log contains {lines.Length} lines, expected {7}");

        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        [Ignore]
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
            _logger.Flush(); // ensure buffered data is on disk before reading
            var lines = ReadLogFile(Path.Combine(_logPath, logName));

            Assert.HasCount(7, lines, $"The log contains {lines.Length} lines, expected {7}");
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
        [Ignore]
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

            var failureLogFileName = _logger.GetCurrentFailureLogFileName() ?? string.Empty;
            _logger.Flush(); // ensure buffered data is on disk before reading
            var lines = ReadLogFile(Path.Combine(_logPath, failureLogFileName));

            Assert.HasCount(8, lines, $"The log contains {lines.Length} lines, expected {8}");

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
        [Ignore]
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

            var failureLogFileName = _logger.GetCurrentFailureLogFileName() ?? string.Empty;
            _logger.Flush(); // ensure buffered data is on disk before reading
            var lines = ReadLogFile(Path.Combine(_logPath, failureLogFileName));

            Assert.HasCount(8, lines, $"The log contains {lines.Length} lines, expected {8}");

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
        [Ignore]
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

            var failureLogFileName = _logger.GetCurrentFailureLogFileName() ?? string.Empty;
            _logger.Flush(); // ensure buffered data is on disk before reading
            var lines = ReadLogFile(Path.Combine(_logPath, failureLogFileName));

            Assert.HasCount(8, lines, $"The log contains {lines.Length} lines, expected {8}");

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

        // ── helpers ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads all non-empty lines from <paramref name="fullPath"/> using
        /// <see cref="FileShare.ReadWrite"/> so the read succeeds even while the
        /// Logger's buffered <see cref="System.IO.StreamWriter"/> still holds the
        /// file open for writing.
        /// </summary>
        private static string[] ReadLogFile(string fullPath)
        {
            using var stream = new FileStream(
                fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            var lines = new List<string>();
            while (reader.ReadLine() is { } line)
                lines.Add(line);
            return lines.ToArray();
        }
    }
}
