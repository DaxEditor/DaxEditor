// The project released under MS-PL license https://daxeditor.codeplex.com/license

using DaxEditor;
using DaxEditor.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Microsoft.AnalysisServices;

namespace DaxEditorSample.Test
{
    [TestClass]
    public class MeasuresContainerTests
    {
        [TestMethod]
        public void MeasuresFormats()
        {
            string input = Utils.ReadFileFromResources("MeasuresFormats.bim");
            var bim = MeasuresContainer.ParseText(input);
            Assert.IsNotNull(bim.Measures);
            Assert.AreEqual(8, bim.Measures.Count);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = input;
            var actual = measuresFromDax.UpdateMeasures(input);
            WindiffAssert.AreEqualNormalizedXmla(expected, actual);
        }

        [TestMethod]
        public void Bim1100()
        {
            string input = Utils.ReadFileFromResources("BIM1100.bim");
            var bim = MeasuresContainer.ParseText(input);
            Assert.IsNotNull(bim.Measures);
            Assert.AreEqual(3, bim.Measures.Count);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = input;
            var actual = measuresFromDax.UpdateMeasures(input);
            WindiffAssert.AreEqualNormalizedXmla(expected, actual);
        }

        [TestMethod]
        public void M1()
        {
            string input = Utils.ReadFileFromResources("M1.bim");
            var bim = MeasuresContainer.ParseText(input);
            Assert.IsNotNull(bim.Measures);
            Assert.AreEqual(68, bim.Measures.Count);
            Assert.AreEqual(20, bim.SupportingMeasures.Count);
            Assert.AreEqual(88, bim.AllMeasures.Count);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = input;
            var actual = measuresFromDax.UpdateMeasures(input);
            WindiffAssert.AreEqualNormalizedXmla(expected, actual);
        }


