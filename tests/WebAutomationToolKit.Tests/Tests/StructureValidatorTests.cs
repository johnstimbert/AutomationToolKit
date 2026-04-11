using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAutomationToolKit.Validators;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class StructureValidatorTests
    {
        private readonly StructureValidator _validator = new();

        // ── BuildCssSelectorBy ──────────────────────────────────────────────

        [TestMethod]
        public void BuildCssSelectorBy_Id_ReturnsCssSelectorById()
        {
            var sd = new SelectorData("el", HtmlTagType.div, HtmlAttributeType.Id, "main");
            var by = _validator.BuildCssSelectorBy(sd);
            StringAssert.Contains(by.ToString(), "#main");
        }

        [TestMethod]
        public void BuildCssSelectorBy_Class_ReturnsCssSelectorByClass()
        {
            var sd = new SelectorData("el", HtmlTagType.div, HtmlAttributeType.Class, "container");
            var by = _validator.BuildCssSelectorBy(sd);
            StringAssert.Contains(by.ToString(), "div[class='container']");
        }

        [TestMethod]
        public void BuildCssSelectorBy_Name_ReturnsCssSelectorByName()
        {
            var sd = new SelectorData("el", HtmlTagType.input, HtmlAttributeType.Name, "q");
            var by = _validator.BuildCssSelectorBy(sd);
            StringAssert.Contains(by.ToString(), "input[name='q']");
        }

        [TestMethod]
        public void BuildCssSelectorBy_Href_ReturnsCssSelectorByHref()
        {
            var sd = new SelectorData("el", HtmlTagType.a, HtmlAttributeType.Href, "/home");
            var by = _validator.BuildCssSelectorBy(sd);
            StringAssert.Contains(by.ToString(), "a[href='/home']");
        }

        [TestMethod]
        public void BuildCssSelectorBy_None_ReturnsCssSelectorByTagOnly()
        {
            var sd = new SelectorData("el", HtmlTagType.p, HtmlAttributeType.None, null);
            var by = _validator.BuildCssSelectorBy(sd);
            StringAssert.Contains(by.ToString(), "p");
        }

        [TestMethod]
        public void BuildCssSelectorBy_FormControlName_ReturnsCssSelector()
        {
            var sd = new SelectorData("el", HtmlTagType.input, HtmlAttributeType.FormControlName, "email");
            var by = _validator.BuildCssSelectorBy(sd);
            StringAssert.Contains(by.ToString(), "input[formcontrolname='email']");
        }

        [TestMethod]
        public void BuildCssSelectorBy_PlaceHolder_ReturnsCssSelector()
        {
            var sd = new SelectorData("el", HtmlTagType.input, HtmlAttributeType.PlaceHolder, "Search…");
            var by = _validator.BuildCssSelectorBy(sd);
            StringAssert.Contains(by.ToString(), "input[placeholder='Search…']");
        }

        // ── ParseInnerText ──────────────────────────────────────────────────

        [TestMethod]
        public void ParseInnerText_NullOrEmpty_ReturnsNull()
        {
            Assert.IsNull(_validator.ParseInnerText(null));
            Assert.IsNull(_validator.ParseInnerText(string.Empty));
        }

        [TestMethod]
        public void ParseInnerText_PlainText_ReturnsSameText()
        {
            const string input = "Hello World";
            Assert.AreEqual(input, _validator.ParseInnerText(input));
        }
    }
}
