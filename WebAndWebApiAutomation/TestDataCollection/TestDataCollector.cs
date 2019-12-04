using System;
using System.Collections.Generic;
using WebAndWebApiAutomation.Exceptions;

namespace WebAndWebApiAutomation.TestDataCollection
{
    internal sealed class TestDataCollector
    {
        private const string _nullTestFailedExceptionMessage = "Exception cannot be null and Exception.Message cannot be empty or whitespace when logging failed test data";
        private const string _testRunChangedMessage = "Cannot change test run id in the middle of a test run";
        private Guid _testRunId;

        private int _totalTestCount = 0;
        private int _totalTestsFailedCount = 0;
        private int _totalTestsPassedCount = 0;

        private List<string> _failedTestsListing;
        private List<string> _passedTestsListing;

        internal TestDataCollector()
        {
            TestExecutor.TestBegun += TestBegun;
            TestExecutor.TestComplete += TestComplete;

            _failedTestsListing = new List<string>();
            _passedTestsListing = new List<string>();
        }

        internal List<string> GetRunSummary()
        {
            var runSummary = new List<string>();

            runSummary.Add($"Summary For Test Run Id : {_testRunId}");
            runSummary.Add($"Execution Date: {DateTime.Today}");
            runSummary.Add("");
            runSummary.Add("");
            runSummary.Add($"{_totalTestCount} tests were run");
            runSummary.Add($"{_totalTestsPassedCount} tests passed");
            runSummary.Add($"{_totalTestsFailedCount} tests failed");
            runSummary.Add("");
            runSummary.Add("");
            runSummary.Add($"Passed test methods");
            foreach(string passed in _passedTestsListing)
            {
                runSummary.Add(passed);
            }
            runSummary.Add("");
            runSummary.Add("");
            runSummary.Add($"Failed test methods");
            foreach (string failed in _failedTestsListing)
            {
                runSummary.Add(failed);
            }

            return runSummary;
        }

        private void TestComplete(Guid runId, string testMethodName, bool isSuccess, Exception error = null)
        {
            try
            {
                //Verify test run id matches the id of the current run. Throw exception if it does not.
                if (_testRunId.Equals(runId))
                {
                    //We should not generate failure data if no message is passed
                    if (!isSuccess &&
                        (error == null || string.IsNullOrEmpty(error.Message) || string.IsNullOrWhiteSpace(error.Message)))
                    {
                        throw new TestDataCollectorException(_nullTestFailedExceptionMessage);
                    }
                    else if (!isSuccess)
                    {
                        _totalTestsFailedCount++;
                        _failedTestsListing.Add($"Test Method {testMethodName} Failed with the following Message {error}");
                    }
                    else if (isSuccess)
                    {
                        _totalTestsPassedCount++;
                        _passedTestsListing.Add(testMethodName);
                    }
                }
                else
                {
                    throw new TestExecutorException(_testRunChangedMessage);
                }
            }
            catch(Exception ex)
            {
                throw new TestDataCollectorException("Test Complete Call Failed", ex);
            }
        }

        private void TestBegun(Guid runId)
        {
            try
            {
                //Assign the run id if this is the first test being executed
                if (_testRunId == null)
                    _testRunId = runId;

                //Verify test run id matches the id of the current run. Throw exception if it does not.
                if (_testRunId.Equals(runId))
                {
                    //Increment the total test count every time this event is raised
                    _totalTestCount++;
                }
                else
                {
                    throw new TestExecutorException(_testRunChangedMessage);
                }
            }
            catch (Exception ex)
            {
                throw new TestDataCollectorException("Test Begun Call Failed", ex);
            }
        }
    }
}
