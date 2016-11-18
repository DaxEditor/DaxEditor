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
            lastMeasure.CalcProperty.Description = scanner.GetText(location);
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