// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DaxEditor
{
    public class ServerCommandProducer
    {
        private string _databaseId;
        private int _dbCompatibilityLevel;
        private string _cubeId;
        private XmlWriterSettings _settings = new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true };

        #region templates
        public const string DoNotModify1100 = @"----------------------------------------------------------
-- PowerPivot measures command (do not modify manually) --
----------------------------------------------------------


";
        public const string DoNotModify1103 = @"
                    ----------------------------------------------------------
                    -- PowerPivot measures command (do not modify manually) --
                    ----------------------------------------------------------


                    ";
        public const string CommonCommandText1100 = @"CALCULATE; 
CREATE MEMBER CURRENTCUBE.Measures.[__No measures defined] AS 1; 
ALTER CUBE CURRENTCUBE UPDATE DIMENSION Measures, Default_Member = [__No measures defined]; ";

        public const string CommonCommandText1103 = @"CALCULATE;
CREATE MEMBER CURRENTCUBE.Measures.[__XL_Count of Models] AS 1, VISIBLE = 0;
ALTER CUBE CURRENTCUBE UPDATE DIMENSION Measures, Default_Member = [__XL_Count of Models]; ";

        public const string CommonCommandTextUnknownVersion = @"
                    CALCULATE;
                    CREATE MEMBER CURRENTCUBE.Measures.[__XL_Count of Models] AS 1, VISIBLE = 0;
                    ALTER CUBE CURRENTCUBE UPDATE DIMENSION Measures, Default_Member = [__XL_Count of Models];
                  ";

        private const string _beginTransaction = @"<BeginTransaction xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"" />";
        private const string _commitTransaction = @"<CommitTransaction xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"" />";
        private const string _rollbackTransaction = @"<RollbackTransaction xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"" />";

        #endregion

        public ServerCommandProducer(string databaseId, int dbCompatibilityLevel, string cubeId)
        {
            _databaseId = databaseId;
            _dbCompatibilityLevel = dbCompatibilityLevel;
            _cubeId = cubeId;
        }

        public ServerCommandProducer(string bimFileContent)
        {
            var bimFileDocument = XDocument.Parse(bimFileContent);
            var databaseElement = bimFileDocument.Descendants(MeasuresContainer.NS + "Database").First();
            _databaseId = databaseElement.Element(MeasuresContainer.NS + "ID").Value;
            var compatibilityLevelElement = databaseElement.Element(MeasuresContainer.NS200 + "CompatibilityLevel");
            _dbCompatibilityLevel = int.Parse(compatibilityLevelElement.Value);
            var cubeIdElement = databaseElement.Element(MeasuresContainer.NS + "Cubes").Element(MeasuresContainer.NS + "Cube").Element(MeasuresContainer.NS + "ID");
            _cubeId = cubeIdElement.Value;
        }

        public string ProduceBeginTransaction()
        {
            return _beginTransaction;
        }

        public string ProduceCommitTransaction()
        {
            return _commitTransaction;
        }

        public string ProduceRollbackTransaction()
        {
            return _rollbackTransaction;
        }

        public string ProduceProcessRecalc()
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, _settings))
            {
                writer.WriteStartElement("Process", "http://schemas.microsoft.com/analysisservices/2003/engine");
                writer.WriteElementString("Type", "ProcessRecalc");
                writer.WriteStartElement("Object");
                writer.WriteElementString("DatabaseID", _databaseId);
                writer.WriteEndElement(); // Object
                writer.WriteEndElement(); // Process
            }
            return sb.ToString();
        }

        public string ProduceAlterScriptElement(IEnumerable<DaxMeasure> daxMeasures)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, _settings))
            {
                WriteMdxScript(daxMeasures, writer);
            }

            return sb.ToString();
        }
        public string ProduceAlterMdxScript(IEnumerable<DaxMeasure> daxMeasures)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, _settings))
            {
                writer.WriteStartElement("Alter", "http://schemas.microsoft.com/analysisservices/2003/engine");
                writer.WriteAttributeString("ObjectExpansion", "ExpandFull");
                writer.WriteStartElement("Object");
                writer.WriteElementString("DatabaseID", _databaseId);
                writer.WriteElementString("CubeID", _cubeId);
                writer.WriteElementString("MdxScriptID", "MdxScript");
                writer.WriteEndElement(); // Object
                writer.WriteStartElement("ObjectDefinition");
                WriteMdxScript(daxMeasures, writer);
                writer.WriteEndElement(); // ObjectDefinition
                writer.WriteEndElement(); // Alter
            }

            return sb.ToString();
        }

        private void WriteMdxScript(IEnumerable<DaxMeasure> daxMeasures, XmlWriter writer)
        {
            writer.WriteStartElement("MdxScript", "http://schemas.microsoft.com/analysisservices/2003/engine");
            writer.WriteElementString("ID", "MdxScript");
            writer.WriteElementString("Name", "MdxScript");
            writer.WriteStartElement("Commands");
            // The fist command that is always the same
            writer.WriteStartElement("Command");
            writer.WriteStartElement("Text");

            if (_dbCompatibilityLevel < 1103)
                writer.WriteValue(CommonCommandText1100);
            else
                writer.WriteValue(CommonCommandText1103);

            writer.WriteEndElement(); // Text
            writer.WriteEndElement(); // Command

            if (_dbCompatibilityLevel < 1103)
            {
                WriteMeasuresCommand1100(daxMeasures, writer);
            }
            else
            {
                foreach (var measure in daxMeasures)
                {
                    WriteMeasureCommand1103(measure, writer);
                }
            }

            writer.WriteEndElement(); // Commands

            writer.WriteStartElement("CalculationProperties");

            foreach (var measure in daxMeasures)
            {
                if (measure.CalcProperty != null)
                    measure.CalcProperty.ToXml(writer, measure.NameInBrackets);
                else
                    DaxCalcProperty.CreateDefaultCalculationProperty().ToXml(writer, measure.NameInBrackets);
            }

            writer.WriteStartElement("CalculationProperty");
            if (_dbCompatibilityLevel < 1103)
                writer.WriteElementString("CalculationReference", "Measures.[__No measures defined]");
            else
                writer.WriteElementString("CalculationReference", "[__XL_Count of Models]");
            writer.WriteElementString("CalculationType", "Member");
            writer.WriteElementString("Visible", "false");
            writer.WriteEndElement(); // CalculationProperty

            writer.WriteEndElement(); // CalculationProperties

            writer.WriteEndElement(); // MdxScript
        }

        private void WriteMeasuresCommand1100(IEnumerable<DaxMeasure> measures, XmlWriter writer)
        {
            string measuresText = string.Concat(measures.Select(i => i.FullText + ";" + Environment.NewLine));
            writer.WriteStartElement("Command");
            writer.WriteStartElement("Text");
            writer.WriteString(DoNotModify1100 + measuresText);
            writer.WriteEndElement(); // Text
            writer.WriteEndElement(); // Command
        }

        private void WriteMeasureCommand1103(DaxMeasure measure, XmlWriter writer)
        {
            writer.WriteStartElement("Command");
            writer.WriteStartElement("Text");
            writer.WriteString(DoNotModify1103 + measure.FullText + ";" + Environment.NewLine);
            writer.WriteEndElement(); // Text
            writer.WriteStartElement("Annotations");
            writer.WriteStartElement("Annotation");
            writer.WriteElementString("Name", "FullName");
            writer.WriteElementString("Value", measure.Name);
            writer.WriteEndElement(); // Annotation
            writer.WriteStartElement("Annotation");
            writer.WriteElementString("Name", "Table");
            writer.WriteElementString("Value", measure.TableName);
            writer.WriteEndElement(); // Annotation
            writer.WriteEndElement(); // Annotations
            writer.WriteEndElement(); // Command
        }
    }
}
