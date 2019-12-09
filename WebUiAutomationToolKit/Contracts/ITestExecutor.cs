using System;

namespace WebUiAutomationToolKit
{
    /// <summary>
    /// Interface that defines methods for TestExecutor objects
    /// </summary>
    public interface ITestExecutor
    {
        /// <summary>
        /// Executes the test provided in the Action delegate.
        /// </summary>
        /// <param name="testMethod"></param>
        /// <param name="webDriverManager"></param>
        void Execute(Action testMethod, IWebDriverManager webDriverManager);
        /// <summary>
        /// Returns formatted test results that can be written to a log file or elsewhere
        /// </summary>
        /// <returns>Formatted test results</returns>
        string GetTestRunSummary();
    }
}
