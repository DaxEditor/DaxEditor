// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Package;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using DaxEditor;

namespace Babel
{
    public class DaxDocumentProperties : DocumentProperties, IDaxDocumentProperties
    {
        DaxDocumentPropertiesBase _props = new DaxDocumentPropertiesBase();

        public DaxDocumentProperties(CodeWindowManager mgr)
            : base(mgr)
        {
        }

        [DisplayNameAttribute("Connection string")]
        [CategoryAttribute("General")]
        [DescriptionAttribute("Connection string to SSAS database")]
        [ReadOnly(true)]
        public string ConnectionString
        {
            get
            {
                return _props.ConnectionString;
            }
            set
            {
                _props.ConnectionString = value;
            }
        }

        [DisplayNameAttribute("ConnectionStringWithoutDatabaseaName")]
        [CategoryAttribute("General")]
        [DescriptionAttribute("Connection string to SSAS database without the database name")]
        [ReadOnly(true)]
        public string ConnectionStringWithoutDatabaseName
        {
            get { return _props.ConnectionStringWithoutDatabaseName; }
        }

        [DisplayNameAttribute("DatabaseName")]
        [CategoryAttribute("General")]
        [DescriptionAttribute("The database name of the connection string to SSAS database")]
        [ReadOnly(true)]
        public string DatabaseName
        {
            get { return _props.DatabaseName; }
        }
    }
}
