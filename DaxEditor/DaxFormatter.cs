// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babel;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Diagnostics;
using Babel.Parser;

namespace DaxEditor
{
    public class DaxFormatter
    {
        public enum EolType { Unknown = 0, Eol, EolIndent, EolIndentPlus, EolIndentMinus, EolHardIndent1 }

        public class TokenEdit
        {
            public TokenInfo TokenInfo { get; set; }
            public EditSpan EditSpan { get; set; }
            public string OriginalText { get; set; }
            public TextSpan TextSpan { get; set; }
            public EolType EolType { get; set; }
        }

        internal class DummyColorState : IVsTextColorState
        {
            #region IVsTextColorState Members

            public int GetColorStateAtStartOfLine(int iLine, out int piState)
            {
                piState = 0;
                return 0;
            }

            public int ReColorizeLines(int iTopLine, int iBottomLine)
            {
                return 0;
            }

            #endregion
        }


        [Serializable]
        public class TokenEditLinkedList : LinkedList<TokenEdit>
        {
            public TokenEditLinkedList()
            {
            }

            protected TokenEditLinkedList(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) :
                base(info, context)
            {
            }

            public LinkedListNode<TokenEdit> PreviousNotBlank(LinkedListNode<TokenEdit> currentNode)
            {
                var result = currentNode;
                while (result != null && !IsBlankNode(result))
                {
                    result = result.Previous;
                }
                return result;
            }

            public LinkedListNode<TokenEdit> NextNotBlank(LinkedListNode<TokenEdit> currentNode)
            {
                var result = currentNode;
                while (result != null && !IsBlankNode(result))
                {
                    result = result.Next;
                }
                return result;
            }

            public static bool IsBlankNode(LinkedListNode<TokenEdit> node)
            {
                Debug.Assert(node != null);
                Debug.Assert(node.Value != null);
                Debug.Assert(node.Value.TokenInfo != null);

                if(node.Value.TokenInfo.Type == TokenType.WhiteSpace || node.Value.TokenInfo.Type== TokenType.Comment)
                    return false;

                return true;
            }
        }

        private Babel.Source _source;
        private EditArray _editManager;
        private TextSpan _formattingSpan;
        private TokenEditLinkedList _tokenEdits = new TokenEditLinkedList();
        private DummyColorState _colorState = new DummyColorState();
        private string _eolText;
        private int _indent = 0;
        private LanguagePreferences _languagePreferences;
        private Stack<LinkedListNode<TokenEdit>> _stackForParenthesiss = new Stack<LinkedListNode<TokenEdit>>();
        private int _indentDepth = 0;
        private int _functionDepth = 0;
        private string _formatterType;

        public DaxFormatter(Babel.Source source, EditArray mgr, TextSpan formattingSpan, string eolText, LanguagePreferences languagePreferences, int indentDepth, string formatterType)
        {
            _source = source;
            _editManager = mgr;
            _formattingSpan = formattingSpan;
            _eolText = eolText;
            _languagePreferences = languagePreferences;
            _indentDepth = indentDepth;
            _formatterType = formatterType;
        }

        public void Format()
        {
            switch (_formatterType)
            {
                case DaxFormattingPage.FormatDaxFormatterEU:
                    FormatByDaxFormatter(DaxFormatterCom.FormattingCulture.EU);
                    break;

                case DaxFormattingPage.FormatDaxFormatterUS:
                    FormatByDaxFormatter(DaxFormatterCom.FormattingCulture.US);
                    break;
                
                case DaxFormattingPage.FormatDaxEditor:
                    FormatByDaxEditor();
                    break;

                default:
                    throw new NotImplementedException("Not implemented formatter.");
            }
        }

