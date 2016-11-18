using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace DaxEditor
{
    public class DaxDocumentPropertiesBase : IDaxDocumentProperties
    {
        private string _connectionString;
        private readonly string _catalogSearchRegex = @"Catalog\s*=([^;]*);*";

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = Application.UserAppDataRegistry.GetValue("ConnectionString") as string;
                }
                return _connectionString;
            }
            set
            {
                _connectionString = value;
                if (!string.IsNullOrEmpty(_connectionString))
                    Application.UserAppDataRegistry.SetValue("ConnectionString", _connectionString);
            }
        }

        public string ConnectionStringWithoutDatabaseName
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(DatabaseName));
                return Regex.Replace(ConnectionString, _catalogSearchRegex, string.Empty);
            }
        }

        public string DatabaseName
        {
            get
            {
                var match = Regex.Match(ConnectionString, _catalogSearchRegex);
                if (match.Success)
                {
                    Debug.Assert(match.Groups.Count > 1);
                    return match.Groups[1].Value;
                }
                else
                {
                    Debug.Assert(false, "No Catalog in the connection string");
                    return string.Empty;
                }
            }
        }
    }
}
