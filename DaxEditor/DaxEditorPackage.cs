// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Babel;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

using EnvDTE;
using System.Windows.Media;
using System.Windows;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Package;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.CommandBars;

namespace DaxEditor
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.7.41510.1", IconResourceID = 400)]
    [Guid(GuidList.guidDaxEditorPkgString)]
    [ProvideService(typeof(Babel.LanguageService))]
    [ProvideLanguageExtension(typeof(Babel.LanguageService), Babel.Configuration.Extension)]
    [ProvideLanguageEditorOptionPage(typeof(DaxFormattingPage), Babel.Configuration.Name, "Formatting", "General", "#2001")]
    [ProvideMenuResource("Menus.ctmenu", 49)]
    [ProvideLanguageService(typeof(Babel.LanguageService), Babel.Configuration.Name, 0,
        MatchBraces = true,
        ShowMatchingBrace = true,
        MatchBracesAtCaret = true,
        EnableFormatSelection = true)
    ]
    //let DAX package load when any solution is opened, so 'Create DAX file' menu item appears for BIM file
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    public sealed class DaxEditorPackage : Package, IOleComponent, IDisposable
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public DaxEditorPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        public static DaxEditorPackage Instance { get; private set; }
        private Babel.LanguageService _languageService;
        private DTE _envDte;
        uint _componentID = 0;

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            _languageService = new Babel.LanguageService();
            _languageService.SetSite(this);

            IServiceContainer serviceContainter = (IServiceContainer)this;
            serviceContainter.AddService(typeof(Babel.LanguageService), _languageService, true);

            // Remeber the DTE for future use;
            _envDte = GetService(typeof(DTE)) as DTE;

            // Remember the dialog page object
            _languageService.FormattingPage = GetDialogPage(typeof(DaxFormattingPage)) as DaxFormattingPage;

            // Register for idle timer callbacks
            IOleComponentManager mgr = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
            if (_componentID == 0 && mgr != null)
            {
                OLECRINFO[] crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)(_OLECRF.olecrfNeedIdleTime | _OLECRF.olecrfNeedPeriodicIdleTime);
                crinfo[0].grfcadvf = (uint)(_OLECADVF.olecadvfModal | _OLECADVF.olecadvfRedrawOff | _OLECADVF.olecadvfWarningsOff);
                crinfo[0].uIdleTimeInterval = 1000;
                mgr.FRegisterComponent(this, crinfo, out _componentID);
            }

            AddMenuButtons();

#if DEBUG
            // Redirect console to Debug so log from Parser is visible
            Console.SetError(new ConsoleToDebugRedirector());
