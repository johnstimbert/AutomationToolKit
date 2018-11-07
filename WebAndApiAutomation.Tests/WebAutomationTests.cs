﻿using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using WebAndWebApiAutomation;
using WebAndWebApiAutomation.Exceptions;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;

namespace WebAndApiAutomation.Tests
{
    [TestClass]
    public class WebAutomationTests
    {
        private const string _driverTestCategory = "Driver_Tests";
        private const string _driverInteractionTestCategory = "Driver_Interaction_Tests";
        private const string _elementLocationMethodsTestCategory = "Element_Location_Method_Tests";

        private WebAutomation _webAutomation;
        private readonly string _navUrl = "https://www.google.com/";
        private IWebDriver _driver = null;
        private readonly string _screenShotPath = @"C:\ScreenShots";
        private readonly string _driverPath = @"C:\Users\j_sti\source\repos\AutomationToolKit\WebAndApiAutomation.Tests\bin\Debug\";

        private readonly string _nullDriverException = "Driver Provided was null. Instantiate the driver before performing actions on or with it";

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

        #region Driver_Tests
        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetChromeDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, _driver.Url, $"Expected url was {_navUrl}, Url found was {_driver.Url}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetFirefoxDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, _driver.Url, $"Expected url was {_navUrl}, Url found was {_driver.Url}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetIEDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, _driver.Url, $"Expected url was {_navUrl}, Url found was {_driver.Url}");
        }

        [TestMethod]
        [TestCategory(_driverTestCategory)]
        public void GetEdgeDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.AreEqual(_navUrl, _driver.Url, $"Expected url was {_navUrl}, Url found was {_driver.Url}");
        }

        #endregion

        #region Driver_Interaction_Tests
        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void TakeScreenShot_Chrome()
        {
            string shotName = "ChromeShot";
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.TakeScreenShot(_driver, _screenShotPath, shotName);

            var dirInfo = new DirectoryInfo(_screenShotPath);
            var fileInfo = dirInfo.GetFiles().FirstOrDefault(x => x.Name.Contains(shotName));
            Assert.IsNotNull(fileInfo, $"Screenshot was not found in {_screenShotPath}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
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
        [TestCategory(_driverInteractionTestCategory)]
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
        [TestCategory(_driverInteractionTestCategory)]
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
        [TestCategory(_driverInteractionTestCategory)]
        public void TakeScreenShot_NullDriver()
        {
            string shotName = "nullDriver";
            _driver = null;
            Assert.IsNull(_driver, "Driver was not null");

            try
            {
                _webAutomation.TakeScreenShot(_driver, _screenShotPath, shotName);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_Chrome()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            Assert.IsNotNull(_driver, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driver);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_Firefox()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            Assert.IsNotNull(_driver, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driver);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_IE()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            Assert.IsNotNull(_driver, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driver);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_Edge()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            var executor = _webAutomation.GetJavaScriptExecutor(_driver);
            Assert.IsNotNull(executor, "JavaScriptExecutor was null");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void GetJavaScriptExecutor_NullDriver()
        {
            _driver = null;
            Assert.IsNull(_driver, "Driver was not null");

            try
            {
                _webAutomation.GetJavaScriptExecutor(_driver);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Chrome()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", WebAutomationEnums.HtmlTagType.a, WebAutomationEnums.HtmlAttributeType.Title, "Selenium Projects");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(startingUrl);

            _driver = _webAutomation.Click(_driver, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driver.Url, $"Expected url was {expectedUrl}, found {_driver.Url}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Firefox()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", WebAutomationEnums.HtmlTagType.a, WebAutomationEnums.HtmlAttributeType.Title, "Selenium Projects");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(startingUrl);

            _driver = _webAutomation.Click(_driver, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driver.Url, $"Expected url was {expectedUrl}, found {_driver.Url}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_IE()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", WebAutomationEnums.HtmlTagType.a, WebAutomationEnums.HtmlAttributeType.Title, "Selenium Projects");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(startingUrl);

            _driver = _webAutomation.Click(_driver, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driver.Url, $"Expected url was {expectedUrl}, found {_driver.Url}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_Edge()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            string expectedUrl = "https://www.seleniumhq.org/projects/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", WebAutomationEnums.HtmlTagType.a, WebAutomationEnums.HtmlAttributeType.Title, "Selenium Projects");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(startingUrl);

            _driver = _webAutomation.Click(_driver, clickTarget);
            Thread.Sleep(3000);
            Assert.AreEqual(expectedUrl, _driver.Url, $"Expected url was {expectedUrl}, found {_driver.Url}");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Click_NullDriver()
        {
            string startingUrl = "https://www.seleniumhq.org/";
            SelectorData clickTarget = new SelectorData("SeleniumProjects", WebAutomationEnums.HtmlTagType.a, WebAutomationEnums.HtmlAttributeType.Title, "Selenium Projects");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(startingUrl);
            _driver.Quit();
            _driver = null;

            try
            {
                _driver = _webAutomation.Click(_driver, clickTarget);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }            
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_Chrome()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendText(_driver, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_Firefox()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendText(_driver, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendText(_driver, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_Edge()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendText(_driver, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendText_NullDriver()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");

            _driver.Quit();
            _driver = null;

            try
            {
                _webAutomation.SendText(_driver, googleSearchInput, text);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_Chrome()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendTextWithDelay(_driver, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_Firefox()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendTextWithDelay(_driver, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendTextWithDelay(_driver, googleSearchInput, text);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_Edge()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendTextWithDelay(_driver, googleSearchInput, text);
        }
               
        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void SendTextWithDelay_NullDriver()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");

            _driver.Quit();
            _driver = null;

            try
            {
                _webAutomation.SendTextWithDelay(_driver, googleSearchInput, text);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_Chrome()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendText(_driver, googleSearchInput, text);
            _webAutomation.Clear(_driver, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_Firefox()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendText(_driver, googleSearchInput, text);
            _webAutomation.Clear(_driver, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_IE()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendText(_driver, googleSearchInput, text);
            _webAutomation.Clear(_driver, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_Edge()
        {
            string text = "ChromeShot";
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");
            _webAutomation.SendText(_driver, googleSearchInput, text);
            _webAutomation.Clear(_driver, googleSearchInput);
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void Clear_NullDriver()
        {
            SelectorData googleSearchInput = new SelectorData("SearchInput", WebAutomationEnums.HtmlTagType.input, WebAutomationEnums.HtmlAttributeType.Type, "text");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl(_navUrl);
            Assert.IsNotNull(_driver, "Driver was null");

            _driver.Quit();
            _driver = null;

            try
            {
                _webAutomation.Clear(_driver, googleSearchInput);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Chrome()
        {
            SelectorData copyrightDiv = new SelectorData("CopyrightDiv", WebAutomationEnums.HtmlTagType.div, WebAutomationEnums.HtmlAttributeType.Class, "copyright");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            _driver = _webAutomation.MoveToElement(_driver, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driver);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Firefox()
        {
            SelectorData copyrightDiv = new SelectorData("CopyrightDiv", WebAutomationEnums.HtmlTagType.div, WebAutomationEnums.HtmlAttributeType.InnerText_Contains, "Intersys Consulting, Inc. All Rights Reserved");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            _driver = _webAutomation.MoveToElement(_driver, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driver);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_IE()
        {
            SelectorData copyrightDiv = new SelectorData("CopyrightDiv", WebAutomationEnums.HtmlTagType.div, WebAutomationEnums.HtmlAttributeType.InnerText_Contains, "Intersys Consulting, Inc. All Rights Reserved");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            _driver = _webAutomation.MoveToElement(_driver, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driver);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_Edge()
        {
            SelectorData copyrightDiv = new SelectorData("CopyrightDiv", WebAutomationEnums.HtmlTagType.div, WebAutomationEnums.HtmlAttributeType.InnerText_Contains, "Intersys Consulting, Inc. All Rights Reserved");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            _driver = _webAutomation.MoveToElement(_driver, copyrightDiv);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(copyrightDiv, _driver);
            Assert.IsTrue((element.Displayed && element.Enabled), "Target element should be on screen");
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void MoveToElement_NullDriver()
        {
            SelectorData copyrightDiv = new SelectorData("CopyrightDiv", WebAutomationEnums.HtmlTagType.div, WebAutomationEnums.HtmlAttributeType.InnerText_Contains, "Intersys Consulting, Inc. All Rights Reserved");

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");

            _driver.Quit();
            _driver = null;

            try
            {
                _webAutomation.MoveToElement(_driver, copyrightDiv);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Chrome()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");            
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driver, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Firefox()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driver, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_IE()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driver, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_Edge()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContain(_driver, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContain_NullDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");

            _driver.Quit();
            _driver = null;

            try
            {
                Assert.IsTrue(_webAutomation.DoesUrlContain(_driver, "intersys"));
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Chrome()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driver, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Firefox()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driver, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_IE()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driver, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_Edge()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");
            Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driver, "intersys"));
        }

        [TestMethod]
        [TestCategory(_driverInteractionTestCategory)]
        public void DoesUrlContainUsingRegex_NullDriver()
        {
            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            _driver.Navigate().GoToUrl("https://www.intersysconsulting.com/");
            Assert.IsNotNull(_driver, "Driver was null");

            _driver.Quit();
            _driver = null;

            try
            {
                Assert.IsTrue(_webAutomation.DoesUrlContainUsingRegex(_driver, "intersys"));
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }
        }

        #endregion

        #region Element_Location_Method_Tests

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Chrome()
        {
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Chrome()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Chrome);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Firefox()
        {
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Firefox()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Firefox);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_IE()
        {
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_IE()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Ie);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_Contains_Edge()
        {
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_Contains, "ge");
            string expectedAttributeValue = "get";

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");
        }

        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_UsingAttributeText_ExactMatch_Edge()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driver = _webAutomation.GetDriver(WebAutomationEnums.DriverType.Edge);
            Assert.IsNotNull(_driver, "Driver was null");
            _driver.Navigate().GoToUrl(_navUrl);
            var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            var attributeValue = element.GetAttribute("method");
            Assert.AreEqual(expectedAttributeValue, attributeValue, $"Expected attribute value was {expectedAttributeValue}, found {attributeValue}");

        }
        
        [TestMethod]
        [TestCategory(_elementLocationMethodsTestCategory)]
        public void CheckElementExistsReturnIWebElement_NullDriver()
        {
            string expectedAttributeValue = "get";
            SelectorData data = new SelectorData("GoogleForm", WebAutomationEnums.HtmlTagType.form, WebAutomationEnums.HtmlAttributeType.AttributeText_ExactMatch, expectedAttributeValue);

            _driver = null;
            Assert.IsNull(_driver, "Driver was not null");

            try
            {
                var element = _webAutomation.CheckElementExistsReturnIWebElement(data, _driver);
            }
            catch (WebAutomationException ex)
            {
                string message = ex.Message;
                Assert.AreEqual(_nullDriverException, message, "The expected null driver exception was not found");
            }

        }

        #endregion

        
    }
}
