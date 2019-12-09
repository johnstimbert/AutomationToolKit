using System;
using System.IO;

namespace WebUiAutomationToolKit.Tests
{
    public static class LoggerTestHelpers
    {
        public static string AppendDateToLogFile(string fileName, string dateFormatProperty)
        {
            var textToAppend = $"_{DateTime.Now.ToString(dateFormatProperty)}";

            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
            {
                fileName += $"{textToAppend}.txt";
            }
            else
            {
                fileName = fileName.Replace(extension, $"{textToAppend}{extension}");
            }

            return fileName;
        }
    }
}
