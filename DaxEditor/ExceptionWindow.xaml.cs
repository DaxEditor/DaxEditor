// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System.Windows;
using System;
using System.Text;
using System.Diagnostics;

namespace DaxEditor
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window
    {
        public ExceptionWindow(Exception e)
        {
            InitializeComponent();

            var sb = new StringBuilder();
            sb.AppendLine(DateTime.Now.ToString());
            sb.Append("DAX Editor version: ");
            var assemblyLocation = typeof(ExceptionWindow).Assembly.Location;
            var fvi = FileVersionInfo.GetVersionInfo(assemblyLocation);
            var version = fvi.FileVersion;
            sb.AppendLine(version);

            ComposeErrorMessage(e, sb);
            _exceptionLog.Text = sb.ToString();
        }

        private void ComposeErrorMessage(Exception e, StringBuilder sb)
        {
            sb.AppendLine("Exception message:");
            sb.AppendLine(e.Message);
            sb.AppendLine("Exception stack:");
            sb.AppendLine(e.StackTrace);

            if (e.InnerException != null)
            {
                sb.AppendLine("--- Inner exception");
                ComposeErrorMessage(e.InnerException, sb);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
