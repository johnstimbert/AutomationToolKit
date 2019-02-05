using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation
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
    }
}