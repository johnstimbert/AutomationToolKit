using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestLoggingAndDataFormatter.Tests
{
    [TestClass]
    public class LoggerTests
    {
        private const string _logPath = @"c:\logger";
        private const string _logFileName = "logger.txt";

        private Logger _logger;


        [TestInitialize]
        public void BeforeEachTest()
        {
            _logger = new Logger(_logFileName, _logFileName);
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
