﻿using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.Helpers;
using WebAutomationToolKit.TestDataCollection;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.InternalImplementations
{
    /// <summary>
    /// Concrete implementation of ITestExecutor interface to manage test execution
    /// </summary>
    internal sealed class TestExecutor : ITestExecutor
    {
        private Logger _logger;
        private readonly TestDataCollector _testDataCollector;

        private string _testId;
        private string _resultsPath;
        private Guid _testRunId;
        

        private TestExecutor() { }
        
        internal TestExecutor(string resultsPath, Logger logger = null, bool collectTestData = false)
        {
            _logger = logger;
            if (collectTestData)
            {
                _testDataCollector = new TestDataCollector();

                if (_testRunId == null)
                    _testRunId = Guid.NewGuid();
            }

            if (string.IsNullOrEmpty(resultsPath) || string.IsNullOrWhiteSpace(resultsPath))
                throw new TestExecutorException("The resultsPath parameter cannot be null, empty or constain only whitespace");

            _resultsPath = Path.Combine(resultsPath, DateTime.Now.ToString("MM.dd.yyyy_h:mm"));
        }

        #region Events and Event Delegates
        internal static event TestBegunEvent TestBegun;
        internal delegate void TestBegunEvent(Guid runId);

        internal static event TestCompleteEvent TestComplete;
        internal delegate void TestCompleteEvent(Guid runId, string testMethodName, bool isSuccess, Exception error = null);

        #endregion

        /// <summary>
        /// Executes the test provided in the Action delegate.
        /// </summary>
        /// <param name="testMethod"></param>
        /// <param name="webDriverManager"></param>
        public void Execute(Action testMethod, IWebDriverManager webDriverManager)
        {
            //Get the test method name
            var testMethodName = GetName(testMethod.Method.Name);
            var browserName = webDriverManager.GetActiveDriverType().ToString();
            _testId = $"{testMethodName}.{browserName}";
            try
            {

                //Raise the TestBegun event to signal the TestDataCollector a new test is being executed
                TestBegun?.Invoke(_testRunId);

                //If the loger is null execute without logging
                if (_logger == null)
                {
                    testMethod();
                }
                else
                {
                    _logger.Log(LogMessageType.TESTINFO, $"[START] ==== Test Method: {testMethodName} - Driver: {browserName} ====");
                    testMethod();
                    _logger.Log(LogMessageType.TESTPASSED, ($"[END]   =============== [PASSED] ==============="));
                }

                //Raise the TestComplete event to signal the TestDataCollector a test is completed. Successfully in this case.
                TestComplete?.Invoke(_testRunId, _testId, true);

            }
            catch(WebAutomationException wae)
            {
                if (_testDataCollector != null)
                {
                    TakeScreenShot(webDriverManager, _testId, _resultsPath);
                    //Raise the TestComplete event to signal the TestDataCollector a test is completed. In this case it failed and we need to pass the exception.
                    TestComplete?.Invoke(_testRunId, _testId, false, wae);
                }

                if (_logger != null)
                {
                    _logger.Log(LogMessageType.TESTFAILED, wae.Message);
                    _logger.Log(LogMessageType.TESTINFO, $"[END]   =============== [FAILED] ===============");
                }

                throw wae;
            }
            catch(Exception ex)
            {
                var tee = new TestExecutorException(ex.ToString());

                if (_testDataCollector != null)
                {
                    TakeScreenShot(webDriverManager, _testId, _resultsPath);
                    //Raise the TestComplete event to signal the TestDataCollector a test is completed. In this case it failed and we need to pass the exception.
                    TestComplete?.Invoke(_testRunId, _testId, false, tee);
                }

                if (_logger != null)
                {
                    _logger.Log(LogMessageType.TESTFAILED, tee.Message);
                    _logger.Log(LogMessageType.TESTINFO, $"[END]   =============== [FAILED] ===============");
                }

                throw tee;
            }
        }

        /// <summary>
        /// Returns formatted string of test results that can be written to a log file or elsewhere
        /// </summary>
        /// <returns>Formatted test results</returns>
        public string GetTestRunSummary()
        {
            if (_testDataCollector == null)
                return "Please enabale test data collection when instantiating the TestExecutor";

            return string.Join(Environment.NewLine, _testDataCollector.GetRunSummary());
        }

        #region Private Methods
        private string GetName(string methodName)
        {
            int start = methodName.IndexOf('<') + 1;
            int end = methodName.IndexOf('>') - 1;
            string name = methodName.Substring(start, end);
            return name;
        }

        private void TakeScreenShot(IWebDriverManager webDriverManager, string testMethodName, string resultsPath)
        {
            try
            {
                WebDriverManager manager = (WebDriverManager)webDriverManager;
                Helper.TakeScreenShot(manager.GetActiveDriver(), Path.Combine(resultsPath, "ScreenShots"), testMethodName);
            }
            catch (Exception) { }
        }

        #endregion
    }
}
