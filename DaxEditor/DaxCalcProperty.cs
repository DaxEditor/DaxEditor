// The project released under MS-PL license https://daxeditor.codeplex.com/license

using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DaxEditor.StringExtensions;

namespace DaxEditor
{
    public class DaxCalcProperty
    {
        public enum FormatType
        {
            General,
            NumberDecimal,
            NumberWhole,
            Percentage,
            Scientific,
            Currency,
            DateTimeCustom,
            DateTimeShortDatePattern,
            DateTimeGeneral,
            Text,
        }

        public FormatType Format { get; set; }
        public int? Accuracy { get; set; }
        public bool? ThousandSeparator { get; set; }
        public string CustomFormat { get; set; }

        public KPI KPI
        {
            get { return Measure.KPI; }
            set { Measure.KPI = value; }
        }

        public Measure Measure { get; set; }
        

        private DaxCalcProperty()
        {
            Measure = new Measure();
        }

        public static DaxCalcProperty CreateDefaultCalculationProperty()
        {
            var rv = new DaxCalcProperty();
            return rv;
        }

        public static DaxCalcProperty CreateFromXElement(XElement cp)
        {
            var rv = CreateDefaultCalculationProperty();

            if (cp != null)
            {
                Debug.Assert(cp.Name.LocalName == "CalculationProperty");
                var annotations = cp.Element(MeasuresContainer.NS + "Annotations");
                var formatStringElement = cp.Element(MeasuresContainer.NS + "FormatString");
                rv.Measure.FormatString = formatStringElement?.Value?.Trim('\'');
                
                //var calculationTypeElement = cp.Element(MeasuresContainer.NS + "CalculationType");
                //rv.CalculationType = calculationTypeElement != null ? calculationTypeElement.Value : _defaultCalculationType;

                var displayFolderElement = cp.Element(MeasuresContainer.NS + "DisplayFolder");
                if (displayFolderElement != null)
                    rv.Measure.DisplayFolder = displayFolderElement.Value;
                
                var visibleElement = cp.Element(MeasuresContainer.NS + "Visible");
                if (visibleElement != null)
                    rv.Measure.IsHidden = !bool.Parse(visibleElement.Value);

                var descriptionElement = cp.Element(MeasuresContainer.NS + "Description");
                if (descriptionElement != null)
                    rv.Measure.Description = descriptionElement.Value.Replace("'", "`");

                if (annotations != null)
                {
                    var formatAnnotationElement = annotations.Elements(MeasuresContainer.NS + "Annotation").FirstOrDefault(i => string.Equals(i.Element(MeasuresContainer.NS + "Name").Value, "Format"));
                    if (formatAnnotationElement != null)
                    {
                        var formatValueElement = formatAnnotationElement.Element(MeasuresContainer.NS + "Value").Element("Format");
                        Debug.Assert(formatValueElement != null);
                        Debug.Assert(formatValueElement.Name == "Format");
                        var formatString = formatValueElement.Attribute("Format").Value;
                        rv.Format = (FormatType)Enum.Parse(typeof(FormatType), formatString);
                        
                        switch (rv.Format)
                        {
                            case FormatType.General:
                            case FormatType.NumberWhole:
                            case FormatType.NumberDecimal:
                            case FormatType.Percentage:
                            case FormatType.Scientific:
                            case FormatType.Currency:
                            case FormatType.DateTimeCustom:
                            case FormatType.DateTimeShortDatePattern:
                            case FormatType.DateTimeGeneral:
                                var accuracyAttribute = formatValueElement.Attribute("Accuracy");
                                if (accuracyAttribute != null)
                                    rv.Accuracy = int.Parse(accuracyAttribute.Value);

                                var thousandSeparatorAttribute = formatValueElement.Attribute("ThousandSeparator");
                                if (thousandSeparatorAttribute != null)
                                    rv.ThousandSeparator = bool.Parse(thousandSeparatorAttribute.Value);

                                if (formatValueElement.FirstNode != null)
                                    rv.CustomFormat = InitCustomFormatString((XElement)formatValueElement.FirstNode);

                                break;

                            case FormatType.Text:
                                break;

                            default:
                                Debug.Assert(false, "Not reachable");
                                break;
                        }
                    }
                }
            }

            return rv;
        }

        public static DaxCalcProperty CreateFromJsonMeasure(Measure measure)
        {
            if (measure == null)
            {
                throw new ArgumentNullException("measure");
            }

            var rv = CreateDefaultCalculationProperty();

            rv.Measure.FormatString = measure.FormatString?.Trim('\'');
            rv.Measure.DisplayFolder = measure.DisplayFolder?.Replace("'", "`");
            rv.Measure.IsHidden = measure.IsHidden;
            rv.Measure.Description = measure.Description?.Replace("'", "`");
            rv.KPI = measure.KPI?.Clone();

            var formatValueString = measure.Annotations?.
                FirstOrDefault(i => string.Equals(i.Name, "Format"))?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(formatValueString))
            {
                return rv;
            }

