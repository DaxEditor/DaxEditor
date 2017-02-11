// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System.Xml.Linq;
using DaxEditor;
using NUnit.Framework;

namespace DaxEditorSample.Test
{
    [TestFixture]
    public class ServerCommandProducerTest
    {
        [Test]
        public void TestProduceMeasuresCompatLevel1103()
        {
            #region expectedResultText
            string expectedResultText = @"<Alter ObjectExpansion=""ExpandFull"" xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
  <Object>
    <DatabaseID>de</DatabaseID>
    <CubeID>Model</CubeID>
    <MdxScriptID>MdxScript</MdxScriptID>
  </Object>
  <ObjectDefinition>
    <MdxScript>
      <ID>MdxScript</ID>
      <Name>MdxScript</Name>
      <Commands>
        <Command>
          <Text>CALCULATE;
CREATE MEMBER CURRENTCUBE.Measures.[__XL_Count of Models] AS 1, VISIBLE = 0;
ALTER CUBE CURRENTCUBE UPDATE DIMENSION Measures, Default_Member = [__XL_Count of Models]; </Text>
        </Command>
        <Command>
          <Text>
                    ----------------------------------------------------------
                    -- PowerPivot measures command (do not modify manually) --
                    ----------------------------------------------------------


                    CREATE MEASURE 'Table1'[Measure 1]=1;
</Text>
          <Annotations>
            <Annotation>
              <Name>FullName</Name>
              <Value>Measure 1</Value>
            </Annotation>
            <Annotation>
              <Name>Table</Name>
              <Value>Table1</Value>
            </Annotation>
          </Annotations>
        </Command>
        <Command>
          <Text>
                    ----------------------------------------------------------
                    -- PowerPivot measures command (do not modify manually) --
                    ----------------------------------------------------------


                    CREATE MEASURE 'Table1'[MeasureCountRows]=COUNTROWS(Table1);
</Text>
          <Annotations>
            <Annotation>
              <Name>FullName</Name>
              <Value>MeasureCountRows</Value>
            </Annotation>
            <Annotation>
              <Name>Table</Name>
              <Value>Table1</Value>
            </Annotation>
          </Annotations>
        </Command>
      </Commands>
      <CalculationProperties>
        <CalculationProperty>
          <Annotations>
            <Annotation>
              <Name>Type</Name>
              <Value>User</Value>
            </Annotation>
            <Annotation>
              <Name>IsPrivate</Name>
              <Value>False</Value>
            </Annotation>
            <Annotation>
              <Name>Format</Name>
              <Value>
                <Format Format=""General"" xmlns="""" />
              </Value>
            </Annotation>
          </Annotations>
          <CalculationReference>[Measure 1]</CalculationReference>
          <CalculationType>Member</CalculationType>
          <FormatString>''</FormatString>
        </CalculationProperty>
        <CalculationProperty>
          <Annotations>
            <Annotation>
              <Name>Type</Name>
              <Value>User</Value>
            </Annotation>
            <Annotation>
              <Name>IsPrivate</Name>
              <Value>False</Value>
            </Annotation>
            <Annotation>
              <Name>Format</Name>
              <Value>
                <Format Format=""General"" xmlns="""" />
              </Value>
            </Annotation>
          </Annotations>
          <CalculationReference>[MeasureCountRows]</CalculationReference>
          <CalculationType>Member</CalculationType>
          <FormatString>''</FormatString>
        </CalculationProperty>
        <CalculationProperty>
          <CalculationReference>[__XL_Count of Models]</CalculationReference>
          <CalculationType>Member</CalculationType>
          <Visible>false</Visible>
        </CalculationProperty>
      </CalculationProperties>
    </MdxScript>
  </ObjectDefinition>
</Alter>
";
            #endregion
            var measures = new DaxMeasure[] {
                new DaxMeasure {TableName = "Table1", Name = "Measure 1", Expression = "1", FullText = "CREATE MEASURE 'Table1'[Measure 1]=1"},
                new DaxMeasure {TableName = "Table1", Name = "MeasureCountRows", Expression = "COUNTROWS(Table1)", FullText = "CREATE MEASURE 'Table1'[MeasureCountRows]=COUNTROWS(Table1)"},
            };
            var cmdProducer = new ServerCommandProducer("de", 1103, "Model");
            var actualResult = cmdProducer.ProduceAlterMdxScript(measures);
            var expected = XDocument.Parse(expectedResultText).ToString(SaveOptions.None);
            var actual = XDocument.Parse(actualResult).ToString(SaveOptions.None);

            WindiffAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestProduceMeasuresCompatLevel1100()
        {
            #region expectedResultText
            string expectedResultText = @"<Alter ObjectExpansion=""ExpandFull"" xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
    <Object>
      <DatabaseID>de</DatabaseID>
      <CubeID>Model</CubeID>
      <MdxScriptID>MdxScript</MdxScriptID>
    </Object>
    <ObjectDefinition>
                        <MdxScript>
                            <ID>MdxScript</ID>
                            <Name>MdxScript</Name>
                            <Commands>
<Command>
                                    <Text>CALCULATE; 
CREATE MEMBER CURRENTCUBE.Measures.[__No measures defined] AS 1; 
ALTER CUBE CURRENTCUBE UPDATE DIMENSION Measures, Default_Member = [__No measures defined]; </Text>
                                </Command>
                                <Command>
                                    <Text>----------------------------------------------------------
-- PowerPivot measures command (do not modify manually) --
----------------------------------------------------------


CREATE MEASURE 'Quantities'[Sum of Qty Served]=SUM([Qty Served]);
CREATE MEASURE 'Quantities'[Sum of Qty Consumed 2]=SUM([Qty Consumed]);
</Text>
                                </Command>
                            </Commands>
                            <CalculationProperties>
                                <CalculationProperty>
                                    <Annotations>
                                        <Annotation>
                                            <Name>Type</Name>
                                            <Value>User</Value>
                                        </Annotation>
                                        <Annotation>
                                            <Name>IsPrivate</Name>
                                            <Value>False</Value>
                                        </Annotation>
                                        <Annotation>
                                            <Name>Format</Name>
                                            <Value>
                                                <Format Format=""General"" xmlns="""" />
                                            </Value>
                                        </Annotation>
                                    </Annotations>
                                    <CalculationReference>[Sum of Qty Served]</CalculationReference>
                                    <CalculationType>Member</CalculationType>
                                    <FormatString>''</FormatString>
                                </CalculationProperty>
                                <CalculationProperty>
                                    <Annotations>
                                        <Annotation>
                                            <Name>Type</Name>
                                            <Value>User</Value>
                                        </Annotation>
                                        <Annotation>
                                            <Name>IsPrivate</Name>
                                            <Value>False</Value>
                                        </Annotation>
                                        <Annotation>
                                            <Name>Format</Name>
                                            <Value>
                                                <Format Format=""General"" xmlns="""" />
                                            </Value>
                                        </Annotation>
                                    </Annotations>
                                    <CalculationReference>[Sum of Qty Consumed 2]</CalculationReference>
                                    <CalculationType>Member</CalculationType>
                                    <FormatString>''</FormatString>
                                </CalculationProperty>
                                <CalculationProperty>
                                    <CalculationReference>Measures.[__No measures defined]</CalculationReference>
                                    <CalculationType>Member</CalculationType>
                                    <Visible>false</Visible>
                                </CalculationProperty>
                             </CalculationProperties>
                        </MdxScript>
            </ObjectDefinition>
</Alter>
";
            #endregion
            var measures = new DaxMeasure[] {
                new DaxMeasure {TableName = "Quantities", Name = "Sum of Qty Served", Expression = "SUM([Qty Served])", FullText = "CREATE MEASURE 'Quantities'[Sum of Qty Served]=SUM([Qty Served])"},
                new DaxMeasure {TableName = "Quantities", Name = "Sum of Qty Consumed 2", Expression = "SUM([Qty Consumed])", FullText = "CREATE MEASURE 'Quantities'[Sum of Qty Consumed 2]=SUM([Qty Consumed])"},
            };
            var cmdProducer = new ServerCommandProducer("de", 1100, "Model");
            var actualResult = cmdProducer.ProduceAlterMdxScript(measures);
            var expected = XDocument.Parse(expectedResultText).ToString(SaveOptions.None);
            var actual = XDocument.Parse(actualResult).ToString(SaveOptions.None);

            WindiffAssert.AreEqual(expected, actual);
        }



        [Test]
        public void TestProduceBeginTransaction()
        {
            const string expected = @"<BeginTransaction xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"" />";
            var cmdProducer = new ServerCommandProducer("de", 1103, "Model");
            var actual = cmdProducer.ProduceBeginTransaction();
            var formattedExpected = XDocument.Parse(expected).ToString(SaveOptions.None);
            var formattedActual = XDocument.Parse(actual).ToString(SaveOptions.None);
            Assert.AreEqual(formattedExpected, formattedActual);
        }

        [Test]
        public void TestProduceCommitTransaction()
        {
        const string expected = @"<CommitTransaction xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"" />";
            var cmdProducer = new ServerCommandProducer("de", 1103, "Model");
            var actual = cmdProducer.ProduceCommitTransaction();
            var formattedExpected = XDocument.Parse(expected).ToString(SaveOptions.None);
            var formattedActual = XDocument.Parse(actual).ToString(SaveOptions.None);
            Assert.AreEqual(formattedExpected, formattedActual);
        }

        [Test]
        public void TestProduceProcessRecalc()
        {
        const string expected = @"<Process xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
  <Type>ProcessRecalc</Type>
  <Object>
    <DatabaseID>de</DatabaseID>
  </Object>
</Process>";
            var cmdProducer = new ServerCommandProducer("de", 1103, "Model");
            var actual = cmdProducer.ProduceProcessRecalc();
            var formattedExpected = XDocument.Parse(expected).ToString(SaveOptions.None);
            var formattedActual = XDocument.Parse(actual).ToString(SaveOptions.None);
            Assert.AreEqual(formattedExpected, formattedActual);
        }


    }
}
