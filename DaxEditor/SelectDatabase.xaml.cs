// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using Microsoft.AnalysisServices.AdomdClient;

namespace DaxEditor
{
    /// <summary>
    /// Interaction logic for SelectDatabase.xaml
    /// </summary>
    public partial class SelectDatabase : Window, IDisposable
    {
        public SelectDatabase(string serverName, string selectedDatabaseName)
        {
            InitializeComponent();
            this.Title = "Databases on server " + serverName;
            _serverName = serverName;
            SelectedDatabaseName = selectedDatabaseName;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
            _worker.RunWorkerAsync();
        }

        public string SelectedDatabaseName { get; set; }
        private BackgroundWorker _worker = new BackgroundWorker();
        private string _serverName;
        private List<string> _databases = new List<string>();

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string connectionString = string.Format("Provider=MSOLAP;Application Name=DAX Editor;Data Source={0}", _serverName);
            try
            {
                using (var _adomdConn = new AdomdConnection(connectionString))
                {
                    _adomdConn.Open();
                    var dataSet = _adomdConn.GetSchemaDataSet("DBSCHEMA_CATALOGS", null);
                    Debug.Assert(dataSet != null);
                    Debug.Assert(dataSet.Tables.Count == 1);
                    Debug.Assert(dataSet.Tables[0] != null);
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        _databases.Add(row[0] as string);
                    }
                }
            }
            catch (Exception)
            {
                _databases.Add("Error while reading list of databases");
            }
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _discoveringLabel.Visibility = System.Windows.Visibility.Hidden;
            _databasesListBox.ItemsSource = _databases;
            foreach (string db in _databasesListBox.ItemsSource)
            {
                if(string.Equals(db, SelectedDatabaseName, StringComparison.CurrentCultureIgnoreCase))
                {
                    _databasesListBox.SelectedItem = db;
                    break;
                }
            }
            _databasesListBox.Visibility = System.Windows.Visibility.Visible;
            _databasesListBox.Focus();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SelectedDatabaseName = _databasesListBox.SelectedItem as string;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _worker.Dispose();
                _worker = null;
            }
        }
        #endregion
    }
}
