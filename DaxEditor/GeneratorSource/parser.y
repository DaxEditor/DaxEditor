/*
The project released under MS-PL license https://daxeditor.codeplex.com/license
*/

%using Microsoft.VisualStudio.TextManager.Interop
%namespace Babel.Parser
%valuetype LexValue
%partial

/* %expect 5 */


%union {
    public string str;
}


%{
    ErrorHandler handler = null;
    public void SetHandler(ErrorHandler hdlr) { handler = hdlr; }
    internal void CallHdlr(string msg, LexLocation val)
    {
        handler.AddError(msg, val.sLin, val.sCol, val.eCol - val.sCol);
    }

    internal TextSpan MkTSpan(LexLocation s)
    {
        TextSpan ts;
        ts.iStartLine = s.sLin;
        ts.iStartIndex = s.sCol;
        ts.iEndLine = s.eLin;
        ts.iEndIndex = s.eCol;
        return ts;
    }

    internal void Match(LexLocation lh, LexLocation rh)
    {
        DefineMatch(MkTSpan(lh), MkTSpan(rh));
    }

    internal void StartFunction(LexLocation s, string token)
    {
        System.Diagnostics.Debug.WriteLine("StartName:" + token);
        Sink.StartName(MkTSpan(s), token); 
    }

    internal void StartParameters(LexLocation s)
    {
        System.Diagnostics.Debug.WriteLine("StartParameters");
        Sink.StartParameters(MkTSpan(s)); 
    }

    internal void EndParameters(LexLocation s)
    {
        System.Diagnostics.Debug.WriteLine("EndParameters");
        Sink.EndParameters(MkTSpan(s));
    }

    internal void NextParameter(LexLocation s)
    {
        System.Diagnostics.Debug.WriteLine("Next Parameter");
        Sink.NextParameter(MkTSpan(s));
    }
%}

%token FUNCTION NUMBER STRING COLUMNNAME TABLENAME ESCAPEDTABLENAME PARTIALCOLUMNNAME PARTIALTABLENAME MDXCODE KWALTER KPI
/* Begin generated list of tokens */
%token KWEVALUATE KWDEFINE KWMEASURE KWORDER KWBY KWTRUE KWFALSE KWASC KWDESC KWDAY KWMONTH KWYEAR KWCREATE KWCALCULATE KWCALCULATION KWPROPERTY KWGENERAL KWNUMBERDECIMAL KWNUMBERWHOLE KWPERCENTAGE KWSCIENTIFIC KWCURRENCY KWDATETIMECUSTOM KWDATETIMESHORTDATEPATTERN KWDATETIMEGENERAL KWTEXT KWACCURACY KWTHOUSANDSEPARATOR KWFORMAT KWADDITIONALINFO KWKPI KWVISIBLE KWDESCRIPTION KWDISPLAYFOLDER KWVAR KWRETURN KWDATATABLE KWBOOLEAN KWDATETIME KWDOUBLE KWINTEGER KWSTRING KWRANKX KWSKIP KWDENSE KWNOT KWKPIDESCRIPTION KWKPITARGETFORMATSTRING KWKPITARGETDESCRIPTION KWKPITARGETEXPRESSION KWKPISTATUSGRAPHIC KWKPISTATUSDESCRIPTION KWKPISTATUSEXPRESSION KWKPITRENDGRAPHIC KWKPITRENDDESCRIPTION KWKPITRENDEXPRESSION KWKPIANNOTATIONS
/* End generated list of tokens */

%token EQ NEQ GT GTE LT LTE POW AMPAMP BARBAR LEFTSQUAREBRACKET RIGHTSQUAREBRACKET
%token maxParseToken 
%token LEX_WHITE LEX_COMMENT LEX_ERROR



%%

Start
    : DaxQueries
    | EQ Expression
    | DaxScript
    | Empty
    ;
    
Order
    : KWASC
    | KWDESC
    ;

OrderByList
    : Expression Order ',' OrderByList
    | Expression ',' OrderByList
    | Expression Order
    | Expression
    ;

OrderBy
    : KWORDER KWBY OrderByList
    ;

