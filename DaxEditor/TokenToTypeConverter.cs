// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System.Diagnostics;
using Babel.Parser;

namespace DaxEditor
{
    public static class TokenToTypeConverter
    {
        public static DaxTokenTypes Convert(Tokens token)
        {
            switch (token)
            {
                case Tokens.error:
                case Tokens.LEX_ERROR:
                    return DaxTokenTypes.Error;

                case Tokens.LEX_COMMENT:
                    return DaxTokenTypes.Comment;

                case Tokens.FUNCTION:
                case Tokens.KWDATATABLE:
                case Tokens.KWRANKX:
                case Tokens.KWFORMAT:
                    return DaxTokenTypes.Function;

                case Tokens.NUMBER:
                    return DaxTokenTypes.NumberLiteral;

                case Tokens.STRING:
                case Tokens.KWBOOLEAN:
                case Tokens.KWCURRENCY:
                case Tokens.KWDATETIME:
                case Tokens.KWDOUBLE:
                case Tokens.KWINTEGER:
                case Tokens.KWSTRING:
                case Tokens.KWSKIP:
                case Tokens.KWDENSE:
                    return DaxTokenTypes.StringLiteral;

                case Tokens.COLUMNNAME:
                case Tokens.TABLENAME:
                case Tokens.ESCAPEDTABLENAME:
                case Tokens.PARTIALCOLUMNNAME:
                case Tokens.PARTIALTABLENAME:
                case Tokens.MDXCODE:
                    return DaxTokenTypes.Identifier;

                case Tokens.KWALTER:
                case Tokens.KWEVALUATE:
                case Tokens.KWDEFINE:
                case Tokens.KWMEASURE:
                case Tokens.KWORDER:
                case Tokens.KWBY:
                case Tokens.KWTRUE:
                case Tokens.KWFALSE:
                case Tokens.KWASC:
                case Tokens.KWDESC:
                case Tokens.KWDAY:
                case Tokens.KWMONTH:
                case Tokens.KWYEAR:
                case Tokens.KWCREATE:
                case Tokens.KWCALCULATE:
                case Tokens.KWVAR:
                case Tokens.KWRETURN:
                    return DaxTokenTypes.Keyword;

                case Tokens.KWCALCULATION:
                case Tokens.KWPROPERTY:
                case Tokens.KWGENERAL:
                case Tokens.KWNUMBERDECIMAL:
                case Tokens.KWNUMBERWHOLE:
                case Tokens.KWPERCENTAGE:
                case Tokens.KWSCIENTIFIC:
                //case Tokens.KWCURRENCY:
                case Tokens.KWDATETIMECUSTOM:
                case Tokens.KWDATETIMESHORTDATEPATTERN:
                case Tokens.KWDATETIMEGENERAL:
                case Tokens.KWACCURACY:
                case Tokens.KWTHOUSANDSEPARATOR:
                case Tokens.KWVISIBLE:
                case Tokens.KWDESCRIPTION:
                case Tokens.KWDISPLAYFOLDER:
                //case Tokens.KWFORMAT:
                case Tokens.KWADDITIONALINFO:
                    return DaxTokenTypes.SpecialFormat;

                case Tokens.EQ:
                case Tokens.NEQ:
                case Tokens.GT:
                case Tokens.GTE:
                case Tokens.LT:
                case Tokens.LTE:
                case Tokens.POW:
                case Tokens.AMPAMP:
                case Tokens.BARBAR:
                case Tokens.LEFTSQUAREBRACKET:
                case Tokens.RIGHTSQUAREBRACKET:
                case Tokens.LEX_WHITE:
                case Tokens.EOF:
                    return DaxTokenTypes.Text;

                default:
                    if(token > Tokens.error)
                        Debug.Assert(false, "Not implemented token to type conversion");  // Tokens smaller than error are valid
                    return DaxTokenTypes.Text;
            }
        }
    }
}
