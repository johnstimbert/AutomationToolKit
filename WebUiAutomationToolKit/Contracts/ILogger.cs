using static WebUiAutomationToolKit.WebUiAutomationEnums;

namespace WebUiAutomationToolKit
{
    /// <summary>
    /// Method definition for objects implementing the ILogger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Called to log the desired message
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        void Log(LogMessageType level, string message);
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
        string GetCurrentFailureLogFileName();
    }
}