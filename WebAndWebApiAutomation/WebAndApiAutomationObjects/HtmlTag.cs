using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation.WebAndApiAutomationObjects
{
    /// <summary>
    /// Object representation of a given html element
    /// </summary>
    public class HtmlTag
    {
        private string _tag;
        private string _status;//Open, Closed, Single
        private string _rawHtml;
        private int _lineNumber;
        private Dictionary<string, string> _attributes;
        private HtmlTag _parentTag;
        private HtmlTag _previousTag;
        private HtmlTag _nextTag;
        private List<HtmlTag> _innerTags;

        internal HtmlTag(string strTag, string strType, string strHTML, int iLineNr, HtmlTag pPreviousTag)
        {
            _tag = strTag.Trim().ToUpper();
            _status = strType.Trim().ToUpper();
            _rawHtml = strHTML.Trim();
            _lineNumber = iLineNr;
            _attributes = new Dictionary<string, string>();
            _parentTag = null;
            _previousTag = pPreviousTag;
            _nextTag = null;
            _innerTags = new List<HtmlTag>();
        }

        /// <summary>
        /// Contains the name/type of this tag.
        /// </summary>
        public string Tag
        {
            get { return _tag; }
            set
            {
                _tag = value.Trim().ToUpper();
            }
        }

        /// <summary>
        /// Holds the encapsulation status for the current tag used for tracking encapsulation during parsing. 
        /// If the tag has the <tag /> form the status is set to "SINGLE", otherwise it is set to "OPEN" until a closing tag is found and it's status set to "CLOSED". 
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value.Trim().ToUpper();
            }
        }

        /// <summary>
        /// Contains the HTML code from which this tag was generated.
        /// </summary>
        public string RawHtml
        {
            get { return _rawHtml; }
            set { _rawHtml = value.Trim(); }
        }

        /// <summary>
        /// Contains the line number in HTML code from which this tag was generated.
        /// </summary>
        public int LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        /// <summary>
        /// Contains the attributes found to be defined with this tag.
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get
            {
                if (_attributes == null) _attributes = new System.Collections.Generic.Dictionary<string, string>();
                return _attributes;
            }
        }

        /// <summary>
        /// Contains the parent tag found to encapsulate this tag.
        /// </summary>
        public HtmlTag Parent
        {
            get { return _parentTag; }
            set { _parentTag = value; }
        }

        /// <summary>
        /// Contains the previous tag found in source before this tag.
        /// </summary>
        public HtmlTag Previous
        {
            get { return _previousTag; }
            set { _previousTag = value; }
        }

        /// <summary>
        /// Contains the next tag found in source after this tag.
        /// </summary>
        public HtmlTag Next
        {
            get { return _nextTag; }
            set { _nextTag = value; }
        }

        /// <summary>
        /// Contains a list of inner tags (children) found to be contained within this tag (parent).
        /// </summary>
        public List<HtmlTag> InnerTags
        {
            get
            {
                if (_innerTags == null) _innerTags = new List<HtmlTag>();
                return _innerTags;
            }
        }

        /// <summary>
        /// Returns the first tag from InnerTags that matches all given search expressions.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attribute names that match.
        /// If attribute value is given it will return tags with any attributes having values that match.
        /// If both attribute and value is given it will return tags with any attribute names that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public HtmlTag FirstTag(SelectorData selectorData)
        {
            string tag = $"<{selectorData.TagType}>";
            string attribute = selectorData.AttributeType.ToString();
            string attributeValue = selectorData.AttributeValue;

            foreach (HtmlTag t in InnerTags)
            {
                bool bFound = true;
                // Check if current Tag matches criteria
                if (!string.IsNullOrEmpty(tag) && !Regex.IsMatch(t.Tag, tag, RegexOptions.IgnoreCase)) bFound = false;
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(attribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }

                // Return current Tag if it matches criteria
                if (bFound) return t;

                // Search InnerTags
                HtmlTag t2 = t.FirstTag(selectorData);
                if (t2 != null) return t2;
            }

            //return null if nothing is found
            return null;
        }

        /// <summary>
        /// Returns the first tag from InnerTags that matches HTML expression.
        /// Takes a case-insensitive regular expression as search string.
        /// </summary>
        public HtmlTag FirstHtml(string htmlPattern)
        {
            foreach (HtmlTag t in InnerTags)
            {
                if (Regex.IsMatch(t.RawHtml, htmlPattern, RegexOptions.IgnoreCase)) return t;
                HtmlTag t2 = t.FirstHtml(htmlPattern);
                if (t2 != null) return t2;
            }

            //return null if nothing is found
            return null;
        }

        /// <summary>
        /// Returns the next subsequent tag that matches all given search expressions regardless of whether it is an inner tag or not.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attribute names that match.
        /// If value is given it will return tags with any attributes having values that match.
        /// If both attribute and value is given it will return tags with any attribute names that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public HtmlTag NextTag(SelectorData selectorData)
        {
            string tag = $"<{selectorData.TagType}>";
            string attribute = selectorData.AttributeType.ToString();
            string attributeValue = selectorData.AttributeValue;

            for (HtmlTag t = Next; t != null; t = t.Next)
            {
                bool bFound = true;
                // Check if current Tag matches criteria
                if (!string.IsNullOrEmpty(tag) && !Regex.IsMatch(t.Tag, tag, RegexOptions.IgnoreCase)) bFound = false;
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(attribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }

                // Return current Tag if it matches criteria
                if (bFound) return t;
            }
            return null;
        }

        /// <summary>
        /// Returns the next subsequent tag that matches HTML expression regardless of whether it is an inner tag or not.
        /// Takes a case-insensitive regular expression as search string.
        /// </summary>
        public HtmlTag NextHtml(string htmlPattern)
        {
            for (HtmlTag t = Next; t != null; t = t.Next)
            {
                if (Regex.IsMatch(t.RawHtml, htmlPattern, RegexOptions.IgnoreCase)) return t;
            }
            return null;
        }

        /// <summary>
        /// Returns the parent tag that matches all given search expressions regardless of whether it is an inner tag or not.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attribute names that match.
        /// If value is given it will return tags with any attributes having values that match.
        /// If both attribute and value is given it will return tags with any attribute names that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public HtmlTag PreviousTag(SelectorData selectorData)
        {
            string tag = $"<{selectorData.TagType}>";
            string attribute = selectorData.AttributeType.ToString();
            string attributeValue = selectorData.AttributeValue;

            for (HtmlTag t = Previous; t != null; t = t.Previous)
            {
                bool bFound = true;
                // Check if current Tag matches criteria
                if (!string.IsNullOrEmpty(tag) && !Regex.IsMatch(t.Tag, tag, RegexOptions.IgnoreCase)) bFound = false;
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(attribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }

                // Return current Tag if it matches criteria
                if (bFound) return t;
            }
            return null;
        }

        /// <summary>
        /// Returns the parent tag that matches HTML expression regardless of whether it is an inner tag or not.
        /// Takes a case-insensitive regular expression as search string.
        /// </summary>
        public HtmlTag PreviousHtml(string htmlPattern)
        {
            for (HtmlTag t = Previous; t != null; t = t.Previous)
            {
                if (Regex.IsMatch(t.RawHtml, htmlPattern, RegexOptions.IgnoreCase)) return t;
            }
            return null;
        }

        /// <summary>
        /// Returns list of tags from InnerTags that match all given search expressions.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attribute names that match.
        /// If value is given it will return tags with any attributes having values that match.
        /// If both attribute and value is given it will return tags with any attribute names that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public List<HtmlTag> Search(SelectorData selectorData)
        {
            string tag = $"<{selectorData.TagType}>";
            string attribute = selectorData.AttributeType.ToString();
            string attributeValue = selectorData.AttributeValue;

            List<HtmlTag> l = new List<HtmlTag>();
            foreach (HtmlTag t in InnerTags)
            {
                // Check if current Tag matches criteria
                bool bFound = true;
                if (!string.IsNullOrEmpty(tag) && !Regex.IsMatch(t.Tag, tag, RegexOptions.IgnoreCase)) bFound = false;
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(attribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }
                else if (!string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) bFound = false; ;
                }

                // Add current Tag to list if it matches criteria
                if (bFound) l.Add(t);

                // Add InnerTags to list that match criteria, causes depth-first search
                l.AddRange(t.Search(selectorData));
            }
            return l;
        }

        /// <summary>
        /// Returns list of tags from InnerTags that match HTML expression.
        /// Takes a case-insensitive regular expression as search string.
        /// </summary>
        public List<HtmlTag> SearchHtml(HtmlTagType htmlTagType)
        {
            string tag = $"<{htmlTagType}>";
            List<HtmlTag> l = new List<HtmlTag>();
            foreach (HtmlTag t in InnerTags)
            {
                // Add current Tag to list if it matches criteria
                if (Regex.IsMatch(t.RawHtml, tag, RegexOptions.IgnoreCase)) l.Add(t);

                // Add InnerTags to list that match criteria, causes depth-first search
                l.AddRange(t.SearchHtml(htmlTagType));
            }
            return l;
        }

        /// <summary>
        /// Extracts the raw html from current tag and its inner tags. If it runs into (BR) or (P) tags they are replced with newlines.
        /// </summary>
        public string ToText()
        {
            string strText = "";
            if (Tag == "<TEXT>") strText = RawHtml;
            else if (Tag == "<BR>" && InnerTags.Count == 0) strText = "\n";
            foreach (HtmlTag t in InnerTags)
            {
                strText += t.ToText();
            }
            if (Tag == "<P>" && InnerTags.Count > 0) strText += "\n\n";
            else if (Tag == "<BR>" && InnerTags.Count > 0) strText += "\n";
            return strText;
        }

        /// <summary>
        /// Returns a string representation of the current tag prefixed with the line number and a count of its attributes and inner tags
        /// </summary>
        public override string ToString()
        {
            return $"Line number {LineNumber}: {Tag} - attribute count {Attributes.Count}, inner tag count {InnerTags.Count}".Trim();
        }

    }
}