            var formatValueElement = XDocument.Parse(formatValueString).Root;
            Debug.Assert(formatValueElement != null);
            Debug.Assert(formatValueElement.Name == "Format");
            var formatString = formatValueElement.Attribute("Format").Value;
            rv.Format = (FormatType)Enum.Parse(typeof(FormatType), formatString);
            
            switch (rv.Format)
            {
                case FormatType.General:
                case FormatType.NumberWhole:
                case FormatType.NumberDecimal:
                case FormatType.Percentage:
                case FormatType.Scientific:
                case FormatType.Currency:
                case FormatType.DateTimeCustom:
                case FormatType.DateTimeShortDatePattern:
                case FormatType.DateTimeGeneral:
                    var accuracyAttribute = formatValueElement.Attribute("Accuracy");
                    if (accuracyAttribute != null)
                        rv.Accuracy = int.Parse(accuracyAttribute.Value);

                    var thousandSeparatorAttribute = formatValueElement.Attribute("ThousandSeparator");
                    if (thousandSeparatorAttribute != null)
                        rv.ThousandSeparator = bool.Parse(thousandSeparatorAttribute.Value);

                    if (formatValueElement.FirstNode != null)
                        rv.CustomFormat = InitCustomFormatString((XElement)formatValueElement.FirstNode);

                    break;

                case FormatType.Text:
                    break;

                default:
                    Debug.Assert(false, "Not reachable");
                    break;
            }

            return rv;
        }

        private static string InitCustomFormatString(XElement customFormatElement)
        {
            Debug.Assert(customFormatElement != null);
            Debug.Assert(customFormatElement.Name.LocalName == "Currency" || customFormatElement.Name.LocalName == "DateTimes");
            if (customFormatElement.Name.LocalName == "DateTimes")
                customFormatElement = customFormatElement.Element("DateTime");

            return string.Join(" ", 
                customFormatElement.Attributes().Select(
                    i => $@"{i.Name}=""{i.Value}"""
                )
            );
        }

        private string AppendIfNotEmpty(string text, string name, string value, string quote = "")
        {
            //Delete ToSystemEnding() after support consistent lines in xml format
            if (!string.IsNullOrWhiteSpace(value))
            {
                text += $"{Environment.NewLine}    {name.ToUpperInvariant()} = {quote}{value.ToSystemEnding()}{quote}";
            }

            return text;
        }

        public string ToDax()
        {
            // skip property only if general and visible
            if (Format == FormatType.General &&
                !Measure.IsHidden &&
                string.IsNullOrWhiteSpace(Measure.FormatString) &&
                string.IsNullOrWhiteSpace(Measure.Description) &&
                string.IsNullOrWhiteSpace(Measure.DisplayFolder) &&
                KPI == null)
            {
                return string.Empty;
            }

            string result;
            switch (Format)
            {
                case FormatType.General:
                case FormatType.NumberDecimal:
                case FormatType.NumberWhole:
                case FormatType.Percentage:
                case FormatType.Scientific:
                case FormatType.Currency:
                case FormatType.DateTimeCustom:
                case FormatType.Text:
                case FormatType.DateTimeShortDatePattern:
                case FormatType.DateTimeGeneral:
                    result = "CALCULATION PROPERTY " + Format.ToString().ToUpperInvariant();
                    if (Measure.IsHidden)
                        result = AppendIfNotEmpty(result, "Visible", "False");

                    if (Accuracy.HasValue)
                        result = AppendIfNotEmpty(result, "Accuracy", Accuracy.Value.ToString());

                    if (ThousandSeparator.HasValue && ThousandSeparator.Value == true)
                        result = AppendIfNotEmpty(result, "ThousandSeparator", "True");

                    result = AppendIfNotEmpty(result, "Format", Measure.FormatString, "\'");
                    result = AppendIfNotEmpty(result, "AdditionalInfo", CustomFormat, "\'");
                    result = AppendIfNotEmpty(result, "DisplayFolder", Measure.DisplayFolder, "\'");
                    result = AppendIfNotEmpty(result, "Description", Measure.Description, "\'");

                    break;
                default:
                    Debug.Assert(false, "Not reachable");
                    result = string.Empty;
                    break;
            }

            if (KPI != null)
            {
                result = AppendIfNotEmpty(result, "KpiDescription", KPI.Description, "\"");
                result = AppendIfNotEmpty(result, "KpiTargetFormatString", KPI.TargetFormatString, "\"");
                result = AppendIfNotEmpty(result, "KpiTargetDescription", KPI.TargetDescription, "\"");
                result = AppendIfNotEmpty(result, "KpiTargetExpression", KPI.TargetExpression);
                result = AppendIfNotEmpty(result, "KpiStatusGraphic", KPI.StatusGraphic, "\"");
                result = AppendIfNotEmpty(result, "KpiStatusDescription", KPI.StatusDescription, "\"");
                result = AppendIfNotEmpty(result, "KpiStatusExpression", KPI.StatusExpression);
                result = AppendIfNotEmpty(result, "KpiTrendGraphic", KPI.TrendGraphic, "\"");
                result = AppendIfNotEmpty(result, "KpiTrendDescription", KPI.TrendDescription, "\"");
                result = AppendIfNotEmpty(result, "KpiTrendExpression", KPI.TrendExpression);
                
                var texts = new List<string>();
                foreach (var annotation in KPI.Annotations)
                {
                    texts.Add($"{annotation.Name} = \"{annotation.Value}\"");
                }
                if (KPI.Annotations.Count > 0)
                {
                    result = AppendIfNotEmpty(result, "KpiAnnotations", string.Join(", ", texts), "\'");
                }
            }

            return result;
        }

