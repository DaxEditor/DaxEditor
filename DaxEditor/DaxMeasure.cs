// The project released under MS-PL license https://daxeditor.codeplex.com/license

namespace DaxEditor
{
    public class DaxMeasure
    {
        public DaxMeasure()
        {}

        public DaxMeasure(string name, string tableName, string expression)
        {
            Name = name;
            TableName = tableName;
            Expression = expression;
            FullText = $"CREATE MEASURE '{TableName ?? ""}'[{Name ?? ""}] = {Expression ?? ""}";
        }

        /// <summary>
        /// Name of the table where measure belongs to.  TableName should be escaped if it is required.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Measure name, without brackets
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Measure expression - text that goes after = in the measure definition.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Full text of a measure, starting with CREATE MEASURE. Does not include ';'.
        /// </summary>
        public string FullText { get; set; }

        /// <summary>
        /// Measure name with brackets
        /// </summary>
        public string NameInBrackets { get { return "[" + Name + "]"; } }

        /// <summary>
        /// Scope
        /// </summary>
        public string Scope { get; set; } = string.Empty;

        public string ToDax()
        {
            var builder = new System.Text.StringBuilder();

            if (!string.IsNullOrWhiteSpace(Scope))
            {
                builder.AppendLine(Scope);
                builder.AppendLine();
                builder.AppendLine();
            }

            builder.Append(FullText);
            var propertyDax = CalcProperty?.ToDax();
            if (!string.IsNullOrWhiteSpace(propertyDax))
            {
                builder.AppendLine();
                builder.Append(propertyDax);
            }
            builder.AppendLine();
            builder.Append(';');
            builder.AppendLine();
            builder.AppendLine();

            return builder.ToString();
        }

        /// <summary>
        /// Calculation property for the measure
        /// </summary>
        public DaxCalcProperty CalcProperty { get; set; }
    }
}
