using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestLoggingAndDataFormatter.Exceptions;

namespace TestLoggingAndDataFormatter.Tests
{
    [TestClass]
    public class LoggerTests
    {
        private const string _loggerTests = "Logger_Tests";

        private const string _logPath = @"c:\logger";
        private const string _logFileName = "logger.txt";
        private string _defaultDateFormatProperty = "MM_dd_yyyy";

        private Logger _logger;


        [TestInitialize]
        public void BeforeEachTest()
        {
            _logger = new Logger(_logFileName, _logPath);
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
            _logger.PreservePreviousLogFiles = false;//Clear the directory
            _logger.AppendDateToLogFile = false;//The default is true
            _logger.Log(LogMessageType.TESTINFO, "Test Message");

            Assert.IsTrue(File.Exists(Path.Combine(_logPath, _logFileName)), $"The file {_logFileName} was not founf in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogFileNameDoesNotChangeWhenAppendDateConfigIsFalse()
        {
            _logger.PreservePreviousLogFiles = false;//Clear the directory
            _logger.AppendDateToLogFile = false;//The default is true
            _logger.Log(LogMessageType.TESTINFO, "Test Message");

            Assert.IsTrue(File.Exists(Path.Combine(_logPath, _logFileName)), $"The file {_logFileName} was not founf in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void LogFileNameIsCorrectWhenAppendDateConfigIsTrue_DefaultDateTimeFormat()
        {
            _logger.PreservePreviousLogFiles = false;//Clear the directory
            _logger.AppendDateToLogFile = true;//The default is true
            _logger.Log(LogMessageType.TESTINFO, "Test Message");

            var fileName = LoggerTestHelpers.AppendDateToLogFile(_logFileName, _defaultDateFormatProperty);

            Assert.IsTrue(File.Exists(Path.Combine(_logPath, fileName)), $"The file {fileName} was not founf in the path {_logPath}");
        }

        [TestMethod]
        [TestCategory(_loggerTests)]
        public void ErrorThrownWhenDateFormatPropertyIsInvalid()
        {
            string badDateFormat = "12345";
            string expectedErrorText = $"The dateTime format {badDateFormat} is not valid. " +
                $"Please reference the folowing article for valid values https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings";

            _logger.PreservePreviousLogFiles = false;//Clear the directory
            _logger.AppendDateToLogFile = true;//The default is true

            try
            {
                _logger.DateFormatProperty = badDateFormat;
            }
            catch(LoggerException ex)
            {
                Assert.AreEqual(expectedErrorText, ex.Message, "Expected exception was not returned");
            }
        }
    }
}
