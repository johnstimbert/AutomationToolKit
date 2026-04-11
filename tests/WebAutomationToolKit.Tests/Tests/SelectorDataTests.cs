using Microsoft.VisualStudio.TestTools.UnitTesting;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class SelectorDataTests
    {
        [TestMethod]
        public void Constructor_HtmlTagType_SetsAllProperties()
        {
            var sd = new SelectorData("MyEl", HtmlTagType.div, HtmlAttributeType.Id, "main");
            Assert.AreEqual("MyEl", sd.Name);
            Assert.AreEqual("div", sd.TagType);
            Assert.AreEqual(HtmlAttributeType.Id, sd.AttributeType);
            Assert.AreEqual("main", sd.AttributeValue);
        }

        [TestMethod]
        public void Constructor_StringTag_SetsTagType()
        {
            var sd = new SelectorData("el", "custom-tag", HtmlAttributeType.Class, "foo");
            Assert.AreEqual("custom-tag", sd.TagType);
        }

        [TestMethod]
        public void Constructor_SetMember_SetsEmptyTagType()
        {
            var sd = new SelectorData("el", HtmlAttributeType.Name, "search");
            Assert.AreEqual(string.Empty, sd.TagType);
            Assert.AreEqual("el", sd.Name);
            Assert.AreEqual(HtmlAttributeType.Name, sd.AttributeType);
            Assert.AreEqual("search", sd.AttributeValue);
        }

        [TestMethod]
        public void WithExpression_OverridesTagType()
        {
            var sd = new SelectorData("el", HtmlAttributeType.Name, "q");
            var updated = sd with { TagType = "textarea" };
            Assert.AreEqual("textarea", updated.TagType);
            // original unchanged
            Assert.AreEqual(string.Empty, sd.TagType);
        }

        [TestMethod]
        public void Record_ValueEquality_SameValuesAreEqual()
        {
            var a = new SelectorData("el", HtmlTagType.input, HtmlAttributeType.Id, "x");
            var b = new SelectorData("el", HtmlTagType.input, HtmlAttributeType.Id, "x");
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void Record_ValueEquality_DifferentValuesNotEqual()
        {
            var a = new SelectorData("a", HtmlTagType.input, HtmlAttributeType.Id, "x");
            var b = new SelectorData("b", HtmlTagType.input, HtmlAttributeType.Id, "x");
            Assert.AreNotEqual(a, b);
        }
    }
}
