using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenQA.Selenium;

namespace WebAndWebApiAutomation.Helpers
{
    internal class RazorViewParser
    {
        private const string _usingStatementLocator = "@using";
        private const string _modelInsatanceLocator = "@model";
        private const string _modelReferenceLocator = "@Model";
        private const string _conditionalRenderLocator = "@if";
        private const string _attributeStartLocator = "=\"";
        private const string _cleanAttributeLocator = "=\"\"";

        private const string _optionalTrue = " templateoptional=\"true\"";
        private const string _multipleTrue = " templatemultiple=\"true\"";

        private readonly string[] _multipleRenderLocators = new string[] { "foreach", "while", "for"};
        private readonly string[] _razorTerminatorLocators = new string[] { "break;", "continue;", "return;" };

        private string[] _linesOfText;

        internal void Parse(string filepath)
        {
            //Get all the lines of text in an array of strings
            _linesOfText = ReadFile(filepath);
            //Remove lines containing only common razor terminators
            ClearRazorTerminators();
            //Remove using statements
            ClearLinesThatContain(_usingStatementLocator);
            //Remove model references
            ClearLinesThatContain(_modelInsatanceLocator);
            //Remove any lines that are only whitespace
            ClearLinesWhereNullEmptyOrWhiteSpace();
            //Remove attribute values and leave empty attribute definitions
            ClearAttributeValues();
            //Remove innerText values that are not html elements
            ClearNonHtmlInnerText();
            //Find elements that render conditionally and add mark them optional for verification
            HandleConditionallyRenderedElements();

            if (filepath.Contains(".cshtml"))
                filepath = filepath.Replace(".cshtml", ".html");

            File.WriteAllLines($"{filepath}", _linesOfText);           
        }

        private void ClearRazorTerminators()
        {
            foreach (var locator in _razorTerminatorLocators)
            {
                if (_linesOfText.Any(line => line.Contains(locator)))
                {
                    var lineIndex = Array.IndexOf(_linesOfText, _linesOfText.First(x => x.Contains(locator)));
                    _linesOfText[lineIndex] = string.Empty;
                }
            }

            ClearLinesWhereNullEmptyOrWhiteSpace();
        }

        private void HandleConditionallyRenderedElements()
        {
            if (_linesOfText.Any(x => x.Contains(_conditionalRenderLocator)))
            {
                //Get the index of the first line where the ConditionalRenderLocator is found
                int startLine = Array.IndexOf(_linesOfText, _linesOfText.FirstOrDefault(text => text.Contains(_conditionalRenderLocator)));
                bool isConditional = false;
                bool isMultiple = false;
                for (int line = startLine; line < _linesOfText.Count(); line++)
                {
                    //If the line contains < we assume it is an element and need to:
                    // 1. Insert the attribute templateoptional="true" if isConditional is true
                    // 2. Insert the attribute templatemultiple="true" if isMultiple is true
                    if (_linesOfText[line].Contains("<") && !_linesOfText[line].Contains("<=") && !_linesOfText[line].Contains(">="))
                    {
                        _linesOfText[line] = AddConditionalAndMultipleAttributes(_linesOfText[line], isConditional, isMultiple);
                    }
                    else
                    {
                        //If the line contains _conditionalRenderLocator we assume it is not an element and need to set isConditional to true
                        if (_linesOfText[line].Contains(_conditionalRenderLocator))
                        {
                            isConditional = true;

                            _linesOfText[line] = string.Empty;
                        }
                        //If it contains any of the values in _multipleRenderLocators set isMultiple to true
                        foreach (var locator in _multipleRenderLocators)
                        {
                            if (_linesOfText[line].Contains(locator))
                            {
                                isMultiple = true;
                                _linesOfText[line] = string.Empty;
                                break;
                            }
                        }

                        //If we encounter }
                        // 1. We need to reset the flags
                        // 2. Replace it with an empty string so it will be removed if it is the only character in that line
                        if (_linesOfText[line].Contains("}"))
                        {
                            isConditional = false;
                            isMultiple = false;

                            _linesOfText[line] = _linesOfText[line].Replace("}", string.Empty);
                        }

                        //If we encounter {
                        // 1. Replace it with an empty string so it will be removed if it is the only character in that line
                        if (_linesOfText[line].Contains("{"))
                        {
                            _linesOfText[line] = _linesOfText[line].Replace("{", string.Empty);
                        }

                        //If we encounter @ without < or > we need to set it to empty
                        if (_linesOfText[line].Contains("@")
                            && (!_linesOfText[line].Contains("<") && !_linesOfText[line].Contains(">")))
                        {
                            _linesOfText[line] = string.Empty;
                        }
                    }
                }
                ClearLinesWhereNullEmptyOrWhiteSpace();
            }
        }

        private string AddConditionalAndMultipleAttributes(string lineToAlter, bool isConditional, bool isMultiple)
        {
            string line = lineToAlter;
            var tagStart = line.IndexOf("<");
            var closingTagStart = line.IndexOf("</");
            var closingTagEnd = line.LastIndexOf(">");

            if (tagStart == closingTagStart)
                return line;

            //If the line contains < or > we assume it is an element and need to:
            // 1. Insert the attribute templateoptional="true" if isConditional is true
            // 2. Insert the attribute templatemultiple="true" if isMultiple is true
            if (isConditional)
            {
                line = line.Insert(line.IndexOf(">"), _optionalTrue);
            }

            if (isMultiple)
            {
                line = line.Insert(line.IndexOf(">"), _multipleTrue);
            }

            return line;
        }