        [TestMethod]
        public void Bim1100_Json()
        {
            var text = Utils.ReadFileFromResources("BIM1100_JSON.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            Assert.AreEqual(3, bim.Measures.Count);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = text;
            var actual = measuresFromDax.UpdateMeasures(text);
            WindiffAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void M1_Json()
        {
            string text = Utils.ReadFileFromResources("M1_JSON.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            Assert.AreEqual(68, bim.Measures.Count);//MB FIX 88
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = text;

            //Fix sorting. But missing properties in the model will be hidden.
            var document = JsonUtilities.Deserialize(text);
            expected = JsonUtilities.Serialize(document);

            var actual = measuresFromDax.UpdateMeasures(text);
            //Ignore empty lines. Parser not support whitespaces before expressions.
            WindiffAssert.AreEqualIgnoreEmptyLinesInExpressions(expected, actual);
        }

        [TestMethod]
        public void NewDaxModel_Json()
        {
            var text = Utils.ReadFileFromResources("NewDaxModel_JSON.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = text;

            //Fix sorting. But missing properties in the model will be hidden.
            var document = JsonUtilities.Deserialize(expected);
            expected = JsonUtilities.Serialize(document);

            var actual = measuresFromDax.UpdateMeasures(text);
            WindiffAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NewMeasuresFuncs_Json()
        {
            var text = Utils.ReadFileFromResources("NewMeasuresFuncs_JSON.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = text;

            //Fix sorting. But missing properties in the model will be hidden.
            var document = JsonUtilities.Deserialize(expected);
            expected = JsonUtilities.Serialize(document);

            var actual = measuresFromDax.UpdateMeasures(text);
            //Ignore empty lines. Parser not support whitespaces before expressions.
            WindiffAssert.AreEqualIgnoreEmptyLinesInExpressions(expected, actual);
        }

        [TestMethod]
        public void WithComments_Json()
        {
            var text = Utils.ReadFileFromResources("WithComments_JSON.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = text;

            //Fix sorting. But missing properties in the model will be hidden.
            var document = JsonUtilities.Deserialize(expected);
            expected = JsonUtilities.Serialize(document);

            var actual = measuresFromDax.UpdateMeasures(text);
            WindiffAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WithTranslations_Json()
        {
            var text = Utils.ReadFileFromResources("WithTranslations_JSON.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = text;

            //Fix sorting. But missing properties in the model will be hidden.
            var document = JsonUtilities.Deserialize(expected);
            expected = JsonUtilities.Serialize(document);

            var actual = measuresFromDax.UpdateMeasures(text);
            WindiffAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WithKPI_Json()
        {
            var text = Utils.ReadFileFromResources("WithKPI_JSON.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = text;

            //Fix sorting. But missing properties in the model will be hidden.
            var document = JsonUtilities.Deserialize(expected);
            expected = JsonUtilities.Serialize(document);

            var actual = measuresFromDax.UpdateMeasures(text);
            //Ignore empty lines. Parser not support whitespaces before expressions.
            WindiffAssert.AreEqualIgnoreEmptyLinesInExpressions(expected, actual);
        }

        [TestMethod]
        public void KPIExample()
        {
            var text = Utils.ReadFileFromResources("KPIExample.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);
            Assert.AreEqual(6, bim.Measures.Count, "bim.Measures.Count != 6");
            Assert.AreEqual(10, bim.SupportingMeasures.Count, "bim.SupportingMeasures.Count != 10");
            Assert.AreEqual(16, bim.AllMeasures.Count, "bim.AllMeasures.Count != 16");

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count, "bim.Measures.Count != measuresFromDax.Measures.Count");
            Assert.AreEqual(bim.SupportingMeasures.Count, measuresFromDax.SupportingMeasures.Count, "bim.SupportingMeasures.Count != measuresFromDax.SupportingMeasures.Count");
            Assert.AreEqual(bim.AllMeasures.Count, measuresFromDax.AllMeasures.Count, "bim.AllMeasures.Count != measuresFromDax.AllMeasures.Count");

            var expected = text;
            var actual = measuresFromDax.UpdateMeasures(text);
            System.IO.File.WriteAllText("test.bim", actual);
            WindiffAssert.AreEqualNormalizedXmla(expected, actual);
        }

        [TestMethod]
        public void KPIExample_Json()
        {
            var text = Utils.ReadFileFromResources("KPIExample_JSON.bim");
            var bim = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(bim.Measures);
            var daxText = bim.GetDaxText();
            Assert.IsNotNull(daxText);

            var measuresFromDax = MeasuresContainer.ParseDaxScript(daxText);
            Assert.AreEqual(bim.Measures.Count, measuresFromDax.Measures.Count);

            var expected = text;

            //Fix sorting. But missing properties in the model will be hidden.
            var document = JsonUtilities.Deserialize(expected);
            expected = JsonUtilities.Serialize(document);

            var actual = measuresFromDax.UpdateMeasures(text);
            //Ignore empty lines. Parser not support whitespaces before expressions.
            WindiffAssert.AreEqualIgnoreEmptyLinesInExpressions(expected, actual);
        }

        [TestMethod]
        public void RMN_Model_Perspective_Load_Save_Load()
        {
            //Open the BIM file you sent in Visual Studio (the .TXT must be replaced with .BIM)
            var text = Utils.ReadFileFromResources("RMN_Model_Perspective.bim");

            //Get measures from BIM file
            var container = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(container.Measures);

            //Remove the "Cost Amount" measure from the DAX file
            var index = -1;
            var i = 0;
            foreach (var measure in container.AllMeasures)
            {
                if (measure.Name.Equals("Cost Amount", StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                }
                ++i;
            }
            container.AllMeasures.RemoveAt(index);

            //Save measures to BIM file
            text = container.UpdateMeasures(text);

            //Open the BIM file
            var server = new Microsoft.AnalysisServices.Tabular.Server();
            server.ID = "ID";
            server.Name = "name";

            var database = JsonUtilities.Deserialize(text);
            server.Databases.Add(database);

            var errors = new ValidationErrorCollection();
            server.Validate(errors);
            Assert.AreEqual(0, errors.Count);

            database.Validate(errors);
            Assert.AreEqual(0, errors.Count);

            var result = database.Model.Validate();
            Assert.AreEqual(0, result.Errors.Count, 
                string.Join(Environment.NewLine, 
                    result.Errors.Select(
                        error => error.Message
                    )
                )
            );
        }
    }
}