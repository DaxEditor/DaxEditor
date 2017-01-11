// The project released under MS-PL license https://daxeditor.codeplex.com/license

using DaxEditor.Json;
using DaxEditor.StringExtensions;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.VisualStudio.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace DaxEditor
{
    public class MeasuresContainer
    {
        public const string NS = "{http://schemas.microsoft.com/analysisservices/2003/engine}";
        public const string NS200 = "{http://schemas.microsoft.com/analysisservices/2010/engine/200}";

        public IList<DaxMeasure> Measures { get; private set; }

        private MeasuresContainer(IList<DaxMeasure> measures)
        {
            Measures = measures;
        }

        public static Measure ToEnding(Measure measure, string ending)
        {
            //Replace all endings to selected endings. Now only for expressions
            measure.Expression = measure.Expression.ToEnding(ending);
            if (measure.KPI != null)
            {
                measure.KPI.TargetExpression = measure.KPI.TargetExpression.ToEnding(ending);
                measure.KPI.StatusExpression = measure.KPI.StatusExpression.ToEnding(ending);
                measure.KPI.TrendExpression = measure.KPI.TrendExpression.ToEnding(ending);
            }

            return measure;
        }

        public static Measure ToSystemEnding(Measure measure)
        {
            return ToEnding(measure, Environment.NewLine);
        }

        public static Measure ToUnixEnding(Measure measure)
        {
            return ToEnding(measure, "\n");
        }

        public static MeasuresContainer ParseText(string text)
        {
            if (JsonUtilities.IsJson(text))
            {
                return ParseJson(text);
            }

            return ParseXmla(text);
        }

        public static MeasuresContainer ParseJson(string text)
        {
            try
            {
                var database = JsonUtilities.Deserialize(text);
                Debug.Assert(database != null);

                return CreateFromDatabase(database);
            }
            catch (Exception exception)
            {
                throw new DaxException(
$@"Error while parsing Json:
Message: {exception.Message}
Input text: {text}", exception);
            }
        }

        public static string GetXmlDatabaseText(string text)
        {
            var alter = XDocument.Parse(text);
            var database = alter.Descendants(NS + "Database").First();
            database.Add(new XAttribute("xmlns", NS.Trim('{', '}')));

            return database.ToString();
        }

        public static MeasuresContainer ParseXmla(string text)
        {
            try
            {
                text = GetXmlDatabaseText(text);
                using (var reader = new XmlTextReader(new StringReader(text)))
                {
                    var database = new Microsoft.AnalysisServices.Database();
                    database = Microsoft.AnalysisServices.Utils.Deserialize(reader, database) as Microsoft.AnalysisServices.Database;

                    return CreateFromXmlDatabase(database);
                }
            }
            catch (Exception exception)
            {
                throw new DaxException(
$@"Error while parsing Xmla.
Message: {exception.Message}
Input text: {text}", exception);
            }
        }

        public static MeasuresContainer CreateFromDatabase(Database database)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }
            if (database.Model == null)
            {
                throw new ArgumentNullException(nameof(database.Model));
            }

            var measures = new List<DaxMeasure>();
            foreach (var table in database.Model.Tables)
            {
                foreach (var tableMeasure in table.Measures)
                {
                    var measure = ToSystemEnding(tableMeasure);
                    var newMeasure = new DaxMeasure();
                    newMeasure.Name = measure.Name;
                    newMeasure.Expression = measure.Expression;
                    newMeasure.TableName = table.Name;
                    newMeasure.CalcProperty = DaxCalcProperty.CreateFromJsonMeasure(measure);
                    newMeasure.FullText =
    $"CREATE MEASURE '{newMeasure.TableName ?? ""}'[{newMeasure.Name ?? ""}] = {newMeasure.Expression ?? ""}";

                    measures.Add(newMeasure);
                }
            }

            return new MeasuresContainer(measures);
        }

        public static MeasuresContainer CreateFromXmlDatabase(Microsoft.AnalysisServices.Database database)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            var allMeasures = new List<DaxMeasure>();
            foreach (Microsoft.AnalysisServices.Cube cube in database.Cubes)
            {
                foreach (Microsoft.AnalysisServices.MdxScript script in cube.MdxScripts)
                {
                    foreach (Microsoft.AnalysisServices.Command command in script.Commands)
                    {
                        var measures = ParseDaxScript(command.Text);
                        foreach (var measure in measures.Measures)
                        {
                            foreach (Microsoft.AnalysisServices.CalculationProperty property in script.CalculationProperties)
                            {
                                if (property.CalculationReference != measure.NameInBrackets)
                                {
                                    continue;
                                }

                                measure.CalcProperty = DaxCalcProperty.CreateFromXmlProperty(property);
                                allMeasures.Add(measure);
                            }
                        }
                    }
                }
            }

            return new MeasuresContainer(allMeasures);
        }

        /// <summary>
        /// Parses measures from the mdx script text.  If there is an error during parsing an exception is thrown.
        /// </summary>
        /// <param name="daxScript">mdx script text</param>
        /// <returns>Enumerable dax measures</returns>
        public static MeasuresContainer ParseDaxScript(string daxScript)
        {
            var handler = new Babel.Parser.ErrorHandler();
            var scanner = new Babel.Lexer.Scanner();
            var parser = new Babel.Parser.Parser();  // use noarg constructor
            parser.scanner = scanner;
            scanner.Handler = handler;
            parser.SetHandler(handler);
            scanner.SetSourceText(daxScript);

            var request = new ParseRequest(true);
            request.Sink = new AuthoringSink(ParseReason.None, 0, 0, Babel.Parser.Parser.MaxErrors);
            parser.MBWInit(request);
            var parseResult = parser.Parse();
            if (handler.Errors)
                throw DaxParsingException.FromHandler(handler, daxScript);

            return new MeasuresContainer(parser.Measures);
        }

        public string GetDaxText()
        {
            var builder = new StringBuilder();
            foreach (var measure in Measures)
            {
                builder.Append(measure.FullText);
                var propertyDax = measure.CalcProperty?.ToDax();
                if (!string.IsNullOrWhiteSpace(propertyDax))
                {
                    builder.AppendLine();
                    builder.Append(propertyDax);
                }
                builder.AppendLine();
                builder.Append(';');
                builder.AppendLine();
                builder.AppendLine();
            }
            builder.AppendLine();

            return builder.ToString();
        }

        /// <summary>
        /// Update input XMLA with the measures of this object.
        /// </summary>
        /// <param name="text">input XMLA text</param>
        /// <exception cref="ArgumentException">Exception if text is null or whitespace</exception>
        /// <returns>updated XMLA with measures from this object</returns>
        public string UpdateMeasuresInXmla(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(nameof(text) + " is empty");
            }

            /*
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8);
            var db = new Microsoft.AnalysisServices.Database();
            Microsoft.AnalysisServices.Utils.Serialize(writer, db, true);
            stream.Position = 0;
            Console.WriteLine(new StreamReader(stream).ReadToEnd());
            //*/

            var producer = new ServerCommandProducer(text);
            var document = XDocument.Parse(text);

            var newScript = producer.ProduceAlterScriptElement(Measures);
            var newScriptDocument = XDocument.Parse(newScript);
            document.Descendants(NS + "MdxScript").First().ReplaceWith(newScriptDocument.Root);

            return document.ToString(System.Xml.Linq.SaveOptions.OmitDuplicateNamespaces);
        }

        public static IDictionary<string, IList<Measure>> ToJsonMeasures(IList<DaxMeasure> measures)
        {
            if (measures == null)
            {
                throw new ArgumentNullException(nameof(measures));
            }

            var dictionary = new Dictionary<string, IList<Measure>>();
            foreach (var measure in measures)
            {
                var jsonMeasure = new Measure();
                jsonMeasure.Name = measure.Name;
                jsonMeasure.Expression = measure.Expression;
                measure.CalcProperty?.ToJsonMeasure(ref jsonMeasure);

                //JsonSerializer does not support /r/n endings.
                jsonMeasure = ToUnixEnding(jsonMeasure);

                if (!dictionary.ContainsKey(measure.TableName))
                {
                    dictionary[measure.TableName] = new List<Measure>();
                }
                dictionary[measure.TableName].Add(jsonMeasure);
            }

            return dictionary;
        }

        /// <summary>
        /// Update input Json with the measures of this object.
        /// </summary>
        /// <param name="text">Input Json text</param>
        /// <exception cref="ArgumentException">Exception if text is null or whitespace</exception>
        /// <returns>updated Json with measures from this object</returns>
        public string UpdateMeasuresInJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(nameof(text) + " is empty");
            }

            var document = JsonUtilities.Deserialize(text);
            var tables = document?.Model?.Tables;
            if (tables == null)
            {
                return text;
            }

            var jsonMeasures = ToJsonMeasures(Measures);
            foreach (var table in tables)
            {
                table.Measures.Clear();
                if (!jsonMeasures.ContainsKey(table.Name))
                {
                    continue;
                }

                foreach (var measure in jsonMeasures[table.Name])
                {
                    table.Measures.Add(measure.Clone());
                }
            }

            //Fix for serialize translation internal error
            foreach (var culture in document.Model.Cultures)
            {
                var newObjects = new List<ObjectTranslation>();
                foreach (var translation in culture.ObjectTranslations)
                {
                    var obj = translation.Object;

                    //Find all the translated actions and create objects for them
                    if (obj.ObjectType == ObjectType.Measure)
                    {
                        var objMeasure = obj as Measure;
                        foreach (var table in document.Model.Tables)
                        {
                            foreach (var measure in table.Measures)
                            {
                                if (measure.Name == objMeasure.Name)
                                {
                                    var newObject = new ObjectTranslation();
                                    newObject.Object = measure;
                                    newObject.Property = translation.Property;
                                    newObject.Value = translation.Value;
                                    newObjects.Add(newObject);
                                }
                            }
                        }
                    }
                    //Readd other objects
                    else
                    {
                        newObjects.Add(translation.Clone());
                    }
                }

                culture.ObjectTranslations.Clear();
                foreach (var newObject in newObjects)
                {
                    culture.ObjectTranslations.Add(newObject);
                }
            }

            return JsonUtilities.Serialize(document);
        }

        public string UpdateMeasures(string text)
        {
            if (JsonUtilities.IsJson(text))
            {
                return UpdateMeasuresInJson(text);
            }

            return UpdateMeasuresInXmla(text);
        }
    }
}
