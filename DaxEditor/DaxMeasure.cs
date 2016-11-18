// The project released under MS-PL license https://daxeditor.codeplex.com/license

namespace DaxEditor
{
    public class DaxMeasure
    {
        /// <summary>
        /// Name of the table where measure belongs to.  TableName should be escaped if it is required.
        /// </summary>
        public string TableName { get; set; }


        private string _name;
        /// <summary>
        /// Measure name, without brackets
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                System.Diagnostics.Debug.Assert(value != null && !value.StartsWith("[") && !value.EndsWith("]"));
                _name = value;
            }
        }

        /// <summary>
        /// Measure expression - text that goes after = in the measure definition.
        /// </summary>
        public string Expression { get; set; }

        private string _fullText;
        /// <summary>
        /// Full text of a measure, starting with CREATE MEASURE. Does not include ';'.
        /// </summary>
        public string FullText
        {
            get
            {
                return _fullText;
            }
            set
            {
                if (value != null)
                {
                    // Normalize EOF as '\r\n'
                    value = value.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", "\r\n");
                }
                _fullText = value;
            }
        }
        public string NameInBrackets
        {
            get
            {
                return "[" + Name + "]";
            }
        }

        /// <summary>
        /// Calculation property for the measure
        /// </summary>
        public DaxCalcProperty CalcProperty { get; set; }
    }
}
