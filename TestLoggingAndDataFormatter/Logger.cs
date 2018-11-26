using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using TestLoggingAndDataFormatter.Exceptions;

namespace TestLoggingAndDataFormatter
{
    /// <summary>
    /// Defines the type of message being logged
    /// </summary>
    public enum LogLevel
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        TESTSTART,
        TESTINFO,
        TESTPASSED,
        TESTFAILED
        #pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// This class provides a common utility for formatting logging
    /// </summary>
    public sealed class Logger
    {
        private readonly string _logFileName;
        private readonly string _logFilePath;
        private string _dateFormatProperty = "MM_dd_yyyy";
 
        private Logger() { }

        /// <summary>
        /// Only valid constructor
        /// </summary>
        /// <param name="LogFileName">The name of the file to log to.  It will be created if it does not exist.</param>
        /// <param name="LogFilePath">The path in which to wrote the log file. It will be created if it does not exist.</param>
        public Logger(string LogFileName, string LogFilePath)
        {
            _logFileName = LogFileName;
            _logFilePath = LogFilePath;
        }

        #region Public Properties
        /// <summary>
        /// Defines the date format to be used, must be a format that would be accepted by the DateTime.Now.ToString("G") method
        /// The default is "MM_dd_yyyy".
        /// </summary>
        public string DateFormatProperty {
            get { return _dateFormatProperty; }
            set { _dateFormatProperty = ValidateDateFormatValueBeingSet(value); }
        }
        /// <summary>
        /// If set to true, a file will be created containing the entries for a failed test
        /// </summary>
        public bool GenerateFailureLog = true;
        /// <summary>
        /// Appends the current date to the log file being written based on the DateFormatProperty.
        /// The default value is true.
        /// </summary>
        public bool AppendDateToLogFile = true;
        /// <summary>
        /// If true, the number of log files preserved are based on the NumberOfLogFilesToPreserve property.
        /// If false, any existing files will be deleted.
        /// The default is true.
        /// </summary>
        public bool PreservePreviousLogFiles = true;
        /// <summary>
        /// Defiens the number of log files to preserve.
        /// The default is 10.
        /// </summary>
        public int NumberOfLogFilesToPreserve = 10;

        #endregion

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
        public void Log(LogLevel level, string message)
        {
            try
            {
                // Get call stack
                StackTrace stackTrace = new StackTrace();

                //Switch case for LogLevel message type
                switch (level)
                {
                    case LogLevel.TESTSTART:

                        WriteEntryToExecutionLog("***Starting Test****", stackTrace.GetFrame(1).GetMethod().Name, message);

                        break;
                    case LogLevel.TESTINFO:

                        WriteEntryToExecutionLog("Test Execution Info", stackTrace.GetFrame(1).GetMethod().Name, message);

                        break;
                    case LogLevel.TESTFAILED:

                        WriteEntryToExecutionLog("******TEST FAILURE******", stackTrace.GetFrame(1).GetMethod().Name, message);

                        break;

                    case LogLevel.TESTPASSED:

                        WriteEntryToExecutionLog("******TEST FAILURE******", stackTrace.GetFrame(1).GetMethod().Name, message);

                        if(GenerateFailureLog)
                            WriteEntryToFailureLog("******TEST FAILURE******", stackTrace.GetFrame(1).GetMethod().Name, message);

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
        private void WriteEntryToExecutionLog(string level, string testMethodName, string message)
        {
            //Append new text to an existing file 
            using (StreamWriter file = new StreamWriter(Path.Combine(_logFilePath, _logFileName), true))
            {
                file.WriteLine($"{DateTime.Now.ToString("G")} - Message Type:{level} Test Method:{testMethodName} - {message}");
            }
        }

        private void WriteEntryToFailureLog(string level, string testMethodName, string message)
        {
            //Append new text to an existing file 
            using (StreamWriter file = new StreamWriter(Path.Combine(_logFilePath, $"{_logFileName}_Failures" ), true))
            {
                file.WriteLine($"{DateTime.Now.ToString("G")} - Message Type:{level} Test Method:{testMethodName} - {message}");
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

        #endregion
    }
}