        public void ToXml(XmlWriter writer, string measureName)
        {
            writer.WriteStartElement("CalculationProperty");
            writer.WriteStartElement("Annotations");

            writer.WriteStartElement("Annotation");
            writer.WriteElementString("Name", "Type");
            writer.WriteElementString("Value", "User");
            writer.WriteEndElement(); // Annotation

            writer.WriteStartElement("Annotation");
            writer.WriteElementString("Name", "IsPrivate");
            writer.WriteElementString("Value", Measure.IsHidden ? "True" : "False");
            writer.WriteEndElement(); // Annotation

            writer.WriteStartElement("Annotation");
            writer.WriteElementString("Name", "Format");

            writer.WriteStartElement("Value");

            writeFormatToXml(writer);

            writer.WriteEndElement(); // Value
            
            writer.WriteEndElement(); // Annotation

            writer.WriteEndElement(); // Annotations
            writer.WriteElementString("CalculationReference", measureName);
            writer.WriteElementString("CalculationType", "Member");
            writer.WriteElementString("FormatString", $"\'{Measure.FormatString}\'");
            if (Measure.IsHidden) {
                writer.WriteElementString("Visible", "false");
            }
            if (!string.IsNullOrEmpty(Measure.DisplayFolder)) {
                writer.WriteElementString("DisplayFolder", Measure.DisplayFolder.Trim('\'').Replace("`", "'"));
            }

            if (!string.IsNullOrEmpty(Measure.Description))
                writer.WriteElementString("Description", Measure.Description.Trim('\'').Replace("`", "'"));
            

            writer.WriteEndElement(); // CalculationProperty
        }

        private void writeFormatToXml(XmlWriter writer)
        {

            writer.WriteStartElement("Format", "");
            writer.WriteAttributeString("Format", Format.ToString());
            if (Accuracy.HasValue)
                writer.WriteAttributeString("Accuracy", Accuracy.Value.ToString());
            if (ThousandSeparator.HasValue && ThousandSeparator.Value == true)
                writer.WriteAttributeString("ThousandSeparator", "True");

            if (!string.IsNullOrEmpty(CustomFormat))
            {
                if (Format == FormatType.Currency)
                {
                    writer.WriteRaw("<Currency ");
                    writer.WriteRaw(CustomFormat);
                    writer.WriteRaw(" />");
                }
                else if (Format == FormatType.DateTimeCustom)
                {
                    writer.WriteStartElement("DateTimes");
                    writer.WriteRaw("<DateTime ");
                    writer.WriteRaw(CustomFormat);
                    writer.WriteRaw(" />");
                    writer.WriteEndElement();
                }
                // No additional format code in case there is a format string for other format types
            }
            writer.WriteEndElement(); // Format
        }

        private string produceFormatXmlString()
        {
            if (Format == FormatType.General)
            {
                return string.Empty;
            }

            var builder = new System.Text.StringBuilder();
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            using (var writer = XmlWriter.Create(builder,settings))
            {
                writeFormatToXml(writer);
            }

            return builder.ToString();
        }

        public void ToJsonMeasure(ref Measure measure, string measureName)
        {
            measure.IsHidden = Measure.IsHidden;

            var formatAnnotationValue = produceFormatXmlString();
            if (!string.IsNullOrWhiteSpace(formatAnnotationValue))
            {
                var annotation = new Annotation();
                annotation.Name = "Format";
                annotation.Value = formatAnnotationValue;
                measure.Annotations.Add(annotation);
            }

            measure.FormatString = Measure.FormatString;
            
            if (!string.IsNullOrWhiteSpace(Measure.DisplayFolder))
            {
                measure.DisplayFolder = Measure.DisplayFolder.Trim('\'').Replace("`", "'");
            }

            if (!string.IsNullOrWhiteSpace(Measure.Description))
            {
                measure.Description = Measure.Description.Trim('\'').Replace("`","'");
            }

            measure.KPI = KPI?.Clone();
        }
    }
}
