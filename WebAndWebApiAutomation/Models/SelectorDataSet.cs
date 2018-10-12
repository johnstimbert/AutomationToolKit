using System;
using System.Collections.Generic;
using System.Linq;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation.Models
{
    public class SelectorDataSet
    {        
        public HtmlTagType TagType { get; set; }
        public List<SelectorData> SelectorDataItems { get; set; }

        public SelectorDataSet(HtmlTagType tag, List<SelectorData> selectorDataItems)
        {
            TagType = tag;

            foreach(var selectorDataItem in selectorDataItems)
            {
                selectorDataItem.TagType = tag;
            }

            SelectorDataItems = selectorDataItems;
        }

        public SelectorDataSet(HtmlTagType tag)
        {
            TagType = tag;            
        }

        public SelectorData GetSelectorDataByName(string name)
        {
            return SelectorDataItems.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public void RemoveSelectorDataByName(string name)
        {
            var toBeReomved = SelectorDataItems.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            SelectorDataItems.Remove(toBeReomved);
        }

        public void AddSelectorDataByName(SelectorData selectorDataToAdd)
        {
            var dupe = SelectorDataItems.FirstOrDefault(x => x.Name.Equals(selectorDataToAdd.Name, StringComparison.CurrentCultureIgnoreCase));
            if (dupe != null)
                throw new Exception($"The collection already contains a SelectorData object with the name {selectorDataToAdd.Name}");

            selectorDataToAdd.TagType = TagType;

            SelectorDataItems.Add(selectorDataToAdd);
        }
    }
}