#endif
            SyncSnippets();

            Instance = this;
            //commandBars.Add("My Command Bar", new Point(400,400), new CommandBarPopup(),)
        }

        public static void WriteToGeneral(string text)
        {
            const int VISIBLE = 1;
            const int DO_NOT_CLEAR_WITH_SOLUTION = 0;

            // Get the output window
            var outputWindow = Instance.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            // The General pane is not created by default. We must force its creation
            var guidPane = VSConstants.OutputWindowPaneGuid.GeneralPane_guid;
            var hr = outputWindow.CreatePane(guidPane, "General", VISIBLE, DO_NOT_CLEAR_WITH_SOLUTION);
            ErrorHandler.ThrowOnFailure(hr);

            // Get the pane
            IVsOutputWindowPane outputWindowPane = null;
            hr = outputWindow.GetPane(guidPane, out outputWindowPane);
            ErrorHandler.ThrowOnFailure(hr);

            // Output the text
            outputWindowPane?.Activate();
            outputWindowPane?.OutputString(text);
        }

        private void SyncSnippets()
        {
            var registryRoot = _envDte.RegistryRoot;
            using(var key = Registry.CurrentUser.OpenSubKey(registryRoot))
            {
                var vsLocation = key.GetValue("VisualStudioLocation") as string;
                if (string.IsNullOrEmpty(vsLocation))
                    return;

                var daxCodeSnippetsLocation = Path.Combine(vsLocation, @"Code Snippets\DAX");
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var packageLocation = Path.GetDirectoryName(assemblyLocation);
                var fvi = FileVersionInfo.GetVersionInfo(assemblyLocation);
                var version = fvi.FileVersion;
                var versionFilePath = Path.Combine(daxCodeSnippetsLocation, "version.txt");

                bool syncRequired = true;
                if (Directory.Exists(daxCodeSnippetsLocation) && File.Exists(versionFilePath))
                {
                    var existingVersion = File.ReadAllText(versionFilePath);
                    if (existingVersion == version)
                        syncRequired = false;
                }

                if (syncRequired)
                {
                    if (Directory.Exists(daxCodeSnippetsLocation))
                        Directory.Delete(daxCodeSnippetsLocation, true);
                    Directory.CreateDirectory(daxCodeSnippetsLocation);
                    var packageSnippetsDirectory = Path.Combine(packageLocation, "Snippets");
                    foreach(var fi in Directory.EnumerateFiles(packageSnippetsDirectory, "*.*", SearchOption.AllDirectories))
                    {
                        var outputFile = Path.Combine(daxCodeSnippetsLocation, fi.Substring(packageSnippetsDirectory.Length + 1));
                        var outputDir = Path.GetDirectoryName(outputFile);
                        if(!Directory.Exists(outputDir))
                            Directory.CreateDirectory(outputDir);
                        File.Copy(fi, outputFile);
                    }
                    File.WriteAllText(versionFilePath, version);
                }
            }
        }

        public const int topLevelMenuCommandId = 0x0601;
        public const int newFileCommandId = 0x00e0;
        public const int connectCommandId = 0x00f0;
        public const int disconnectCommandId = 0x00f1;
        public const int getMeasuresAndCalculateCommandId = 0x1601;
        public const int saveMeasuresAndCalculateCommandId = 0x1602;
        public const int executeQueryCommandId = 0x1621;
        public const int reformatSelectionCommandId = 0x00f7;
        public const int reformatDocumentCommandId = 0x00f8;
        public const int increaseDepthCommandId = 0x0100;
        public const int decreaseDepthCommandId = 0x0110;
        public const int getMeasuresFromFileCommandId = 0x1801;
        public const int saveMeasuresToFileCommandId = 0x1802;

        private void AddMenuButtons()
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            Debug.Assert(mcs != null, "Can not get the OleMenuCommandService.");

            if (null != mcs)
            {
                CommandID commandID;
                OleMenuCommand command;

                // Top level menu
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, topLevelMenuCommandId);
                command = new OleMenuCommand(null, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusVisible);
                mcs.AddCommand(command);

                // New File
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, newFileCommandId);
                command = new OleMenuCommand(NewFileCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Connect
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, connectCommandId);
                command = new OleMenuCommand(ConnectCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Disconnect
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, disconnectCommandId);
                command = new OleMenuCommand(DisconnectCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Get measures and calculated columns
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, getMeasuresAndCalculateCommandId);
                command = new OleMenuCommand(GetMeasuresAndCalculatedColumnsCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Save measures and calculated columns
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, saveMeasuresAndCalculateCommandId);
                command = new OleMenuCommand(SaveMeasuresAndCalculatedColumnsCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // ExecuteQuery
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, executeQueryCommandId);
                command = new OleMenuCommand(ExecuteQueryCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Reformat selection
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, reformatSelectionCommandId);
                command = new OleMenuCommand(ReformatSelectionCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Reformat document
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, reformatDocumentCommandId);
                command = new OleMenuCommand(ReformatDocumentCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Increase depth button
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, increaseDepthCommandId);
                command = new OleMenuCommand(EditAdvancedIncreaseIndentDepthCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Decrease depth button
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, decreaseDepthCommandId);
                command = new OleMenuCommand(EditAdvancedDecreaseIndentDepthCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnDaxCommandQueryStatusEnabled);
                mcs.AddCommand(command);

                // Get measures from BIM file button
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, getMeasuresFromFileCommandId);
                command = new OleMenuCommand(EditAdvancedGetBimFileContentCommandExecute, commandID);
                mcs.AddCommand(command);

                // Save measures to BIM file button
                commandID = new CommandID(GuidList.guidDaxEditorCmdSet, saveMeasuresToFileCommandId);
                command = new OleMenuCommand(EditAdvancedSaveBimFileContentCommandExecute, commandID);
                command.BeforeQueryStatus += new EventHandler(OnSaveDaxToBimQueryStatus);
                mcs.AddCommand(command);
            }
        }

        private void NewFileCommandExecute(object sender, EventArgs e)
        {
            _envDte.ExecuteCommand(@"File.NewFile ""DAXFile.dax""");
        }

        private void ConnectCommandExecute(object sender, EventArgs e)
        {
            try
            {
                var documentProperties = GetDocumentProperties();
                var currentActiveWindow = Application.Current.Windows.Cast<System.Windows.Window>().SingleOrDefault(x => x.IsActive);
                var wnd = new ConnectWindow(documentProperties.ConnectionString);
                wnd.Owner = currentActiveWindow;
                wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                wnd.ShowDialog();
                if (!string.IsNullOrEmpty(wnd.ConnectionString))
                {
                    documentProperties.ConnectionString = wnd.ConnectionString;
                    var activeTextView = GetActiveTextView();
                    var bismInfoProvider = GetBismInfoProvider();
                    bismInfoProvider.SetUpdateEditorMargin(GetEditorMargin(activeTextView));
                    bismInfoProvider.Connect();
                }
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void DisconnectCommandExecute(object sender, EventArgs e)
        {
            try
            {
                var documentProperties = GetDocumentProperties();
                documentProperties.ConnectionString = string.Empty;
                GetBismInfoProvider().Disconnect();
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void ExecuteQueryCommandExecute(object sender, EventArgs e)
        {
            try
            {
                var activeTextView = GetActiveTextView();

                string queryToExecute;
                ErrorHandler.ThrowOnFailure(activeTextView.GetSelectedText(out queryToExecute));
                if (string.IsNullOrEmpty(queryToExecute))
                {
                    IVsTextLines textLines = null;
                    ErrorHandler.ThrowOnFailure(activeTextView.GetBuffer(out textLines));
                    int iLineCount;
                    int iIndex;
                    ErrorHandler.ThrowOnFailure(textLines.GetLastLineIndex(out iLineCount, out iIndex));
                    ErrorHandler.ThrowOnFailure(textLines.GetLineText(0, 0, iLineCount, iIndex, out queryToExecute));
                }

                var bismInfoProvider = GetBismInfoProvider();
                bismInfoProvider.SetUpdateEditorMargin(GetEditorMargin(activeTextView));
                bismInfoProvider.ExecuteQuery(queryToExecute);
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void GetMeasuresAndCalculatedColumnsCommandExecute(object sender, EventArgs e)
        {
            try
            {
                var bismInfoProvider = GetBismInfoProvider();
                string measuresText = bismInfoProvider.GetMeasuresText();
                var activeTextView = GetActiveTextView();

                EditArray editArray = new EditArray(GetSource(), activeTextView, false, "Load measures and calculated columns");
                if (editArray != null)
                {
                    TextSpan span = new TextSpan();
                    span.iStartLine = 0;
                    span.iStartIndex = 0;
                    span.iEndLine = 9999;
                    span.iEndIndex = 0;

                    editArray.Add(new EditSpan(span, measuresText));
                    editArray.ApplyEdits();
                }
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void SaveMeasuresAndCalculatedColumnsCommandExecute(object sender, EventArgs e)
        {
            try
            {
                var activeTextView = GetActiveTextView();
                string viewText;
                IVsTextLines textLines = null;
                ErrorHandler.ThrowOnFailure(activeTextView.GetBuffer(out textLines));
                int iLineCount;
                int iIndex;
                ErrorHandler.ThrowOnFailure(textLines.GetLastLineIndex(out iLineCount, out iIndex));
                ErrorHandler.ThrowOnFailure(textLines.GetLineText(0, 0, iLineCount, iIndex, out viewText));

                var bismInfoProvider = GetBismInfoProvider();
                bismInfoProvider.SaveMeasuresAndCalcColumns(viewText);
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void ReformatSelectionCommandExecute(object sender, EventArgs e)
        {
            try
            {
                _envDte.ExecuteCommand("Edit.FormatSelection");
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void ReformatDocumentCommandExecute(object sender, EventArgs e)
        {
            try
            {
                _envDte.ExecuteCommand("Edit.FormatDocument");
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void EditAdvancedIncreaseIndentDepthCommandExecute(object sender, EventArgs e)
        {
            try
            {
                _languageService.FormattingPage.IndentDepthInFunctions = Math.Min(15, _languageService.FormattingPage.IndentDepthInFunctions + 1);
                _envDte.ExecuteCommand("Edit.FormatDocument");
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void EditAdvancedDecreaseIndentDepthCommandExecute(object sender, EventArgs e)
        {
            try
            {
                _languageService.FormattingPage.IndentDepthInFunctions = Math.Max(1, _languageService.FormattingPage.IndentDepthInFunctions - 1);
                _envDte.ExecuteCommand("Edit.FormatDocument");
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void EditAdvancedGetBimFileContentCommandExecute(object sender, EventArgs e)
        {
            try
            {
                Document activeDocument = _envDte.ActiveDocument;
                if (activeDocument != null)
                {
                    string bimFileName = activeDocument.FullName;
                    if (bimFileName == null || !bimFileName.EndsWith(".bim", StringComparison.InvariantCultureIgnoreCase))
                    {
                        System.Windows.Forms.MessageBox.Show("Command to be called on active .BIM file", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }

                    string daxFilePath = BimFileIntegration.ConvertBimPathToDaxPath(bimFileName);
                    if (File.Exists(daxFilePath) && (new FileInfo(daxFilePath)).IsReadOnly)
                    {
                        System.Windows.Forms.MessageBox.Show(string.Format("{0} file is read only", daxFilePath), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }

                    string fileContents = BimFileIntegration.GetMeasuresFromBimFile(bimFileName);
                    if (fileContents != null)
                    {
                        System.IO.File.WriteAllText(daxFilePath, fileContents);
                        _envDte.ExecuteCommand(string.Format(@"File.OpenFile ""{0}""", daxFilePath));
                    }
                }
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void EditAdvancedSaveBimFileContentCommandExecute(object sender, EventArgs e)
        {
            try
            {
                _envDte.ExecuteCommand("File.SaveSelectedItems");
                Document activeDocument = _envDte.ActiveDocument;
                BimFileIntegration.SaveMeasuresToBimFile(activeDocument.FullName);
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        private void CreateDaxFileCommandExecute(object sender, EventArgs e)
        {
            try
            {
                Document activeDocument = _envDte.ActiveDocument;
                string daxFilePath = BimFileIntegration.CreateDaxFile(activeDocument.FullName);
                ProjectItem daxItem = _envDte.ActiveDocument.ProjectItem.ContainingProject.ProjectItems.AddFromFile(daxFilePath);
                var daxWindow = daxItem.Open();
                daxItem.Document.Activate();
            }
            catch (Exception exc)
            {
                DisplayExceptionWindow(exc);
            }
        }

        public static void DisplayExceptionWindow(Exception e)
        {
            var exceptionWindow = new ExceptionWindow(e);
            var currentActiveWindow = Application.Current.Windows.Cast<System.Windows.Window>().SingleOrDefault(x => x.IsActive);
            exceptionWindow.Owner = currentActiveWindow;
            exceptionWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            exceptionWindow.ShowDialog();
        }

        private Microsoft.VisualStudio.Package.Source GetSource()
        {
            var codeWindowManager = GetCodeWindowManager();
            return codeWindowManager.Source;
        }

        private BismInfoProvider GetBismInfoProvider()
        {
            Babel.Source babelSource = GetSource() as Babel.Source;
            return babelSource.BismInfoProvider;
        }

        private DaxDocumentProperties GetDocumentProperties()
        {
            var codeWindowManager = GetCodeWindowManager();

            return ((DaxDocumentProperties)codeWindowManager.Properties);
        }

        private CodeWindowManager GetCodeWindowManager()
        {
            IVsTextView vTextView = GetActiveTextView();

            var codeWindowManager = this._languageService.GetCodeWindowManagerForView(vTextView);
            return codeWindowManager;
        }

        private IVsTextView GetActiveTextView()
        {
            var textMgr = GetService(typeof(SVsTextManager)) as IVsTextManager;
            IVsTextView vTextView = null;
            textMgr.GetActiveView(fMustHaveFocus: 1, pBuffer: null, ppView: out vTextView);
            return vTextView;
        }

        private DaxEditorMargin GetEditorMargin(IVsTextView vsTextView)
        {
            if (vsTextView == null)
                return null;

            IVsUserData propertyBag = vsTextView as IVsUserData;
            object textViewHost;
            var _guidIWpfTextViewHost = new Guid("8C40265E-9FDB-4f54-A0FD-EBB72B7D0476");
            ErrorHandler.ThrowOnFailure(propertyBag.GetData(ref _guidIWpfTextViewHost, out textViewHost));
            return (textViewHost as IWpfTextViewHost).GetTextViewMargin(DaxEditorMargin.MarginName) as DaxEditorMargin;
        }

        private void OnDaxCommandQueryStatusVisible(object sender, EventArgs e)
        {
            try
            {
                OleMenuCommand myCommand = sender as OleMenuCommand;
                if (myCommand != null)
                {
                    bool isVisible = false;
                    if (_envDte != null)
                    {
                        var wnd = _envDte.ActiveWindow;
                        if (wnd != null && wnd.Caption != null && wnd.Caption.ToLower().EndsWith(".dax"))
                            isVisible = true;
                    }
                    myCommand.Visible = isVisible;
                }
            }
            catch (Exception exc)
            {
                Debug.Fail(exc.Message, exc.StackTrace);
            }
        }

        private void OnDaxCommandQueryStatusEnabled(object sender, EventArgs e)
        {
            try
            {
                var command = sender as OleMenuCommand;
                if (command == null)
                {
                    return;
                }

                var id = command.CommandID.ID;
                var isOnline = _languageService.FormattingPage.IsOnline;
                var isBaseCommand =
                    id == newFileCommandId ||
                    id == reformatSelectionCommandId ||
                    id == reformatDocumentCommandId ||
                    id == increaseDepthCommandId ||
                    id == decreaseDepthCommandId ||
                    id == getMeasuresFromFileCommandId ||
                    id == saveMeasuresToFileCommandId;
                var isDepthCommand =
                    id == increaseDepthCommandId ||
                    id == decreaseDepthCommandId;
                var isDaxEditorFormatting = 
                    _languageService.FormattingPage.FormatterType == DaxFormattingPage.FormatDaxEditor;
                var isDaxFile = _envDte?.ActiveWindow?.Caption.ToLower().EndsWith(".dax") ?? false;
                command.Visible = (isBaseCommand || isOnline) && (isDepthCommand ? isDaxEditorFormatting : true);
                command.Enabled = isDaxFile;
            }
            catch (Exception exc)
            {
                Debug.Fail(exc.Message, exc.StackTrace);
            }
        }

        private void OnSaveDaxToBimQueryStatus(object sender, EventArgs e)
        {
            try
            {
                OleMenuCommand myCommand = sender as OleMenuCommand;
                if (myCommand != null)
                {
                    bool isEnabled = false;
                    if (_envDte != null)
                    {
                        var wnd = _envDte.ActiveWindow;
                        if (wnd != null && wnd.Caption != null && wnd.Caption.ToLower().EndsWith(".dax"))
                        {
                            Document activeDocument = _envDte.ActiveDocument;
                            if (activeDocument != null && activeDocument.FullName != null)
                            {
                                isEnabled = BimFileIntegration.IsAvailable(activeDocument.FullName);
                            }
                        }
                    }
                    myCommand.Enabled = isEnabled;
                }
            }
            catch (Exception exc)
            {
                Debug.Fail(exc.Message, exc.StackTrace);
            }
        }

        private void OnCreateDaxFileQueryStatus(object sender, EventArgs e)
        {
            try
            {
                OleMenuCommand myCommand = sender as OleMenuCommand;
                if (myCommand != null)
                {
                    bool isVisible = false;
                    if (_envDte != null)
                    {
                        var wnd = _envDte.ActiveWindow;
                        isVisible = (wnd != null && wnd.Caption != null && wnd.Caption.ToLower().EndsWith(".bim"));
                    }
                    myCommand.Visible = isVisible;
                }
            }
            catch (Exception exc)
            {
                Debug.Fail(exc.Message, exc.StackTrace);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _languageService.Dispose();
                _languageService = null;
            }

            try
            {
                if (_componentID != 0)
                {
                    IOleComponentManager mgr = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
                    if (mgr != null)
                        mgr.FRevokeComponent(_componentID);

                    _componentID = 0;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion


        #region IOleComponent Members

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FDoIdle(uint grfidlef)
        {
            if (_languageService != null)
                _languageService.OnIdle((grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0);

            return 0;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void Terminate()
        {
        }

        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
