using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebAndApiAutomation.Tests
{
    [TestClass]
    public class WebAutomationTests
    {
        private WebAutomation _webAutomation;
        private readonly string _navUrl = "https://www.google.com/";
        private readonly string _driverPath = @"D:\Projects\AutomationToolKit\WebAndApiAutomation\bin\Debug\netstandard2.0\";

        [TestInitialize]
        public void BeforeEachTest()
        {
            _webAutomation = new WebAutomation();
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            
        }

        [TestMethod]
        public void GetChromeDriver()
        {
            var driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome, _driverPath);
            Assert.IsNotNull(driver, "ChromeDriver was null");
            driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, driver.Url, $"Expected url was {_navUrl}, Url found was {driver.Url}");
            driver.Quit();
        }

        [TestMethod]
        public void GetFireFoxDriver()
        {
            var driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox, _driverPath);
            Assert.IsNotNull(driver, "FireFoxDriver was null");
            driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, driver.Url, $"Expected url was {_navUrl}, Url found was {driver.Url}");
            driver.Quit();
        }

        [TestMethod]
        public void GetIEDriver()
        {
            var driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie, _driverPath);
            Assert.IsNotNull(driver, "IEDriver was null");
            driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, driver.Url, $"Expected url was {_navUrl}, Url found was {driver.Url}");
            driver.Quit();
        }

        [TestMethod]
        public void GetEdgeDriver()
        {
            var driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge, _driverPath);
            Assert.IsNotNull(driver, "EdgeDriver was null");
            driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, driver.Url, $"Expected url was {_navUrl}, Url found was {driver.Url}");
            driver.Quit();
        }
    }
}
