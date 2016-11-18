// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Linq;
using Babel.Parser;

namespace DaxEditor
{
    [Serializable]
    public class DaxParsingException : Exception
    {
        public DaxParsingException() { }

        public DaxParsingException(string message) : base(message) { }

        public DaxParsingException(string message, Exception inner) : base(message, inner) { }

        static public DaxParsingException FromHandler(ErrorHandler handler, string daxScript)
        {
            var sortedErrors = handler.SortedErrorList();
            var messages = "Error in next DAX script: " + Environment.NewLine + 
                daxScript + Environment.NewLine + 
                string.Join(Environment.NewLine, sortedErrors.Take(Parser.MaxErrors).Select(i => i.ToString()));
            if (sortedErrors.Count > Parser.MaxErrors)
            {
                messages = $"First {Parser.MaxErrors} errors:{Environment.NewLine}{messages}";
            }

            return new DaxParsingException(messages);
        }
    }

}
