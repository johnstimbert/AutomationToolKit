using static WebAutomationToolKit.WebAutomationEnums;

namespace WebAutomationToolKit
{
    /// <summary>
    /// Common object for interaction with the WebAutomation utility
    /// </summary>
    public record SelectorData
    {
        /// <summary>
        /// Used to uniquely identify a SelectorData object when used as part of a set
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Denotes the Html Element Name or Tag
        /// </summary>
        public string TagType { get; init; }
        /// <summary>
        /// Denotes the attribute name, unless set to 'None'
        /// </summary>
        public HtmlAttributeType AttributeType { get; init; }
        /// <summary>
        /// Denotes the attribute value text, unless the AttributeType property is set to 'None' 
        /// </summary>
        public string AttributeValue { get; init; }

        /// <summary>
        /// Use this constructor when creating a stand-alone SelectorData object
        /// </summary>
        public SelectorData(string name, HtmlTagType tag, HtmlAttributeType attType, string attValue)
        {
            Name = name;
            TagType = tag.ToString();
            AttributeType = attType;
            AttributeValue = attValue;
        }

        /// <summary>
        /// Use this constructor when creating a stand-alone SelectorData object and the HtmlTagType enum does not contain your tag type
        /// </summary>
        public SelectorData(string name, string tag, HtmlAttributeType attType, string attValue)
        {
            Name = name;
            TagType = tag;
            AttributeType = attType;
            AttributeValue = attValue;
        }

        /// <summary>
        /// Use this constructor when creating a SelectorData object that will be included as part of a SelectorDataSet.
        /// TagType will be set to the set's tag when the item is added to the set.
        /// </summary>
        public SelectorData(string name, HtmlAttributeType attType, string attValue)
        {
            Name = name;
            TagType = string.Empty;
            AttributeType = attType;
            AttributeValue = attValue;
        }
    }
}
