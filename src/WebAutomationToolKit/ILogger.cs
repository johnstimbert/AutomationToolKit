using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit
{
    /// <summary>
    /// Method definition for objects implementing the ILogger interface
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// Called to log the desired message using the shared default context.
        /// For parallel-safe, per-test history isolation use the
        /// <see cref="Log(LogMessageType, string, LogContext)"/> overload instead.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        void Log(LogMessageType level, string message);

        /// <summary>
        /// Called to log the desired message within the supplied <paramref name="context"/>.
        /// Each <see cref="LogContext"/> tracks its own in-test state and history so that
        /// parallel test executions never cross-contaminate the failure log.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="context">
        /// The <see cref="LogContext"/> created by the caller for this test execution.
        /// </param>
        void Log(LogMessageType level, string message, LogContext context);

        /// <summary>
        /// Returns an xml respresentation of the proivided object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string ToXML(object obj);

        /// <summary>
        /// Returns the name of the log file currently in use
        /// </summary>
        /// <returns>string </returns>
        string GetCurrentLogFileName();

        /// <summary>
        /// Returns the name of the failure log file currently in use
        /// </summary>
        /// <returns>string </returns>
        string? GetCurrentFailureLogFileName();

        /// <summary>
        /// Flushes all buffered log entries to the underlying file.
        /// Call this before examining log file contents or at the end of a test run.
        /// </summary>
        void Flush();
    }
}