// The project released under MS-PL license https://daxeditor.codeplex.com/license

using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

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
        public string FormatString { get; set; }
        public string CalculationType { get; set; }
        public string CustomFormat { get; set; }
        public bool? Visible { get; set; }
        public string DisplayFolder { get; set; } 
        public string Description { get; set; }

        private static readonly string _defaultFormatString = "''";
        private static readonly string _defaultDisplayFolder = string.Empty;
        private static readonly string _defaultCalculationType = "Member";

        private DaxCalcProperty()
        {
        }

        public static DaxCalcProperty CreateDefaultCalculationProperty()
        {
            var rv = new DaxCalcProperty();
            rv.FormatString = _defaultFormatString;
            rv.DisplayFolder = _defaultDisplayFolder;
            rv.CalculationType = _defaultCalculationType;
            rv.Description = string.Empty;
            return rv;
        }

        public static DaxCalcProperty CreateFromXElement(XElement cp)
        {
            var rv = new DaxCalcProperty();

            if (cp != null)
            {
                Debug.Assert(cp.Name.LocalName == "CalculationProperty");
                var annotations = cp.Element(MeasuresContainer.NS + "Annotations");
                var formatStringElement = cp.Element(MeasuresContainer.NS + "FormatString");
                rv.FormatString = formatStringElement != null ? formatStringElement.Value : _defaultFormatString;
                
                var calculationTypeElement = cp.Element(MeasuresContainer.NS + "CalculationType");
                rv.CalculationType = calculationTypeElement != null ? calculationTypeElement.Value : _defaultCalculationType;

                var displayFolderElement = cp.Element(MeasuresContainer.NS + "DisplayFolder");
                if (displayFolderElement != null)
                    rv.DisplayFolder = displayFolderElement != null ? displayFolderElement.Value : _defaultDisplayFolder;
                
                var visibleElement = cp.Element(MeasuresContainer.NS + "Visible");
                if (visibleElement != null)
                    rv.Visible = bool.Parse ( visibleElement.Value );

                var descriptionElement = cp.Element(MeasuresContainer.NS + "Description");
                if (descriptionElement != null)
                    rv.Description = descriptionElement.Value.Replace("'", "`");

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
            else
            {
                rv.FormatString = "''";
                rv.CalculationType = "Member";
                rv.DisplayFolder = _defaultDisplayFolder;
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

            rv.FormatString = measure.FormatString != null ? "'" + measure.FormatString + "'" : _defaultFormatString;
            rv.DisplayFolder = measure.DisplayFolder?.ToString()?.Replace("'", "`") ?? string.Empty;
            rv.Visible = !measure.IsHidden;
            rv.Description = measure.Description?.ToString()?.Replace("'", "`") ?? string.Empty;

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

            string customFormat = "'";
            customFormat += string.Join(" ", customFormatElement.Attributes().Select(i => string.Format(@"{0}=""{1}""", i.Name, i.Value)));
            customFormat += "'";
            return customFormat;
        }

        public string ToDax()
        {
            // skip property only if general and visible
            if (Format == FormatType.General && 
                FormatString == _defaultFormatString &&
                !(Visible.HasValue && Visible.Value == false)
                && string.IsNullOrWhiteSpace(Description) 
                && string.IsNullOrWhiteSpace(DisplayFolder) )
                return string.Empty;

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
                    result = "CALCULATION PROPERTY " + Format.ToString();
                    if (Visible.HasValue && Visible.Value == false)
                        result += " Visible=False";

                    if (Accuracy.HasValue)
                        result += " Accuracy=" + Accuracy.Value;

                    if (ThousandSeparator.HasValue && ThousandSeparator.Value == true)
                        result += " ThousandSeparator=True";

                    if (!string.IsNullOrEmpty(FormatString) && !string.Equals("''", FormatString))
                        result += " Format=" + FormatString;

                    if (!string.IsNullOrEmpty(CustomFormat))
                        result += " AdditionalInfo=" + CustomFormat;

                    if (!string.IsNullOrEmpty(DisplayFolder))
                        result += " DisplayFolder='" + DisplayFolder + "\'";

                    if (!string.IsNullOrEmpty(Description))
                        result += " Description='" + Description + "\'";

                    break;
                default:
                    Debug.Assert(false, "Not reachable");
                    result = string.Empty;
                    break;
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
            writer.WriteElementString("Value", (Visible.HasValue && (Visible.Value == false)) ? "True" : "False");
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
            writer.WriteElementString("FormatString", FormatString);
            if (Visible.HasValue && (Visible.Value == false)) {
                writer.WriteElementString("Visible", "false");
            }
            if (!string.IsNullOrEmpty(DisplayFolder)) {
                writer.WriteElementString("DisplayFolder", DisplayFolder.Trim('\'').Replace("`", "'"));
            }

            if (!string.IsNullOrEmpty(Description))
                writer.WriteElementString("Description", Description.Trim('\'').Replace("`", "'"));
            

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
                    writer.WriteRaw(CustomFormat.Trim('\''));
                    writer.WriteRaw(" />");
                }
                else if (Format == FormatType.DateTimeCustom)
                {
                    writer.WriteStartElement("DateTimes");
                    writer.WriteRaw("<DateTime ");
                    writer.WriteRaw(CustomFormat.Trim('\''));
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
            //var nameAnnotation = new Annotation();
            //nameAnnotation.Name = "Type";
            //nameAnnotation.Value = "User";
            //measure.Annotations = measure.Annotations ?? new List<Annotation>();
            //measure.Annotations.Add(nameAnnotation);

            var isPrivate = Visible.HasValue && Visible.Value == false;
            if (isPrivate)
            {
                measure.IsHidden = true;

                //var annotation = new Annotation();
                //annotation.Name = "IsPrivate";
                //annotation.Value = new Json.Tabular.String("True");
                //measure.Annotations = measure.Annotations ?? new List<Annotation>();
                //measure.Annotations.Add(annotation);
            }

            var formatAnnotationValue = produceFormatXmlString();
            if (!string.IsNullOrWhiteSpace(formatAnnotationValue))
            {
                var annotation = new Annotation();
                annotation.Name = "Format";
                annotation.Value = formatAnnotationValue;
                //measure.Annotations = measure.Annotations ?? new List<Annotation>();
                measure.Annotations.Add(annotation);
            }

            measure.FormatString = FormatString == _defaultFormatString ? null : FormatString.Trim('\'');
            
            if (!string.IsNullOrWhiteSpace(DisplayFolder))
            {
                measure.DisplayFolder = DisplayFolder.Trim('\'').Replace("`", "'");
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                measure.Description = Description.Trim('\'').Replace("`","'");
            }
        }
    }
}
