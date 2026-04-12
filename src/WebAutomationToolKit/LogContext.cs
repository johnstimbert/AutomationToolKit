using System.Text;

namespace WebAutomationToolKit
{
    /// <summary>
    /// Identifies a single test execution for logging purposes.
    /// Create one instance per <see cref="ITestExecutor.Execute"/> call and pass it to the
    /// context-aware <see cref="ILogger.Log(WebAutomationEnums.LogMessageType, string, LogContext)"/>
    /// overload so that parallel test executions never share log-history state.
    /// </summary>
    public sealed class LogContext
    {
        /// <summary>
        /// Gets the unique identifier for this test execution (typically method name + browser).
        /// </summary>
        public string TestId { get; }

        /// <summary>
        /// Gets the name of the browser driver in use for this test execution.
        /// </summary>
        public string BrowserName { get; }

        /// <summary>
        /// Accumulates formatted log entries for the current test so they can be
        /// replayed into the failure log if the test fails.
        /// Internal — only <see cref="InternalImplementations.Logger"/> manipulates this.
        /// </summary>
        internal StringBuilder TestLogHistory { get; } = new();

        /// <summary>
        /// Tracks whether a test is currently in progress for this context.
        /// Internal — only <see cref="InternalImplementations.Logger"/> manipulates this.
        /// </summary>
        internal bool InTest { get; set; }

        /// <summary>
        /// Initialises a new <see cref="LogContext"/> for the specified test and browser.
        /// </summary>
        /// <param name="testId">
        /// A unique identifier for the test execution, e.g. <c>"MyTest.Chrome"</c>.
        /// </param>
        /// <param name="browserName">The name of the browser driver in use.</param>
        public LogContext(string testId, string browserName)
        {
            TestId = testId;
            BrowserName = browserName;
        }
    }
}
