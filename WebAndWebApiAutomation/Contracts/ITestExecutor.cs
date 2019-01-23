using System;

namespace WebAndWebApiAutomation
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
    }
}
