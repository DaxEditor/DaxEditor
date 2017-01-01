// The project released under MS-PL license https://daxeditor.codeplex.com/license

using DaxEditor.Json;
//using DaxEditor.Json.Tabular;
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

        public static MeasuresContainer ParseText(string text)
        {
            if (JsonUtilities.IsJson(text))
            {
                return ParseJsonAsText(text);
            }

            return ParseXmlaAsText(text);
        }

        public static MeasuresContainer ParseJsonAsText(string jsonText)
        {
            try
            {
                var jsonObject = JsonUtilities.Deserialize(jsonText);
                Debug.Assert(null != jsonObject);
                
                return ParseJsonAsJsonObject(jsonObject);
            }
            catch (Exception e)
            {
                throw new DaxException(string.Format("Error while parsing jsonScript:{0}{2}{0}{1}", Environment.NewLine, jsonText, e.Message), e);
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

        public static MeasuresContainer ParseJsonAsJsonObject(Database jsonObject)
        {
            if (jsonObject == null)
            {
                throw new ArgumentNullException("jsonObject");
            }

            var allMeasures = new List<DaxMeasure>();
            jsonObject.Model?.Tables?.ForEach(jsonTable =>
            {
                jsonTable.Measures?.ForEach(jsonMeasure =>
                {
                    var measure = new DaxMeasure();
                    measure.Name = jsonMeasure.Name;
                    measure.Expression = jsonMeasure.Expression;
                    measure.TableName = jsonTable.Name;
                    measure.CalcProperty = DaxCalcProperty.CreateFromJsonMeasure(jsonMeasure);
                    measure.FullText = 
                        $"CREATE MEASURE '{measure.TableName ?? ""}'[{measure.Name ?? ""}] = {measure.Expression ?? ""}";

                    allMeasures.Add(measure);
                });
            });

            return new MeasuresContainer(allMeasures);
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
                throw new ArgumentNullException("measures");
            }

            var dictionary = new Dictionary<string, IList<Measure>>();
            measures.ForEach(measure =>
            {
                var jsonMeasure = new Measure();
                jsonMeasure.Name = measure.Name;
                jsonMeasure.Expression = measure.Expression.Replace("\r\n", "\n");
                if (!dictionary.ContainsKey(measure.TableName))
                {
                    dictionary[measure.TableName] = new List<Measure>();
                }

                measure.CalcProperty?.ToJsonMeasure(ref jsonMeasure, measure.Name);
                dictionary[measure.TableName].Add(jsonMeasure);
            });

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
            
            //Fix Internal Exception
            //var culturesText = JsonUtilities.SerializeCultures(document?.Model?.Cultures);
            //document?.Model?.Cultures?.Clear();
            
            var tables = document?.Model?.Tables;
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

            /*
            foreach (var culture in document?.Model?.Cultures)
            {
                foreach (var translation in culture.ObjectTranslations)
                {
                    foreach (var error in translation.Validate().Errors)
                    {
                        Console.WriteLine(error.Message);
                        Console.WriteLine(translation.Object);
                    }
                    var obj = translation.Object;
                    if (obj.ObjectType == ObjectType.Measure)
                    {
                        var objMeasure = obj as Measure;
                        foreach (var table in document?.Model?.Tables)
                        {
                            foreach (var measure in table.Measures)
                            {
                                if (measure.Name == objMeasure.Name)
                                {
                                    translation.Object = measure;
                                }
                            }
                        }
                    }
                }
            }
            */

            var json = JsonUtilities.Serialize(document);
            //var indexOfTables = json.IndexOf("\"tables\":");
            //json = indexOfTables != -1 ? json.Insert(indexOfTables, culturesText) : json;
            //Console.WriteLine(json);
            return json;
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
