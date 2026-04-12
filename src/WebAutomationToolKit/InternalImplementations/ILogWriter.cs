namespace WebAutomationToolKit.InternalImplementations
{
    /// <summary>
    /// Abstracts the persistence mechanism used by <see cref="Logger"/>.
    /// Implementations are responsible for thread-safe file I/O, StreamWriter lifecycle,
    /// and log-file rotation. <see cref="Logger"/> has zero file-system knowledge.
    /// </summary>
    internal interface ILogWriter : IDisposable
    {
        /// <summary>
        /// Writes a single pre-formatted entry to the execution log.
        /// Callers must not hold any external locks when calling this method.
        /// </summary>
        void WriteEntry(string formattedEntry);

        /// <summary>
        /// Appends <paramref name="formattedContent"/> to the failure log.
        /// Only called when failure-log generation is enabled.
        /// </summary>
        void WriteFailureEntry(string formattedContent);

        /// <summary>Flushes all buffered entries to the underlying storage.</summary>
        void Flush();

        /// <summary>Returns the resolved name of the execution log file currently in use.</summary>
        string GetLogFileName();

        /// <summary>
        /// Returns the name of the failure log file, or <see langword="null"/> if no failure
        /// entry has been written yet.
        /// </summary>
        string? GetFailureLogFileName();
    }
}
