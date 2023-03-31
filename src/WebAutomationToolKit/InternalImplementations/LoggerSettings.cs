namespace WebAutomationToolKit
{
    /// <summary>
    /// Defines the option settings for object implementing the ILogger interface
    /// </summary>
    public sealed class LoggerSettings
    {
        /// <summary>
        /// Defines the date format to be used, must be a format that would be accepted by the DateTime.Now.ToString("G") method
        /// The default is "MM_dd_yyyy".
        /// </summary>
        public string DateFormat { get; set; } = "MM_dd_yyyy";
        /// <summary>
        /// Defiens the number of log files to preserve.
        /// The default is 10.
        /// </summary>
        public int NumberOfLogFilesToPreserve { get; set; } = 10;
        /// <summary>
        /// If set to true, a file will be created containing the entries for a failed test
        /// The default value is true.
        /// </summary>
        public bool GenerateFailureLog { get; set; } = true;
        /// <summary>
        /// Appends the current date to the log file being written based on the DateFormatProperty.
        /// The default value is true.
        /// </summary>
        public bool AppendDateToLogFile { get; set; } = true;
        /// <summary>
        /// The name of the file to log to. The file will be created if it does not exist.
        /// </summary>
        public string LogFileName { get; set; }
        /// <summary>
        /// The path in which to write the log file. The path will be created if it does not exist.
        /// </summary>
        public string LogFilePath { get; set; }
    }
}
