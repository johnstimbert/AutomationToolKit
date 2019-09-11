using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.Helpers;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation
{

    /// <summary>
    /// This class provides a common utility for formatting logging
    /// </summary>
    internal sealed class LoggerWebAutoInternal : ILogger
    {
        private string _logFileName;
        private string _failureLogFileName;
        private string _logFilePath;
        private string _dateFormatProperty;
        private bool _generateFailureLog;
        private bool _appendDateToLogFile;
        private int _numberOfLogFilesToPreserve;

        private LogFileHelper _fileHelper;
        private bool _inTest = false;
        private bool _newTestRun = true;
        StringBuilder _testLogHistory = new StringBuilder();
 
        private LoggerWebAutoInternal() { }

        internal LoggerWebAutoInternal(LoggerSettings loggerSettings)
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
                // Get call stack
                StackTrace stackTrace = new StackTrace();

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

                        break;
                    case LogMessageType.TESTPASSED:

                        WriteEntryToExecutionLog(level.ToString(), $"****** TEST PASSED ****** {message}");

                        _inTest = false;
                        _testLogHistory.Clear();

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
        public string GetCurrentFailureLogFileName()
        {
            return _failureLogFileName;
        }

        #endregion

        #region private methods
        private void WriteEntryToExecutionLog(string level, string message, bool addToTestHistory = false)
        {
            string fullefileName = Path.Combine(_logFilePath, _logFileName);

            string logEntry = $"{DateTime.Now.ToString("G")} - Message Type:{level} - {message}";

            if (addToTestHistory)
                _testLogHistory.AppendLine(logEntry);

            //Append new text to an existing file 
            using (StreamWriter file = new StreamWriter(fullefileName, true))
            {
                file.WriteLine(logEntry);
            }
        }

        private void WriteEntryToFailureLog(string level, string message)
        {
            _failureLogFileName = Path.Combine(_logFilePath, $"{_logFileName}_Failures");

            //Append new text to an existing file 
            using (StreamWriter file = new StreamWriter(_failureLogFileName, true))
            {
                file.WriteLine(message);
            }
        }

        private string ValidateDateFormatValueBeingSet(string value)
        {
            if (DateTime.TryParse(value, out DateTime expectedDate))
            {
                return value;
            }

            throw new LoggerException($"The dateTime format {value} is not valid. " +
                $"Please reference the folowing article for valid values https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings");
        }

        private void ModifyExistingLogsBasedOnConfigs(int numberOfLogFilesToPreserve, string logFileName)
        {
            var logHelper = new LogFileHelper(_logFilePath, _logFileName);
            if(numberOfLogFilesToPreserve > 0)
            {
                logHelper.IncrementLogFiles(numberOfLogFilesToPreserve);
            }
            else
            {
                logHelper.ClearLogFiles(logFileName);
            }
        }

        #endregion
    }
}
