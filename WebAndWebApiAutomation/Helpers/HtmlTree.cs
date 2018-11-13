using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebAndWebApiAutomation.WebAndApiAutomationObjects;

namespace WebAndWebApiAutomation.Helpers
{
    /// <summary>
    /// HtmlTree parses HTML code into a tree-like structure of objects and provides a tool-set for extracting data from it.
    /// </summary>
    public class HtmlTree
    {
        private List<HtmlTag> _allTags;
        private List<HtmlTag> _innerTags;
        
        /// <summary>
        /// Creates an HtmlTree object using the raw html provided
        /// </summary>
        /// <param name="rawHtml"></param>
        public HtmlTree(string rawHtml)
        {
            _innerTags = new List<HtmlTag>();
            _allTags = new List<HtmlTag>();
            Parse(rawHtml);
        }

        /// <summary>
        /// Contains a list of all tags found to be contained within parsed HTML code.
        /// </summary>
        public List<HtmlTag> AllTags
        {
            get
            {
                if (_allTags == null) _allTags = new List<HtmlTag>();
                return _allTags;
            }
        }
       
        /// <summary>
        /// Contains a list of top-level tags found to be contained within parsed HTML code.
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
        /// Parses the given HTML code.
        /// </summary>
        internal void Parse(string rawHtml)
        {
            #region Init/Clear local variables
            HtmlTag lastTag = null;
            Int32 currLineNr = 1;
            AllTags.Clear();
            InnerTags.Clear();
            #endregion

            do
            {
                Match mCurrHtml;
                // Process comments
                if ((mCurrHtml = Regex.Match(rawHtml, @"^\s*(<!--((?!-->).)*-->|<![^<>]+>|<!\[[^\s/<>]+\[((?!\]\]>).)*\]\]>)\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    HtmlTag t = new HtmlTag("<COMMENT>", "SINGLE", mCurrHtml.Groups[0].Value, currLineNr, lastTag);

                    InnerTags.Add(t);
                    if (lastTag != null) lastTag.Next = t; lastTag = t;
                    AllTags.Add(t);

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    rawHtml = Regex.Replace(rawHtml, @"^\s*(<!--((?!-->).)*-->|<![^<>]+>|<!\[[^\s/<>]+\[((?!\]\]>).)*\]\]>)\s*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                // Process scripts
                else if ((mCurrHtml = Regex.Match(rawHtml, @"^\s*(<script(?<a>\s+[^>]*)?>(?<s>((?!</script>).)*)</script>)\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    HtmlTag t = new HtmlTag("<SCRIPT>", "SINGLE", mCurrHtml.Groups[0].Value, currLineNr, lastTag);

                    InnerTags.Add(t);
                    if (lastTag != null) lastTag.Next = t; lastTag = t;
                    AllTags.Add(t);

                    // Process tag attributes
                    MatchCollection mAttributeCollection = Regex.Matches(mCurrHtml.Groups["a"].Value, "\\s+(?<a>[^\\n/>\"'’=]+)\\s*=\\s*\"(?<v>[^\"]*)\"|\\s+(?<a>[^\\n/>\"'’=]+)\\s*=\\s*'(?<v>[^']*)'|\\s+(?<a>[^\\n/>\"'’=]+)\\s*=\\s*(?<v>[^\\s]*)|\\s+(?<a>[^\\n/>\"'’=]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    foreach (Match mAttribute in mAttributeCollection)
                    {
                        if (t.Attributes.ContainsKey(mAttribute.Groups["a"].Value.Trim())) t.Attributes[mAttribute.Groups["a"].Value.Trim()] = mAttribute.Groups["v"].Value.Trim();
                        else t.Attributes.Add(mAttribute.Groups["a"].Value.Trim(), mAttribute.Groups["v"].Value.Trim());
                    }

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    rawHtml = Regex.Replace(rawHtml, @"^\s*(<script(?<a>\s+[^>]*)?>(?<s>((?!</script>).)*)</script>)\s*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                // Process tags
                else if ((mCurrHtml = Regex.Match(rawHtml, @"^\s*<(?<t>[^\s/<>]+)(?<a>\s+[^>]*?)?\s*(?<c>/\s*)?>\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    HtmlTag t = new HtmlTag("<" + mCurrHtml.Groups["t"].Value + ">", "OPEN", mCurrHtml.Groups[0].Value, currLineNr, lastTag);

                    InnerTags.Add(t);
                    if (lastTag != null) lastTag.Next = t; lastTag = t;
                    AllTags.Add(t);

                    // Process tag attributes
                    MatchCollection mAttributeCollection = Regex.Matches(mCurrHtml.Groups["a"].Value, "\\s+(?<a>[^\\n/>\"'’=]+)\\s*=\\s*\"(?<v>[^\"]*)\"|\\s+(?<a>[^\\n/>\"'’=]+)\\s*=\\s*'(?<v>[^']*)'|\\s+(?<a>[^\\n/>\"'’=]+)\\s*=\\s*(?<v>[^\\s]*)|\\s+(?<a>[^\\n/>\"'’=]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    foreach (Match mAttribute in mAttributeCollection)
                    {
                        if (t.Attributes.ContainsKey(mAttribute.Groups["a"].Value.Trim())) t.Attributes[mAttribute.Groups["a"].Value.Trim()] = mAttribute.Groups["v"].Value.Trim();
                        else t.Attributes.Add(mAttribute.Groups["a"].Value.Trim(), mAttribute.Groups["v"].Value.Trim());
                    }

                    // Mark tag as single tag if it has /> ending
                    if (mCurrHtml.Groups["c"].Value == "/") t.Status = "SINGLE";

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    rawHtml = Regex.Replace(rawHtml, @"^\s*<(?<t>[^\s/<>]+)(?<a>\s+[^>]*?)?\s*(?<c>/\s*)?>\s*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                // Process closing tags
                else if ((mCurrHtml = Regex.Match(rawHtml, @"^\s*</(?<t>[^\s/<>]+)( /)?>\s*", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    // Try to locate the opening tag
                    HtmlTag openingTag = lastTag;
                    while (openingTag != null && (openingTag.Tag != "<" + mCurrHtml.Groups["t"].Value.ToUpper() + ">" || openingTag.Status != "OPEN" || openingTag.InnerTags.Count > 0))
                    {
                        openingTag = openingTag.Previous;
                    }

                    if (openingTag != null)
                    {
                        for (HtmlTag t = openingTag.Next; t != null; t = t.Next)
                        {
                            if (t.Parent != openingTag.Parent) continue;
                            if (t.Parent != null) t.Parent.InnerTags.Remove(t);
                            InnerTags.Remove(t);
                            t.Parent = openingTag;
                            openingTag.InnerTags.Add(t);
                        }
                        openingTag.Status = "CLOSED";
                    }

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    rawHtml = Regex.Replace(rawHtml, @"^\s*</(?<t>[^\s/<>]+)( /)?>\s*", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                // Process text in between tags
                else if ((mCurrHtml = Regex.Match(rawHtml, @"^\s*([^<]+)(?=<)", RegexOptions.IgnoreCase | RegexOptions.Singleline)).Success)
                {
                    HtmlTag t = new HtmlTag("<TEXT>", "SINGLE", mCurrHtml.Groups[0].Value, currLineNr, lastTag);

                    InnerTags.Add(t);
                    if (lastTag != null) lastTag.Next = t; lastTag = t;
                    AllTags.Add(t);

                    currLineNr += mCurrHtml.Groups[0].Value.Split('\n').Length - 1;
                    rawHtml = Regex.Replace(rawHtml, @"^\s*([^<]+)(?=<)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                else
                {
                    // Remove unrecognized stuff and just keep on going. May of course cause missing elements so if you find it skipping something important please let me know.
                    if (rawHtml != "") rawHtml = Regex.Replace(rawHtml, "^<+(?=[<])|^((?!<).)+|^<.*?(?=<)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }

                // Generate an error if number of html tags exceed 10.000.000
                if (AllTags.Count > 10000000) throw new Exception("Number of tags in document have exceeded 10.000.000 entries! Surely something must be wrong!");

            } while (!Regex.IsMatch(rawHtml, @"^\s*$", RegexOptions.Singleline));

        }

        /// <summary>
        /// Returns the first tag from AllTags that matches all given search expressions.
        /// If Tag is given it will return tags with names that match.
        /// If attribute is given it return tags with attribute names that match.
        /// If value is given it will return tags with any attributes having values that match.
        /// If both attribute and value is given it will return tags with any attribute names that match having a value that match.
        /// All parameters take a case-insensitive regular expression as search string.
        /// </summary>
        public HtmlTag FirstTag(SelectorData selectorData)
        {
            string tag = $"<{selectorData.TagType}>";
            string attribute = selectorData.AttributeType.ToString();
            string attributeValue = selectorData.AttributeValue;

            foreach (HtmlTag t in AllTags)
            {
                bool found = true;
                // Check if current Tag matches criteria
                if (!string.IsNullOrEmpty(tag) && !Regex.IsMatch(t.Tag, tag, RegexOptions.IgnoreCase)) found = false;
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeValue))
                {
                    bool foundAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], attributeValue, RegexOptions.IgnoreCase)) foundAttVal = true;
                    if (!foundAttVal) found = false; ;
                }
                else if (!string.IsNullOrEmpty(attribute))
                {
                    bool foundAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase)) foundAttVal = true;
                    if (!foundAttVal) found = false; ;
                }
                else if (!string.IsNullOrEmpty(attributeValue))
                {
                    bool foundAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, attributeValue, RegexOptions.IgnoreCase)) foundAttVal = true;
                    if (!foundAttVal) found = false; ;
                }

                // Return current Tag if it matches criteria
                if (found) return t;
            }

            //return null if nothing is found
            return null;
        }
        
        /// <summary>
        /// Returns the first tag from AllTags that matches HTML expression.
        /// Takes a case-insensitive regular expression as search string.
        /// </summary>
        public HtmlTag FirstHtml(string htmlPattern)
        {
            foreach (HtmlTag t in AllTags)
            {
                if (Regex.IsMatch(t.RawHtml, htmlPattern, RegexOptions.IgnoreCase)) return t;
            }

            //return null if nothing is found
            return null;
        }
        
        /// <summary>
        /// Returns list of tags from AllTags that match all given search expressions.
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
            foreach (HtmlTag t in AllTags)
            {
                // Check if current Tag matches criteria
                if (!string.IsNullOrEmpty(tag) && !Regex.IsMatch(t.Tag, tag, RegexOptions.IgnoreCase)) continue;
                if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase) && Regex.IsMatch(t.Attributes[s], attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) continue;
                }
                else if (!string.IsNullOrEmpty(attribute))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Keys) if (Regex.IsMatch(s, attribute, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) continue;
                }
                else if (!string.IsNullOrEmpty(attributeValue))
                {
                    bool bAttVal = false;
                    foreach (string s in t.Attributes.Values) if (Regex.IsMatch(s, attributeValue, RegexOptions.IgnoreCase)) bAttVal = true;
                    if (!bAttVal) continue;
                }

                // Add current Tag to list if it matches criteria
                l.Add(t);
            }
            return l;
        }
                
        /// <summary>
        /// Returns list of tags from AllTags that match HTML expression.
        /// Takes a case-insensitive regular expression as search string.
        /// </summary>
        public List<HtmlTag> SearchHtml(string htmlPattern)
        {
            List<HtmlTag> l = new List<HtmlTag>();
            foreach (HtmlTag t in AllTags)
            {
                // Add current Tag to list if it matches criteria
                if (Regex.IsMatch(t.RawHtml, htmlPattern, RegexOptions.IgnoreCase)) l.Add(t);
            }
            return l;
        }
        
    }
}
