// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babel;

namespace DaxEditor.GeneratorSource
{
    public class DaxKeywords : List<DaxKeywordDescription>
    {
        public DaxKeywords()
        {
/* Begin generated code */
      this.Add(new DaxKeywordDescription() { Name = @"EVALUATE", Description = @"Evaluate keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"DEFINE", Description = @"Define keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"MEASURE", Description = @"Measure keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"ORDER", Description = @"Order keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"BY", Description = @"BY keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"TRUE", Description = @"True constant"});
      this.Add(new DaxKeywordDescription() { Name = @"FALSE", Description = @"False constant"});
      this.Add(new DaxKeywordDescription() { Name = @"ASC", Description = @"Ascending sort order"});
      this.Add(new DaxKeywordDescription() { Name = @"DESC", Description = @"Descending sort order"});
      this.Add(new DaxKeywordDescription() { Name = @"DAY", Description = @"Period equal to 1 day in time intelligence functions"});
      this.Add(new DaxKeywordDescription() { Name = @"MONTH", Description = @"Period equal to 1 month in time intelligence functions"});
      this.Add(new DaxKeywordDescription() { Name = @"YEAR", Description = @"Period equal to 1 year in time intelligence functions"});
      this.Add(new DaxKeywordDescription() { Name = @"CREATE", Description = @"CREATE keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"CALCULATE", Description = @"CALCULATE keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"CALCULATION", Description = @"CALCULATION keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"PROPERTY", Description = @"PROPERTY keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"GENERAL", Description = @"General calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"NUMBERDECIMAL", Description = @"NumberDecimal calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"NUMBERWHOLE", Description = @"NumberWhole calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"PERCENTAGE", Description = @"Percentage calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"SCIENTIFIC", Description = @"Scientific calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"CURRENCY", Description = @"Currency calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"DATETIMECUSTOM", Description = @"DateTimeCustom calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"DATETIMESHORTDATEPATTERN", Description = @"DateTimeShortDatePattern calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"DATETIMEGENERAL", Description = @"DateTimeGeneral calculation property format type"});
      this.Add(new DaxKeywordDescription() { Name = @"TEXT", Description = @"Text format type"});
      this.Add(new DaxKeywordDescription() { Name = @"ACCURACY", Description = @"Accuracy keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"THOUSANDSEPARATOR", Description = @"ThousandSeparator keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"FORMAT", Description = @"Format keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"ADDITIONALINFO", Description = @"AdditionalInfo keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"KPI", Description = @"Key Performance Indicator"});
      this.Add(new DaxKeywordDescription() { Name = @"VISIBLE", Description = @"Measure Visible Property"});
      this.Add(new DaxKeywordDescription() { Name = @"DESCRIPTION", Description = @"Measure Description Property"});
      this.Add(new DaxKeywordDescription() { Name = @"DISPLAYFOLDER", Description = @"Measure DisplayFolder Property"});
      this.Add(new DaxKeywordDescription() { Name = @"VAR", Description = @"Var keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"RETURN", Description = @"Return keyword"});
      this.Add(new DaxKeywordDescription() { Name = @"DATATABLE", Description = @"DATATABLE Function"});
      this.Add(new DaxKeywordDescription() { Name = @"BOOLEAN", Description = @"DataTable Boolean Column Type"});
      this.Add(new DaxKeywordDescription() { Name = @"DATETIME", Description = @"DataTable DateTime Column Type"});
      this.Add(new DaxKeywordDescription() { Name = @"DOUBLE", Description = @"DataTable Double Column Type"});
      this.Add(new DaxKeywordDescription() { Name = @"INTEGER", Description = @"DataTable Integer Column Type"});
      this.Add(new DaxKeywordDescription() { Name = @"STRING", Description = @"DataTable String Column Type"});
      this.Add(new DaxKeywordDescription() { Name = @"RANKX", Description = @"RANKX Function"});
      this.Add(new DaxKeywordDescription() { Name = @"SKIP", Description = @"RANKX Ties Enum Element"});
      this.Add(new DaxKeywordDescription() { Name = @"DENSE", Description = @"RANKX Ties Enum Element"});
      this.Add(new DaxKeywordDescription() { Name = @"NOT", Description = @"NOT operator"});
      this.Add(new DaxKeywordDescription() { Name = @"KPIDESCRIPTION", Description = @"Measure KpiDescription Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPITARGETFORMATSTRING", Description = @"Measure KpiTargetFormatString Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPITARGETDESCRIPTION", Description = @"Measure KpiTargetDescription Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPITARGETEXPRESSION", Description = @"Measure KpiTargetExpression Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPISTATUSGRAPHIC", Description = @"Measure KpiStatusGraphic Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPISTATUSDESCRIPTION", Description = @"Measure KpiStatusDescription Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPISTATUSEXPRESSION", Description = @"Measure KpiStatusExpression Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPITRENDGRAPHIC", Description = @"Measure KpiTrendGraphic Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPITRENDDESCRIPTION", Description = @"Measure KpiTrendDescription Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPITRENDEXPRESSION", Description = @"Measure KpiTrendExpression Property"});
      this.Add(new DaxKeywordDescription() { Name = @"KPIANNOTATIONS", Description = @"Measure KpiAnnotations Property"});
/* End generated code */
        }

        public IEnumerable<Declaration> GetDeclarations()
        {
            foreach (var keyword in this)
            {
                yield return (new Declaration() { Description = keyword.Description, DisplayText = keyword.Name, Glyph = 36, Name = keyword.Name });
            }
        }
    }
}





































