using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using WebAndWebApiAutomation;

namespace WebAndApiAutomation.Tests
{
    [TestClass]
    public class WebAutomationTests
    {
        private WebAutomation _webAutomation;
        private readonly string _navUrl = "https://www.google.com/";
        private IWebDriver _driver = null;
        private readonly string _screenShotPath = @"C:\ScreenShots";
        private readonly string _driverPath = @"C:\Users\j_sti\source\repos\AutomationToolKit\WebAndApiAutomation.Tests\bin\Debug\";

        [TestInitialize]
        public void BeforeEachTest()
        {
            _webAutomation = new WebAutomation(5, _driverPath);
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            if (_driver != null)
                _driver.Quit();

            if (Directory.Exists(_screenShotPath))
            {
                var dirInfo = new DirectoryInfo(_screenShotPath);
                dirInfo.Delete(true);
            }
        }

        [TestMethod]
        [TestCategory("Driver_Creation")]
        public void GetChromeDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, _driver.Url, $"Expected url was {_navUrl}, Url found was {_driver.Url}");
        }

        [TestMethod]
        [TestCategory("Driver_Creation")]
        public void GetFirefoxDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, _driver.Url, $"Expected url was {_navUrl}, Url found was {_driver.Url}");
        }

        [TestMethod]
        [TestCategory("Driver_Creation")]
        public void GetIEDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, _driver.Url, $"Expected url was {_navUrl}, Url found was {_driver.Url}");
        }

        [TestMethod]
        [TestCategory("Driver_Creation")]
        public void GetEdgeDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, _driver.Url, $"Expected url was {_navUrl}, Url found was {_driver.Url}");
        }

        [TestMethod]
        [TestCategory("DriverExtensionMethods")]
        public void TakeScreenShot_Chrome()
        {
            string shotName = "ChromeShot";
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            _webAutomation.TakeScreenShot(_driver, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory("DriverExtensionMethods")]
        public void TakeScreenShot_Firefox()
        {
            string shotName = "FireFoxShot";
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            _webAutomation.TakeScreenShot(_driver, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory("DriverExtensionMethods")]
        public void TakeScreenShot_IE()
        {
            string shotName = "IEShot";
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            _webAutomation.TakeScreenShot(_driver, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory("DriverExtensionMethods")]
        public void TakeScreenShot_Edge()
        {
            string shotName = "EdgeShot";
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            _webAutomation.TakeScreenShot(_driver, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory("DriverExtensionMethods")]
        public void GetJavaScriptExecutor_Chrome()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            Assert.IsNotNull(_driver, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driver);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory("DriverExtensionMethods")]
        public void GetJavaScriptExecutor_Firefox()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            Assert.IsNotNull(_driver, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driver);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory("DriverExtensionMethods")]
        public void GetJavaScriptExecutor_IE()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            Assert.IsNotNull(_driver, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driver);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory("DriverExtensionMethods")]
        public void GetJavaScriptExecutor_Edge()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driver);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

    }
}
