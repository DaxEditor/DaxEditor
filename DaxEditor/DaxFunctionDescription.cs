// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaxEditor
{
    public enum LibraryName
    {
        Unknown,
        Scalar
    };

    public enum InterfaceName
    {
        Unknown,
        DateTime,
        Info,
        Logical,
        MathTrig,
        Text,
        Filter,
        ParentChild,
        Statistical,
    };

    public class DaxFunctionDescription
    {
        public DaxFunctionDescription()
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public InterfaceName InterfaceName { get; set; }
        public LibraryName LibraryName { get; set; }

        public IList<Babel.Parameter> Parameters { get; set; }
    }
}
