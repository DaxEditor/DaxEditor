// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Linq;
using DaxEditor;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaxEditorSample.Test
{
    /// <summary>
    /// Unut tests for parser
    /// </summary>
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseQueryWithTrue()
        {
            Babel.Parser.Parser parser = ParseText(@"
DEFINE MEASURE Table1[M1] = CountRows ( Table1 )
EVALUATE CalculateTable (
   ADDCOLUMNS (
      FILTER(ALL(Table3),MOD([M1],2) = 1),
	  ""X"",
	  CALCULATE(CALCULATE([M1],AllSelected(Table3)))
   )
   , Table5[BOOL] = TRUE
   , Table3[IntMeasure] > 0
   , Table3[PK] > 4
)
");
            Assert.IsNotNull(parser);
        }

        [TestMethod]
        public void ParseQueryWithOrderBy()
        {
            Babel.Parser.Parser parser = ParseText(@"DEFINE 
	MEASURE Table1[PercentProfit]=SUMX(Table1,[SalesAmount] - [TotalProductCost]) 
	MEASURE DimProductSubCategory[CountProducts]=COUNTROWS(DimProduct) 
EVALUATE
	ADDCOLUMNS(
		FILTER(
			CROSSJOIN(
				VALUES(DimDate[CalendarYear]),
				VALUES(DimProductSubCategory[EnglishProductSubCategoryName])
			),
			NOT(ISBLANK(DimDate[CalendarYear])) && 	
			NOT(ISBLANK(DimProductSubCategory[EnglishProductSubCategoryName])) && 
			(
				NOT(ISBLANK(CALCULATE(SUM(Table1[SalesAmount])))) ||
				NOT(ISBLANK(Table1[PercentProfit])) ||
				NOT(ISBLANK(DimProductSubCategory[CountProducts]))
			)
		),
		""Amount"", CALCULATE(SUM(Table1[SalesAmount])),
		""PercentProfit"", Table1[PercentProfit],
		""CountProducts"", DimProductSubCategory[CountProducts]
	)	
ORDER BY 
	DimDate[CalendarYear] DESC,
	DimProductSubCategory[EnglishProductSubCategoryName] ASC
");
            Assert.IsNotNull(parser);
        }

        [TestMethod]
        public void ParseSimpleMeasure()
        {
            Babel.Parser.Parser parser = ParseText("CREATE MEASURE t[B]=Now()");

            Assert.AreEqual(1, parser.Measures.Count);
            var measure = parser.Measures.First();
            Assert.AreEqual("t", measure.TableName);
            Assert.AreEqual("B", measure.Name);
            Assert.AreEqual("Now()", measure.Expression);
            Assert.AreEqual("CREATE MEASURE t[B]=Now()", measure.FullText);
        }

        [TestMethod]
        public void ParseTrueFalse()
        {
            Babel.Parser.Parser parser = ParseText("CREATE MEASURE t[B]=TRUE() && TRUE || FALSE || FALSE()");

            Assert.AreEqual(1, parser.Measures.Count);
            var measure = parser.Measures.First();
            Assert.AreEqual("t", measure.TableName);
            Assert.AreEqual("B", measure.Name);
            Assert.AreEqual("TRUE() && TRUE || FALSE || FALSE()", measure.Expression);
            Assert.AreEqual("CREATE MEASURE t[B]=TRUE() && TRUE || FALSE || FALSE()", measure.FullText);
        }

        [TestMethod]
        public void ParseTI()
        {
            var parser = ParseText(@"CREATE MEASURE B[M1]=CALCULATE (
    [Net Value],
    DATEADD ( Calendar[Calendar Date], -1, MONTH )
)");

            Assert.AreEqual(1, parser.Measures.Count);
            var measure = parser.Measures.First();
            Assert.AreEqual("B", measure.TableName);
            Assert.AreEqual("M1", measure.Name);
            Assert.AreEqual(@"CALCULATE (
    [Net Value],
    DATEADD ( Calendar[Calendar Date], -1, MONTH )
)", measure.Expression);
        }

        [TestMethod]
        public void ParseTableNameTime()
        {
            Babel.Parser.Parser parser = ParseText("CREATE MEASURE 'Table1'[Hourly Avg CallCount]=AVERAGEX(CROSSJOIN(VALUES('Date'[DateID]), VALUES(Time[Hour])), [Count]);");

            Assert.AreEqual(1, parser.Measures.Count);
            var measure = parser.Measures.First();
            Assert.AreEqual("Table1", measure.TableName);
            Assert.AreEqual("Hourly Avg CallCount", measure.Name);
            Assert.AreEqual("AVERAGEX(CROSSJOIN(VALUES('Date'[DateID]), VALUES(Time[Hour])), [Count])", measure.Expression);
            Assert.AreEqual("CREATE MEASURE 'Table1'[Hourly Avg CallCount]=AVERAGEX(CROSSJOIN(VALUES('Date'[DateID]), VALUES(Time[Hour])), [Count])", measure.FullText);
        }

        [TestMethod]
        public void SeveralMeasures()
        {
            var text = @"CALCULATE; 
CREATE MEMBER CURRENTCUBE.Measures.[__XL_Count of Models] AS 1, VISIBLE = 0; 
ALTER CUBE CURRENTCUBE UPDATE DIMENSION Measures, Default_Member = [__XL_Count of Models]; 
----------------------------------------------------------
-- PowerPivot measures command (do not modify manually) --
----------------------------------------------------------


CREATE MEASURE Table1[Measure 1]=1;

----------------------------------------------------------
-- PowerPivot measures command (do not modify manually) --
----------------------------------------------------------


CREATE MEASURE 'Table1'[MeasureCountRows]=COUNTROWS(Table1);

";

            Babel.Parser.Parser parser = ParseText(text);

            Assert.AreEqual(2, parser.Measures.Count);
            var measure1 = parser.Measures.First();
            Assert.AreEqual("Table1", measure1.TableName);
            Assert.AreEqual("Measure 1", measure1.Name);
            Assert.AreEqual("1", measure1.Expression);
            Assert.AreEqual("CREATE MEASURE Table1[Measure 1]=1", measure1.FullText);
            var measure2 = parser.Measures.Skip(1).First();
            Assert.AreEqual("Table1", measure2.TableName);
            Assert.AreEqual("MeasureCountRows", measure2.Name);
            Assert.AreEqual("COUNTROWS(Table1)", measure2.Expression);
            Assert.AreEqual("CREATE MEASURE 'Table1'[MeasureCountRows]=COUNTROWS(Table1)", measure2.FullText);
        }

        [TestMethod]
        public void YearDayMonth()
        {
            var text = @" CREATE MEASURE 'TRANSACTIONS'[ThisYear]=Date(Year(Now()), Month(Now()), Day(Now()))";
            Babel.Parser.Parser parser = ParseText(text);
            Assert.AreEqual(1, parser.Measures.Count);
            var measure1 = parser.Measures.First();
            Assert.AreEqual("TRANSACTIONS", measure1.TableName);
            Assert.AreEqual("ThisYear", measure1.Name);
        }

        [TestMethod]
        public void CalculateShortcut()
        {
            var text = @" CREATE MEASURE 't3'[shortcut]=[M1](All(T)) + 't'[m 2](All(T2))";
            Babel.Parser.Parser parser = ParseText(text);
            Assert.AreEqual(1, parser.Measures.Count);
            var measure1 = parser.Measures.First();
            Assert.AreEqual("t3", measure1.TableName);
            Assert.AreEqual("shortcut", measure1.Name);
        }

        [TestMethod]
        public void ParseMeasureWithCalcProperty1()
        {
            var text = @"CREATE MEASURE 'Table1'[C]=1 CALCULATION PROPERTY NumberDecimal Accuracy=5 ThousandSeparator=True Format='#,0.00000'";

            Babel.Parser.Parser parser = ParseText(text);

            Assert.AreEqual(1, parser.Measures.Count);
            var measure1 = parser.Measures.First();
            Assert.AreEqual("Table1", measure1.TableName);
            Assert.AreEqual("C", measure1.Name);
            Assert.AreEqual("1", measure1.Expression);
            Assert.AreEqual("CREATE MEASURE 'Table1'[C]=1", measure1.FullText);
            Assert.IsNotNull(measure1.CalcProperty);
            Assert.AreEqual(DaxCalcProperty.FormatType.NumberDecimal, measure1.CalcProperty.Format);
            Assert.AreEqual("Member", measure1.CalcProperty.CalculationType);
            Assert.IsTrue(measure1.CalcProperty.Accuracy.HasValue);
            Assert.AreEqual(5, measure1.CalcProperty.Accuracy.Value);
        }

        [TestMethod]
        public void ParseMeasureCalcPropertyWrongFormatType()
        {
            var text = @"CREATE MEASURE 'Table1'[C]=1 CALCULATION PROPERTY WrongFormatType";
            try
            {
                Babel.Parser.Parser parser = ParseText(text);
                Assert.Fail("Exception expected");
            }
            catch (Exception e)
            {
                StringAssert.Contains(e.Message, "Wrong calculation property type");
            }
        }

        [TestMethod]
        public void ParseMultipleDaxQueries()
        {
            var text = @"EVALUATE t1 EVALUATE t2";
            Babel.Parser.Parser parser = ParseText(text);
            // Expect NOT to fail on parsing
        }

        [TestMethod]
        public void ParseKPI()
        {
            var text = @"CREATE KPI CURRENTCUBE.[Products with Negative Stock] AS Measures.[Products with Negative Stock], ASSOCIATED_MEASURE_GROUP = 'Product Inventory', GOAL = Measures.[_Products with Negative Stock Goal], STATUS = Measures.[_Products with Negative Stock Status], STATUS_GRAPHIC = 'Three Symbols UnCircled Colored';";
            var parser = ParseText(text);
        }

        [TestMethod]
        public void ParseNumberThatStartsWithDot()
        {
            var text = @"EVALUATE ROW(""a"", .1)";
            Babel.Parser.Parser parser = ParseText(text);
        }

        [TestMethod]
        public void ParseFunctionWithoutParameterAfterComma()
        {
            try
            {
                var text = @"EVALUATE ROW(""a"", Calculate([m], ))";
                Babel.Parser.Parser parser = ParseText(text);
                Assert.Fail("Exception expected");
            }
            catch (Exception e)
            {
                StringAssert.Contains(e.Message, "syntax error");
            }
        }

        [TestMethod]
        public void ParseSimpleVarExpression()
        {
            var text = @"=
                VAR
                    CurrentSales = SUM ( Sales[Quantity] )
                VAR
                    SalesLastYear = CALCULATE (
                        SUM ( Sales[Quantity] ),
                        SAMEPERIODLASTYEAR ( 'Date'[Date] )
                    )
                RETURN
                    IF (
                        AND ( CurrentSales <> 0, SalesLastYear <> 0 ),
                        DIVIDE (
                            CurrentSales - SalesLastYear,
                            SalesLastYear
                        )
                    )";
            Babel.Parser.Parser parser = ParseText(text);
        }

        [TestMethod]
        public void ParseSimpleVarExpression2()
        {
            var text = @"=
                CALCULATETABLE (
                    ADDCOLUMNS (
                        VAR
                            OnePercentOfSales = [SalesAmount] * 0.01
                        RETURN
                            FILTER (
                                VALUES ( Product[Product Name] ),
                                [SalesAmount] >= OnePercentOfSales
                            ),
                        ""SalesOfProduct"", [SalesAmount]
                    ),
                    Product[Color] = ""Black""
                )";
            Babel.Parser.Parser parser = ParseText(text);
        }

        [TestMethod]
        public void ParseSimpleDataTable()
        {
            var text = @" = 
                DATATABLE (
                    ""Price Range"", STRING, 
                    ""Min Price"", CURRENCY, 
                    ""Max Price"", CURRENCY, 
                    {
                        { ""Low"", 0, 10 }, 
                        { ""Medium"", 10, 100 }, 
                        { ""High"", 100, 9999999 }
                    } 
                )";
            var parser = ParseText(text);
        }

        [TestMethod]
        public void ParseVeryBigFile()
        {
            var text = Utils.ReadFileFromResources("VeryBigFile.dax");
            //var parser = ParseText(text);
            //Console.WriteLine(parser.Measures.Count);
        }

        [TestMethod]
        public void ParseDifficultDataTable()
        {
            var text = @" = 
                DATATABLE (
                    ""Quarter"", STRING,
                    ""StartDate"", DATETIME,
                    ""EndDate"", DATETIME,
                    {
                        { ""Q1"", BLANK(), ""2015-03-31"" },
                        { ""Q2"", ""2015-04-01"", DATE(2009,4,15)+TIME(2,45,21) },
                        { ""Q3"",, ""2015-09-30"" },
                        { ""Q4"", ""2015-010-01"", ""2015-12-31"" }
                    }
                )";
            Babel.Parser.Parser parser = ParseText(text);
        }

        [TestMethod]
        public void ParseExpressionRankX()
        {
            var text =
                @"CREATE MEASURE 'TaxRefund'[Market Ranking TFS] = RANKX(All(Nationalities[Country]), [Net Tax Refund Sales], [Other measure], TRUE, DENSE);";
            var parser = ParseText(text);

            Assert.AreEqual(1, parser.Measures.Count);

            var text2 =
    @"CREATE MEASURE 'TaxRefund'[Market Ranking TFS] = RANKX(All(Nationalities[Country]), [Net Tax Refund Sales], , TRUE, DENSE);";
            var parser2 = ParseText(text2);

            Assert.AreEqual(1, parser2.Measures.Count);

            var text3 =
@"CREATE MEASURE 'TaxRefund'[Market Ranking TFS] = RANKX ( 'Product Category', [A], , ASC, Dense);";
            var parser3 = ParseText(text3);

            Assert.AreEqual(1, parser3.Measures.Count);

            var text4 = @"CREATE MEASURE 'Test'[Test] = RANKX(Inventory, [InventoryCost],,,Dense)";
            var parser4 = ParseText(text4);

            var measure1 = parser.Measures[0];
            Assert.AreEqual("TaxRefund", measure1.TableName);
            Assert.AreEqual("Market Ranking TFS", measure1.Name);
            Assert.AreEqual(@"RANKX(All(Nationalities[Country]), [Net Tax Refund Sales], [Other measure], TRUE, DENSE)", measure1.Expression);
            Assert.AreEqual(@"CREATE MEASURE 'TaxRefund'[Market Ranking TFS] = RANKX(All(Nationalities[Country]), [Net Tax Refund Sales], [Other measure], TRUE, DENSE)", measure1.FullText);
            Assert.IsNull(measure1.CalcProperty);
        }

        [TestMethod]
        public void ParseExpressionRankX2()
        {
            var text = @"
CREATE MEASURE 'Leases'[Variance % Department Yield TY vs LY MTD Sequence] = IF (
            NOT ( ISBLANK ( [Variance % Yield TY vs LY MTD] ) ),
            RANKX (
                ALLSELECTED (  Leases[Department] ),
                IF (
                    ISBLANK ( [Variance % Yield TY vs LY MTD] ),
                    MINX ( ALL ( Leases ), [Variance % Yield TY vs LY MTD] ) - 1,
                    [Variance % Yield TY vs LY MTD]
                ),
                ,
                0,
                SKIP
            ),
            BLANK ()
        )
CALCULATION PROPERTY General Format='0.00%;-0.00%;0.00%' DisplayFolder='Sequence Ranking';";
            var parser = ParseText(text);

            Assert.AreEqual(1, parser.Measures.Count);

            var measure1 = parser.Measures[0];
            Assert.AreEqual("Leases", measure1.TableName);
            Assert.AreEqual("Variance % Department Yield TY vs LY MTD Sequence", measure1.Name);
            Assert.IsNotNull(measure1.Expression);
            Assert.IsNotNull(measure1.FullText);
            Assert.IsNotNull(measure1.CalcProperty);
        }

        [TestMethod]
        public void ParseExpressionWithComments()
        {
            var text = @"CREATE MEASURE 'Sales'[a] = CALCULATE ( // Comment with slash
    [b], -- Comment with dash
    Sales[Quantity]  > 0 /* Comment
    on multiple 
    lines */
 )
 // Final comment after expression
 /* Multiline final comment 
  * after 
  * expression */
CALCULATION PROPERTY General Format='0';

CREATE MEASURE 'Sales'[b] = 99
/* Comment after measure definition */
// Single line comment after measure definition
CALCULATION PROPERTY General Format='0';

CREATE MEASURE 'Sales'[c] = 1
// Final comment without CALCULATION
/* Final check
*/
";
            var parser = ParseText(text);
            
            Assert.AreEqual(3, parser.Measures.Count);

            var measure1 = parser.Measures.First();
            Assert.AreEqual("Sales", measure1.TableName);
            Assert.AreEqual("a", measure1.Name);
            Assert.AreEqual(@"CALCULATE ( // Comment with slash
    [b], -- Comment with dash
    Sales[Quantity]  > 0 /* Comment
    on multiple 
    lines */
 )
 // Final comment after expression
 /* Multiline final comment 
  * after 
  * expression */", measure1.Expression);
            Assert.AreEqual(@"CREATE MEASURE 'Sales'[a] = CALCULATE ( // Comment with slash
    [b], -- Comment with dash
    Sales[Quantity]  > 0 /* Comment
    on multiple 
    lines */
 )
 // Final comment after expression
 /* Multiline final comment 
  * after 
  * expression */", measure1.FullText);
            Assert.IsNotNull(measure1.CalcProperty);
            Assert.AreEqual(DaxCalcProperty.FormatType.General, measure1.CalcProperty.Format);
            Assert.AreEqual("Member", measure1.CalcProperty.CalculationType);
            Assert.IsFalse(measure1.CalcProperty.Accuracy.HasValue);

            var measure2 = parser.Measures[ 1 ];
            Assert.AreEqual("Sales", measure2.TableName);
            Assert.AreEqual("b", measure2.Name);
            Assert.AreEqual(@"99
/* Comment after measure definition */
// Single line comment after measure definition", measure2.Expression);
            Assert.AreEqual(@"CREATE MEASURE 'Sales'[b] = 99
/* Comment after measure definition */
// Single line comment after measure definition", measure2.FullText);
            Assert.IsNotNull(measure2.CalcProperty);
            Assert.AreEqual(DaxCalcProperty.FormatType.General, measure2.CalcProperty.Format);
            Assert.AreEqual("Member", measure2.CalcProperty.CalculationType);
            Assert.IsFalse(measure2.CalcProperty.Accuracy.HasValue);

            var measure3 = parser.Measures[2];
            Assert.AreEqual("Sales", measure3.TableName);
            Assert.AreEqual("c", measure3.Name);
            Assert.AreEqual(@"1
// Final comment without CALCULATION
/* Final check
*/", measure3.Expression);
            Assert.AreEqual(@"CREATE MEASURE 'Sales'[c] = 1
// Final comment without CALCULATION
/* Final check
*/", measure3.FullText);
            Assert.IsNull(measure3.CalcProperty);
        }

        [TestMethod]
        public void ParseExpressionWithKPI1()
        {
            var text = @"CREATE MEASURE 'Test'[Sales1] =  SUM ( Test[Value] )
CALCULATION 
    PROPERTY Currency 
	Accuracy=2 
	Description=""Description of Value""

    KpiDescription = ""Description of KPI""
    KpiTargetDescription = ""Description of Target""
    KpiTargetExpression = 'Test'[Budget]
    KpiStatusGraphic = ""Traffic Light - Single""
    KpiStatusDescription = ""Description of Status""

    KpiStatusExpression =
     var x = 'Test'[Sales1] / 'Test'[_Sales1 Goal]

     return

         if (ISBLANK(x),BLANK(),
			 If(x < 0.4, -1,
                 If(x < 0.8, 0, 1)
             )
		 )
    KpiAnnotations = 'GoalType=""Measure"", KpiStatusType= ""Linear"", KpiThresholdType=""Percentage"", KpiThresholdOrdering=""Ascending"", KpiThresholdCount=""2"", KpiThreshold_0=""40"",KpiThreshold_1 =""80""'
	;";
            var parser = ParseText(text);

            Assert.AreEqual(1, parser.Measures.Count);

            var measure1 = parser.Measures.First();
            Assert.AreEqual("Test", measure1.TableName);
            Assert.AreEqual("Sales1", measure1.Name);
            Assert.AreEqual(@"SUM ( Test[Value] )", measure1.Expression);
            Assert.AreEqual(@"CREATE MEASURE 'Test'[Sales1] =  SUM ( Test[Value] )", measure1.FullText);
            Assert.IsNotNull(measure1.CalcProperty);
            Assert.AreEqual(DaxCalcProperty.FormatType.Currency, measure1.CalcProperty.Format);
            Assert.AreEqual(2, measure1.CalcProperty.Accuracy);
            Assert.AreEqual(@"Description of Value", measure1.CalcProperty.Measure.Description);

            Assert.IsNotNull(measure1.CalcProperty.KPI);
            Assert.AreEqual(@"Description of KPI", measure1.CalcProperty.KPI.Description);
            Assert.AreEqual(@"Description of Target", measure1.CalcProperty.KPI.TargetDescription);
            Assert.AreEqual(@"'Test'[Budget]", measure1.CalcProperty.KPI.TargetExpression);
            Assert.AreEqual(@"Traffic Light - Single", measure1.CalcProperty.KPI.StatusGraphic);
            Assert.AreEqual(@"Description of Status", measure1.CalcProperty.KPI.StatusDescription);
            Assert.AreEqual(@"var x = 'Test'[Sales1] / 'Test'[_Sales1 Goal]

     return

         if (ISBLANK(x),BLANK(),
			 If(x < 0.4, -1,
                 If(x < 0.8, 0, 1)
             )
		 )", measure1.CalcProperty.KPI.StatusExpression);
            Assert.AreEqual(@"Measure", measure1.CalcProperty.KPI.Annotations["GoalType"].Value);
            Assert.AreEqual(@"Linear", measure1.CalcProperty.KPI.Annotations["KpiStatusType"].Value);
            Assert.AreEqual(@"Percentage", measure1.CalcProperty.KPI.Annotations["KpiThresholdType"].Value);
            Assert.AreEqual(@"Ascending", measure1.CalcProperty.KPI.Annotations["KpiThresholdOrdering"].Value);
            Assert.AreEqual(@"2", measure1.CalcProperty.KPI.Annotations["KpiThresholdCount"].Value);
            Assert.AreEqual(@"40", measure1.CalcProperty.KPI.Annotations["KpiThreshold_0"].Value);
            Assert.AreEqual(@"80", measure1.CalcProperty.KPI.Annotations["KpiThreshold_1"].Value);
        }

        [TestMethod]
        public void ParseExpressionWithKPI2()
        {
            var text = @"CREATE MEASURE 'Test'[Sales2] =  SUM ( Test[Value] ) / 2
CALCULATION 
    PROPERTY General 
    KpiTargetExpression = 8
    KpiStatusGraphic = ""Five Bars Colored""
	KpiStatusExpression = 
var x='Test'[Sales2] 
return
    if(ISBLANK(x),BLANK(),
        If(x<3.2,0,
            If(x<1.6,-2,-1),
			If(x<4.8,0,
				If(x<6.4,1,2)
            )
        )
	)
    KpiAnnotations = 'GoalType=""StaticValue"",KpiStatusType=""Linear"",KpiThresholdType=""Absolute"",KpiThresholdOrdering=""Ascending"",KpiThresholdCount=""4"",KpiThreshold_0=""1.6"",KpiThreshold_1=""3.2"",KpiThreshold_2= ""4.8"",KpiThreshold_3=""6.4""'
	;";
            var parser = ParseText(text);

            Assert.AreEqual(1, parser.Measures.Count);

            var measure1 = parser.Measures.First();
            Assert.AreEqual("Test", measure1.TableName);
            Assert.AreEqual("Sales2", measure1.Name);
            Assert.AreEqual(@"SUM ( Test[Value] ) / 2", measure1.Expression);
            Assert.AreEqual(@"CREATE MEASURE 'Test'[Sales2] =  SUM ( Test[Value] ) / 2", measure1.FullText);
            Assert.IsNotNull(measure1.CalcProperty);
            Assert.AreEqual(DaxCalcProperty.FormatType.General, measure1.CalcProperty.Format);
            Assert.IsFalse(measure1.CalcProperty.Accuracy.HasValue);
            Assert.IsTrue(string.IsNullOrWhiteSpace(measure1.CalcProperty.Measure.Description));

            Assert.IsNotNull(measure1.CalcProperty.KPI);
            Assert.IsTrue(string.IsNullOrWhiteSpace(measure1.CalcProperty.KPI.Description));
            Assert.IsTrue(string.IsNullOrWhiteSpace(measure1.CalcProperty.KPI.TargetDescription));
            Assert.AreEqual(@"8", measure1.CalcProperty.KPI.TargetExpression);
            Assert.AreEqual(@"Five Bars Colored", measure1.CalcProperty.KPI.StatusGraphic);
            Assert.IsTrue(string.IsNullOrWhiteSpace(measure1.CalcProperty.KPI.StatusDescription));
            Assert.AreEqual(@"var x='Test'[Sales2] 
return
    if(ISBLANK(x),BLANK(),
        If(x<3.2,0,
            If(x<1.6,-2,-1),
			If(x<4.8,0,
				If(x<6.4,1,2)
            )
        )
	)", measure1.CalcProperty.KPI.StatusExpression);
            Assert.AreEqual(@"StaticValue", measure1.CalcProperty.KPI.Annotations["GoalType"].Value);
            Assert.AreEqual(@"Linear", measure1.CalcProperty.KPI.Annotations["KpiStatusType"].Value);
            Assert.AreEqual(@"Absolute", measure1.CalcProperty.KPI.Annotations["KpiThresholdType"].Value);
            Assert.AreEqual(@"Ascending", measure1.CalcProperty.KPI.Annotations["KpiThresholdOrdering"].Value);
            Assert.AreEqual(@"4", measure1.CalcProperty.KPI.Annotations["KpiThresholdCount"].Value);
            Assert.AreEqual(@"1.6", measure1.CalcProperty.KPI.Annotations["KpiThreshold_0"].Value);
            Assert.AreEqual(@"3.2", measure1.CalcProperty.KPI.Annotations["KpiThreshold_1"].Value);
            Assert.AreEqual(@"4.8", measure1.CalcProperty.KPI.Annotations["KpiThreshold_2"].Value);
            Assert.AreEqual(@"6.4", measure1.CalcProperty.KPI.Annotations["KpiThreshold_3"].Value);
        }

        [TestMethod]
        public void ParseExpressionWithKPI3()
        {
            var text = @"CREATE MEASURE 'Test'[Sales3] =  SUM ( Test[Value] ) / 3
CALCULATION 
    PROPERTY General
    KpiTargetExpression = 'Test'[Budget]
    KpiStatusGraphic = ""Traffic Light - Single""
	KpiStatusExpression = 
var x='Test'[Sales3]/'Test'[_Sales3 Goal] 
return
    if(ISBLANK(x),BLANK(),
        If(x<0.67,
            If(x<0.34,-1,0),
			If(x<1.33,1,
				If(x<1.66,0,-1)
			)
		)
	)
    KpiAnnotations = 'GoalType=""Measure"",KpiStatusType=""Centered"",KpiThresholdType=""Percentage"",KpiThresholdOrdering=""Ascending"",KpiThresholdCount=""4"",KpiThreshold_0=""34"",KpiThreshold_1=""67"",KpiThreshold_2= ""133"",KpiThreshold_3=""166""'
	;";
            var parser = ParseText(text);

            Assert.AreEqual(1, parser.Measures.Count);

            var measure1 = parser.Measures.First();
            Assert.AreEqual("Test", measure1.TableName);
            Assert.AreEqual("Sales3", measure1.Name);
            Assert.AreEqual(@"SUM ( Test[Value] ) / 3", measure1.Expression);
            Assert.AreEqual(@"CREATE MEASURE 'Test'[Sales3] =  SUM ( Test[Value] ) / 3", measure1.FullText);
            Assert.IsNotNull(measure1.CalcProperty);
            Assert.AreEqual(DaxCalcProperty.FormatType.General, measure1.CalcProperty.Format);
            Assert.IsFalse(measure1.CalcProperty.Accuracy.HasValue);
            Assert.IsTrue(string.IsNullOrWhiteSpace(measure1.CalcProperty.Measure.Description));

            Assert.IsNotNull(measure1.CalcProperty.KPI);
            Assert.IsTrue(string.IsNullOrWhiteSpace(measure1.CalcProperty.KPI.Description));
            Assert.IsTrue(string.IsNullOrWhiteSpace(measure1.CalcProperty.KPI.TargetDescription));
            Assert.AreEqual(@"'Test'[Budget]", measure1.CalcProperty.KPI.TargetExpression);
            Assert.AreEqual(@"Traffic Light - Single", measure1.CalcProperty.KPI.StatusGraphic);
            Assert.IsTrue(string.IsNullOrWhiteSpace(measure1.CalcProperty.KPI.StatusDescription));
            Assert.AreEqual(@"var x='Test'[Sales3]/'Test'[_Sales3 Goal] 
return
    if(ISBLANK(x),BLANK(),
        If(x<0.67,
            If(x<0.34,-1,0),
			If(x<1.33,1,
				If(x<1.66,0,-1)
			)
		)
	)", measure1.CalcProperty.KPI.StatusExpression);
            Assert.AreEqual(@"Measure", measure1.CalcProperty.KPI.Annotations["GoalType"].Value);
            Assert.AreEqual(@"Centered", measure1.CalcProperty.KPI.Annotations["KpiStatusType"].Value);
            Assert.AreEqual(@"Percentage", measure1.CalcProperty.KPI.Annotations["KpiThresholdType"].Value);
            Assert.AreEqual(@"Ascending", measure1.CalcProperty.KPI.Annotations["KpiThresholdOrdering"].Value);
            Assert.AreEqual(@"4", measure1.CalcProperty.KPI.Annotations["KpiThresholdCount"].Value);
            Assert.AreEqual(@"34", measure1.CalcProperty.KPI.Annotations["KpiThreshold_0"].Value);
            Assert.AreEqual(@"67", measure1.CalcProperty.KPI.Annotations["KpiThreshold_1"].Value);
            Assert.AreEqual(@"133", measure1.CalcProperty.KPI.Annotations["KpiThreshold_2"].Value);
            Assert.AreEqual(@"166", measure1.CalcProperty.KPI.Annotations["KpiThreshold_3"].Value);
        }

        private static Babel.Parser.Parser ParseText(string text)
        {
            Babel.Parser.ErrorHandler handler = new Babel.Parser.ErrorHandler();
            Babel.Lexer.Scanner scanner = new Babel.Lexer.Scanner();
            Babel.Parser.Parser parser = new Babel.Parser.Parser();  // use noarg constructor
            parser.Trace = true;
            parser.scanner = scanner;
            scanner.Handler = handler;
            parser.SetHandler(handler);
            scanner.SetSourceText(text);

            var request = new ParseRequest(true);
            request.Sink = new AuthoringSink(ParseReason.None, 0, 0, Babel.Parser.Parser.MaxErrors);
            parser.MBWInit(request);
            var result = parser.Parse();
            if (handler.Errors)
                throw new Exception(handler.ToString());
            return parser;


        }

    }
}