        private void FormatByDaxFormatter(DaxFormatterCom.FormattingCulture formattingCulture)
        {
            string originalText = _source.GetText(_formattingSpan);
            if (!string.IsNullOrEmpty(originalText))
            {
                try
                {
                    var formattedText = DaxFormatterCom.Format(originalText, formattingCulture);
                    _editManager.Add(new EditSpan(_formattingSpan, formattedText));
                }
                catch(Exception e)
                {
                    DaxEditorPackage.WriteToGeneral(e.Message);
                    System.Windows.MessageBox.Show(e.Message, "Exception from daxformatter.com", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void FormatByDaxEditor()
        {
            int lineCont = _source.GetLineCount();

            for (int line = 0; line < lineCont; line++)
            {
                int lineEnd = 0;
                TokenInfo[] lineInfos = _source.GetColorizer().GetLineInfo(_source.GetTextLines(), line, _colorState);
                if (lineInfos != null)
                {
                    for (int i = 0; i < lineInfos.Length; i++)
                    {
                        // Store it for future usage
                        TokenInfo tokenInfo = lineInfos[i];
                        var te = new TokenEdit() { TokenInfo = tokenInfo };
                        te.TextSpan = new TextSpan() { iStartLine = line, iStartIndex = tokenInfo.StartIndex, iEndLine = line, iEndIndex = tokenInfo.EndIndex + 1 };
                        lineEnd = tokenInfo.EndIndex + 1;
                        te.OriginalText = _source.GetText(te.TextSpan);
                        if (te.TokenInfo.Token == (int)Tokens.LEX_ERROR)
                        {
                            te = ReplaceInvalidChars(te);
                        }
                        _tokenEdits.AddLast(te);
                    }
                }

                // Insert token for EOL
                var tokenInfoEOL = new TokenInfo() { Type = TokenType.WhiteSpace };
                var teEOL = new TokenEdit() { TokenInfo = tokenInfoEOL };
                teEOL.TextSpan = new TextSpan() { iStartLine = line, iStartIndex = lineEnd, iEndLine = line + 1, iEndIndex = 0 };
                teEOL.OriginalText = "\r\n";
                _tokenEdits.AddLast(teEOL);
            }

            NormilizeWhitespaces();
            InsertEol();
            CheckHeadAndTail();

            ApplyEdits();
        }

        /// <summary>
        /// Replace invalid chars in DAX with valid.  Example '&npsp' with a space
        /// </summary>
        /// <param name="te">input token edit</param>
        /// <returns></returns>
        private TokenEdit ReplaceInvalidChars(TokenEdit te)
        {
            Debug.Assert(te.TokenInfo.Token == (int) Tokens.LEX_ERROR);
            switch (te.OriginalText)
            {
                case "–": // dash
                    te.TokenInfo.Token = (int)'-';
                    te.TokenInfo.Type = TokenType.Operator;
                    te.EditSpan = new EditSpan(te.TextSpan, "-");
                    break;

                case " ": // &nbsp
                    te.TokenInfo.Token = (int) Tokens.LEX_WHITE;
                    te.TokenInfo.Type = TokenType.WhiteSpace;
                    te.EditSpan = new EditSpan(te.TextSpan, " ");
                    break;
            }

            return te;
        }

        /// <summary>
        /// Replaces any whitespace with ' '(=space) and insert a whitespace after every token (except comment)
        /// Also expand change token type for multi line comment.  It is required because Lexer does analyzis
        /// line by line and it does not have information that the current token might be a comment token.
        /// </summary>
        private void NormilizeWhitespaces()
        {
            var currentNode = _tokenEdits.First;
            while (currentNode != null)
            {
                var previous = currentNode.Previous;

                // Expand multi line comments
                // Lexer used in the callee function works line by line
                // So if it scans 2 lines /*\nabc\n the first line is a comment
                // but 'abc' on the second line is not a comment.
                // Fix it with this check
                if (previous != null && currentNode.Value.TokenInfo.Type != TokenType.Comment && previous.Value.TokenInfo.Type == TokenType.Comment && !string.Equals(previous.Value.OriginalText, @"*/"))
                    currentNode.Value.TokenInfo.Type = TokenType.Comment;

                switch (currentNode.Value.TokenInfo.Type)
                {
                    case TokenType.LineComment:
                    case TokenType.Comment:
                        break;

                    case TokenType.Delimiter:
                    case TokenType.Identifier:
                    case TokenType.Keyword:
                    case TokenType.Literal:
                    case TokenType.Operator:
                    case TokenType.String:
                    case TokenType.Text:
                        // Insert whitespace before these tokens.  There are some exceptions - like there must be no whitespace between function and opening Parenthesis
                        if (previous != null)
                        {
                            if (previous.Value.TokenInfo.Type != TokenType.WhiteSpace)
                            {
                                // No whitespace between 2 identifiers
                                if (previous.Value.TokenInfo.Type == TokenType.Identifier && currentNode.Value.TokenInfo.Type == TokenType.Identifier)
                                    break;
                                // No whitespaces before ';'
                                if (currentNode.Value.TokenInfo.Type == TokenType.Delimiter && string.Equals(currentNode.Value.OriginalText, ";"))
                                    break;

                                var te = new TokenEdit();
                                te.TokenInfo = new TokenInfo() {Type = TokenType.WhiteSpace };
                                te.TextSpan = new TextSpan()
                                    {
                                        iStartLine = currentNode.Value.TextSpan.iStartLine,
                                        iStartIndex = currentNode.Value.TextSpan.iStartIndex,
                                        iEndLine = currentNode.Value.TextSpan.iStartLine,
                                        iEndIndex = currentNode.Value.TextSpan.iStartIndex
                                    };

                                te.EditSpan = new EditSpan(
                                    te.TextSpan,
                                    " ");

                                _tokenEdits.AddAfter(previous, te);
                                currentNode = previous; // Need to walk at the currently added node.  Side effect - this block might be executed 2 times
                            }
                        }
                        break;

                    case TokenType.Unknown:
                        Debug.Assert(false);
                        break;

                    case TokenType.WhiteSpace:
                        TextSpan span;
                        string newWhitespace = " ";
                        // Do not insert whitespace after comments
                        if (previous != null && (previous.Value.TokenInfo.Type == TokenType.LineComment || previous.Value.TokenInfo.Type == TokenType.Comment))
                        {
                            break;
                        }
                        // If previous token is a whitespace as well - merge their spans and remove the previos token
                        if (previous != null && previous.Value.TokenInfo.Type == TokenType.WhiteSpace)
                        {
                            if (previous.Previous != null && (previous.Previous.Value.TokenInfo.Type == TokenType.LineComment || previous.Previous.Value.TokenInfo.Type == TokenType.Comment))
                                break;
                            span.iStartLine = previous.Value.TextSpan.iStartLine;
                            span.iStartIndex = previous.Value.TextSpan.iStartIndex;
                            span.iEndLine = currentNode.Value.TextSpan.iEndLine;
                            span.iEndIndex = currentNode.Value.TextSpan.iEndIndex;
                            _tokenEdits.Remove(previous);
                            previous = currentNode.Previous;
                        }
                        else
                        {
                            span = currentNode.Value.TextSpan;
                        }
                        // In serven cases whitespace needs to be removed or replaced with EOL
                        if (previous != null && currentNode.Next != null)
                        {
                            if (
                                // [cube].
                                (currentNode.Next.Value.TokenInfo.Type == TokenType.Delimiter && string.Equals(currentNode.Next.Value.OriginalText, "."))
                                // .[cube]
                                || (previous.Value.TokenInfo.Type == TokenType.Delimiter && string.Equals(previous.Value.OriginalText, "."))
                                // TableRef[ColRef]
                                || (previous.Value.TokenInfo.Type == TokenType.Identifier && currentNode.Next.Value.TokenInfo.Type == TokenType.Identifier)
                                // just before comma
                                || (currentNode.Next.Value.TokenInfo.Type == TokenType.Delimiter && string.Equals(currentNode.Next.Value.OriginalText, ","))
                                )
                            {
                                newWhitespace = "";
                            }
                            else if (previous.Value.TokenInfo.Type == TokenType.Keyword && string.Equals("evaluate", previous.Value.OriginalText, StringComparison.CurrentCultureIgnoreCase))
                            {
                                currentNode.Value.EolType = EolType.Eol;
                            }
                            else if (currentNode.Next.Value.TokenInfo.Type == TokenType.Keyword)
                            {
                                // Measure starts with a new line, 1 indent, only if the previous token is not CREATE
                                if (string.Equals("measure", currentNode.Next.Value.OriginalText, StringComparison.CurrentCultureIgnoreCase)
                                    && previous != null && !string.Equals("create", previous.Value.OriginalText, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    currentNode.Value.EolType = EolType.EolHardIndent1;
                                    newWhitespace = _eolText;
                                }
                                // EVALUATE, ORDER and CREATE always starts with a new line, 0 indent
                                else if (string.Equals("evaluate", currentNode.Next.Value.OriginalText, StringComparison.CurrentCultureIgnoreCase)
                                    || string.Equals("order", currentNode.Next.Value.OriginalText, StringComparison.CurrentCultureIgnoreCase)
                                    || string.Equals("create", currentNode.Next.Value.OriginalText, StringComparison.CurrentCultureIgnoreCase)
                                 )
                                {
                                    newWhitespace = _eolText;
                                    currentNode.Value.EolType = EolType.Eol;
                                }
                            }
                        }


                        currentNode.Value.EditSpan = new EditSpan(span, newWhitespace);
                        currentNode.Value.TextSpan = span;
                        break;

                    default:
                        break;
                }

                currentNode = currentNode.Next;
            }
        }

        /// <summary>
        /// Insert EOL with or without indent
        /// </summary>
        private void InsertEol()
        {
            var currentNode = _tokenEdits.First;
            while (currentNode != null)
            {
                var previous = currentNode.Previous;

                switch (currentNode.Value.TokenInfo.Type)
                {
                    case TokenType.Delimiter:
                        if (string.Equals(currentNode.Value.OriginalText, "("))
                            OnOpeningParenthesis(currentNode);
                        else if (string.Equals(currentNode.Value.OriginalText, ")"))
                                OnClosingParenthesis(currentNode);
                        else if (string.Equals(currentNode.Value.OriginalText, ","))
                            _stackForParenthesiss.Push(currentNode);

                        break;
                    case TokenType.Identifier:
                        if (currentNode.Value.TokenInfo.Token == (int) Tokens.FUNCTION)
                        {
                            _stackForParenthesiss.Push(currentNode);
                            _functionDepth++;
                        }
                        break;

                    case TokenType.LineComment:
                    case TokenType.Comment:
                    case TokenType.Keyword:
                    case TokenType.Literal:
                    case TokenType.Operator:
                    case TokenType.String:
                    case TokenType.Text:
                    case TokenType.WhiteSpace:
                        break;

                    default:
                    case TokenType.Unknown:
                        Debug.Assert(false);
                        break;
                }

                currentNode = currentNode.Next;
            }
        }

        // Remove empty whitespaces from the beggining and the end of the query
        private void CheckHeadAndTail()
        {
            if (_tokenEdits.First.Value != null && _tokenEdits.First.Value.OriginalText == null)
            {
                _tokenEdits.RemoveFirst();
            }

            if (_tokenEdits.Last.Value != null && _tokenEdits.Last.Value.OriginalText == null)
            {
                _tokenEdits.RemoveLast();
            }
        }

        private void OnOpeningParenthesis(LinkedListNode<TokenEdit> openningParenthesis)
        {
            if (_stackForParenthesiss.Count > 0 && _stackForParenthesiss.Peek().Value.TokenInfo.Type == TokenType.Identifier)
            {
                // EOL on the next whitespace
                var whitespaceAfter = openningParenthesis.Next;
                if (whitespaceAfter != null && whitespaceAfter.Value.TokenInfo.Type == TokenType.WhiteSpace)
                {
                    whitespaceAfter.Value.EolType = EolType.EolIndentPlus;
                }

            }
            _stackForParenthesiss.Push(openningParenthesis);
        }

        private void OnClosingParenthesis(LinkedListNode<TokenEdit> closingParenthesisEdit)
        {
            if (_stackForParenthesiss.Count == 0)
                return;
            List<LinkedListNode<TokenEdit>> parentheses = new List<LinkedListNode<TokenEdit>>();
            while (_stackForParenthesiss.Count > 0 && !string.Equals(_stackForParenthesiss.Peek().Value.OriginalText, "("))
            {
                var parenthesis = _stackForParenthesiss.Pop();
                Debug.Assert(string.Equals(parenthesis.Value.OriginalText, ","));
                parentheses.Add(parenthesis);
            }

            var openParenthesisEdit = _stackForParenthesiss.Pop();
            // Do work only if a function is just before the openeing Parenthesis
            if (_stackForParenthesiss.Count > 0 && _stackForParenthesiss.Peek().Value.TokenInfo.Type == TokenType.Identifier)
            {
                string textBetweenParenthesiss = _source.GetText(openParenthesisEdit.Value.TextSpan.iStartLine
                    , openParenthesisEdit.Value.TextSpan.iStartIndex
                    , closingParenthesisEdit.Value.TextSpan.iEndLine
                    , closingParenthesisEdit.Value.TextSpan.iEndIndex);
#if OLD_CODE
                bool indentFunctionContent = !IsLongStringBetweenParenthesiss(textBetweenParenthesiss);
#endif
                bool hasInnerOpenParenthesis = textBetweenParenthesiss.IndexOf('(', 1) > 0;

                bool indentFunctionContent = (_indentDepth > _functionDepth) && hasInnerOpenParenthesis;

                _stackForParenthesiss.Pop();
                _functionDepth--;
                var whitespace = closingParenthesisEdit.Previous;
                if (indentFunctionContent == true && whitespace != null && whitespace.Value.TokenInfo.Type == TokenType.WhiteSpace)
                {
                    // Functions without parameters (e.g. Blank()) use the same whitespace inside the parentheses for both IndentPlus and Minus, optimize it for just Indent
                    if (whitespace.Value.EolType == EolType.EolIndentPlus)
                        whitespace.Value.EolType = EolType.EolIndent;
                    else
                        whitespace.Value.EolType = EolType.EolIndentMinus;

                    // Insert EOL after every parenthesis
                    foreach (var par in parentheses)
                    {
                        Debug.Assert(par.Next.Value.TokenInfo.Type == TokenType.WhiteSpace);
                        par.Next.Value.EolType = EolType.EolIndent;
                    }
                }

                if (!indentFunctionContent)
                {
                    Debug.Assert(openParenthesisEdit.Next.Value.TokenInfo.Type == TokenType.WhiteSpace);
                    openParenthesisEdit.Next.Value.EolType = EolType.Unknown;
                }
            }
        }


        /// <summary>
        /// Determines whether a string between function Parenthesiss is long enough to be separated on multiple lines or not.
        /// </summary>
        /// <param name="textBetweenParenthesiss">The text between Parenthesiss.</param>
        /// <returns>
        /// 	<c>true</c> if a string between function Parenthesiss is long enough to be separated on multiple lines; otherwise, <c>false</c>.
        /// </returns>
        private bool IsLongStringBetweenParenthesiss(string textBetweenParenthesiss)
        {
            char[] chars = textBetweenParenthesiss.ToCharArray();
            int numOfNotWhiteSpaces = 0;
            foreach(char ch in chars)
            {
                if(!char.IsWhiteSpace(ch))
                    numOfNotWhiteSpaces ++;

                if(numOfNotWhiteSpaces > 100)
                    return true;
            }

            return false;
        }

        private string IndentString()
        {
            if(_languagePreferences.InsertTabs)
                return new string('\t', _indent);

            return new string(' ', _languagePreferences.IndentSize * _indent);
        }

        private void ApplyEdits()
        {
            var currentNode = _tokenEdits.First;
            while (currentNode != null)
            {
                if ( TextSpanHelper.Intersects(_formattingSpan, currentNode.Value.TextSpan) )
                {
                    if(currentNode.Value.EolType != EolType.Unknown)
                    {
                        switch (currentNode.Value.EolType)
                        {
                            case EolType.Eol:
                                currentNode.Value.EditSpan = new EditSpan(currentNode.Value.TextSpan, _eolText);
                                _indent = 0;
                                break;
                            case EolType.EolIndent:
                                currentNode.Value.EditSpan = new EditSpan(currentNode.Value.TextSpan, _eolText + IndentString());
                                break;
                            case EolType.EolIndentPlus:
                                _indent++;
                                currentNode.Value.EditSpan = new EditSpan(currentNode.Value.TextSpan, _eolText + IndentString());
                                break;
                            case EolType.EolIndentMinus:
                                _indent--;
                                Debug.Assert(_indent >= 0);
                                if (_indent < 0)
                                    _indent = 0;
                                currentNode.Value.EditSpan = new EditSpan(currentNode.Value.TextSpan, _eolText + IndentString());
                                break;
                            case EolType.EolHardIndent1:
                                _indent = 1;
                                currentNode.Value.EditSpan = new EditSpan(currentNode.Value.TextSpan, _eolText + IndentString());
                                break;
                            case EolType.Unknown:
                            default:
                                Debug.Fail("Unreashable code");
                                break;
                        }
                    }
                    
                    if(currentNode.Value.EditSpan != null)
                    {
                        _editManager.Add(currentNode.Value.EditSpan);
                    }
                }
                currentNode = currentNode.Next;
            }
        }
    }
}
