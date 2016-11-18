// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace DaxEditor
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false)]
    public class DaxFormattingPage : DialogPage
    {
        public const string FormatDaxEditor = "DAX Editor";
        public const string FormatDaxFormatterUS = "www.daxformatter.com US";
        public const string FormatDaxFormatterEU = "www.daxformatter.com EU";

        public class FormatterConverter : StringConverter
        {
            private static List<string> _formattingOptions = new List<string>() { FormatDaxEditor, FormatDaxFormatterUS /* , FormatDaxFormatterEU - EU is not yet supported*/ };

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(_formattingOptions);
            }
        }

        private int _indentDepthInFunctions = 3;
        private string _formatterType = FormatDaxFormatterUS;
        private bool _isOnline = false;

        [Category("Indent depth")]
        [DisplayName("Indent depth")]
        [Description("Defines how many nested DAX functions to be formatted with indent")]
        public int IndentDepthInFunctions
        {
            get { return _indentDepthInFunctions; }
            set { _indentDepthInFunctions = value; }
        }

        [Category("Formatter")]
        [DisplayName("Formatter")]
        [Description("What formatter to use")]
        [Browsable(true)]
        [TypeConverter(typeof(FormatterConverter))]
        public string FormatterType
        {
            get { return _formatterType; }
            set { _formatterType = value; }
        }

        [Category("Is Online")]
        [DisplayName("Enable DAX changes to online database")]
        [Description("Enable DAX changes to online database")]
        [Browsable(true)]
        public bool IsOnline {
            get { return _isOnline; }
            set { _isOnline = value; }
        }
    }
}
