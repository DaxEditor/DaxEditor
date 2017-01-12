// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AnalysisServices;

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

        public const string CommonCommandTextUnknownVersion2 = @"CALCULATE; 
CREATE MEMBER CURRENTCUBE.Measures.[__No measures defined] AS 1, VISIBLE = 0; 
ALTER CUBE CURRENTCUBE UPDATE DIMENSION Measures, Default_Member = [__No measures defined]; ";

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

        public ServerCommandProducer(string text)
        {
            var database = GetDatabase(text);
            _databaseId = database.ID;
            _dbCompatibilityLevel = database.CompatibilityLevel;
            _cubeId = database.Cubes.Count > 0 ? database.Cubes[0].ID : null;
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
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, _settings))
            {
                writer.WriteStartElement("Process", "http://schemas.microsoft.com/analysisservices/2003/engine");
                writer.WriteElementString("Type", "ProcessRecalc");
                writer.WriteStartElement("Object");
                writer.WriteElementString("DatabaseID", _databaseId);
                writer.WriteEndElement(); // Object
                writer.WriteEndElement(); // Process
            }
            return builder.ToString();
        }

        public static string GetDatabaseText(string text)
        {
            var alter = XDocument.Parse(text);
            var database = alter.Descendants(MeasuresContainer.NS + "Database").First();
            database.Add(new XAttribute("xmlns", MeasuresContainer.NS.Trim('{', '}')));

            return database.ToString();
        }

        public static Database GetDatabase(string text)
        {
            text = GetDatabaseText(text);
            using (var reader = new XmlTextReader(new System.IO.StringReader(text)))
            {
                var database = new Database();

                return Utils.Deserialize(reader, database) as Database;
            }
        }

        public string ProduceAlterScriptElement(MeasuresContainer container)
        {
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8);
            var obj = container.ToMdxScript(_dbCompatibilityLevel);
            Utils.Serialize(writer, obj, true);
            stream.Position = 0;

            var text = new StreamReader(stream).ReadToEnd();

            //Delete default nodes
            var document = XDocument.Parse(text);
            document.Descendants(MeasuresContainer.NS + "CreatedTimestamp").Remove();
            document.Descendants(MeasuresContainer.NS + "LastSchemaUpdate").Remove();
            document.Descendants(MeasuresContainer.NS + "Value").Where(i=>i.IsEmpty).Remove();
            document.Element(MeasuresContainer.NS + "MdxScript").RemoveAttributes();

            return document.ToString(SaveOptions.OmitDuplicateNamespaces);
        }

        public string ProduceAlterMdxScript(IList<DaxMeasure> measures)
        {
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, _settings))
            {
                writer.WriteStartElement("Alter", "http://schemas.microsoft.com/analysisservices/2003/engine");
                writer.WriteAttributeString("ObjectExpansion", "ExpandFull");
                writer.WriteStartElement("Object");
                writer.WriteElementString("DatabaseID", _databaseId);
                writer.WriteElementString("CubeID", _cubeId);
                writer.WriteElementString("MdxScriptID", "MdxScript");
                writer.WriteEndElement(); // Object
                writer.WriteStartElement("ObjectDefinition");
                writer.WriteEndElement(); // ObjectDefinition
                writer.WriteEndElement(); // Alter
            }

            var template = builder.ToString();
            var templateDocument = XDocument.Parse(template);

            var script = ProduceAlterScriptElement(new MeasuresContainer(measures));
            var document = XDocument.Parse(script);
            templateDocument.Descendants(MeasuresContainer.NS + "ObjectDefinition").
                First().Add(document.Root);

            return templateDocument.ToString(SaveOptions.OmitDuplicateNamespaces);
        }
    }
}