DaxQueries
    : DaxQuery
    | DaxQueries DaxQuery
    ;

DaxQuery
    : KWDEFINE Definitions KWEVALUATE Expression OrderBy
    | KWDEFINE Definitions KWEVALUATE Expression
    | KWEVALUATE Expression OrderBy
    | KWEVALUATE Expression
    ;

DaxScript
    : CreateMeasure
    | CreateMeasure ';'
    | CreateMeasure ';' DaxScript
    | CreateKpi
    | CreateKpi ';'
    | CreateKpi ';' DaxScript
    | CreateMember
    | CreateMember ';'
    | CreateMember ';' DaxScript
    | Calculate
    | Calculate ';'
    | Calculate ';' DaxScript
    | Alter
    | Alter ';'
    | Alter ';' DaxScript
    ;

Empty
    : EOF
    ;

CubeName
    : TABLENAME
    | COLUMNNAME
    ;

MeasureExpression
    : Expression                    { SpecifyMeasureExpression(@1); }
    ;

CreateMeasure
    : KWCREATE KWMEASURE CubeName '.' MeasureName EQ MeasureExpression                          { SpecifyFullMeasureText(@1, @7); }
    | KWCREATE KWMEASURE CubeName '.' MeasureName EQ MeasureExpression CalculationProperty      { SpecifyFullMeasureText(@1, @7); }
    | KWCREATE KWMEASURE error '.' MeasureName EQ MeasureExpression                             { CallHdlr("Cube name expected before '.'", @3); }
    | KWCREATE KWMEASURE MeasureName EQ MeasureExpression                                       { SpecifyFullMeasureText(@1, @5); }
    | KWCREATE KWMEASURE MeasureName EQ MeasureExpression  CalculationProperty                  { SpecifyFullMeasureText(@1, @5); }
    | KWCREATE KWMEASURE error EQ MeasureExpression                                             { CallHdlr("Measure name expected", @3); }
    ;

CreateKpi
    : KWCREATE KWKPI
    ;

CreateMember
    : KWCREATE MDXCODE
    ;

Calculate
    : KWCALCULATE
    ;

Alter
    : KWALTER
    ;

CalculationPropertyFormatType
    : KWGENERAL                                                                         { SpecifyCalculationProperty(@1); }
    | KWNUMBERDECIMAL                                                                   { SpecifyCalculationProperty(@1); }
    | KWNUMBERWHOLE                                                                     { SpecifyCalculationProperty(@1); }
    | KWPERCENTAGE                                                                      { SpecifyCalculationProperty(@1); }
    | KWSCIENTIFIC                                                                      { SpecifyCalculationProperty(@1); }
    | KWCURRENCY                                                                        { SpecifyCalculationProperty(@1); }
    | KWDATETIMECUSTOM                                                                  { SpecifyCalculationProperty(@1); }
    | KWDATETIMESHORTDATEPATTERN                                                        { SpecifyCalculationProperty(@1); }
    | KWDATETIMEGENERAL                                                                 { SpecifyCalculationProperty(@1); }
    | KWTEXT                                                                            { SpecifyCalculationProperty(@1); }
    ;

CalculationPropertyAccuracy
    : KWACCURACY EQ NUMBER                                                              { SpecifyCalcPropAccuracy(@3); }
    | KWACCURACY error                                                                  { CallHdlr("'=' is not specified", @2); }
    | KWACCURACY EQ error                                                               { CallHdlr("Value of Accuracy is not a number", @3); }
    ;

CalculationPropertyVisible
    : KWVISIBLE EQ  KWTRUE                                                              { SpecifyCalcPropIsHidden(false); }
    | KWVISIBLE EQ  KWFALSE                                                             { SpecifyCalcPropIsHidden(true); }
    | KWVISIBLE error                                                                   { CallHdlr("'=' is not specified", @2); }
    | KWVISIBLE EQ error                                                                { CallHdlr("Visible can be either TRUE or FALSE", @3); }
    ;

CalculationPropertyDescriptionContent
    : ESCAPEDTABLENAME
    | STRING
    ;

