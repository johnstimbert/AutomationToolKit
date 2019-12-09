using OpenQA.Selenium;
using System;
using WebUiAutomationToolKit.Contracts;
using WebUiAutomationToolKit.Exceptions;
using WebUiAutomationToolKit.Validators;
using WebUiAutomationToolKit.WebAndApiAutomationObjects;

namespace WebUiAutomationToolKit.ConcreteClasses
{    
    internal sealed class DomTree : IDomTree
    {
        private readonly StructureValidator _structureValidator;
        private By _rootNodeSelector;

        private By[] _currentNodeSiblings;
        
        private DomNode _rootNode;
        private DomNode _previousNode;
        private DomNode _currentNode;

        private DomTree() { }

        internal DomTree(StructureValidator structureValidator)
        {
            _structureValidator = structureValidator;
        }

        /// <summary>
        /// Builds a tree structure beginning with the SelectorData provided as the root node
        /// </summary>
        /// <param name="rootNode">SelectorData used to locate the root node</param>
        /// <param name="webDriverManager">Must have an active driver set and navigated to the page to build the tree from</param>
        public void Build(SelectorData rootNode, IWebDriverManager webDriverManager)
        {
            try
            {
                WebDriverManager manager = (WebDriverManager)webDriverManager;
                //Build the root node CssSelector
                _rootNodeSelector = _structureValidator.BuildCssSelectorBy(rootNode);
                //Make sure we can locate an element and throw an error if we can't
                var root = _structureValidator.CheckElementExistsReturnIWebElement(rootNode, manager.GetActiveDriver(), manager.GetWait());
                if (root == null)
                    throw new WebUiAutomationException($"Could not locate an element using the locator {_rootNodeSelector}");

                _rootNode = new DomNode(_rootNodeSelector, manager);//set the root
                _currentNode = _rootNode;//set the current node to the root
            }
            catch(Exception ex)
            {
                throw new WebUiAutomationException(ex.Message);
            }
        }

        /// <summary>
        /// Moves to the first(when reading the DOM top to Bottom) child node of the current node. If no child is found, null is returned
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns>IDomNode</returns>
        public IDomNode MoveToFirstChild(IWebDriverManager webDriverManager)
        { 
            try
            { 
                //If there are no children of the current node return null
                if (!_currentNode.HasChildren(webDriverManager))
                    return null;
                //Set the previous node to the current node since we are changing position
                _previousNode = _currentNode;
                //Set the siblings to the children of the current node
                _currentNodeSiblings = _previousNode.GetChildren(webDriverManager);
                //Create and set the new current node with the previous node as the parent and siblings from the previous node
                _currentNode = new DomNode(_previousNode.NodeSelector, _currentNodeSiblings[0], GetConcreteManager(webDriverManager));

                return _currentNode;
            }
            catch (IndexOutOfRangeException)//If the target is out of range return null
            {
                return null;
            }
        }

        /// <summary>
        /// Moves to the nth(when reading the DOM top to Bottom) child node of the current node. If no child is found, null is returned
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <param name="nthChild">Zero-based index of child node to move to</param>
        /// <returns>IDomNode</returns>
        public IDomNode MoveToNthChild(IWebDriverManager webDriverManager, int nthChild)
        {
            try
            { 
                //If there are no children of the current node return null
                if (!_currentNode.HasChildren(webDriverManager))
                    return null;

                //Set the previous node to the current node since we are changing position
                _previousNode = _currentNode;
                //Set the siblings to the children of the current node
                _currentNodeSiblings = _previousNode.GetChildren(webDriverManager);
                //Create and set the new current node
                _currentNode = new DomNode(_currentNode.NodeSelector, _currentNodeSiblings[nthChild], GetConcreteManager(webDriverManager));

                return _currentNode;
            }
            catch (IndexOutOfRangeException)//If the target is out of range return null
            {
                return null;
            }
        }

