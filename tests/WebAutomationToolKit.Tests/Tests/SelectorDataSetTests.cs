using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAutomationToolKit.Exceptions;
using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class SelectorDataSetTests
    {
        private static SelectorData Item(string name)
            => new SelectorData(name, HtmlAttributeType.Id, name + "-id");

        [TestMethod]
        public void Constructor_HtmlTagType_NormalizesAllItemTags()
        {
            var items = new List<SelectorData> { Item("a"), Item("b") };
            var set = new SelectorDataSet(HtmlTagType.div, items);
            Assert.IsTrue(set.SelectorDataItems.All(x => x.TagType == "div"));
        }

        [TestMethod]
        public void Constructor_StringTag_NormalizesAllItemTags()
        {
            var items = new List<SelectorData> { Item("x") };
            var set = new SelectorDataSet("span", items);
            Assert.AreEqual("span", set.SelectorDataItems[0].TagType);
        }

        [TestMethod]
        public void Constructor_Empty_StartsEmpty()
        {
            var set = new SelectorDataSet(HtmlTagType.button);
            Assert.AreEqual(0, set.SelectorDataItems.Count);
            Assert.AreEqual("button", set.TagType);
        }

        [TestMethod]
        public void AddSelectorDataByName_NormalizesTagType()
        {
            var set = new SelectorDataSet(HtmlTagType.input);
            set.AddSelectorDataByName(Item("search"));
            Assert.AreEqual("input", set.SelectorDataItems[0].TagType);
        }

        [TestMethod]
        public void AddSelectorDataByName_DuplicateThrows()
        {
            var set = new SelectorDataSet(HtmlTagType.div);
            set.AddSelectorDataByName(Item("nav"));
            try
            {
                set.AddSelectorDataByName(Item("nav"));
                Assert.Fail("Expected WebAutomationException for duplicate name");
            }
            catch (WebAutomationException ex)
            {
                StringAssert.Contains(ex.Message, "nav");
            }
        }

        [TestMethod]
        public void GetSelectorDataByName_ReturnsCorrectItem()
        {
            var set = new SelectorDataSet(HtmlTagType.div);
            set.AddSelectorDataByName(Item("header"));
            var result = set.GetSelectorDataByName("HEADER"); // case-insensitive
            Assert.IsNotNull(result);
            Assert.AreEqual("header", result.Name);
        }

        [TestMethod]
        public void GetSelectorDataByName_NotFound_ReturnsNull()
        {
            var set = new SelectorDataSet(HtmlTagType.div);
            Assert.IsNull(set.GetSelectorDataByName("missing"));
        }

        [TestMethod]
        public void RemoveSelectorDataByName_RemovesItem()
        {
            var set = new SelectorDataSet(HtmlTagType.div);
            set.AddSelectorDataByName(Item("footer"));
            set.RemoveSelectorDataByName("footer");
            Assert.AreEqual(0, set.SelectorDataItems.Count);
        }

        [TestMethod]
        public void RemoveSelectorDataByName_NotFound_Throws()
        {
            var set = new SelectorDataSet(HtmlTagType.div);
            try
            {
                set.RemoveSelectorDataByName("ghost");
                Assert.Fail("Expected WebAutomationException for missing item");
            }
            catch (WebAutomationException ex)
            {
                StringAssert.Contains(ex.Message, "ghost");
            }
        }

        [TestMethod]
        public void OriginalItemTagType_NotMutated_AfterAdd()
        {
            // Because SelectorData is now a record, the original item is unchanged;
            // only the copy stored in the set has the set's tag.
            var original = Item("el");
            var set = new SelectorDataSet(HtmlTagType.form);
            set.AddSelectorDataByName(original);
            Assert.AreEqual(string.Empty, original.TagType, "Original record must not be mutated");
            Assert.AreEqual("form", set.SelectorDataItems[0].TagType, "Stored copy must have set tag");
        }
    }
}
