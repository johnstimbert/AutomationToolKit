﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAutomationToolKit;
using WebAutomationToolKit.InternalImplementations;

namespace WebAndApiAutomation.Tests.Tests
{
    [TestClass]
    public class TestExecutorTests
    {
        private static WebAutomation _webAutomation;
        private static IWebDriverManager _driverManager;
        LoggerSettings _loggerSettings;
        ILogger _logger;
        WebAutomationToolKit.ITestExecutor _testExecutor;

        private const string _loggerTests = "Logger_Tests";

        private const string _logPath = @"e:\logger";
        private const string _logFileName = "logger.txt";
        private const string _failedTestLogFileName = "logger.txt_Failures";
        private string _defaultDateFormatProperty = "MM_dd_yyyy";
        private static readonly string _driverPath = null;// @"E:\source\AutomationToolKit\WebUiAutomationToolKit\bin\Release";

        [TestInitialize]
        public void BeforeEachTest()
        {
            if (_webAutomation == null)
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
            {
                var logFiles = Directory.GetFiles(_logPath);
                foreach (var logFile in logFiles)
                {
                    File.Delete(logFile);
                }
            }
        }

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    _testExecutor.Execute(() => {




        //    }, _driverManager);
        //}
    }
}
