// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DaxEditor
{
    /// <summary>
    /// Ternary tree that stores strings.  Provide fast search of a string OR sequence of strings for autocomplete.
    /// 
    /// The search is Case insensitive.  The tree contains all chars in lower case;
    /// 
    /// How it should be used:
    /// <example>
    /// var tree = new TernaryTree();
    /// tree.AddWord("word1");
    /// tree.AddWord("word2");
    /// tree.PrepareForSearch();
    /// tree.Contains("word");
    /// </example>
    /// 
    /// </summary>
    public class TernaryTree<T>
    {
        private class TernaryTreeNode
        {
            public TernaryTreeNode Left { get; set; }
            public TernaryTreeNode Right { get; set; }
            public TernaryTreeNode Middle { get; set; }
            public char CurrentChar;
            public T CurrentObject { get; set; }
        }
        private Dictionary<string, T> words = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        private TernaryTreeNode Root;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TernaryTree"/> is prepared for search.
        /// </summary>
        /// <value><c>true</c> if prepared for search; otherwise, <c>false</c>.</value>
        public bool IsPreparedForSearch { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TernaryTree"/> class.
        /// </summary>
        public TernaryTree()
        {
        }

        /// <summary>
        /// Adds the word.
        /// </summary>
        /// <param name="word">The word.</param>
        public void AddWord(string word, T theObject)
        {
            if (string.IsNullOrEmpty(word))
            {
                throw new ArgumentNullException("word");
            }
            if (theObject == null)
            {
                throw new ArgumentNullException("theObject");
            }
            //TODO: BUG: duplicate keys to be allowed - e.g. keyword TRUE and function TRUE(); Same for table names
            if(words.ContainsKey(word))
                return;

            words[word] = theObject;
        }

        /// <summary>
        /// Prepares for search.
        /// </summary>
        public void PrepareForSearch()
        {
            // Initialize the root
            Root = new TernaryTreeNode();

            // Sort the array
            List<string> keys = new List<string>(words.Keys.ToArray<string>());
            keys.Sort(StringComparer.InvariantCultureIgnoreCase);
    
            // Start filling the tree starting from the middle until all the elements are in.  It should give good balancing
            DichotomicTraversal(keys, 0, words.Count - 1);

            IsPreparedForSearch = true;
        }

        /// <summary>
        /// Dichotomic ( = take a middle element from left to right and call the same function for 2 intervals [left:middle], [middle:right]) the traversal.
        /// </summary>
        /// <param name="left">The index of left element.</param>
        /// <param name="right">The index of right element.</param>
        private void DichotomicTraversal(List<string> keys, int left, int right)
        {
            if (left > right || right < left)
                return;

            int middle = (left + right) / 2;
            string word = keys[middle];
            char[] chars = word.ToLowerInvariant().ToCharArray();
            StoreWordInTree(word, chars, 0, chars[0], Root);

            // Do calls for left and right parts
            DichotomicTraversal(keys, left, middle - 1);
            DichotomicTraversal(keys, middle + 1, right);
        }

        /// <summary>
        /// Stores the word in the tree.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="chars">The array of characters (must be in lower case).</param>
        /// <param name="position">The position.</param>
        /// <param name="currentChar">The current char.</param>
        /// <param name="node">The node.</param>
        private void StoreWordInTree(string word, char[] chars, int position, char currentChar, TernaryTreeNode node)
        {
            if (node.CurrentChar == 0)
            {
                Debug.Assert(node.Middle == null);
                node.CurrentChar = currentChar;
                node.Middle = new TernaryTreeNode();
                if (position + 1 == chars.Length)
                {
                    Debug.Assert(node.CurrentObject == null);
                    node.CurrentObject = words[word];
                    return;
                }
                StoreWordInTree(word, chars, position + 1, chars[position + 1], node.Middle);
            }
            else if (node.CurrentChar == currentChar)
            {
                Debug.Assert(node.Middle != null);
                if (position + 1 == chars.Length)
                {
                    Debug.Assert(node.CurrentObject == null);
                    node.CurrentObject = words[word];
                    return;
                }
                StoreWordInTree(word, chars, position + 1, chars[position + 1], node.Middle);
            }
            else if (node.CurrentChar > currentChar)
            {
                if (node.Left == null)
                {
                    node.Left = new TernaryTreeNode();
                }
                StoreWordInTree(word, chars, position, chars[position], node.Left);
            }
            else
            {
                if (node.Right == null)
                {
                    node.Right = new TernaryTreeNode();
                }
                StoreWordInTree(word, chars, position, chars[position], node.Right);
            }
        }

        /// <summary>
        /// Determines whether the tree contains the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>
        /// 	<c>true</c> if the tree contains the specified word; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string word)
        {
            if(!IsPreparedForSearch)
                throw new InvalidOperationException("The tree has to be prepared before searching.");

            char[] chars = word.ToLowerInvariant().ToCharArray();
            return Contains(chars, 0, Root);
        }

        /// <summary>
        /// Recursive function that dDetermines whether the tree contains the specified word.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="position">The position.</param>
        /// <param name="node">The node.</param>
        /// <returns>
        /// 	<c>true</c> if the tree contains the specified word; otherwise, <c>false</c>.
        /// </returns>
        private bool Contains(char[] chars, int position, TernaryTreeNode node)
        {
            if (node == null)
                return false;
            char currentChar = chars[position];
            if (node.CurrentChar == currentChar)
            {
                if (position + 1 == chars.Length)
                {
                    if (node.CurrentObject != null)
                        return true;
                    return false;
                }
                return Contains(chars, position + 1, node.Middle);
            }
            else if (node.CurrentChar < currentChar)
            {
                return Contains(chars, position, node.Right);
            }
            else
            {
                return Contains(chars, position, node.Left);
            }
        }

        /// <summary>
        /// Find matches witht the given prefix.
        /// </summary>
        /// <param name="prefix">The prefix.  Can be empty - all words from the tree are returned in this case.</param>
        /// <returns></returns>
        public IList<T> Matches(string prefix)
        {
            if (!IsPreparedForSearch)
                throw new InvalidOperationException("The tree has to be prepared before searching.");

            var result = new List<T>();
            // Step 1 - find the node node
            string prefixNormilized = string.Empty;
            if (!string.IsNullOrEmpty(prefix))
            {
                prefixNormilized = prefix.ToLowerInvariant();
            }
            TernaryTreeNode start = FindStartNode(prefixNormilized.ToCharArray(), 0, Root);
            // Step 2 - get all words starting from the node node
            result = ComposeMatchList(start, result);

            return result;
        }

        /// <summary>
        /// Finds the start node for matching.
        /// </summary>
        /// <param name="chars">The array of chars.</param>
        /// <param name="position">The current position.</param>
        /// <param name="node">The current node.</param>
        /// <returns></returns>
        private TernaryTreeNode FindStartNode(char[] chars, int position, TernaryTreeNode node)
        {
            if (position == chars.Length)
                return node;

            char currentChar = chars[position];
            if (currentChar == node.CurrentChar)
            {
                return FindStartNode(chars, position + 1, node.Middle);
            }
            else if (node.CurrentChar < currentChar)
            {
                return FindStartNode(chars, position, node.Right);
            }
            else
            {
                return FindStartNode(chars, position, node.Left);
            }
        }

        /// <summary>
        /// Composes the match list.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <param name="result">The result that contains all the words.</param>
        /// <returns></returns>
        private List<T> ComposeMatchList(TernaryTreeNode node, List<T> result)
        {
            if (node.CurrentObject != null)
            {
                result.Add(node.CurrentObject);
            }
            if (node.Left != null)
            {
                result = ComposeMatchList(node.Left, result);
            }
            if (node.Middle != null)
            {
                result = ComposeMatchList(node.Middle, result);
            }
            if (node.Right != null)
            {
                result = ComposeMatchList(node.Right, result);
            }

            return result;
        }
    }
}
