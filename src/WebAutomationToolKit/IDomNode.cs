namespace WebAutomationToolKit.Contracts
{
    /// <summary>
    /// This interface provides access to the methods available for interacting with objects of type IDomNode
    /// </summary>
    public interface IDomNode
    {
        /// <summary>
        /// Returns any attributes found in the form of a dictionary where the keys are attributes names and the values are the attribute values
        /// </summary>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page the tree was built from</param>
        /// <returns></returns>
        Dictionary<string, object> GetAttributes(IWebDriverManager webDriverManager);
        /// <summary>
        /// Gets the tag name of this node
        /// </summary>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page the tree was built from</param>
        /// <returns></returns>
        string GetTagName(IWebDriverManager webDriverManager);
        /// <summary>
        /// Gets the innerText of this node, without any leading or trailing whitespace, and with other whitespace collapsed
        /// </summary>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page the tree was built from</param>
        /// <returns></returns>
        string GetText(IWebDriverManager webDriverManager);
        /// <summary>
        /// Indicates if this node has attributes or not
        /// </summary>
        bool HasAttributes(IWebDriverManager webDriverManager);
        /// <summary>
        /// Indicates if this node has child nodes or not
        /// </summary>
        bool HasChildren(IWebDriverManager webDriverManager);
    }
}