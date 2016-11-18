// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaxEditor
{
    public class CompletionContext
    {
        /// <summary>
        /// Token to complete
        /// </summary>
        public int TokenToComplete { set; get; }

        /// <summary>
        /// The beggining of the token to complete
        /// </summary>
        public string TokenBeggining { set; get; }

        /// <summary>
        /// Name of the parent object (in case of Columns, Measures)
        /// </summary>
        public string ParentTokenName { set; get; }

        /// <summary>
        /// Specifies whether it completion should add the closing (or opening in case of functions) brace
        /// </summary>
        public bool ShouldAppendClosingOpeningBrace { set; get; }
    }
}
