// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using DaxEditor.GeneratorSource;
using DaxEditor;
using Microsoft.AnalysisServices;
using Babel.Parser;

namespace Babel
{
    public class Resolver : Babel.IASTResolver
    {
        /// <summary>
        /// Ternary tree for DAX functions
        /// </summary>
        private TernaryTree<Declaration> _declarationsTree = new TernaryTree<Declaration>();

        /// <summary>
        /// BISM Info Provider for online context mode
        /// </summary>
        private BismInfoProvider _bismInfoPerovider;

        public Resolver(Source source)
        {
            // Initialize DAX functions ternary tree
            DaxFunctions functions = new DaxFunctions();
            foreach (var function in functions)
            {
                _declarationsTree.AddWord(function.Name, new Declaration() { Description = function.Description, DisplayText = function.Name, Glyph = 72, Name = function.Name });
            }
            DaxKeywords keywords = new DaxKeywords();
            foreach (var keywordDeclaration in keywords.GetDeclarations())
            {
                _declarationsTree.AddWord(keywordDeclaration.Name, keywordDeclaration);
            }
            _declarationsTree.PrepareForSearch();

            this._bismInfoPerovider = source.BismInfoProvider;
        }

        #region IASTResolver Members
        public IList<Declaration> FindCompletions(object result, int line, int col)
        {
            CompletionContext completionContext = result as CompletionContext;

            if (completionContext == null)
                return new List<Babel.Declaration>();

            List<Declaration> declarations = null;
            switch (completionContext.TokenToComplete)
            {
                case (int)Tokens.TABLENAME:
                case (int)Tokens.FUNCTION:
                default:
                    // Both Function names, Table names and KEYWORDS are IDs hence should consider
                    if (this._bismInfoPerovider != null && this._bismInfoPerovider.IsDaxFunctionInformationAvailable())
                    {
                        declarations = new List<Declaration>();
                        declarations.AddRange(this._bismInfoPerovider.GetTableDeclarations());
                        declarations.AddRange(this._bismInfoPerovider.GetTableMemberDeclarations(null));
                        declarations.AddRange(this._bismInfoPerovider.GetDaxFunctionDeclarations());

                        // Add keyword declarations
                        DaxKeywords keywords = new DaxKeywords();
                        foreach (var keywordDeclaration in keywords.GetDeclarations())
                        {
                            declarations.Add(keywordDeclaration);
                        }
                    }
                    else
                    {
                        // Get the declarations from static tree
                        return _declarationsTree.Matches(string.Empty);
                    }
                    break;

                case (int)Tokens.PARTIALTABLENAME:
                    // We have incomplete table name starting with a single quote, no function/keyword should be considered
                    if (this._bismInfoPerovider != null)
                    {
                        declarations = this._bismInfoPerovider.GetTableDeclarations();
                    }
                    break;

                case (int)Tokens.PARTIALCOLUMNNAME:
                case (int)Tokens.COLUMNNAME:
                    // We have incomplete column/measure name starting with a square bracket
                    if (this._bismInfoPerovider != null)
                    {
                        declarations = this._bismInfoPerovider.GetTableMemberDeclarations(completionContext.ParentTokenName);
                    }
                    break;
            }

            if (declarations != null)
            {
                declarations.Sort(delegate(Declaration a, Declaration b) { return a.DisplayText.CompareTo(b.DisplayText); });
            }

            return declarations;
        }

        public IList<Declaration> FindMembers(object result, int line, int col)
        {
            List<Declaration> declarations;

            string parentName = result as string;
            if (string.IsNullOrEmpty(parentName))
            {
                declarations = _bismInfoPerovider.GetMeasureDeclarations();
            }
            else
            {
                parentName = parentName.Trim('\'');
                declarations = _bismInfoPerovider.GetTableMemberDeclarations(parentName);
            }

            if (declarations != null)
            {
                declarations.Sort(delegate(Declaration a, Declaration b) { return a.DisplayText.CompareTo(b.DisplayText); });
            }

            return declarations;
        }

        public string FindQuickInfo(object result, int line, int col)
        {
            return null;
        }

        public IList<Babel.Method> FindMethods(object result, int line, int col, string name)
        {
            var resultList = new List<Babel.Method>();

            var functions = new DaxFunctions();
            foreach (var func in functions)
            {
                if(string.Equals(name, func.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    var method = new Babel.Method()
                    {
                        Name = func.Name,
                        Description = func.Description,
                        Parameters = func.Parameters,
                    };
                    resultList.Add(method);
                }
            }

            return resultList;
        }
        #endregion
    }
}
