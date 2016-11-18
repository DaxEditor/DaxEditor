// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows;
using System.Windows.Documents;
using System.Text;
using System.Windows.Threading;
using System.Data;
using System.Windows.Forms.Integration;

namespace DaxEditor
{
    class DaxEditorMargin : StackPanel, IWpfTextViewMargin, IUpdateEditorMargin
    {
        public const string MarginName = "DaxEditorMargin";
        private IWpfTextView _textView;
        private bool _isDisposed = false;
        private Border _splitter;
        private TabControl _tabControl;
        private TabItem _resultTab;
        private TabItem _schemaTab;
        private TabItem _xmlaResultTab;
        private TabItem _logResultTab;
        private WebBrowser _schemaWebBrowser;
        private TextBox _xmlaResultTextBox;
        private TextBox _logTextBox;
        private System.Windows.Forms.DataGridView _dataGridView;
        private WindowsFormsHost _host;

        /// <summary>
        /// Creates a <see cref="DaxEditorMargin"/> for a given <see cref="IWpfTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public DaxEditorMargin(IWpfTextView textView)
        {
            _textView = textView;
            ClipToBounds = true;

            _splitter = new Border()
            {
                Height = 5,
                Background = new SolidColorBrush(Colors.Goldenrod),
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Cursor = Cursors.SizeNS,
                ForceCursor = true,
            };

            _splitter.MouseMove += OnSplitterMouseMove;
            _splitter.MouseDown += OnSplitterMouseDown;
            _splitter.MouseUp += OnSplitterMouseUp;
            _splitter.MouseLeave += OnSplitterMouseLeave;

            this.Children.Add(_splitter);

            _tabControl = new TabControl() { Height = 300 };
            _resultTab = new TabItem() { Header = "Result" };
            _schemaTab = new TabItem() { Header = "Schema" };
            _xmlaResultTab = new TabItem() { Header = "XMLA Result" };
            _logResultTab = new TabItem() { Header = "Log" };

            _tabControl.Items.Add(_resultTab);
            _tabControl.Items.Add(_schemaTab);
            _tabControl.Items.Add(_xmlaResultTab);
            _tabControl.Items.Add(_logResultTab);

            _dataGridView = new System.Windows.Forms.DataGridView()
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = true,
                AllowUserToResizeRows = true,
                AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells,
            };

            _host = new WindowsFormsHost();
            _host.Child = _dataGridView;

            _resultTab.Content = _host;

            _schemaWebBrowser = new WebBrowser();
            _schemaWebBrowser.Height = _host.Height;
            _schemaTab.Content = _schemaWebBrowser;

            _xmlaResultTextBox = new TextBox() { IsReadOnly = true, IsReadOnlyCaretVisible = true, };
            var resultScrollViewer = new ScrollViewer();
            resultScrollViewer.Height = _host.Height;
            resultScrollViewer.Content = _xmlaResultTextBox;
            _xmlaResultTextBox.Height = _host.Height;
            _xmlaResultTab.Content = _xmlaResultTextBox;

            _logTextBox = new TextBox() { IsReadOnly = true, IsReadOnlyCaretVisible = true, };
            var logScrollViewer = new ScrollViewer();
            logScrollViewer.Height = _host.Height;
            logScrollViewer.Content = _logTextBox;
            _logResultTab.Content = logScrollViewer;

            this.Children.Add(_tabControl);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }

        #region Splitter move support
        void OnSplitterMouseMove(object sender, MouseEventArgs e)
        {
            if (this._splitter.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                _tabControl.Height = Math.Max(ActualHeight - e.GetPosition(this).Y, 1);
            }
        }

        void OnSplitterMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == this._splitter)
            {
                Mouse.Capture(this._splitter);
            }
        }

        void OnSplitterMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this._splitter.IsMouseCaptured)
            {
                Mouse.Capture(null);
            }
        }

        void OnSplitterMouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }
        #endregion

        #region IWpfTextViewMargin Members

        /// <summary>
        /// The <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation
        /// of the margin.
        /// </summary>
        public System.Windows.FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        #endregion

        #region ITextViewMargin Members

        public double MarginSize
        {
            // Since this is a horizontal margin, its width will be bound to the width of the text view.
            // Therefore, its size is its height.
            get
            {
                ThrowIfDisposed();
                return this.ActualHeight;
            }
        }

        public bool Enabled
        {
            // The margin should always be enabled
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        /// <summary>
        /// Returns an instance of the margin if this is the margin that has been requested.
        /// </summary>
        /// <param name="marginName">The name of the margin requested</param>
        /// <returns>An instance of EditorMarginVstsTest or null</returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == DaxEditorMargin.MarginName) ? (IWpfTextViewMargin)this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_host != null)
                    _host.Dispose();

                if (_schemaWebBrowser != null)
                    _schemaWebBrowser.Dispose();

                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }
        #endregion

        public void UpdateResult(DataTable result)
        {
            if (_dataGridView.InvokeRequired)
            {
                _dataGridView.Invoke(
                    new Action(
                        () =>
                        {
                            _dataGridView.DataSource = result;
                        }
                    )
                );
            }
            else
            {
                _dataGridView.DataSource = result;
            }
        }

        public void UpdateSchema(string schemaHtml)
        {
            if (!_schemaWebBrowser.Dispatcher.CheckAccess())
            {
                _schemaWebBrowser.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    TimeSpan.FromSeconds(5),
                    new Action(
                        () =>
                        {
                            _schemaWebBrowser.NavigateToString(schemaHtml);
                        }
                    )
                );
            }
            else
            {
                _schemaWebBrowser.NavigateToString(schemaHtml);
            }
        }

        public void UpdateXmlaResult(string xmlaResult)
        {
            if (!_xmlaResultTextBox.Dispatcher.CheckAccess())
            {
                _xmlaResultTextBox.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    TimeSpan.FromSeconds(10),
                    new Action(
                        () =>
                        {
                            _xmlaResultTextBox.Text = xmlaResult;
                        }
                    )
                );
            }
            else
            {
                _xmlaResultTextBox.Text = xmlaResult;
            }
        }

        public void WriteLog(string logMessage, bool switchToLogWindow)
        {
            if (!_logTextBox.Dispatcher.CheckAccess())
            {
                _logTextBox.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    TimeSpan.FromSeconds(3),
                    new Action(
                        () =>
                        {
                            WriteLogImpl(logMessage, switchToLogWindow);
                        }
                    )
                );
            }
            else
            {
                WriteLogImpl(logMessage, switchToLogWindow);
            }
        }

        private void WriteLogImpl(string logMessage, bool switchToLogWindow)
        {
            int selectionStart = _logTextBox.SelectionStart;
            bool moveCursorToEnd = selectionStart == _logTextBox.Text.Length;
            _logTextBox.AppendText(logMessage);
            if (moveCursorToEnd)
            {
                _logTextBox.Select(_logTextBox.Text.Length, 0);
                _logTextBox.ScrollToEnd();
            }

            if (switchToLogWindow)
                _logResultTab.IsSelected = true;
        }
    }
}
