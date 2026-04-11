using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAutomationToolKit.Exceptions;

namespace WebAutomationToolKit.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class ExceptionTests
    {
        // ── WebAutomationException ──────────────────────────────────────────

        [TestMethod]
        public void WebAutomationException_MessageConstructor_PreservesMessage()
        {
            var ex = new WebAutomationException("test message");
            Assert.AreEqual("test message", ex.Message);
            Assert.IsNull(ex.InnerException);
        }

        [TestMethod]
        public void WebAutomationException_InnerExceptionConstructor_PreservesBoth()
        {
            var inner = new InvalidOperationException("inner");
            var ex = new WebAutomationException("outer", inner);
            Assert.AreEqual("outer", ex.Message);
            Assert.AreSame(inner, ex.InnerException);
        }

        [TestMethod]
        public void WebAutomationException_IsException()
        {
            var ex = new WebAutomationException("msg");
            Assert.IsInstanceOfType(ex, typeof(Exception));
        }

        // ── LoggerException ─────────────────────────────────────────────────

        [TestMethod]
        public void LoggerException_MessageConstructor_PreservesMessage()
        {
            var ex = new LoggerException("logger error");
            Assert.AreEqual("logger error", ex.Message);
            Assert.IsNull(ex.InnerException);
        }

        [TestMethod]
        public void LoggerException_InnerExceptionConstructor_PreservesBoth()
        {
            var inner = new IOException("disk full");
            var ex = new LoggerException("wrap", inner);
            Assert.AreEqual("wrap", ex.Message);
            Assert.AreSame(inner, ex.InnerException);
        }

        // ── TestExecutorException ───────────────────────────────────────────

        [TestMethod]
        public void TestExecutorException_MessageConstructor_PreservesMessage()
        {
            var ex = new TestExecutorException("executor error");
            Assert.AreEqual("executor error", ex.Message);
        }

        [TestMethod]
        public void TestExecutorException_InnerExceptionConstructor_PreservesBoth()
        {
            var inner = new ArgumentNullException("param");
            var ex = new TestExecutorException("outer", inner);
            Assert.AreEqual("outer", ex.Message);
            Assert.AreSame(inner, ex.InnerException);
        }

        // ── TestDataCollectorException ──────────────────────────────────────

        [TestMethod]
        public void TestDataCollectorException_MessageConstructor_PreservesMessage()
        {
            var ex = new TestDataCollectorException("collector error");
            Assert.AreEqual("collector error", ex.Message);
        }

        [TestMethod]
        public void TestDataCollectorException_InnerExceptionConstructor_PreservesBoth()
        {
            var inner = new OverflowException("too many");
            var ex = new TestDataCollectorException("wrap", inner);
            Assert.AreEqual("wrap", ex.Message);
            Assert.AreSame(inner, ex.InnerException);
        }
    }
}
