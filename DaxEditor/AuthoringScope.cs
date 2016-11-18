// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;
using DaxEditor.GeneratorSource;
using Babel.Parser;
using DaxEditor;

namespace Babel
{
    public class AuthoringScope : Microsoft.VisualStudio.Package.AuthoringScope
    {
        public AuthoringScope(Source source)
        {
            this._source = source;
        }

        public AuthoringScope(object parseResult, Source source)
        {
            this._parseResult = parseResult;
            this._source = source;

            // how should this be set?
            this._resolver = new Resolver(this._source);
        }

        Source _source;
        object _parseResult;
        IASTResolver _resolver;

        // ParseReason.QuickInfo
        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
            TokenInfo tokenInfo = this._source.GetTokenInfo(line, col);

            span = new TextSpan();
            span.iStartLine = line;
            span.iEndLine = line;
            span.iStartIndex = tokenInfo.StartIndex;
            span.iEndIndex = tokenInfo.EndIndex + 1;

            if(TokenType.Identifier == tokenInfo.Type)
            {
                // TODO: Hack, to be re-writen when tree with Declaration is ready
                foreach (var function in new DaxFunctions())
                {
                    if(string.Equals(function.Name, this._source.GetText(span), StringComparison.InvariantCultureIgnoreCase))
                    {
                        return function.Description;
                    }
                }
            }

            return null;
        }

        // ParseReason.CompleteWord
        // ParseReason.DisplayMemberList
        // ParseReason.MemberSelect
        // ParseReason.MemberSelectAndHilightBraces
        public override Microsoft.VisualStudio.Package.Declarations GetDeclarations(IVsTextView view, int line, int col, TokenInfo info, ParseReason reason)
        {
            return new Declarations(new List<Declaration>());
        }

        // ParseReason.GetMethods
        public override Microsoft.VisualStudio.Package.Methods GetMethods(int line, int col, string name)
        {
            return new Methods(_resolver.FindMethods(_parseResult, line, col, name));
        }

        // ParseReason.Goto
        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col, out TextSpan span)
        {
            // throw new System.NotImplementedException();
            span = new TextSpan();
            return null;
        }
    }
}