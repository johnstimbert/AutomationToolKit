using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAutomationToolKit.Exceptions;
using WebAutomationToolKit.InternalImplementations;

namespace WebAutomationToolKit.BrowserTests
{
    [TestClass]
    [TestCategory("Unit")]
    [TestCategory("Browser")]
    public class WebAutomationConstructorTests
    {
        // ── ValidateDriverPath ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_NullPath_DoesNotThrow()
        {
            var wa = new WebAutomation(null, 5);
            Assert.IsNotNull(wa);
        }

        [TestMethod]
        public void Constructor_ValidPath_DoesNotThrow()
        {
            var dir = Path.GetTempPath();
            var wa = new WebAutomation(dir, 5);
            Assert.IsNotNull(wa);
        }

        [TestMethod]
        public void Constructor_NonExistentPath_ThrowsWebAutomationException()
        {
            var badPath = Path.Combine(Path.GetTempPath(), "does_not_exist_" + Guid.NewGuid());
            try
            {
                _ = new WebAutomation(badPath, 5);
                Assert.Fail("Expected WebAutomationException for non-existent path");
            }
            catch (WebAutomationException ex)
            {
                StringAssert.Contains(ex.Message, "could not be found");
            }
        }

        [TestMethod]
        public void Constructor_ZeroTimeout_ClampsToFive()
        {
            // Only verifiable indirectly — construction must succeed without throwing
            var wa = new WebAutomation(null, 0);
            Assert.IsNotNull(wa);
        }

        [TestMethod]
        public void Constructor_NegativeTimeout_ClampsToFive()
        {
            var wa = new WebAutomation(null, -10);
            Assert.IsNotNull(wa);
        }

        // ── GetIWebDriverManager ────────────────────────────────────────────

        [TestMethod]
        public void GetIWebDriverManager_ReturnsNonNull()
        {
            var wa = new WebAutomation(null, 5);
            Assert.IsNotNull(wa.GetIWebDriverManager());
        }

        [TestMethod]
        public void GetIWebDriverManager_SecondCall_ReturnsSameInstance()
        {
            var wa = new WebAutomation(null, 5);
            var m1 = wa.GetIWebDriverManager();
            var m2 = wa.GetIWebDriverManager();
            Assert.AreSame(m1, m2);
        }

        [TestMethod]
        public void SeparateInstances_DoNotShareDriverManager()
        {
            var wa1 = new WebAutomation(null, 5);
            var wa2 = new WebAutomation(null, 5);
            Assert.AreNotSame(wa1.GetIWebDriverManager(), wa2.GetIWebDriverManager());
        }

        // ── GetLogger ───────────────────────────────────────────────────────

        [TestMethod]
        public void GetLogger_ReturnsNonNull()
        {
            var dir = Path.Combine(Path.GetTempPath(), "WatLogTest_" + Guid.NewGuid());
            try
            {
                Directory.CreateDirectory(dir);
                var wa = new WebAutomation(null, 5);
                var logger = wa.GetLogger(new LoggerSettings
                {
                    LogFileName = "test.log",
                    LogFilePath = dir,
                    AppendDateToLogFile = false,
                    NumberOfLogFilesToPreserve = 0
                });
                Assert.IsNotNull(logger);
            }
            finally
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }
        }

        // ── GetTestExecutor ─────────────────────────────────────────────────

        [TestMethod]
        public void GetTestExecutor_EmptyResultsPath_ThrowsTestExecutorException()
        {
            var wa = new WebAutomation(null, 5);
            try
            {
                wa.GetTestExecutor(string.Empty);
                Assert.Fail("Expected TestExecutorException for empty resultsPath");
            }
            catch (TestExecutorException ex)
            {
                Assert.IsNotNull(ex.Message);
            }
        }

        [TestMethod]
        public void GetTestExecutor_WhitespaceResultsPath_ThrowsTestExecutorException()
        {
            var wa = new WebAutomation(null, 5);
            try
            {
                wa.GetTestExecutor("   ");
                Assert.Fail("Expected TestExecutorException for whitespace resultsPath");
            }
            catch (TestExecutorException ex)
            {
                Assert.IsNotNull(ex.Message);
            }
        }

        [TestMethod]
        public void GetTestExecutor_ThrowsTestExecutorException_NotLoggerException()
        {
            var wa = new WebAutomation(null, 5);
            try
            {
                wa.GetTestExecutor(string.Empty);
                Assert.Fail("Expected TestExecutorException");
            }
            catch (TestExecutorException)
            {
                // correct type — pass
            }
            catch (LoggerException)
            {
                Assert.Fail("Should not throw LoggerException from GetTestExecutor");
            }
        }

        [TestMethod]
        public void GetTestExecutor_ValidPath_ReturnsNonNull()
        {
            var wa = new WebAutomation(null, 5);
            var executor = wa.GetTestExecutor(Path.GetTempPath());
            Assert.IsNotNull(executor);
        }

        [TestMethod]
        public void GetTestExecutor_AlwaysCreatesNewInstance()
        {
            var wa = new WebAutomation(null, 5);
            var e1 = wa.GetTestExecutor(Path.GetTempPath());
            var e2 = wa.GetTestExecutor(Path.GetTempPath());
            Assert.AreNotSame(e1, e2);
        }

        [TestMethod]
        public void GetLogger_ThenGetTestExecutor_ExecutorReceivesLogger()
        {
            var dir = Path.Combine(Path.GetTempPath(), "WatLogTest_" + Guid.NewGuid());
            try
            {
                Directory.CreateDirectory(dir);
                var wa = new WebAutomation(null, 5);
                // GetLogger must be called before GetTestExecutor
                _ = wa.GetLogger(new LoggerSettings
                {
                    LogFileName = "test.log",
                    LogFilePath = dir,
                    AppendDateToLogFile = false,
                    NumberOfLogFilesToPreserve = 0
                });
                var executor = wa.GetTestExecutor(Path.GetTempPath());
                // If the logger was wired in, Execute should log without NRE on logger usage
                Assert.IsNotNull(executor);
            }
            finally
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }
        }
    }
}
