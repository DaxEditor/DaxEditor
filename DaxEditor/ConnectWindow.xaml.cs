// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics;

namespace DaxEditor
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        public ConnectWindow(string connectionString)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(connectionString))
            {
                connectionString = ExtractConnectionStringParameter(connectionString, "Data Source", _serverTextBox);
                connectionString = ExtractConnectionStringParameter(connectionString, "Catalog", _databaseTextBox);
                if (!string.IsNullOrWhiteSpace(connectionString))
                    this._extraConnectionStringTextBox.Text = connectionString;
            }

            _serverTextBox.Focus();
        }

        public string ConnectionString { get; set; }

        private string ExtractConnectionStringParameter(string connectionString, string searchedParameter, TextBox textBox)
        {
            var match = Regex.Match(connectionString, string.Format("{0}=([^;]*);*", searchedParameter));
            if (match.Success)
            {
                Debug.Assert(match.Groups.Count > 1);
                textBox.Text = match.Groups[1].Value;
                connectionString = connectionString.Remove(match.Groups[0].Index, match.Groups[0].Length);
            }
            return connectionString;
        }

        private void InputTextChanged(object sender, TextChangedEventArgs e)
        {
            var result = new StringBuilder();
            if(!string.IsNullOrEmpty(_serverTextBox.Text))
            {
                result.Append("Data Source=");
                result.Append(_serverTextBox.Text.Trim());
                result.Append(';');
            }
            if (!string.IsNullOrEmpty(_databaseTextBox.Text))
            {
                result.Append("Catalog=");
                result.Append(_databaseTextBox.Text.Trim());
                result.Append(';');
            }
            if (!string.IsNullOrEmpty(_extraConnectionStringTextBox.Text))
            {
                result.Append(_extraConnectionStringTextBox.Text.Trim());
            }

            _fullConnectionStringTextBox.Text = result.ToString();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            ConnectionString = _fullConnectionStringTextBox.Text;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _buttonDatabases_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new SelectDatabase(_serverTextBox.Text, _databaseTextBox.Text);
            wnd.Owner = this;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wnd.ShowDialog();
            if (wnd.SelectedDatabaseName != null)
            {
                _databaseTextBox.Text = wnd.SelectedDatabaseName;
            }
        }
    }
}
