using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAutomationToolKit.Helpers;

namespace WebAutomationToolKit.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class LogFileHelperTests
    {
        private string _tempDir = null!;

        [TestInitialize]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "WatLogTests_" + Guid.NewGuid());
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void Teardown()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }

        [TestMethod]
        public void AppendDateToLogFile_NoExtension_AppendsDatAndTxtExtension()
        {
            var helper = new LogFileHelper(_tempDir, "log");
            var result = helper.AppendDateToLogFile("log", "MM_dd_yyyy");
            StringAssert.Contains(result, ".txt");
            StringAssert.Contains(result, "_");
        }

        [TestMethod]
        public void AppendDateToLogFile_WithExtension_InsertsDateBeforeExtension()
        {
            var helper = new LogFileHelper(_tempDir, "log.txt");
            var result = helper.AppendDateToLogFile("log.txt", "MM_dd_yyyy");
            Assert.IsTrue(result.EndsWith(".txt"));
            Assert.IsTrue(result.Contains("_"));
        }

        [TestMethod]
        public void ClearLogFiles_DeletesMatchingFilesOnly()
        {
            var fileName = "test.log";
            File.WriteAllText(Path.Combine(_tempDir, fileName), "data");
            File.WriteAllText(Path.Combine(_tempDir, "other.log"), "other");

            var helper = new LogFileHelper(_tempDir, fileName);
            helper.ClearLogFiles(fileName);

            Assert.IsFalse(File.Exists(Path.Combine(_tempDir, fileName)));
            Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "other.log")));
        }

        [TestMethod]
        public void ClearLogFiles_DirectoryMissing_DoesNotThrow()
        {
            var helper = new LogFileHelper(Path.Combine(_tempDir, "nonexistent"), "log.txt");
            helper.ClearLogFiles("log.txt"); // should not throw
        }

        [TestMethod]
        public void IncrementLogFiles_NoExistingFiles_DoesNotThrow()
        {
            var helper = new LogFileHelper(_tempDir, "log.txt");
            helper.IncrementLogFiles(3); // should not throw on empty directory
        }
    }
}
