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
        private readonly string _logFileName;
        private readonly string _logFilePath;
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
                    var logFileName = _logFileName;

                    if (_appendDateToLogFile)
                        logFileName = _fileHelper.AppendDateToLogFile(logFileName, _dateFormatProperty);

                    ModifyExistingLogsBasedOnConfigs(_numberOfLogFilesToPreserve, logFileName);
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
                            WriteEntryToExecutionLog(level.ToString(), stackTrace.GetFrame(1).GetMethod().Name, $"**** STARTING TEST ****: {message}", true);
                            _inTest = true;
                        }
                        else
                        {
                            WriteEntryToExecutionLog(level.ToString(), stackTrace.GetFrame(1).GetMethod().Name, $"Test Execution Info: {message}", true);
                            _inTest = true;
                        }

                        break;
                    case LogMessageType.TESTFAILED:

                        WriteEntryToExecutionLog(level.ToString(), stackTrace.GetFrame(1).GetMethod().Name, $"****** TEST FAILURE ****** {message}", true);

                        if (_generateFailureLog)
                            WriteEntryToFailureLog(level.ToString(), _testLogHistory.ToString());

                        _inTest = false;
                        _testLogHistory.Clear();

                        break;
                    case LogMessageType.TESTPASSED:

                        WriteEntryToExecutionLog(level.ToString(), stackTrace.GetFrame(1).GetMethod().Name, $"****** TEST PASSED ****** {message}");

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
        
        #endregion

        #region private methods
        private void WriteEntryToExecutionLog(string level, string testMethodName, string message, bool addToTestHistory = false)
        {
            string fileName = Path.Combine(_logFilePath, _logFileName);
            if (_appendDateToLogFile)
                fileName = _fileHelper.AppendDateToLogFile(fileName, _dateFormatProperty);

            string logEntry = $"{DateTime.Now.ToString("G")} - Message Type:{level} Test Method:{testMethodName} - {message}";

            if (addToTestHistory)
                _testLogHistory.AppendLine(logEntry);

            //Append new text to an existing file 
            using (StreamWriter file = new StreamWriter(fileName, true))
            {
                file.WriteLine(logEntry);
            }
        }

        private void WriteEntryToFailureLog(string level, string message)
        {
            string fileName = Path.Combine(_logFilePath, $"{_logFileName}_Failures");
            if (_appendDateToLogFile)
                fileName = _fileHelper.AppendDateToLogFile(fileName, _dateFormatProperty);

            //Append new text to an existing file 
            using (StreamWriter file = new StreamWriter(fileName, true))
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
