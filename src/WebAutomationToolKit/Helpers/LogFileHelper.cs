namespace WebAutomationToolKit.Helpers
{
    internal sealed class LogFileHelper
    {
        private readonly string _logFilePath;
        private readonly string _logFileName;

        private const string _logVersionStringConstant = "_logNum";

        internal LogFileHelper(string logFilePath, string logFileName)
        {
            _logFilePath = logFilePath;
            _logFileName = logFileName;
        }

        internal void ClearLogFiles(string logFileName)
        {
            if(Directory.Exists(_logFilePath))
            {
                var logFiles = Directory.GetFiles(_logFilePath);
                foreach(var logFile in logFiles)
                {
                    if (logFile.Contains(logFileName))
                        File.Delete(logFile);
                }
            }
        }

        internal void IncrementLogFiles(int numberOfLogFilesToPreserve)
        {
            var existingLogFiles = GetSortedLogFilesAndRemoveOldFiles(numberOfLogFilesToPreserve);

            for(int i = existingLogFiles.Count - 1; i >= 0; i--)
            {
                int logNum = GetlogNumFromFileName(existingLogFiles[i].Name);

                if (logNum == 0)
                {
                    existingLogFiles[i].CopyTo($"{existingLogFiles[i].FullName}{_logVersionStringConstant}1", false);
                    existingLogFiles[i].Delete();
                }
                else
                {
                    var filePath = existingLogFiles[i].FullName.Substring(0, existingLogFiles[i].FullName.Length - existingLogFiles[i].FullName.LastIndexOf("\\"));
                    existingLogFiles[i].CopyTo($"{filePath}\\{_logVersionStringConstant}{logNum+1}", false);
                    existingLogFiles[i].Delete();
                }                
            }
        }

        internal string AppendDateToLogFile(string fileName, string dateFormatProperty)
        {
            var textToAppend = $"_{DateTime.Now.ToString(dateFormatProperty)}";

            var extension = Path.GetExtension(fileName);
            
            if(string.IsNullOrEmpty(extension))
            {
                fileName += $"{textToAppend}.txt";
            }
            else
            {
                fileName = fileName.Replace(extension, $"{textToAppend}{extension}");
            }

            return fileName;
        }

        private List<FileInfo> GetSortedLogFilesAndRemoveOldFiles(int numberOfLogFilesToPreserve)
        {
            // If the directory does not exist there are no log files to get, return an empty list
            if (!Directory.Exists(_logFilePath))
                return new List<FileInfo>();
            //Get all the log files where the name contains the log file name.
            var unsortedFiles = new DirectoryInfo(_logFilePath).GetFiles().AsEnumerable().Where(x => x.Name.ToLower().Contains(_logFileName.ToLower())).ToList();
            //Sort all the files found by the creation date and take the amount specified in the numberOfLogFilesToPreserve paramater
            var sortedfiles = unsortedFiles.OrderBy(file => file.CreationTime).ToList().Take(numberOfLogFilesToPreserve).ToList();

            //Remove all the files not found in the sortedFiles list
            foreach (var file in unsortedFiles)
            {
                if (!sortedfiles.Contains(file) && !IsFileLocked(file))
                    file.Delete();
            }

            return sortedfiles;
        }

        private int GetlogNumFromFileName(string name)
        {
            int logNum = 0;

            var logNumString = name.Substring(name.IndexOf(_logVersionStringConstant) + 6);

            if (int.TryParse(logNumString, out int nope))
                logNum = Convert.ToInt32(logNumString) + 1;

            return logNum;
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