        /// <summary>
        /// Moves to the parent node of the current node. If no parent is found or if the current node is the root node, null is returned
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns>IDomNode</returns>
        public IDomNode MoveToParent(IWebDriverManager webDriverManager)
        {
            //Root node has no parent so we should return null
            if (_currentNode == _rootNode)
                return null;

            //Set the previous node to the current node
            _previousNode = _currentNode;
            //Get the parent node selector from the previous node
            var parentSelector = _previousNode.ParentNodeSelector;
            //Create a DomNode instance for the parent node we are moving to
            _currentNode = new DomNode(parentSelector, GetConcreteManager(webDriverManager));
            //Create a DomNode instance for the parent of the new current node
            var newParent = new DomNode(_currentNode.ParentNodeSelector, GetConcreteManager(webDriverManager));
            //Get the siblings of the new current node
            _currentNodeSiblings = newParent.GetChildren(webDriverManager);

            return _currentNode;
        }

        /// <summary>
        /// Moves to the next(when reading the DOM top to Bottom) sibling node of the current node. 
        /// If no sibling is found or if the current node is the root node, null is returned
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns>IDomNode</returns>
        public IDomNode NextSibling(IWebDriverManager webDriverManager)
        {
            try
            {
                //Root node has no parent so we should return null
                if (_currentNode == _rootNode)
                    return null;
                //Attempt to get the sibling locator By. If there is no next sibling an IndexOutOfRangeException is expected
                var siblingBy = GetSiblingBy(1);
                //Set the previous node to the current node
                _previousNode = _currentNode;
                //Create a DomNode instance for the sibling of the current node and set it to be our new current node
                _currentNode = new DomNode(siblingBy, GetConcreteManager(webDriverManager));
                //Create a DomNode for the parent of the new current node
                var parentNode = new DomNode(_currentNode.ParentNodeSelector, GetConcreteManager(webDriverManager));
                //Get the children if the parent to get the current siblings
                _currentNodeSiblings = parentNode.GetChildren(webDriverManager);

                return _currentNode;
            }
            catch(IndexOutOfRangeException)//If the target is out of range return null
            {
                return null;
            }
        }

        /// <summary>
        /// Moves to the previous(when reading the DOM top to Bottom) sibling node of the current node. 
        /// If no sibling is found or if the current node is the root node, null is returned
        /// </summary>
        /// <param name="webDriverManager"></param>
        /// <returns>IDomNode</returns>
        public IDomNode PreviouSibling(IWebDriverManager webDriverManager)
        {
            try
            {
                //Root node has no parent so we should return null
                if (_currentNode == _rootNode)
                    return null;
                //Attempt to get the sibling locator By. If there is no previous sibling an IndexOutOfRangeException is expected
                var siblingBy = GetSiblingBy(-1);
                //Set the previous node to the current node
                _previousNode = _currentNode;
                //Create a DomNode instance for the sibling of the current node and set it to be our new current node
                _currentNode = new DomNode(siblingBy, GetConcreteManager(webDriverManager));
                //Create a DomNode for the parent of the new current node
                var parentNode = new DomNode(_currentNode.ParentNodeSelector, GetConcreteManager(webDriverManager));
                //Get the children if the parent to get the current siblings
                _currentNodeSiblings = parentNode.GetChildren(webDriverManager);

                return _currentNode;
            }
            catch (IndexOutOfRangeException)//If the target is out of range return null
            {
                return null;
            }
        }


        private By GetSiblingBy(int indexModifier)
        {
            //Get the index of the current node in the list of siblings
            var currentIndex = Array.IndexOf(_currentNodeSiblings, _currentNode.NodeSelector);
            //Attempt to get the target sibling and return it 
            return (By)_currentNodeSiblings.GetValue(currentIndex + indexModifier);
        }

        private WebDriverManager GetConcreteManager(IWebDriverManager webDriverManager)
        {
            return (WebDriverManager)webDriverManager;
        }

        private IWebElement GetElementReference(IWebDriverManager webDriverManager)
        {
            try
            {
                WebDriverManager manager = (WebDriverManager)webDriverManager;
                return manager.GetActiveDriver().FindElement(_rootNodeSelector);
            }
            catch (NoSuchElementException)
            {
                throw new WebUiAutomationException($"Could not locate any element using the locator {_rootNodeSelector}");
            }
            catch (Exception ex)
            {
                throw new WebUiAutomationException(ex.Message);
            }
        }
    }
}