CalculationPropertyDescription
    : KWDESCRIPTION EQ CalculationPropertyDescriptionContent                            { SpecifyCalcPropDescription(@3);  }
    | KWDESCRIPTION error                                                               { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyDisplayFolder
    : KWDISPLAYFOLDER EQ ESCAPEDTABLENAME                                               { SpecifyCalcPropDisplayFolder(@3);  }
    | KWDISPLAYFOLDER error                                                             { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyThousandSeparator
    : KWTHOUSANDSEPARATOR EQ  KWTRUE                                                    { SpecifyCalcPropThousandSeparator(true); }
    | KWTHOUSANDSEPARATOR EQ  KWFALSE                                                   { SpecifyCalcPropThousandSeparator(false); }
    | KWTHOUSANDSEPARATOR error                                                         { CallHdlr("'=' is not specified", @2); }
    | KWTHOUSANDSEPARATOR EQ error                                                      { CallHdlr("ThousandSeparator can be either TRUE or FALSE", @3); }
    ;

CalculationPropertyFormat
    : KWFORMAT EQ ESCAPEDTABLENAME                                                      { SpecifyCalcPropFormat(@3); }
    ;

CalculationPropertyAdditionalInfo
    : KWADDITIONALINFO EQ ESCAPEDTABLENAME                                              { SpecifyCalcPropAdditionalInfo(@3); }
    ;

CalculationPropertyKpiDescription
    : KWKPIDESCRIPTION EQ CalculationPropertyDescriptionContent                         { SpecifyCalcPropKpiDescription(@3);  }
    | KWKPIDESCRIPTION error                                                            { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiTargetFormatString
    : KWKPITARGETFORMATSTRING EQ CalculationPropertyDescriptionContent                  { SpecifyCalcPropKpiTargetFormatString(@3);  }
    | KWKPITARGETFORMATSTRING error                                                     { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiTargetDescription
    : KWKPITARGETDESCRIPTION EQ CalculationPropertyDescriptionContent                   { SpecifyCalcPropKpiTargetDescription(@3);  }
    | KWKPITARGETDESCRIPTION error                                                      { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiTargetExpression
    : KWKPITARGETEXPRESSION EQ Expression                                               { SpecifyCalcPropKpiTargetExpression(@3);  }
    | KWKPITARGETEXPRESSION error                                                       { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiStatusGraphic
    : KWKPISTATUSGRAPHIC EQ CalculationPropertyDescriptionContent                       { SpecifyCalcPropKpiStatusGraphic(@3);  }
    | KWKPISTATUSGRAPHIC error                                                          { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiStatusDescription
    : KWKPISTATUSDESCRIPTION EQ CalculationPropertyDescriptionContent                   { SpecifyCalcPropKpiStatusDescription(@3);  }
    | KWKPISTATUSDESCRIPTION error                                                      { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiStatusExpression
    : KWKPISTATUSEXPRESSION EQ Expression                                               { SpecifyCalcPropKpiStatusExpression(@3);  }
    | KWKPISTATUSEXPRESSION error                                                       { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiTrendGraphic
    : KWKPITRENDGRAPHIC EQ CalculationPropertyDescriptionContent                        { SpecifyCalcPropKpiTrendGraphic(@3);  }
    | KWKPITRENDGRAPHIC error                                                           { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiTrendDescription
    : KWKPITRENDDESCRIPTION EQ CalculationPropertyDescriptionContent                    { SpecifyCalcPropKpiTrendDescription(@3);  }
    | KWKPITRENDDESCRIPTION error                                                       { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiTrendExpression
    : KWKPITRENDEXPRESSION EQ Expression                                                { SpecifyCalcPropKpiTrendExpression(@3);  }
    | KWKPITRENDEXPRESSION error                                                        { CallHdlr("'=' is not specified", @2); }
    ;

CalculationPropertyKpiAnnotations
    : KWKPIANNOTATIONS EQ ESCAPEDTABLENAME                                              { SpecifyCalcPropKpiAnnotations(@3);  }
    | KWKPIANNOTATIONS error                                                            { CallHdlr("'=' is not specified", @2); }
    ;
    
VarName
    : TABLENAME
    ;

VarDeclaration
    : KWVAR VarName EQ Expression
    | KWVAR error EQ Expression            { CallHdlr("Invalid var name", @2); }
    ;

VarDeclarations
    : VarDeclaration
    | VarDeclarations VarDeclaration
    ;
    
VarExpression
    : VarDeclarations KWRETURN Expression
    ;

CalculationPropertyParams
    : CalculationPropertyVisible
    | CalculationPropertyVisible CalculationPropertyParams
    | CalculationPropertyAccuracy
    | CalculationPropertyAccuracy CalculationPropertyParams
    | CalculationPropertyThousandSeparator
    | CalculationPropertyThousandSeparator CalculationPropertyParams
    | CalculationPropertyFormat
    | CalculationPropertyFormat CalculationPropertyParams
    | CalculationPropertyAdditionalInfo
    | CalculationPropertyAdditionalInfo CalculationPropertyParams
    | CalculationPropertyDescription
    | CalculationPropertyDescription CalculationPropertyParams
    | CalculationPropertyDisplayFolder
    | CalculationPropertyDisplayFolder CalculationPropertyParams
	/* KPI properties */
    | CalculationPropertyKpiDescription
    | CalculationPropertyKpiDescription CalculationPropertyParams
    | CalculationPropertyKpiTargetFormatString
    | CalculationPropertyKpiTargetFormatString CalculationPropertyParams
    | CalculationPropertyKpiTargetDescription
    | CalculationPropertyKpiTargetDescription CalculationPropertyParams
    | CalculationPropertyKpiTargetExpression
    | CalculationPropertyKpiTargetExpression CalculationPropertyParams
    | CalculationPropertyKpiStatusGraphic
    | CalculationPropertyKpiStatusGraphic CalculationPropertyParams
    | CalculationPropertyKpiStatusDescription
    | CalculationPropertyKpiStatusDescription CalculationPropertyParams
    | CalculationPropertyKpiStatusExpression
    | CalculationPropertyKpiStatusExpression CalculationPropertyParams
    | CalculationPropertyKpiTrendGraphic
    | CalculationPropertyKpiTrendGraphic CalculationPropertyParams
    | CalculationPropertyKpiTrendDescription
    | CalculationPropertyKpiTrendDescription CalculationPropertyParams
    | CalculationPropertyKpiTrendExpression
    | CalculationPropertyKpiTrendExpression CalculationPropertyParams
    | CalculationPropertyKpiAnnotations
    | CalculationPropertyKpiAnnotations CalculationPropertyParams
    ;

CalculationProperty
    : KWCALCULATION KWPROPERTY CalculationPropertyFormatType
    | KWCALCULATION KWPROPERTY error                                                    { CallHdlr("Wrong calculation property type.  Expected types: General, NumberDecimal, NumberWhole, Percentage, Scientific, Currency, DateTimeCustom, Visible, Description, DisplayFolder", @2); }
    | KWCALCULATION KWPROPERTY CalculationPropertyFormatType CalculationPropertyParams
    ;

QueryMeasureExpression
    : Expression
    ;

Definitions
    : KWMEASURE QueryMeasureName EQ QueryMeasureExpression
    | KWMEASURE QueryMeasureName EQ QueryMeasureExpression Definitions
    ;

ParamSeparator
    : ','                                { NextParameter(@1); }
    ;

ParamExpression
    : Expression
    ;

Params1
    : ParamExpression
    | Params1 ParamSeparator ParamExpression
    ;

StartArg
    : '('                                 { StartParameters(@1); }
    ;
    
EndArg
    : ')'                                 { EndParameters(@1); }
    ;

ParenthesisParameters
    :  StartArg EndArg                    { Match(@1, @2); }
    |  StartArg Params1 EndArg            { Match(@1, @3); }
    |  StartArg Params1 error             { CallHdlr("unmatched parentheses", @3); }
    |  StartArg error EndArg              { $$ = $3;
                                            CallHdlr("error in parameters", @2); }
    ;

FunctionArgs
    : ParenthesisParameters
    ;

FunctionName
    : FUNCTION                            { StartFunction(@1, $1.str); }
    | KWCALCULATE                         { StartFunction(@1, $1.str); }
    | KWTRUE                              { StartFunction(@1, $1.str); }
    | KWFALSE                             { StartFunction(@1, $1.str); }
    | KWYEAR                              { StartFunction(@1, $1.str); }
    | KWDAY                               { StartFunction(@1, $1.str); }
    | KWMONTH                             { StartFunction(@1, $1.str); }
    | KWFORMAT                            { StartFunction(@1, $1.str); }
    | KWNOT                               { StartFunction(@1, $1.str); }
    ;

RankXTies
    : /* empty */
    | KWSKIP
    | KWDENSE
    ;

RankXOrder
    : /* empty */
    | NUMBER
    | KWTRUE
    | KWFALSE
	| KWASC
	| KWDESC
    ;

RankXValue
    : /* empty */
    | ScalarExpression
    ;

RankX
    : KWRANKX '(' Expression ',' Expression ')'                                                
    | KWRANKX '(' Expression ',' Expression ',' RankXValue ')'                                
    | KWRANKX '(' Expression ',' Expression ',' RankXValue ',' RankXOrder ')'                
    | KWRANKX '(' Expression ',' Expression ',' RankXValue ',' RankXOrder ',' RankXTies ')'
    ;

RankXFunction
    : RankX                         { StartFunction(@1, $1.str); }
    ;

FunctionCall
    : FunctionName FunctionArgs
    | RankXFunction
    | KWNOT FunctionName FunctionArgs
    | KWNOT RankXFunction
    ;

ColumnRef
    : COLUMNNAME
    ;

DataTableValue
    : /* empty */
    | ScalarExpression
    ;

DataTableValues
    : DataTableValue
    | DataTableValues ',' DataTableValue
    ;

DataTableRows
    : '{' DataTableValues '}'
    | '{' DataTableValues '}' ',' DataTableRows
    ;

DataTableColumnType
    : KWBOOLEAN
    | KWCURRENCY
    | KWDATETIME
    | KWDOUBLE
    | KWINTEGER
    | KWSTRING
    ;

DataTableColumn
    : STRING ',' DataTableColumnType
    ;

DataTableColumns
    : DataTableColumn
    | DataTableColumns ',' DataTableColumn
    ;

DataTable
    : KWDATATABLE '(' DataTableColumns ',' '{' DataTableRows '}' ')'
    ;

DataTableFunction
    : DataTable                     { StartFunction(@1, $1.str); }
    ;

TableRef
    : TABLENAME 
    | ESCAPEDTABLENAME
    | FUNCTION  /* Very special case when a table name is the same as function name, but it is not wrapped in quotes */
    | DataTableFunction
    ;

TableExpression
    : TableRef
    ;

MeasureName
    : TableRef ColumnRef                { CreateNewMeasure(@1, @2); }
    ;

QueryMeasureName
    : TableRef ColumnRef
    ;

ColMeasureRef
    : TableRef ColumnRef
    | ColumnRef
    ;

ParenthesisExpression
    :  '(' ')'                   { Match(@1, @2); }
    |  '(' Params1 ')'           { Match(@1, @3); }
    |  '(' Params1 error         { CallHdlr("unmatched parentheses", @3); }
    ;

CalculateShortcut
    :   ColMeasureRef ParenthesisExpression
    ;

PrimaryExpression
    : ColMeasureRef
    | FunctionCall
    | ParenthesisExpression
    | CalculateShortcut
    | NUMBER
    | STRING
    | KWTRUE
    | KWFALSE
    | KWDAY
    | KWMONTH
    | KWYEAR
    | TableExpression
    ;

UnaryOperator
    : '!' | '-'| '+'
    ;

UnaryExpression
    : PrimaryExpression
    | UnaryOperator UnaryExpression
    ;

BinaryOperator
    : '+' | '-' | '*'| '/'| AMPAMP | BARBAR | '&'
    | GT | GTE | LT | LTE | EQ | NEQ
    ;

BinaryExpression
    : UnaryExpression
    | BinaryExpression BinaryOperator UnaryExpression
    ;

ScalarExpression
    : BinaryExpression
    ;

Expression
    : ScalarExpression
    | VarExpression
    ;

%%












