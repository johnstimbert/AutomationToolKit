using WebUiAutomationToolKit.WebAndApiAutomationObjects;

namespace WebUiAutomationToolKit.Contracts
{
    /// <summary>
    /// This interface provides access to the methods available for interacting with objects of type IDomTree
    /// </summary>
    public interface IDomTree
    {
        /// <summary>
        /// Builds a tree structure beginning with the SelectorData provided as the root node
        /// </summary>
        /// <param name="rootNode">SelectorData used to locate the root node</param>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page to build the tree from</param>
        void Build(SelectorData rootNode, IWebDriverManager webDriverManager);
        /// <summary>
        /// Moves to the first(when reading the DOM top to Bottom) child node of the current node. If no child is found, null is returned
        /// </summary>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page the tree was built from</param>
        /// <returns>IDomNode</returns>
        IDomNode MoveToFirstChild(IWebDriverManager webDriverManager);
        /// <summary>
        /// Moves to the nth(when reading the DOM top to Bottom) child node of the current node. If no child is found, null is returned
        /// </summary>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page the tree was built from</param>
        /// <param name="nthChild">Zero-based index of child node to move to</param>
        /// <returns>IDomNode</returns>
        IDomNode MoveToNthChild(IWebDriverManager webDriverManager, int nthChild);
        /// <summary>
        /// Moves to the parent node of the current node. If no parent is found or if the current node is the root node, null is returned
        /// </summary>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page the tree was built from</param>
        /// <returns>IDomNode</returns>
        IDomNode MoveToParent(IWebDriverManager webDriverManager);
        /// <summary>
        /// Moves to the next(when reading the DOM top to Bottom) sibling node of the current node. 
        /// If no sibling is found or if the current node is the root node, null is returned
        /// </summary>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page the tree was built from</param>
        /// <returns>IDomNode</returns>
        IDomNode NextSibling(IWebDriverManager webDriverManager);
        /// <summary>
        /// Moves to the previous(when reading the DOM top to Bottom) sibling node of the current node. 
        /// If no sibling is found or if the current node is the root node, null is returned
        /// </summary>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page the tree was built from</param>
        /// <returns>IDomNode</returns>
        IDomNode PreviouSibling(IWebDriverManager webDriverManager);
    }
}