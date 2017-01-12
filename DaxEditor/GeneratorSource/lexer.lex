/*
The project released under MS-PL license https://daxeditor.codeplex.com/license
*/

%using Babel;
%using Babel.Parser;
%using DaxEditor.GeneratorSource;
%using System.Diagnostics;

%namespace Babel.Lexer


%x COMMENT

%{
       int GetIdToken(string txt)
       {
            if(DaxFunctions.GetFunctionsTree().Contains(txt))
            {
                return (int)Tokens.FUNCTION;
            }

            return (int)Tokens.TABLENAME;
       }
       
       internal void LoadYylval()
       {
           yylval.str = tokTxt;
           yylloc = new LexLocation(tokLin, tokCol, tokLin, tokECol);
       }
       
       public override void yyerror(string s, params object[] a)
       {
           if (handler != null) handler.AddError(s, tokLin, tokCol, tokECol - tokCol + 1, 2);  // Sev 2 = error
       }

       public void SetSourceText(string text)
       {
            SetSource(text, 0);
            InitializeLineStartArray(text);
       }

       private int[] _lineStartArray = null;

       public override string GetText(LexLocation location)
       {
            Debug.Assert(_lineStartArray != null);

            int startPosition = FindBegginingOfLinePosition(location.sLin) + location.sCol;
            int endPosition = FindBegginingOfLinePosition(location.eLin) + location.eCol;

            Debug.Assert(endPosition > startPosition);

            return buffer.GetString(startPosition, endPosition);
       }

       private void InitializeLineStartArray(string text)
       {
            Debug.Assert(_lineStartArray == null);
            var lineStartList = new List<int>();
            lineStartList.Add(0);

            // Expected EOL - \n and \r\n
            var textChars = text.ToCharArray();
            for(int position = 1; position < textChars.Length; ++position)
            {
                char prevChar = textChars[position - 1];
                if(prevChar == '\n')
                    lineStartList.Add(position);
            }
            lineStartList.Add(textChars.Length - 1);

            _lineStartArray = lineStartList.ToArray();
       }

       /// <summary>
       /// Returns start position for the specified line.  Line is 1-based index.
       /// </summary>
       private int FindBegginingOfLinePosition(int line)
       {
            Debug.Assert(line > 0 && line < _lineStartArray.Length);
            return _lineStartArray[line - 1];
       }
%}


White0          [ \t\r\f\v]
White           {White0}|\n

