// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Package;
using Babel.Parser;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Babel
{
    public static partial class Configuration
    {
        public const string Name = "DAX";
        public const string Extension = ".dax";

        static CommentInfo commentInfo;
        public static CommentInfo MyCommentInfo { get { return commentInfo; } }

        static Configuration()
        {
            commentInfo.BlockEnd = "*/";
            commentInfo.BlockStart = "/*";
            commentInfo.LineStart = "--";
            commentInfo.UseLineComments = true;

            // default colors - currently, these need to be declared
            CreateColor("DAX Keyword", COLORINDEX.CI_BLUE, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("DAX Comment", COLORINDEX.CI_DARKGREEN, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("DAX Identifier", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("DAX String", COLORINDEX.CI_MAROON, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("DAX Number", COLORINDEX.CI_PURPLE, COLORINDEX.CI_USERTEXT_BK);
            CreateColor("DAX Text", COLORINDEX.CI_SYSPLAINTEXT_FG, COLORINDEX.CI_USERTEXT_BK);
            TokenColor delimeterColor = CreateColor("DAX Delimeter", COLORINDEX.CI_BLACK, COLORINDEX.CI_USERTEXT_BK);
            TokenColor functionColor = CreateColor("DAX Function", COLORINDEX.CI_AQUAMARINE, COLORINDEX.CI_USERTEXT_BK);
            TokenColor error = CreateColor("DAX Error", COLORINDEX.CI_RED, COLORINDEX.CI_USERTEXT_BK, false, true);

            //
            // map tokens to color classes
            //
            ColorToken((int)Tokens.KWEVALUATE, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWDEFINE, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWMEASURE, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWORDER, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWBY, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWTRUE, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWFALSE, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWASC, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWDESC, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWCREATE, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWVAR, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWRETURN, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);

            ColorToken((int)Tokens.FUNCTION, TokenType.Identifier, functionColor, TokenTriggers.None);
            ColorToken((int)Tokens.KWDATATABLE, TokenType.Identifier, functionColor, TokenTriggers.None);
            ColorToken((int)Tokens.KWRANKX, TokenType.Identifier, functionColor, TokenTriggers.None);
            ColorToken((int)Tokens.KWFORMAT, TokenType.Identifier, functionColor, TokenTriggers.None);
            ColorToken((int)Tokens.KWCALCULATE, TokenType.Identifier, functionColor, TokenTriggers.None);
            ColorToken((int)Tokens.TABLENAME, TokenType.Identifier, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.ESCAPEDTABLENAME, TokenType.Identifier, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.COLUMNNAME, TokenType.Identifier, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.PARTIALTABLENAME, TokenType.Identifier, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.PARTIALCOLUMNNAME, TokenType.Identifier, TokenColor.Text, TokenTriggers.None);


            ColorToken((int)Tokens.NUMBER, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.STRING, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.KWBOOLEAN, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.KWCURRENCY, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.KWDATETIME, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.KWDOUBLE, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.KWINTEGER, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.KWSTRING, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.KWSKIP, TokenType.Literal, TokenColor.String, TokenTriggers.None);
            ColorToken((int)Tokens.KWDENSE, TokenType.Literal, TokenColor.String, TokenTriggers.None);

            ColorToken((int)Tokens.KWCALCULATION, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
            ColorToken((int)Tokens.KWPROPERTY, TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);

            ColorToken((int)'(', TokenType.Delimiter, delimeterColor, TokenTriggers.MatchBraces | TokenTriggers.ParameterStart);
            ColorToken((int)')', TokenType.Delimiter, delimeterColor, TokenTriggers.MatchBraces | TokenTriggers.ParameterEnd);
            ColorToken((int)Tokens.LEFTSQUAREBRACKET, TokenType.Delimiter, delimeterColor, TokenTriggers.MatchBraces | TokenTriggers.MemberSelect);
            ColorToken((int)Tokens.RIGHTSQUAREBRACKET, TokenType.Delimiter, delimeterColor, TokenTriggers.MatchBraces);
            ColorToken((int)',', TokenType.Delimiter, delimeterColor, TokenTriggers.ParameterNext);
            ColorToken((int)';', TokenType.Delimiter, delimeterColor, TokenTriggers.None);
            ColorToken((int)'.', TokenType.Delimiter, delimeterColor, TokenTriggers.None);

            ColorToken((int)Tokens.POW, TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)'+', TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)'-', TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)'*', TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)'/', TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)'!', TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)'&', TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)'|', TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.EQ, TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.NEQ, TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.GT, TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.GTE, TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.LT, TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.LTE, TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.AMPAMP, TokenType.Operator, TokenColor.Text, TokenTriggers.None);
            ColorToken((int)Tokens.BARBAR, TokenType.Operator, TokenColor.Text, TokenTriggers.None);


            //// Extra token values internal to the scanner
            ColorToken((int)Tokens.LEX_ERROR, TokenType.Unknown, error, TokenTriggers.None);
            ColorToken((int)Tokens.LEX_COMMENT, TokenType.Comment, TokenColor.Comment, TokenTriggers.None);
            ColorToken((int)Tokens.LEX_WHITE, TokenType.WhiteSpace, TokenColor.Text, TokenTriggers.None);

        }
    }
}
