namespace DaxEditor.Test
{
    using Json;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using Microsoft.AnalysisServices;

    [TestFixture]
    public class MeasuresContainerTests
    {
        static public void ValidateDatabase(string text)
        {
            var server = new Microsoft.AnalysisServices.Tabular.Server();
            server.ID = "ID";
            server.Name = "Name";

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

        static public string BaseTest(string text)
        {
            var container = MeasuresContainer.ParseText(text);
            Assert.IsNotNull(container, "container != null");
            Assert.IsNotNull(container.Measures, "container.Measures != null");
            Assert.IsNotNull(container.SupportingMeasures, "container.SupportingMeasures != null");
            Assert.IsNotNull(container.AllMeasures, "container.AllMeasures != null");

            var dax = container.GetDaxText();
            Assert.IsNotNull(dax, "dax != null");

            var daxContainer = MeasuresContainer.ParseDaxScript(dax);
            Assert.AreEqual(container.Measures.Count, daxContainer.Measures.Count,
                "container.Measures.Count != daxContainer.Measures.Count");
            //Assert.AreEqual(container.SupportingMeasures.Count, daxContainer.SupportingMeasures.Count, "container.SupportingMeasures.Count != daxContainer.SupportingMeasures.Count");
            //Assert.AreEqual(container.AllMeasures.Count, daxContainer.AllMeasures.Count, "container.AllMeasures.Count != daxContainer.AllMeasures.Count");

            return daxContainer.UpdateMeasures(text);
        }

        static public void BaseTestJson(string text, bool ignoreEmptyLines = false)
        {
            var actual = BaseTest(text);
            var expected = text;

            //Fix sorting. But missing properties in the model will be hidden.
            var database = JsonUtilities.Deserialize(expected);
            expected = JsonUtilities.Serialize(database);

            if (ignoreEmptyLines)
            {
                //Ignore empty lines. Parser not support whitespaces before expressions.
                WindiffAssert.AreEqualIgnoreEmptyLinesInExpressions(expected, actual);
            }
            else
            {
                WindiffAssert.AreEqual(expected, actual);
            }

            ValidateDatabase(text);
        }

        static public void BaseTestXml(string text, bool normalize = false)
        {
            var actual = BaseTest(text);
            var expected = text;

            if (normalize)
            {
                WindiffAssert.AreEqualNormalizedXmla(expected, actual);
            }
            else
            {
                WindiffAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void MeasuresFormats()
        {
            var text = Utils.ReadFileFromResources("MeasuresFormats.bim");
            BaseTestXml(text, normalize: true);

            var container = MeasuresContainer.ParseText(text);
            Assert.AreEqual(8, container.Measures.Count);
            Assert.AreEqual(0, container.SupportingMeasures.Count);
            Assert.AreEqual(8, container.AllMeasures.Count);
        }

        [Test]
        public void Bim1100()
        {
            var text = Utils.ReadFileFromResources("BIM1100.bim");
            BaseTestXml(text, normalize: true);

            var container = MeasuresContainer.ParseText(text);
            Assert.AreEqual(3, container.Measures.Count);
            Assert.AreEqual(0, container.SupportingMeasures.Count);
            Assert.AreEqual(3, container.AllMeasures.Count);
        }

        [Test]
        public void M1()
        {
            var text = Utils.ReadFileFromResources("M1.bim");
            BaseTestXml(text, normalize: true);

            var container = MeasuresContainer.ParseText(text);
            Assert.AreEqual(68, container.Measures.Count);
            Assert.AreEqual(20, container.SupportingMeasures.Count);
            Assert.AreEqual(88, container.AllMeasures.Count);
        }


        [Test]
        public void Bim1100_Json()
        {
            var text = Utils.ReadFileFromResources("BIM1100_JSON.bim");
            BaseTestJson(text);

            var container = MeasuresContainer.ParseText(text);
            Assert.AreEqual(3, container.Measures.Count);
            Assert.AreEqual(0, container.SupportingMeasures.Count);
            Assert.AreEqual(3, container.AllMeasures.Count);
        }

        [Test]
        public void M1_Json()
        {
            var text = Utils.ReadFileFromResources("M1_JSON.bim");
            BaseTestJson(text, ignoreEmptyLines: true);

            var container = MeasuresContainer.ParseText(text);
            Assert.AreEqual(68, container.Measures.Count);
            Assert.AreEqual(0, container.SupportingMeasures.Count);
            Assert.AreEqual(68, container.AllMeasures.Count);
        }

        [Test]
        public void NewDaxModel_Json()
        {
            var text = Utils.ReadFileFromResources("NewDaxModel_JSON.bim");
            BaseTestJson(text);
        }

        [Test]
        public void NewMeasuresFuncs_Json()
        {
            var text = Utils.ReadFileFromResources("NewMeasuresFuncs_JSON.bim");
            BaseTestJson(text, ignoreEmptyLines: true);
        }

        [Test]
        public void MeasureUnderscore_Json()
        {
            var text = Utils.ReadFileFromResources("MeasureUnderscore_JSON.bim");
            BaseTestJson(text, ignoreEmptyLines: true);
        }

        [Test]
        public void WithComments_Json()
        {
            var text = Utils.ReadFileFromResources("WithComments_JSON.bim");
            BaseTestJson(text);
        }

        [Test]
        public void WithTranslations_Json()
        {
            var text = Utils.ReadFileFromResources("WithTranslations_JSON.bim");
            BaseTestJson(text);
        }

        [Test]
        public void WithKPI_Json()
        {
            var text = Utils.ReadFileFromResources("WithKPI_JSON.bim");
            BaseTestJson(text, ignoreEmptyLines: true);
        }

        [Test]
        public void WithKPI2_Json()
        {
            var text = Utils.ReadFileFromResources("WithKPI2_JSON.bim");

            var container = MeasuresContainer.ParseText(text);
            var measure = container.Measures[0];
            Assert.AreEqual(measure.Name, "# SKUs");

            BaseTestJson(text, ignoreEmptyLines: true);
        }

        [Test]
        public void KPIExample()
        {
            var text = Utils.ReadFileFromResources("KPIExample.bim");
            BaseTestXml(text, normalize: true);

            var container = MeasuresContainer.ParseText(text);
            Assert.AreEqual(6, container.Measures.Count, "container.Measures.Count != 6");
            Assert.AreEqual(10, container.SupportingMeasures.Count, "container.SupportingMeasures.Count != 10");
            Assert.AreEqual(16, container.AllMeasures.Count, "container.AllMeasures.Count != 16");
        }

        [Test]
        public void KPIExample_Json()
        {
            var text = Utils.ReadFileFromResources("KPIExample_JSON.bim");
            BaseTestJson(text, ignoreEmptyLines: true);
        }

        [Test]
        public void RMN_Model_Perspective_Load_Save_Load()
        {
            //Open the BIM file you sent in Visual Studio (the .TXT must be replaced with .BIM)
            var text = Utils.ReadFileFromResources("RMN_Model_Perspective.bim");
            BaseTestJson(text, ignoreEmptyLines: true);

            //Get measures from BIM file
            var container = MeasuresContainer.ParseText(text);

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
            BaseTestJson(text, ignoreEmptyLines: true);
        }

        [Test]
        public void WithScope()
        {
            var text = Utils.ReadFileFromResources("WithScope.bim");
            BaseTestXml(text, normalize: true);
        }
    }
}