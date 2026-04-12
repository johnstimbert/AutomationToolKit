using WebAutomationToolKit.Helpers;

namespace WebAutomationToolKit.InternalImplementations
{
    /// <summary>
    /// File-system implementation of <see cref="ILogWriter"/>.
    /// Owns all <see cref="StreamWriter"/> lifecycle, log-file rotation, and the single
    /// write-lock that serialises concurrent calls from parallel test executions.
    /// </summary>
    internal sealed class FileLogWriter : ILogWriter
    {
        private readonly object _lock = new();

        private readonly string _logFilePath;
        private readonly string _dateFormatProperty;
        private readonly bool _appendDateToLogFile;
        private readonly int _numberOfLogFilesToPreserve;

        private string _logFileName;
        private string? _failureLogFileName;
        private bool _newRunSetup = true;

        private StreamWriter? _executionLogWriter;
        private StreamWriter? _failureLogWriter;

        private readonly LogFileHelper _fileHelper;

        /// <summary>
        /// Initialises a new <see cref="FileLogWriter"/> from the supplied settings.
        /// Precondition: <see cref="LoggerSettings.LogFileName"/> and
        /// <see cref="LoggerSettings.LogFilePath"/> have already been validated non-null
        /// by <see cref="Logger"/> before this constructor is called.
        /// </summary>
        internal FileLogWriter(LoggerSettings settings)
        {
            _logFilePath = settings.LogFilePath!;
            _logFileName = settings.LogFileName!;
            _dateFormatProperty = settings.DateFormat;
            _appendDateToLogFile = settings.AppendDateToLogFile;
            _numberOfLogFilesToPreserve = settings.NumberOfLogFilesToPreserve;

            _fileHelper = new LogFileHelper(_logFilePath, _logFileName);

            if (!Directory.Exists(_logFilePath))
                Directory.CreateDirectory(_logFilePath);
        }

        /// <inheritdoc/>
        public void WriteEntry(string formattedEntry)
        {
            lock (_lock)
            {
                EnsureNewRunSetup();

                string fullPath = Path.Combine(_logFilePath, _logFileName);
                _executionLogWriter ??= new StreamWriter(fullPath, append: true) { AutoFlush = false };
                _executionLogWriter.WriteLine(formattedEntry);
            }
        }

        /// <inheritdoc/>
        public void WriteFailureEntry(string formattedContent)
        {
            lock (_lock)
            {
                _failureLogFileName = Path.Combine(_logFilePath, $"{_logFileName}_Failures");
                _failureLogWriter ??= new StreamWriter(_failureLogFileName, append: true) { AutoFlush = false };
                _failureLogWriter.WriteLine(formattedContent);
            }
        }

        /// <inheritdoc/>
        public void Flush()
        {
            lock (_lock)
            {
                _executionLogWriter?.Flush();
                _failureLogWriter?.Flush();
            }
        }

        /// <inheritdoc/>
        public string GetLogFileName()
        {
            lock (_lock)
                return _logFileName;
        }

        /// <inheritdoc/>
        public string? GetFailureLogFileName()
        {
            lock (_lock)
                return _failureLogFileName;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lock)
            {
                _executionLogWriter?.Dispose();
                _executionLogWriter = null;
                _failureLogWriter?.Dispose();
                _failureLogWriter = null;
            }
        }

        // ── private helpers ───────────────────────────────────────────────────────

        /// <summary>
        /// Performs one-time run setup: closes any open writer so the file is released
        /// before rotation/deletion attempts, applies the date suffix if configured, then
        /// delegates to <see cref="LogFileHelper"/> for rotation or clearing.
        /// Must be called while <see cref="_lock"/> is held.
        /// </summary>
        private void EnsureNewRunSetup()
        {
            if (!_newRunSetup)
                return;

            // Release the handle before LogFileHelper attempts to rename or delete the file.
            _executionLogWriter?.Dispose();
            _executionLogWriter = null;

            if (_appendDateToLogFile)
                _logFileName = _fileHelper.AppendDateToLogFile(_logFileName, _dateFormatProperty);

            if (_numberOfLogFilesToPreserve > 0)
                _fileHelper.IncrementLogFiles(_numberOfLogFilesToPreserve);
            else
                _fileHelper.ClearLogFiles(_logFileName);

            _newRunSetup = false;
        }
    }
}
