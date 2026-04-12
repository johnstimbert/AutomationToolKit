using System.Text;
using System.Xml.Serialization;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.Helpers;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.InternalImplementations
{

    /// <summary>
    /// This class provides a common utility for formatting logging
    /// </summary>
    internal sealed class Logger : ILogger
    {
        private string _logFileName;
        private string? _failureLogFileName;
        private string _logFilePath;
        private string _dateFormatProperty;
        private bool _generateFailureLog;
        private bool _appendDateToLogFile;
        private int _numberOfLogFilesToPreserve;

        private LogFileHelper _fileHelper;
        private bool _inTest = false;
        private bool _newTestRun = true;
        StringBuilder _testLogHistory = new StringBuilder();

        private StreamWriter? _executionLogWriter;
        private StreamWriter? _failureLogWriter;

        internal Logger(LoggerSettings loggerSettings)
        {
            _logFileName = loggerSettings.LogFileName ?? throw new LoggerException($"Log File Name Must Be Deinfed");
            _logFilePath = loggerSettings.LogFilePath ?? throw new LoggerException($"Log File Path Must Be Deinfed");

            _dateFormatProperty = loggerSettings.DateFormat;
            _generateFailureLog = loggerSettings.GenerateFailureLog;
            _appendDateToLogFile = loggerSettings.AppendDateToLogFile;
            _numberOfLogFilesToPreserve = loggerSettings.NumberOfLogFilesToPreserve;

            _fileHelper = new LogFileHelper(_logFilePath, _logFileName);

            if (!Directory.Exists(_logFilePath))
                Directory.CreateDirectory(_logFilePath);
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
            catch(Exception ex)
            {
                throw new LoggerException($"An exception was thrown while serializing an object to XML", ex);
            }
        }

        /// <summary>
        /// Called to log the desired message
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogMessageType level, string message)
        {
            try
            {
                if (_newTestRun)
                {
                    if (_appendDateToLogFile)
                        _logFileName = _fileHelper.AppendDateToLogFile(_logFileName, _dateFormatProperty);

                    ModifyExistingLogsBasedOnConfigs(_numberOfLogFilesToPreserve, _logFileName);

                    _newTestRun = false;
                }

                //Switch case for LogLevel message type
                switch (level)
                {
                    case LogMessageType.TESTINFO:

                        if (!_inTest)
                        {
                            _testLogHistory.Clear();
                            WriteEntryToExecutionLog(level.ToString(), $"**** STARTING TEST ****: {message}", true);
                            _inTest = true;
                        }
                        else
                        {
                            WriteEntryToExecutionLog(level.ToString(), $"Test Execution Info: {message}", true);
                            _inTest = true;
                        }

                        break;
                    case LogMessageType.TESTFAILED:

                        WriteEntryToExecutionLog(level.ToString(), $"****** TEST FAILURE ****** {message}", true);

                        if (_generateFailureLog)
                            WriteEntryToFailureLog(level.ToString(), _testLogHistory.ToString());

                        _inTest = false;
                        _testLogHistory.Clear();
                        Flush();

                        break;
                    case LogMessageType.TESTPASSED:

                        WriteEntryToExecutionLog(level.ToString(), $"****** TEST PASSED ****** {message}");

                        _inTest = false;
                        _testLogHistory.Clear();
                        Flush();

                        break;
                }
            }
            catch(Exception ex)
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
        {
            return _logFileName;
        }

        /// <summary>
        /// Returns the name of the failure log file currently in use
        /// </summary>
        /// <returns>string </returns>
        public string? GetCurrentFailureLogFileName()
        {
            return _failureLogFileName;
        }

        /// <summary>
        /// Flushes all buffered log entries to the underlying file.
        /// Call this before examining log file contents or at the end of a test run.
        /// </summary>
        public void Flush()
        {
            _executionLogWriter?.Flush();
            _failureLogWriter?.Flush();
        }

        #endregion

        #region private methods
        private void WriteEntryToExecutionLog(string level, string message, bool addToTestHistory = false)
        {
            string fullFileName = Path.Combine(_logFilePath, _logFileName);

            string logEntry = $"{DateTime.Now.ToString("G")} - Message Type:{level} - {message}";

            if (addToTestHistory)
                _testLogHistory.AppendLine(logEntry);

            _executionLogWriter ??= new StreamWriter(fullFileName, true) { AutoFlush = false };
            _executionLogWriter.WriteLine(logEntry);
        }

        private void WriteEntryToFailureLog(string level, string message)
        {
            _failureLogFileName = Path.Combine(_logFilePath, $"{_logFileName}_Failures");

            _failureLogWriter ??= new StreamWriter(_failureLogFileName, true) { AutoFlush = false };
            _failureLogWriter.WriteLine(message);
        }

        private void ModifyExistingLogsBasedOnConfigs(int numberOfLogFilesToPreserve, string logFileName)
        {
            if (numberOfLogFilesToPreserve > 0)
            {
                _fileHelper.IncrementLogFiles(numberOfLogFilesToPreserve);
            }
            else
            {
                _fileHelper.ClearLogFiles(logFileName);
            }
        }

        #endregion
    }
}