CmntStart            \/\*
CmntEnd              \*\/
LineComment1         --
LineComment2         \/\/
ABStar               [^\*\n]*
StringBeginEnd       \"
StringContent        [^\"\n]*
ColumnName           [^\]\n]+
TableNameEscaped     [^\'\n]+
TableNameNotEscaped  [a-zA-Z_][a-zA-Z0-9_]

%%
/* Begin generated code */
[Ee][Vv][Aa][Ll][Uu][Aa][Tt][Ee]                                                                        { return (int)Tokens.KWEVALUATE; }
[Dd][Ee][Ff][Ii][Nn][Ee]                                                                                { return (int)Tokens.KWDEFINE; }
[Mm][Ee][Aa][Ss][Uu][Rr][Ee]                                                                            { return (int)Tokens.KWMEASURE; }
[Oo][Rr][Dd][Ee][Rr]                                                                                    { return (int)Tokens.KWORDER; }
[Bb][Yy]                                                                                                { return (int)Tokens.KWBY; }
[Tt][Rr][Uu][Ee]                                                                                        { return (int)Tokens.KWTRUE; }
[Ff][Aa][Ll][Ss][Ee]                                                                                    { return (int)Tokens.KWFALSE; }
[Aa][Ss][Cc]                                                                                            { return (int)Tokens.KWASC; }
[Dd][Ee][Ss][Cc]                                                                                        { return (int)Tokens.KWDESC; }
[Dd][Aa][Yy]                                                                                            { return (int)Tokens.KWDAY; }
[Mm][Oo][Nn][Tt][Hh]                                                                                    { return (int)Tokens.KWMONTH; }
[Yy][Ee][Aa][Rr]                                                                                        { return (int)Tokens.KWYEAR; }
[Cc][Rr][Ee][Aa][Tt][Ee]                                                                                { return (int)Tokens.KWCREATE; }
[Cc][Aa][Ll][Cc][Uu][Ll][Aa][Tt][Ee]                                                                    { return (int)Tokens.KWCALCULATE; }
[Cc][Aa][Ll][Cc][Uu][Ll][Aa][Tt][Ii][Oo][Nn]                                                            { return (int)Tokens.KWCALCULATION; }
[Pp][Rr][Oo][Pp][Ee][Rr][Tt][Yy]                                                                        { return (int)Tokens.KWPROPERTY; }
[Gg][Ee][Nn][Ee][Rr][Aa][Ll]                                                                            { return (int)Tokens.KWGENERAL; }
[Nn][Uu][Mm][Bb][Ee][Rr][Dd][Ee][Cc][Ii][Mm][Aa][Ll]                                                    { return (int)Tokens.KWNUMBERDECIMAL; }
[Nn][Uu][Mm][Bb][Ee][Rr][Ww][Hh][Oo][Ll][Ee]                                                            { return (int)Tokens.KWNUMBERWHOLE; }
[Pp][Ee][Rr][Cc][Ee][Nn][Tt][Aa][Gg][Ee]                                                                { return (int)Tokens.KWPERCENTAGE; }
[Ss][Cc][Ii][Ee][Nn][Tt][Ii][Ff][Ii][Cc]                                                                { return (int)Tokens.KWSCIENTIFIC; }
[Cc][Uu][Rr][Rr][Ee][Nn][Cc][Yy]                                                                        { return (int)Tokens.KWCURRENCY; }
[Dd][Aa][Tt][Ee][Tt][Ii][Mm][Ee][Cc][Uu][Ss][Tt][Oo][Mm]                                                { return (int)Tokens.KWDATETIMECUSTOM; }
[Dd][Aa][Tt][Ee][Tt][Ii][Mm][Ee][Ss][Hh][Oo][Rr][Tt][Dd][Aa][Tt][Ee][Pp][Aa][Tt][Tt][Ee][Rr][Nn]        { return (int)Tokens.KWDATETIMESHORTDATEPATTERN; }
[Dd][Aa][Tt][Ee][Tt][Ii][Mm][Ee][Gg][Ee][Nn][Ee][Rr][Aa][Ll]                                            { return (int)Tokens.KWDATETIMEGENERAL; }
[Tt][Ee][Xx][Tt]                                                                                        { return (int)Tokens.KWTEXT; }
[Aa][Cc][Cc][Uu][Rr][Aa][Cc][Yy]                                                                        { return (int)Tokens.KWACCURACY; }
[Tt][Hh][Oo][Uu][Ss][Aa][Nn][Dd][Ss][Ee][Pp][Aa][Rr][Aa][Tt][Oo][Rr]                                    { return (int)Tokens.KWTHOUSANDSEPARATOR; }
[Ff][Oo][Rr][Mm][Aa][Tt]                                                                                { return (int)Tokens.KWFORMAT; }
[Aa][Dd][Dd][Ii][Tt][Ii][Oo][Nn][Aa][Ll][Ii][Nn][Ff][Oo]                                                { return (int)Tokens.KWADDITIONALINFO; }
[Kk][Pp][Ii]                                                                                            { return (int)Tokens.KWKPI; }
[Vv][Ii][Ss][Ii][Bb][Ll][Ee]                                                                            { return (int)Tokens.KWVISIBLE; }
[Dd][Ee][Ss][Cc][Rr][Ii][Pp][Tt][Ii][Oo][Nn]                                                            { return (int)Tokens.KWDESCRIPTION; }
[Dd][Ii][Ss][Pp][Ll][Aa][Yy][Ff][Oo][Ll][Dd][Ee][Rr]                                                    { return (int)Tokens.KWDISPLAYFOLDER; }
[Vv][Aa][Rr]                                                                                            { return (int)Tokens.KWVAR; }
[Rr][Ee][Tt][Uu][Rr][Nn]                                                                                { return (int)Tokens.KWRETURN; }
[Dd][Aa][Tt][Aa][Tt][Aa][Bb][Ll][Ee]                                                                    { return (int)Tokens.KWDATATABLE; }
[Bb][Oo][Oo][Ll][Ee][Aa][Nn]                                                                            { return (int)Tokens.KWBOOLEAN; }
[Dd][Aa][Tt][Ee][Tt][Ii][Mm][Ee]                                                                        { return (int)Tokens.KWDATETIME; }
[Dd][Oo][Uu][Bb][Ll][Ee]                                                                                { return (int)Tokens.KWDOUBLE; }
[Ii][Nn][Tt][Ee][Gg][Ee][Rr]                                                                            { return (int)Tokens.KWINTEGER; }
[Ss][Tt][Rr][Ii][Nn][Gg]                                                                                { return (int)Tokens.KWSTRING; }
[Rr][Aa][Nn][Kk][Xx]                                                                                    { return (int)Tokens.KWRANKX; }
[Ss][Kk][Ii][Pp]                                                                                        { return (int)Tokens.KWSKIP; }
[Dd][Ee][Nn][Ss][Ee]                                                                                    { return (int)Tokens.KWDENSE; }
[Nn][Oo][Tt]                                                                                            { return (int)Tokens.KWNOT; }
[Aa][Ss]                                                                                                { return (int)Tokens.KWAS; }
[Aa][Ss][Ss][Oo][Cc][Ii][Aa][Tt][Ee][Dd][__][Mm][Ee][Aa][Ss][Uu][Rr][Ee][__][Gg][Rr][Oo][Uu][Pp]        { return (int)Tokens.KWASSOCIATED_MEASURE_GROUP; }
[Gg][Oo][Aa][Ll]                                                                                        { return (int)Tokens.KWGOAL; }
[Ss][Tt][Aa][Tt][Uu][Ss]                                                                                { return (int)Tokens.KWSTATUS; }
[Ss][Tt][Aa][Tt][Uu][Ss][__][Gg][Rr][Aa][Pp][Hh][Ii][Cc]                                                { return (int)Tokens.KWSTATUS_GRAPHIC; }
[Tt][Rr][Ee][Nn][Dd]                                                                                    { return (int)Tokens.KWTREND; }
[Tt][Rr][Ee][Nn][Dd][__][Gg][Rr][Aa][Pp][Hh][Ii][Cc]                                                    { return (int)Tokens.KWTREND_GRAPHIC; }
[Kk][Pp][Ii][Dd][Ee][Ss][Cc][Rr][Ii][Pp][Tt][Ii][Oo][Nn]                                                { return (int)Tokens.KWKPIDESCRIPTION; }
[Kk][Pp][Ii][Tt][Aa][Rr][Gg][Ee][Tt][Ff][Oo][Rr][Mm][Aa][Tt][Ss][Tt][Rr][Ii][Nn][Gg]                    { return (int)Tokens.KWKPITARGETFORMATSTRING; }
[Kk][Pp][Ii][Tt][Aa][Rr][Gg][Ee][Tt][Dd][Ee][Ss][Cc][Rr][Ii][Pp][Tt][Ii][Oo][Nn]                        { return (int)Tokens.KWKPITARGETDESCRIPTION; }
[Kk][Pp][Ii][Tt][Aa][Rr][Gg][Ee][Tt][Ee][Xx][Pp][Rr][Ee][Ss][Ss][Ii][Oo][Nn]                            { return (int)Tokens.KWKPITARGETEXPRESSION; }
[Kk][Pp][Ii][Ss][Tt][Aa][Tt][Uu][Ss][Gg][Rr][Aa][Pp][Hh][Ii][Cc]                                        { return (int)Tokens.KWKPISTATUSGRAPHIC; }
[Kk][Pp][Ii][Ss][Tt][Aa][Tt][Uu][Ss][Dd][Ee][Ss][Cc][Rr][Ii][Pp][Tt][Ii][Oo][Nn]                        { return (int)Tokens.KWKPISTATUSDESCRIPTION; }
[Kk][Pp][Ii][Ss][Tt][Aa][Tt][Uu][Ss][Ee][Xx][Pp][Rr][Ee][Ss][Ss][Ii][Oo][Nn]                            { return (int)Tokens.KWKPISTATUSEXPRESSION; }
[Kk][Pp][Ii][Tt][Rr][Ee][Nn][Dd][Gg][Rr][Aa][Pp][Hh][Ii][Cc]                                            { return (int)Tokens.KWKPITRENDGRAPHIC; }
[Kk][Pp][Ii][Tt][Rr][Ee][Nn][Dd][Dd][Ee][Ss][Cc][Rr][Ii][Pp][Tt][Ii][Oo][Nn]                            { return (int)Tokens.KWKPITRENDDESCRIPTION; }
[Kk][Pp][Ii][Tt][Rr][Ee][Nn][Dd][Ee][Xx][Pp][Rr][Ee][Ss][Ss][Ii][Oo][Nn]                                { return (int)Tokens.KWKPITRENDEXPRESSION; }
[Kk][Pp][Ii][Aa][Nn][Nn][Oo][Tt][Aa][Tt][Ii][Oo][Nn][Ss]                                                { return (int)Tokens.KWKPIANNOTATIONS; }
/* End generated code */

[Mm][Ee][Mm][Bb][Ee][Rr][^;]*           { return (int)Tokens.MDXCODE; }
[Al][Ll][Tt][Ee][Rr][^;]*               { return (int)Tokens.KWALTER; }

\[{ColumnName}                          { return (int)Tokens.PARTIALCOLUMNNAME; }
'{TableNameEscaped}                     { return (int)Tokens.PARTIALTABLENAME; }
\[{ColumnName}\]                        { return (int)Tokens.COLUMNNAME; }
'{TableNameEscaped}'                    { return (int)Tokens.ESCAPEDTABLENAME; }
{TableNameNotEscaped}                   { return GetIdToken(yytext); }

[a-zA-Z_][a-zA-Z0-9_.]*                  { return GetIdToken(yytext); }
[0-9]+(\.[0-9]+([eE]([\+\-])*[0-9]+)*)*                       { return (int)Tokens.NUMBER; }
\.[0-9]+([eE]([\+\-])*[0-9]+)*                                { return (int)Tokens.NUMBER; }

;                         { return (int)';';    }
,                         { return (int)',';    }
\(                        { return (int)'(';    }
\)                        { return (int)')';    }
\{                        { return (int)'{';    }
\}                        { return (int)'}';    }
\^                        { return (int) Tokens.POW;    }
\+                        { return (int)'+';    }
\-                        { return (int)'-';    }
\*                        { return (int)'*';    }
\/                        { return (int)'/';    }
\!                        { return (int)'!';    }
=                         { return (int)Tokens.EQ;  }
\<\>                      { return (int)Tokens.NEQ;   }
\>                        { return (int)Tokens.GT; }
\>=                       { return (int)Tokens.GTE;    }
\<                        { return (int)Tokens.LT;     }
\<=                       { return (int)Tokens.LTE;    }
\&                        { return (int)'&';    }
\&\&                      { return (int)Tokens.AMPAMP; }
\|                        { return (int)'|';    }
\|\|                      { return (int)Tokens.BARBAR; }
\.                        { return (int)'.';    }
\[                        { return (int) Tokens.LEFTSQUAREBRACKET;	}
\]                        { return (int) Tokens.RIGHTSQUAREBRACKET;    }

{StringBeginEnd}{StringContent}{StringBeginEnd}         { return (int)Tokens.STRING; }

{CmntStart}{ABStar}\**{CmntEnd} { return (int)Tokens.LEX_COMMENT; }
{CmntStart}{ABStar}\**          { BEGIN(COMMENT); return (int)Tokens.LEX_COMMENT; }
<COMMENT>\n                     |                                
<COMMENT>{ABStar}\**            { return (int)Tokens.LEX_COMMENT; }
<COMMENT>{ABStar}\**{CmntEnd}   { BEGIN(INITIAL); return (int)Tokens.LEX_COMMENT; }
\**{CmntEnd}                    { return (int)Tokens.LEX_COMMENT; }

{LineComment1}{ABStar}\n { return (int)Tokens.LEX_COMMENT; }
{LineComment2}{ABStar}\n { return (int)Tokens.LEX_COMMENT; }

{White0}+                  { return (int)Tokens.LEX_WHITE; }
\n                         { return (int)Tokens.LEX_WHITE; }
.                          { yyerror("illegal char");
                             return (int)Tokens.LEX_ERROR; }

%{
                      LoadYylval();
%}

%%

/* .... */































