using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation.WebAndApiAutomationObjects
{
    public class SelectorData
    {
        public string Name { get; set; }
        public HtmlTagType TagType { get; set; }
        public HtmlAttributeType AttributeType { get; set; }
        public string AttributeValue { get; set; }

        /// <summary>
        /// Use this contructor when creating a stand alone SelectorData object
        /// </summary>
        public SelectorData(string name, HtmlTagType tag, HtmlAttributeType attType, string attValue)
        {
            Name = name;
            TagType = tag;
            AttributeType = attType;
            AttributeValue = attValue;
        }

        /// <summary>
        /// Use this contructor when creating a SelectorData object that will be included as part of a SelectorDataSet
        /// </summary>
        public SelectorData(string name, HtmlAttributeType attType, string attValue)
        {
            Name = name;
            AttributeType = attType;
            AttributeValue = attValue;
        }
    }
}
