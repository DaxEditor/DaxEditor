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

            return ParseXmlaAsText(text);
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
{text}", exception);
            }
        }

        public static MeasuresContainer ParseXmlaAsText(string xmlaText)
        {
            try
            {
                var alterScriptDoc = XDocument.Parse(xmlaText);
                var mdxScript = alterScriptDoc.Descendants(NS + "MdxScript");
                Debug.Assert(1 == mdxScript.Count());
                return ParseXmlaAsXElement(mdxScript.First());
            }
            catch (Exception e)
            {
                throw new DaxException(string.Format("Error while parsing xmlaScript:{0}{2}{0}{1}", Environment.NewLine, xmlaText, e.Message), e);
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

        public static MeasuresContainer ParseXmlaAsXElement(XElement mdxScript)
        {
            var allMeasures = new List<DaxMeasure>();
            foreach (var mdxCommand in mdxScript.Element(NS + "Commands").Elements(NS + "Command"))
            {
                var commandText = mdxCommand.Element(NS + "Text").Value;
                var measures = ParseDaxScript(commandText);
                foreach (var measure in measures.Measures)
                {
                    var calcProperty = mdxScript.Element(NS + "CalculationProperties").Elements().FirstOrDefault(i => i.Element(NS + "CalculationReference").Value == measure.NameInBrackets);
                    measure.CalcProperty = DaxCalcProperty.CreateFromXElement(calcProperty);
                    allMeasures.Add(measure);
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
            var sb = new StringBuilder();
            foreach (var measure in Measures)
            {
                var measureText = measure.FullText;
                sb.Append(measure.FullText);
                if (measure.CalcProperty != null)
                {
                    var calcPropertyDax = measure.CalcProperty.ToDax();
                    if (!string.IsNullOrEmpty(calcPropertyDax))
                    {
                        sb.AppendLine();
                        sb.Append(calcPropertyDax);
                    }
                }
                sb.AppendLine();
                sb.Append(';');
                sb.AppendLine();
                sb.AppendLine();
            }
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Update input XMLA with the measures of this object.
        /// </summary>
        /// <param name="inputXmla">input XMLA</param>
        /// <returns>updated XMLA with measures from this object</returns>
        public string UpdateMeasuresInXmla(string inputXmla)
        {
            if (string.IsNullOrEmpty(inputXmla))
                throw new ArgumentException("input");

            var serverCommandProducer = new ServerCommandProducer(inputXmla);
            var newMdxScript = serverCommandProducer.ProduceAlterScriptElement(Measures);
            var inputDocument = XDocument.Parse(inputXmla);

            var actualMdxScriptDocument = XDocument.Parse(newMdxScript);
            inputDocument.Descendants(MeasuresContainer.NS + "MdxScript").First().ReplaceWith(actualMdxScriptDocument.Root);

            return inputDocument.ToString(System.Xml.Linq.SaveOptions.OmitDuplicateNamespaces);
        }

        public static IDictionary<string, IList<Measure>> MeasuresToJsonMeasures(IList<DaxMeasure> measures)
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
                measure.CalcProperty?.ToJsonMeasure(ref jsonMeasure, measure.Name);

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
        /// <param name="inputJson">input Json</param>
        /// <returns>updated Json with measures from this object</returns>
        public string UpdateMeasuresInJson(string inputJson)
        {
            if (string.IsNullOrWhiteSpace(inputJson))
                throw new ArgumentException("inputJson");

            var document = JsonUtilities.Deserialize(inputJson);
            var jsonMeasures = MeasuresToJsonMeasures(Measures);

            Debug.Assert(document?.Model != null);

            var tables = document.Model.Tables;
            if (tables != null)
            {
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
