// The project released under MS-PL license https://daxeditor.codeplex.com/license

using DaxEditor.Json;
using DaxEditor.StringExtensions;
using Microsoft.VisualStudio.Package;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.AnalysisServices;
using Database = Microsoft.AnalysisServices.Tabular.Database;
using Measure = Microsoft.AnalysisServices.Tabular.Measure;
using ObjectTranslation = Microsoft.AnalysisServices.Tabular.ObjectTranslation;
using ObjectType = Microsoft.AnalysisServices.Tabular.ObjectType;

namespace DaxEditor
{
    public class MeasuresContainer
    {
        public const string NS = "{http://schemas.microsoft.com/analysisservices/2003/engine}";
        public const string NS200 = "{http://schemas.microsoft.com/analysisservices/2010/engine/200}";

        public IList<DaxMeasure> AllMeasures { get; private set; }

        public IList<DaxMeasure> Measures
        {
            get { return AllMeasures.Where(i => !i.Name.StartsWith("_")).ToList(); }
        }

        public IList<DaxMeasure> SupportingMeasures
        {
            get { return AllMeasures.Where(i => i.Name.StartsWith("_")).ToList(); }
        }

        public MeasuresContainer(IList<DaxMeasure> measures)
        {
            AllMeasures = measures;
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

        public static MeasuresContainer ParseXmla(string text)
        {
            try
            {
                var database = ServerCommandProducer.GetDatabase(text);
                Debug.Assert(database != null);

                return CreateFromXmlDatabase(database);
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
                    var newMeasure = new DaxMeasure(measure.Name, table.Name, measure.Expression);
                    newMeasure.CalcProperty = DaxCalcProperty.CreateFromJsonMeasure(measure);
                    
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
            foreach (Cube cube in database.Cubes)
            {
                foreach (MdxScript script in cube.MdxScripts)
                {
                    foreach (Command command in script.Commands)
                    {
                        var measures = ParseDaxScript(command.Text);
                        foreach (var measure in measures.AllMeasures)
                        {
                            foreach (CalculationProperty property in script.CalculationProperties)
                            {
                                if (property.CalculationReference != measure.NameInBrackets)
                                {
                                    continue;
                                }

                                var kpi = measure.CalcProperty?.KPI.Clone();
                                var newProperty = DaxCalcProperty.CreateFromXmlProperty(property);
                                measure.CalcProperty = newProperty;
                                if (newProperty.KPI != null)
                                {
                                    foreach (var annotation in newProperty.KPI.Annotations)
                                    {
                                        kpi.Annotations.Add(annotation.Clone());
                                    }
                                }
                                measure.CalcProperty.KPI = kpi;

                                allMeasures.Add(measure);
                            }
                        }
                    }
                }
            }

            return new MeasuresContainer(allMeasures);
        }

        public DaxMeasure GetSupportMeasure(string name)
        {
            return SupportingMeasures.Where(i => name == i.Name).FirstOrDefault();
        }

        public MdxScript ToMdxScript(int compatibilityLevel)
        {
            var script = new MdxScript();
            script.ID = "MdxScript";
            script.Name = "MdxScript";

            var firstCommand = new Command();
            firstCommand.Text = compatibilityLevel < 1103 ? 
                ServerCommandProducer.CommonCommandText1100 :
                ServerCommandProducer.CommonCommandText1103;
            script.Commands.Add(firstCommand);

            if (compatibilityLevel < 1103)
            {
                //Add all measures full text in one command
                var command = new Command();
                command.Text = ServerCommandProducer.DoNotModify1100 + string.Concat(
                                   Measures.Select(i => i.FullText + ";" + Environment.NewLine)
                               );
                script.Commands.Add(command);
            }
            else
            {
                foreach (var measure in Measures)
                {
                    var command = new Command();
                    command.Text = ServerCommandProducer.DoNotModify1103 + measure.FullText + ";" + Environment.NewLine;

                    if (measure.CalcProperty?.KPI != null)
                    {
                        var goalName = "_" + measure.Name +" Goal";
                        var goalMeasure = GetSupportMeasure(goalName);
                        if (goalMeasure != null)
                        {
                            command.Text += goalMeasure.FullText + "; " + Environment.NewLine;
                        }

                        var statusName = "_" + measure.Name + " Status";
                        var statusMeasure = GetSupportMeasure(statusName);
                        if (statusMeasure != null)
                        {
                            command.Text += statusMeasure.FullText + Environment.NewLine + "; " + Environment.NewLine;
                        }

                        var trendName = "_" + measure.Name + " Trend";
                        var trendMeasure = GetSupportMeasure(trendName);
                        if (trendMeasure != null)
                        {
                            command.Text += trendMeasure.FullText + Environment.NewLine + "; " + Environment.NewLine;
                        }

                        //ASSOCIATED_MEASURE_GROUP = 'DimCurrency'
                        command.Text += "CREATE KPI CURRENTCUBE." + measure.NameInBrackets +
                            " AS Measures." + measure.NameInBrackets;
                        command.Text += ", ASSOCIATED_MEASURE_GROUP = '" + measure.TableName + "'";
                        if (goalMeasure != null)
                        {
                            command.Text += ", GOAL = Measures." + goalMeasure.NameInBrackets;
                        }
                        if (statusMeasure != null)
                        {
                            command.Text += ", STATUS = Measures." + statusMeasure.NameInBrackets;
                        }
                        if (!string.IsNullOrWhiteSpace(measure.CalcProperty.KPI.StatusGraphic))
                        {
                            command.Text += ", STATUS_GRAPHIC = " + measure.CalcProperty.KPI.StatusGraphic;
                        }
                        if (trendMeasure != null)
                        {
                            command.Text += ", TREND = Measures." + trendMeasure.NameInBrackets;
                        }
                        if (!string.IsNullOrWhiteSpace(measure.CalcProperty.KPI.TrendGraphic))
                        {
                            command.Text += ", TREND_GRAPHIC = " + measure.CalcProperty.KPI.TrendGraphic;
                        }
                        command.Text += ";" + Environment.NewLine;
                    }
                    command.Annotations.Insert(0, "FullName", measure.Name);
                    command.Annotations.Insert(1, "Table", measure.TableName);

                    script.Commands.Add(command);
                }
            }


            foreach (var measure in Measures)
            {
                var property = measure.CalcProperty != null ?
                    measure.CalcProperty.ToXmlCalculationProperty(measure.NameInBrackets) :
                    DaxCalcProperty.CreateDefaultCalculationProperty().ToXmlCalculationProperty(measure.NameInBrackets);
                
                script.CalculationProperties.Add(property);

                if (measure.CalcProperty?.KPI != null)
                {
                    var kpiProperties = measure.CalcProperty.GetKpiProperties(measure.Name);
                    foreach (var kpiProperty in kpiProperties)
                    {
                        script.CalculationProperties.Add(kpiProperty);
                    }
                }
            }

            var lastProperty = new CalculationProperty();
            lastProperty.CalculationReference = compatibilityLevel < 1103 ? 
                "Measures.[__No measures defined]" :
                "[__XL_Count of Models]";
            lastProperty.CalculationType = CalculationType.Member;
            lastProperty.Visible = false;
            script.CalculationProperties.Add(lastProperty);

            return script;
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

            //Add support measures if measure contains KPIs
            var measures = parser.AllMeasures;
            var supportMeasures = new List<DaxMeasure>();
            var containsSupportMeasures = measures.Where(i => i.Name.StartsWith("_")).Count() != 0;
            if (!containsSupportMeasures)
            {
                foreach (var measure in measures)
                {
                    if (measure.CalcProperty?.KPI == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(measure.CalcProperty.KPI.TargetExpression))
                    {
                        var newMeasure = new DaxMeasure(
                            "_" + measure.Name + " Goal", 
                            measure.TableName, 
                            measure.CalcProperty.KPI.TargetExpression
                            );
                        supportMeasures.Add(newMeasure);
                    }

                    if (!string.IsNullOrWhiteSpace(measure.CalcProperty.KPI.StatusExpression))
                    {
                        var newMeasure = new DaxMeasure(
                            "_" + measure.Name + " Status",
                            measure.TableName,
                            measure.CalcProperty.KPI.StatusExpression
                            );
                        supportMeasures.Add(newMeasure);
                    }

                    if (!string.IsNullOrWhiteSpace(measure.CalcProperty.KPI.TrendExpression))
                    {
                        var newMeasure = new DaxMeasure(
                            "_" + measure.Name + " Trend",
                            measure.TableName,
                            measure.CalcProperty.KPI.TrendExpression
                            );
                        supportMeasures.Add(newMeasure);
                    }
                }
            }

            return new MeasuresContainer(measures.Union(supportMeasures).ToList());
        }

        public string GetDaxText()
        {
            var builder = new StringBuilder();
            foreach (var measure in Measures)
            {
                if (measure.Name.StartsWith("_"))
                {
                    continue;
                }

                builder.Append(measure.ToDax());
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

            var document = XDocument.Parse(text);

            var producer = new ServerCommandProducer(text);
            var newScript = producer.ProduceAlterScriptElement(this);
            var newScriptDocument = XDocument.Parse(newScript);
            document.Descendants(NS + "MdxScript").First().ReplaceWith(newScriptDocument.Root);
            
            return document.ToString(SaveOptions.OmitDuplicateNamespaces);
        }

        public static IDictionary<string, IList<Measure>> ToJsonMeasures(IEnumerable<DaxMeasure> measures)
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
