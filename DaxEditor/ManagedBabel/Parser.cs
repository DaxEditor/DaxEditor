/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;
using DaxEditor;
using Babel.ParserGenerator;
using System.Linq;
using System.Diagnostics;
using Microsoft.AnalysisServices.Tabular;

namespace Babel.Parser
{
    public partial class Parser
    {
        public static readonly int MaxErrors = 30;

        public void MBWInit(ParseRequest request)
        {
            this.request = request;
            braces = new List<TextSpan[]>();
        }

        ParseRequest request;
        IList<TextSpan[]> braces;
        IList<DaxMeasure> measures = new List<DaxMeasure>();

        public IList<TextSpan[]> Braces
        {
            get { return this.braces; }
        }

        public ParseRequest Request
        {
            get { return this.request; }
        }

        public IList<DaxMeasure> Measures
        {
            get { return this.measures; }
        }

        public AuthoringSink Sink
        {
            get { return this.request.Sink; }
        }

        // brace matching, pairs and triples
        public void DefineMatch(int priority, params TextSpan[] locations)
        {
            if (locations.Length == 2)
                braces.Add(new TextSpan[] { locations[0], 
					locations[1]});

            else if (locations.Length >= 3)
                braces.Add(new TextSpan[] { locations[0], 
					locations[1],
					locations[2]});
        }

        public void DefineMatch(params TextSpan[] locations)
        {
            DefineMatch(0, locations);
        }

        // hidden regions - not working?
        public void DefineRegion(TextSpan span)
        {
            Sink.AddHiddenRegion(span);
        }

        public void CreateNewMeasure(LexLocation tableRefLocation, LexLocation measureNameLocation)
        {
            var tableName = scanner.GetText(tableRefLocation);
            var measureName = scanner.GetText(measureNameLocation);

            if (tableName.StartsWith("'") && tableName.EndsWith("'"))
                tableName = tableName.Substring(1, tableName.Length - 2);

            if (measureName.StartsWith("[") && measureName.EndsWith("]"))
                measureName = measureName.Substring(1, measureName.Length - 2);

            measures.Add(new DaxMeasure() { TableName = tableName, Name = measureName });
        }

        public int GetFirstNonEmptyIndex(string str, int startIndex = 0)
        {
            return str.Substring(startIndex).
                TakeWhile(c => char.IsWhiteSpace(c) && c!='\n').Count() + startIndex;
        }

        private string GetLineText(int line)
        {
            return scanner.GetText(new LexLocation(line, 0, line + 1, 0));
        }

        public void SpecifyMeasureExpression(LexLocation s)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure.Expression == null);

            //Empty line before VAR fix
            for (int i = s.sLin; i >= 0; --i)
            {
                var line = i == s.sLin ?
                    scanner.GetText(new LexLocation(i, 0, i, s.sCol)) : 
                    scanner.GetText(new LexLocation(i, 0, i + 1, 0));
                var index = line.LastIndexOf('=');
                if (index != -1)
                {
                    s.sLin = i;
                    s.sCol = GetFirstNonEmptyIndex(line, index + 1);
                    break;
                }
            }

            //Append comments to expression
            //Add all content before non commented symvol(exclude whitespaces) or EOF
            var lastNonWhitespaceLine = s.eLin;
            var lastNonWhitespaceCol = s.eCol - 1;
            try
            {
                var inMultilineComment = false;
                for (var line = s.eLin; ; ++line)
                {
                    var inSingleLineComment = false;
                    var lineText = GetLineText(line);
                    var prevSymvol = '\n';
                    for (var col = line == s.eLin ? s.eCol : 0; col < lineText.Length - 1; ++col)
                    {
                        var c = lineText[col];

                        if (prevSymvol == '/' && c == '*')
                        {
                            Debug.WriteLine($"Start multiline comment. Line: {line}, Col: {col}, char: {c}");
                            inMultilineComment = true;
                        }
                        if (prevSymvol == '*' && c == '/')
                        {
                            Debug.WriteLine($"End multiline comment. Line: {line}, Col: {col}, char: {c}");
                            inMultilineComment = false;
                            lastNonWhitespaceLine = line;
                            lastNonWhitespaceCol = col;
                        }
                        if (prevSymvol == '/' && c == '/')
                        {
                            Debug.WriteLine($"Start single line comment. Line: {line}, Col: {col}, char: {c}");
                            inSingleLineComment = true;
                        }

                        //As soon as we meet the first uncommented symbol - 
                        //we exit the loop
                        if (!inMultilineComment &&
                            !inSingleLineComment &&
                            !char.IsWhiteSpace(c) &&
                            !char.IsControl(c) &&
                            c != '/')
                        {
                            Debug.WriteLine($"Final Line: {line}, Col: {col}, char: {c}");
                            throw new Exception("Instead goto");
                        }

                        if (!char.IsWhiteSpace(c) &&
                            !char.IsControl(c))
                        {
                            Debug.WriteLine($"Line: {line}, Col: {col}, char: {c}");
                            lastNonWhitespaceLine = line;
                            lastNonWhitespaceCol = col;
                        }
                        prevSymvol = c;
                    }
                }
            }
            catch (Exception e)
            {
                //We have reached the end of the file or received our exception.
                s.eLin = lastNonWhitespaceLine;
                s.eCol = lastNonWhitespaceCol + 1;
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }

            lastMeasure.Expression = scanner.GetText(s);
        }

        public void SpecifyFullMeasureText(LexLocation startLocation, LexLocation endLocation)
        {
            var completeLocation = startLocation.Merge(endLocation);
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure.FullText == null);
            lastMeasure.FullText = scanner.GetText(completeLocation);
        }

        public void SpecifyCalculationProperty(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure.FullText == null);
            var formatTypeText = scanner.GetText(location);
            try
            {
                var formatType = (DaxCalcProperty.FormatType)Enum.Parse(typeof(DaxCalcProperty.FormatType), formatTypeText, true);
                lastMeasure.CalcProperty = DaxCalcProperty.CreateDefaultCalculationProperty();
                lastMeasure.CalcProperty.Format = formatType;
            }
            catch (ArgumentException)
            {
                ReportError(MkTSpan(location), string.Format("Undefined calculation property format type '{0}'", formatTypeText));
            }
        }

        public void SpecifyCalcPropAccuracy(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            var accuracyText = scanner.GetText(location);
            lastMeasure.CalcProperty.Accuracy = int.Parse(accuracyText);
        }

        public void SpecifyCalcPropVisible(bool isVisible) {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.Visible = isVisible;
        }

        public void SpecifyCalcPropDescription(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.Description = scanner.GetText(location).Trim('\"');
        }

        public void SpecifyCalcPropKpiDescription(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.Description = scanner.GetText(location).Trim('\"');
        }

        public void SpecifyCalcPropKpiTargetFormatString(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.TargetFormatString = scanner.GetText(location).Trim('\"');
        }

        public void SpecifyCalcPropKpiTargetDescription(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.TargetDescription = scanner.GetText(location).Trim('\"');
        }

        public void SpecifyCalcPropKpiTargetExpression(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.TargetExpression = scanner.GetText(location);
        }

        public void SpecifyCalcPropKpiStatusGraphic(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.StatusGraphic = scanner.GetText(location).Trim('\"');
        }

        public void SpecifyCalcPropKpiStatusDescription(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.StatusDescription = scanner.GetText(location).Trim('\"');
        }

        public void SpecifyCalcPropKpiStatusExpression(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.StatusExpression = scanner.GetText(location);
        }

        public void SpecifyCalcPropKpiTrendGraphic(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.TrendGraphic = scanner.GetText(location).Trim('\"');
        }

        public void SpecifyCalcPropKpiTrendDescription(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.TrendDescription = scanner.GetText(location).Trim('\"');
        }

        public void SpecifyCalcPropKpiTrendExpression(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();
            lastMeasure.CalcProperty.KPI.TrendExpression = scanner.GetText(location);
        }

        public void SpecifyCalcPropKpiAnnotations(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.KPI = lastMeasure.CalcProperty.KPI ?? new KPI();

            //'GoalType=""Measure"", KpiStatusType= ""Linear"", KpiThresholdType=""Percentage"", KpiThresholdOrdering=""Ascending"", KpiThresholdCount=""2"", KpiThreshold_0=""40"",KpiThreshold_1 =""80""'
            var text = scanner.GetText(location).Trim('\'');
            var propertiesData = text.Split(',');

            foreach (var propertyData in propertiesData)
            {
                var values = propertyData.Trim(' ').Split('=');
                Debug.Assert(values.Length == 2);

                var name = values[0].Trim(' ');
                var value = values[1].Trim('\"', ' ');

                var annotation = new Annotation();
                annotation.Name = name;
                annotation.Value = value;

                lastMeasure.CalcProperty.KPI.Annotations.Add(annotation);
            }
        }

        public void SpecifyCalcPropDisplayFolder(LexLocation location) {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.DisplayFolder = scanner.GetText(location);
        }

        public void SpecifyCalcPropThousandSeparator(bool hasThousandSeparator)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.ThousandSeparator = hasThousandSeparator;
        }

        public void SpecifyCalcPropFormat(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.FormatString = scanner.GetText(location);
        }

        public void SpecifyCalcPropAdditionalInfo(LexLocation location)
        {
            var lastMeasure = measures.Last();
            Debug.Assert(lastMeasure != null);
            Debug.Assert(lastMeasure.CalcProperty != null);
            lastMeasure.CalcProperty.CustomFormat = scanner.GetText(location);
        }

        // error reporting
        public void ReportError(TextSpan span, string message, Severity severity)
        {
            Sink.AddError(request.FileName, message, span, severity);
        }

        #region Error Overloads (Severity)
        public void ReportError(TextSpan location, string message)
        {
            ReportError(location, message, Severity.Error);
        }

        public void ReportFatal(TextSpan location, string message)
        {
            ReportError(location, message, Severity.Fatal);
        }

        public void ReportWarning(TextSpan location, string message)
        {
            ReportError(location, message, Severity.Warning);
        }

        public void ReportHint(TextSpan location, string message)
        {
            ReportError(location, message, Severity.Hint);
        }
        #endregion

        #region TextSpan Conversion
        public TextSpan TextSpan(int startLine, int startIndex, int endIndex)
        {
            return TextSpan(startLine, startIndex, startLine, endIndex);
        }

        public TextSpan TextSpan(int startLine, int startIndex, int endLine, int endIndex)
        {
            TextSpan ts;
            ts.iStartLine = startLine - 1;
            ts.iStartIndex = startIndex;
            ts.iEndLine = endLine - 1;
            ts.iEndIndex = endIndex;
            return ts;
        }
        #endregion
    }
}