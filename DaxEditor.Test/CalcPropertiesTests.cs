// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System.Xml.Linq;
using DaxEditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaxEditorSample.Test
{
    [TestClass]
    public class CalcPropertiesTests
    {
        [TestMethod]
        public void CalcProperty_GeneralFormat()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                  <CalculationReference>[Date]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>''</FormatString>
                </CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.General, calcProp.Format);
            Assert.IsNull(calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.AreEqual("''", calcProp.FormatString);

            Assert.AreEqual(string.Empty, calcProp.ToDax());
        }

        [TestMethod]
        public void CalcProperty_NumberDecimalFormat()
        {
            string xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                        <Format Format=""NumberDecimal"" Accuracy=""5"" ThousandSeparator=""True"" xmlns="""" />
                      </Value>
                    </Annotation>
                  </Annotations>
                  <CalculationReference>[Decimal number]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>'#,0.00000'</FormatString>
                </CalculationProperty>";
            var calcPropDoc = XDocument.Parse(xml);
            var calcProp = DaxCalcProperty.CreateFromXElement(calcPropDoc.Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.NumberDecimal, calcProp.Format);
            Assert.AreEqual(5, calcProp.Accuracy);
            Assert.AreEqual(true, calcProp.ThousandSeparator);
            Assert.AreEqual("'#,0.00000'", calcProp.FormatString);

            string expectedDaxCalculatedPropertyString = "CALCULATION PROPERTY NumberDecimal Accuracy=5 ThousandSeparator=True Format='#,0.00000'";
            Assert.AreEqual(expectedDaxCalculatedPropertyString, calcProp.ToDax());
        }

        [TestMethod]
        public void CalcProperty_NumberWholeFormat()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                        <Format Format=""NumberWhole"" xmlns="""" />
                      </Value>
                    </Annotation>
                  </Annotations>
                  <CalculationReference>[Whole number]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>'0'</FormatString>
                </CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.NumberWhole, calcProp.Format);
            Assert.IsNull(calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.AreEqual("'0'", calcProp.FormatString);

            string expectedDaxCalculatedPropertyString = "CALCULATION PROPERTY NumberWhole Format='0'";
            Assert.AreEqual(expectedDaxCalculatedPropertyString, calcProp.ToDax());
        }

        [TestMethod]
        public void CalcProperty_PercentageFormat()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                        <Format Format=""Percentage"" Accuracy=""2"" xmlns="""" />
                      </Value>
                    </Annotation>
                  </Annotations>
                  <CalculationReference>[Percentage]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>'0.00 %;-0.00 %;0.00 %'</FormatString>
                </CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.Percentage, calcProp.Format);
            Assert.AreEqual(2, calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.AreEqual("'0.00 %;-0.00 %;0.00 %'", calcProp.FormatString);

            string expectedDaxCalculatedPropertyString = "CALCULATION PROPERTY Percentage Accuracy=2 Format='0.00 %;-0.00 %;0.00 %'";
            Assert.AreEqual(expectedDaxCalculatedPropertyString, calcProp.ToDax());
        }

        [TestMethod]
        public void CalcProperty_ScientificFormat()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                        <Format Format=""Scientific"" Accuracy=""2"" xmlns="""" />
                      </Value>
                    </Annotation>
                  </Annotations>
                  <CalculationReference>[Scientific]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>'0.00E+000'</FormatString>
                </CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.Scientific, calcProp.Format);
            Assert.AreEqual(2, calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.AreEqual("'0.00E+000'", calcProp.FormatString);

            string expectedDaxCalculatedPropertyString = "CALCULATION PROPERTY Scientific Accuracy=2 Format='0.00E+000'";
            Assert.AreEqual(expectedDaxCalculatedPropertyString, calcProp.ToDax());
        }

        [TestMethod]
        public void CalcProperty_CurrencyFormat()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                        <Format Format=""Currency"" Accuracy=""3"" xmlns="""">
                          <Currency LCID=""1040"" DisplayName=""€ Euro(€ 123)"" Symbol=""€"" PositivePattern=""2"" NegativePattern=""9"" />
                        </Format>
                      </Value>
                    </Annotation>
                  </Annotations>
                  <CalculationReference>[Currency number]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>'""€"" #,0.00;-""€"" #,0.00;""€"" #,0.00'</FormatString>
                </CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.Currency, calcProp.Format);
            Assert.AreEqual(3, calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.AreEqual(@"'LCID=""1040"" DisplayName=""€ Euro(€ 123)"" Symbol=""€"" PositivePattern=""2"" NegativePattern=""9""'", calcProp.CustomFormat);
            Assert.AreEqual(@"'""€"" #,0.00;-""€"" #,0.00;""€"" #,0.00'", calcProp.FormatString);

            string expectedDaxCalculatedPropertyString = @"CALCULATION PROPERTY Currency Accuracy=3 Format='""€"" #,0.00;-""€"" #,0.00;""€"" #,0.00' AdditionalInfo='LCID=""1040"" DisplayName=""€ Euro(€ 123)"" Symbol=""€"" PositivePattern=""2"" NegativePattern=""9""'";
            Assert.AreEqual(expectedDaxCalculatedPropertyString, calcProp.ToDax());
        }

        [TestMethod]
        public void CalcProperty_DateTimeCustomFormat()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
        <Format Format=""DateTimeCustom"" xmlns="""">
          <DateTimes>
            <DateTime LCID=""1033"" Group=""GeneralLongDateTime"" FormatString=""M/d/yyyy HH:mm:ss"" />
          </DateTimes>
        </Format>
      </Value>
    </Annotation>
  </Annotations>
  <CalculationReference>[Date Custom Format]</CalculationReference>
  <CalculationType>Member</CalculationType>
  <FormatString>'M/d/yyyy HH:mm:ss'</FormatString>
</CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.DateTimeCustom, calcProp.Format);
            Assert.IsNull(calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.AreEqual(@"'LCID=""1033"" Group=""GeneralLongDateTime"" FormatString=""M/d/yyyy HH:mm:ss""'", calcProp.CustomFormat);
            Assert.AreEqual(@"'M/d/yyyy HH:mm:ss'", calcProp.FormatString);

            var expected = @"CALCULATION PROPERTY DateTimeCustom Format='M/d/yyyy HH:mm:ss' AdditionalInfo='LCID=""1033"" Group=""GeneralLongDateTime"" FormatString=""M/d/yyyy HH:mm:ss""'";
            var actual = calcProp.ToDax();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalcProperty_DateTimeShortDatePattern()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
        <Format Format=""DateTimeShortDatePattern"" xmlns=""""/>
      </Value>
    </Annotation>
  </Annotations>
  <CalculationReference>[Date Time Short Date Pattern]</CalculationReference>
  <CalculationType>Member</CalculationType>
  <FormatString>'M/d/yyyy HH:mm:ss'</FormatString>
</CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.DateTimeShortDatePattern, calcProp.Format);
            Assert.IsNull(calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.IsNull(calcProp.CustomFormat);
            Assert.AreEqual(@"'M/d/yyyy HH:mm:ss'", calcProp.FormatString);

            var expected = @"CALCULATION PROPERTY DateTimeShortDatePattern Format='M/d/yyyy HH:mm:ss'";
            var actual = calcProp.ToDax();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalcProperty_DateTimeGeneral()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                        <Format Format=""DateTimeGeneral"" xmlns=""""/>
                      </Value>
                    </Annotation>
                  </Annotations>
                  <CalculationReference>[Date Time General]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>'M/d/yyyy HH:mm:ss'</FormatString>
                </CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.DateTimeGeneral, calcProp.Format);
            Assert.IsNull(calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.IsNull(calcProp.CustomFormat);
            Assert.AreEqual(@"'M/d/yyyy HH:mm:ss'", calcProp.FormatString);

            var expected = @"CALCULATION PROPERTY DateTimeGeneral Format='M/d/yyyy HH:mm:ss'";
            var actual = calcProp.ToDax();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalcProperty_Text()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                        <Format Format=""Text"" xmlns="""" />
                      </Value>
                    </Annotation>
                  </Annotations>
                  <CalculationReference>[Distinct Count of PAT_ID]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>''</FormatString>
                </CalculationProperty>";

            var calcProp = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(calcProp);
            Assert.AreEqual(DaxCalcProperty.FormatType.Text, calcProp.Format);
            Assert.IsNull(calcProp.Accuracy);
            Assert.IsNull(calcProp.ThousandSeparator);
            Assert.AreEqual("''", calcProp.FormatString);

            string expectedDaxCalculatedPropertyString = "CALCULATION PROPERTY Text";
            Assert.AreEqual(expectedDaxCalculatedPropertyString, calcProp.ToDax());
        }
        [TestMethod]
        public void CalcProperty_Description()
        {
            var xml = @"<CalculationProperty xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
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
                  <CalculationReference>[Date]</CalculationReference>
                  <CalculationType>Member</CalculationType>
                  <FormatString>''</FormatString>
                  <Description>Some Description</Description>
                </CalculationProperty>";

            var property = DaxCalcProperty.CreateFromXElement(XDocument.Parse(xml).Root);

            Assert.IsNotNull(property);
            Assert.AreEqual("Some Description", property.Description);

            var expected = "CALCULATION PROPERTY General Description='Some Description'";
            var actual = property.ToDax();
            Assert.AreEqual(expected, actual);
        }
    }
}
