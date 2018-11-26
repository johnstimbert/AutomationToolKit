﻿using System;
using System.Collections.Generic;
using System.Linq;
using WebAndWebApiAutomation.Exceptions;
using static WebAndWebApiAutomation.WebAutomationEnums;

namespace WebAndWebApiAutomation.WebAndApiAutomationObjects
{
    /// <summary>
    /// Represents a collection of SelectorData objects
    /// </summary>
    public class SelectorDataSet
    {        
        /// <summary>
        /// Returns the tag type for all the items in this collection
        /// </summary>
        public HtmlTagType TagType { get; private set; }

        /// <summary>
        /// Returns the items in this collection
        /// </summary>
        public List<SelectorData> SelectorDataItems { get; private set; }

        /// <param name="tag">Tag type for all the items in this collection</param>
        /// <param name="selectorDataItems">Items to add to this collection</param>
        public SelectorDataSet(HtmlTagType tag, List<SelectorData> selectorDataItems)
        {
            TagType = tag;

            foreach(var selectorDataItem in selectorDataItems)
            {
                selectorDataItem.TagType = tag;
            }

            SelectorDataItems = selectorDataItems;
        }

        /// <param name="tag">Tag type for all the items in this collection</param>
        public SelectorDataSet(HtmlTagType tag)
        {
            TagType = tag;            
        }

        /// <summary>
        /// Returns the SelectorData object from the collection matching name value provided
        /// </summary>
        /// <param name="name">Name to search for, not case sensitive</param>
        /// <returns cref="SelectorData">Null if not match is found</returns>
        public SelectorData GetSelectorDataByName(string name)
        {
            return SelectorDataItems.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Removes the SelectorData object from the collection matching name value provided
        /// </summary>
        /// <param name="name">Name to search for, not case sensitive</param>
        /// <exception cref="WebAutomationException"/>
        public void RemoveSelectorDataByName(string name)
        {
            var toBeReomved = SelectorDataItems.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (toBeReomved == null)
                throw new WebAutomationException($"An item with the name {name} was not found");

            SelectorDataItems.Remove(toBeReomved);
        }

        /// <summary>
        /// Adds the provided SelectorData provided to the collection. If a diplicate is found an error will be thrown.
        /// NOTE: The tag type on the item being added will be overridden using the value set in the HtmlTag property
        /// </summary>
        /// <param name="selectorDataToAdd">Object to add</param>
        /// <exception cref="WebAutomationException"/>
        public void AddSelectorDataByName(SelectorData selectorDataToAdd)
        {
            var dupe = SelectorDataItems.FirstOrDefault(x => x.Name.Equals(selectorDataToAdd.Name, StringComparison.CurrentCultureIgnoreCase));
            if (dupe != null)
                throw new WebAutomationException($"The collection already contains a SelectorData object with the name {selectorDataToAdd.Name}");

            selectorDataToAdd.TagType = TagType;

            SelectorDataItems.Add(selectorDataToAdd);
        }
    }
}