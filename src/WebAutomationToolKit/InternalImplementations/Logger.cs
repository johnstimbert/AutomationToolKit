using System.Xml.Serialization;
using WebAutomationToolKit.Exceptions;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.InternalImplementations
{
    /// <summary>
    /// Pure orchestration layer for logging.
    /// Formats messages, manages per-test state via <see cref="LogContext"/>, and
    /// delegates all file I/O to an injected <see cref="ILogWriter"/>.
    /// This class has zero file-system knowledge.
    /// </summary>
    internal sealed class Logger : ILogger
    {
        private readonly ILogWriter _writer;
        private readonly LogContext _defaultContext;
        private readonly bool _generateFailureLog;

        /// <summary>
        /// Production constructor. Validates settings, then creates a <see cref="FileLogWriter"/>.
        /// </summary>
        internal Logger(LoggerSettings loggerSettings)
        {
            if (loggerSettings.LogFileName is null)
                throw new LoggerException("Log File Name Must Be Deinfed");
            if (loggerSettings.LogFilePath is null)
                throw new LoggerException("Log File Path Must Be Deinfed");

            _generateFailureLog = loggerSettings.GenerateFailureLog;
            _writer = new FileLogWriter(loggerSettings);
            _defaultContext = new LogContext("__global__", "N/A");
        }

        /// <summary>
        /// Testability constructor. Accepts any <see cref="ILogWriter"/> implementation,
        /// enabling unit tests to use an in-memory writer without touching the file system.
        /// </summary>
        internal Logger(ILogWriter writer, bool generateFailureLog = true)
        {
            _writer = writer;
            _generateFailureLog = generateFailureLog;
            _defaultContext = new LogContext("__global__", "N/A");
        }

        #region public methods

        /// <summary>
        /// Returns an xml respresentation of the proivided object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string ToXML(object obj)
        {
            try
            {
                var stringwriter = new StringWriter();
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stringwriter, obj);
                return stringwriter.ToString();
            }
            catch (Exception ex)
            {
                throw new LoggerException($"An exception was thrown while serializing an object to XML", ex);
            }
        }

        /// <summary>
        /// Called to log the desired message using the shared default context.
        /// Not safe for concurrent use; use <see cref="Log(LogMessageType, string, LogContext)"/>
        /// when parallel test executions share this logger instance.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogMessageType level, string message)
            => Log(level, message, _defaultContext);

        /// <summary>
        /// Called to log the desired message within the supplied <paramref name="context"/>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="context">Per-test context that isolates in-test state and history.</param>
        public void Log(LogMessageType level, string message, LogContext context)
        {
            try
            {
                switch (level)
                {
                    case LogMessageType.TESTINFO:

                        if (!context.InTest)
                        {
                            context.TestLogHistory.Clear();
                            string startEntry = FormatEntry(level, $"**** STARTING TEST ****: {message}");
                            context.TestLogHistory.AppendLine(startEntry);
                            _writer.WriteEntry(startEntry);
                            context.InTest = true;
                        }
                        else
                        {
                            string infoEntry = FormatEntry(level, $"Test Execution Info: {message}");
                            context.TestLogHistory.AppendLine(infoEntry);
                            _writer.WriteEntry(infoEntry);
                        }

                        break;

                    case LogMessageType.TESTFAILED:

                        string failEntry = FormatEntry(level, $"****** TEST FAILURE ****** {message}");
                        context.TestLogHistory.AppendLine(failEntry);
                        _writer.WriteEntry(failEntry);

                        if (_generateFailureLog)
                            _writer.WriteFailureEntry(context.TestLogHistory.ToString());

                        context.InTest = false;
                        context.TestLogHistory.Clear();
                        _writer.Flush();

                        break;

                    case LogMessageType.TESTPASSED:

                        _writer.WriteEntry(FormatEntry(level, $"****** TEST PASSED ****** {message}"));

                        context.InTest = false;
                        context.TestLogHistory.Clear();
                        _writer.Flush();

                        break;
                }
            }
            catch (Exception ex)
            {
                throw new LoggerException($"An Execption was thrown while attempting to log the message {message} " +
                    $"with level {level}", ex);
            }
        }

        /// <summary>
        /// Returns the name of the log file currently in use
        /// </summary>
        /// <returns>string </returns>
        public string GetCurrentLogFileName()
            => _writer.GetLogFileName();

        /// <summary>
        /// Returns the name of the failure log file currently in use
        /// </summary>
        /// <returns>string </returns>
        public string? GetCurrentFailureLogFileName()
            => _writer.GetFailureLogFileName();

        /// <summary>
        /// Flushes all buffered log entries to the underlying file.
        /// Call this before examining log file contents or at the end of a test run.
        /// </summary>
        public void Flush()
            => _writer.Flush();

        /// <summary>
        /// Releases the underlying <see cref="ILogWriter"/> and its file handles.
        /// </summary>
        public void Dispose()
            => _writer.Dispose();

        #endregion

        #region private methods

        private static string FormatEntry(LogMessageType level, string message)
            => $"{DateTime.Now:G} - Message Type:{level} - {message}";

        #endregion
    }
}