        private void ClearNonHtmlInnerText()
        {
            if (_linesOfText.Any(x => x.Contains("<") && x.Contains(">")))
            {
                for (int line = 0; line < _linesOfText.Count(); line++)
                {
                    //Split the line on > then remove empties
                    var splitLine = _linesOfText[line].Split(new[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
                    //If the array has less than 2 elements it is not handled here
                    if (splitLine.Length < 2)
                        continue;

                    int splitLinePosition = 0;
                    /*
                     * Since we are splitting the file into an array where each element in the array is a line 
                     * the only items handled here would be formatted as follows(single element on a single line as an example):
                     * <p class="sub-heading">@Model.Item1.BodyCopy</p> 
                     * 1. This contains < and >
                     * 2. It produces an array of at least two elements when split on > and empties are removed
                     *      So the the array for the above example would be:
                     *      index 0 = <p class="sub-heading"
                     *      index 1 = @Model.Item1.BodyCopy</p
                     *      
                     * Following that pattern each element could contain the innerText followed by <.
                     * This allows us to find the first instance of < and use that to remove all the characters up to that index
                     */
                    while (splitLinePosition < splitLine.Length)
                    {
                        //Get the firs index of <
                        var potentialEndOfInnerText = splitLine[splitLinePosition].IndexOf('<');
                        //If the index of < is -1 or 0 there is nothing to remove
                        if (potentialEndOfInnerText > 0)
                        {
                            //Get a substring using the index of the first < and replace it in the array addding the > at the end of each element
                            var lineWithoutInnerText = splitLine[splitLinePosition].Substring(potentialEndOfInnerText);
                            splitLine[splitLinePosition] = $"{lineWithoutInnerText}>";
                        }

                        splitLinePosition++;
                    }

                    _linesOfText[line] = string.Join("", splitLine);
                }
            }
        }

        private void ClearAttributeValues()
        {
            if (_linesOfText.Any(x => x.Contains(_attributeStartLocator)))
            {
                for (int i = 0; i < _linesOfText.Count(); i++)
                {
                    string line = null;
                    bool AttributesCleared = AllAttributesCleared(_linesOfText[i]);
                    while (!AttributesCleared)
                    {
                        line = CleanAttributes(_linesOfText[i]);
                        AttributesCleared = AllAttributesCleared(line);
                    }

                    if (!string.IsNullOrEmpty(line))
                        _linesOfText[i] = line;
                }
            }
        }

        private string CleanAttributes(string line)
        {
            string lineToMod = line;
            int startIndex = 0;
            while (startIndex < lineToMod.Length - 1)
            {
                //Find an instance of =" based on the starting index
                var attributeStart = lineToMod.IndexOf(_attributeStartLocator, startIndex, StringComparison.CurrentCultureIgnoreCase);
                //Find an instance of ="" based on the starting index
                var cleanAttributeStart = lineToMod.IndexOf(_cleanAttributeLocator, startIndex, StringComparison.CurrentCultureIgnoreCase);
                //If the index of =" does not match the index of ="" this attribute value is not clean and we need to clean it
                if (attributeStart != cleanAttributeStart)
                {
                    var indexOfNextQuote = lineToMod.IndexOf("\"", attributeStart+3);
                    //Calculate the substring length
                    var substringLength = indexOfNextQuote - attributeStart;
                    var substringToReplace = lineToMod.Substring(attributeStart + 2, substringLength-2);
                    lineToMod = lineToMod.Replace(substringToReplace, string.Empty);
                }
                else//If the index of =" matches the index of ="" this attribute value is clean and we can increment the startIndex
                {
                    if (attributeStart < 0 && cleanAttributeStart < 0)
                        break;

                    startIndex = cleanAttributeStart + 3;
                }
            }

            return lineToMod;
        }

        private bool AllAttributesCleared(string line)
        {
            int startIndex = 0;
            while (startIndex < line.Length - 1)
            {
                //Find an instance of =" based on the starting index
                var attributeStart = line.IndexOf(_attributeStartLocator, startIndex, StringComparison.CurrentCultureIgnoreCase);
                //Find an instance of ="" based on the starting index
                var cleanAttributeStart = line.IndexOf(_cleanAttributeLocator, startIndex, StringComparison.CurrentCultureIgnoreCase);

                if (attributeStart <= 0)
                    break;

                if (attributeStart != cleanAttributeStart)
                    return false;

                startIndex = cleanAttributeStart+3;
            }

            return true;
        }

        private void ClearLinesThatOnlyContain(string value)
        {
            var lines = new List<string>();

            if (_linesOfText.Any(x => x.Contains(value) && x.Trim().Length == 1))
            {
                for (int i = 0; i < _linesOfText.Count(); i++)
                {
                    if (!_linesOfText[i].Contains(value) && _linesOfText[i].Trim().Length > 1)
                        lines.Add(_linesOfText[i]);
                }

                _linesOfText = lines.ToArray();
            }
        }

        private void ClearLinesWhereNullEmptyOrWhiteSpace()
        {
            _linesOfText = _linesOfText.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            _linesOfText = _linesOfText.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }

        private void ClearLinesThatContain(string locator)
        {
            var lines = new List<string>();

            //If the file contains usings remove them
            if (_linesOfText.Any(x => x.Contains(locator)))
            {
                for (int i = 0; i < _linesOfText.Count(); i++)
                {
                    if (!_linesOfText[i].Contains(locator))
                        lines.Add(_linesOfText[i]);
                }

                _linesOfText = lines.ToArray();
            }
        }

        private string[] ReadFile(string filepath) => File.ReadAllLines(filepath);
    }
}
