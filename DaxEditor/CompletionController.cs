// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using DaxEditor;
using Microsoft.VisualStudio.Text.Operations;
using MSXML;
using Babel;

namespace DaxEditor
{
    #region Command Filter

    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("DAX")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class VsTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        IVsEditorAdaptersFactoryService _adapterService = null;

        [Import]
        internal ICompletionBroker _completionBroker = null;

        [Import]
        internal SVsServiceProvider _serviceProvider = null;

        [Import]
        internal ITextStructureNavigatorSelectorService _navigatorService = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ITextView textView = _adapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            Func<CommandFilter> createCommandHandler = delegate() { return new CommandFilter(textViewAdapter, textView, this); };
            textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
        }
    }

    [ProvideLanguageCodeExpansion(
        GuidList.guidDaxLanguageService,
        "DAX",                                      // the language name
        0,                                          //the resource id of the language
        "DAX",                                      //the language ID used in the .snippet files
        @"%MyDocs%\Code Snippets\DAX\SnippetsIndex.xml", //the path of the index file
        ShowRoots = false)]
    internal sealed class CommandFilter : IOleCommandTarget, IVsExpansionClient
    {
        private ICompletionSession _session;
        private IOleCommandTarget _nextCommandHandler;
        private ITextView _textView;
        private VsTextViewCreationListener _provider;
        private IVsTextView _vsTextView;
        private IVsExpansionManager _exManager;
        private IVsExpansionSession _exSession;

        public CommandFilter(IVsTextView textViewAdapter, ITextView textView, VsTextViewCreationListener provider)
        {
            _vsTextView = textViewAdapter;
            _textView = textView;
            _provider = provider;

            // Get the text manager from the service provider
            IVsTextManager2 textManager = (IVsTextManager2)_provider._serviceProvider.GetService(typeof(SVsTextManager));
            textManager.GetExpansionManager(out _exManager);
            _exSession = null;

            // Add the command to the command chain
            textViewAdapter.AddCommandFilter(this, out _nextCommandHandler);
        }

        public IWpfTextView TextView { get; private set; }
        public ICompletionBroker Broker { get; private set; }
        public IOleCommandTarget Next { get; set; }

        private char GetTypeChar(IntPtr pvaIn)
        {
            return (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (VsShellUtilities.IsInAutomationFunction(_provider._serviceProvider))
            {
                return _nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            // Make a copy of this so we can look at it after forwarding some commands 
            uint commandID = nCmdID;
            char typedChar = char.MinValue;
            // Make sure the input is a char before getting it 
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
            {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }

            //the snippet picker code starts here 
            if (nCmdID == (uint)VSConstants.VSStd2KCmdID.INSERTSNIPPET)
            {
                IVsTextManager2 textManager = (IVsTextManager2)_provider._serviceProvider.GetService(typeof(SVsTextManager));

                textManager.GetExpansionManager(out _exManager);

                _exManager.InvokeInsertionUI(
                    _vsTextView,
                    this,      //the expansion client 
                    new Guid(GuidList.guidDaxLanguageService),
                    null,       //use all snippet types
                    0,          //number of types (0 for all)
                    0,          //ignored if iCountTypes == 0 
                    null,       //use all snippet kinds
                    0,          //use all snippet kinds
                    0,          //ignored if iCountTypes == 0 
                    "DAX Snippets", //the text to show in the prompt 
                    string.Empty);  //only the ENTER key causes insert  

                return VSConstants.S_OK;
            }

            //the expansion insertion is handled in OnItemChosen 
            //if the expansion session is still active, handle tab/backtab/return/cancel 
            if (_exSession != null)
            {
                if (nCmdID == (uint)VSConstants.VSStd2KCmdID.BACKTAB)
                {
                    _exSession.GoToPreviousExpansionField();
                    return VSConstants.S_OK;
                }
                else if (nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB)
                {

                    _exSession.GoToNextExpansionField(0); //false to support cycling through all the fields 
                    return VSConstants.S_OK;
                }
                else if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN || nCmdID == (uint)VSConstants.VSStd2KCmdID.CANCEL)
                {
                    if (_exSession.EndCurrentExpansion(0) == VSConstants.S_OK)
                    {
                        _exSession = null;
                        return VSConstants.S_OK;
                    }
                }
            }

            //neither an expansion session nor a completion session is open, but we got a tab, so check whether the last word typed is a snippet shortcut  
            if (/*m_session == null && */ _exSession == null && nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB)
            {
                //get the word that was just added 
                CaretPosition pos = _textView.Caret.Position;
                TextExtent word = _provider._navigatorService.GetTextStructureNavigator(_textView.TextBuffer).GetExtentOfWord(pos.BufferPosition - 1); //use the position 1 space back 
                string textString = word.Span.GetText(); //the word that was just added 
                //if it is a code snippet, insert it, otherwise carry on 
                if (InsertAnyExpansion(textString, null, null))
                    return VSConstants.S_OK;
            }

            // Check for a commit character 
            if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
                || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB
                || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar)))
            {
                // Check for a a selection 
                if (_session != null && !_session.IsDismissed)
                {
                    // If the selection is fully selected, commit the current session 
                    if (_session.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        _session.Commit();
                        // Also, don't add the character to the buffer 
                        return VSConstants.S_OK;
                    }
                    else
                    {
                        // If there is no selection, dismiss the session
                        _session.Dismiss();
                    }
                }
            }

            // Pass along the command so the char is added to the buffer 
            int retVal = _nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            bool handled = false;
            if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar))
            {
                if (_session == null || _session.IsDismissed) // If there is no active session, bring up completion
                {
                    this.TriggerCompletion();
                    _session.Filter();
                }
                else     // The completion session is already active, so just filter
                {
                    _session.Filter();
                }
                handled = true;
            }
            else if (commandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE   // Redo the filter if there is a deletion
                || commandID == (uint)VSConstants.VSStd2KCmdID.DELETE)
            {
                if (_session != null && !_session.IsDismissed)
                    _session.Filter();
                handled = true;
            }
            

            if (handled) return VSConstants.S_OK;
            return retVal;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (!VsShellUtilities.IsInAutomationFunction(_provider._serviceProvider))
            {
                if (pguidCmdGroup == VSConstants.VSStd2K && cCmds > 0)
                {
                    // make the Insert Snippet command appear on the context menu  
                    if ((uint)prgCmds[0].cmdID == (uint)VSConstants.VSStd2KCmdID.INSERTSNIPPET)
                    {
                        prgCmds[0].cmdf = (int)Constants.MSOCMDF_ENABLED | (int)Constants.MSOCMDF_SUPPORTED;
                        return VSConstants.S_OK;
                    }
                }
            }

            return _nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private bool TriggerCompletion()
        {
            // The caret must be in a non-projection location 
            SnapshotPoint? caretPoint =
            _textView.Caret.Position.Point.GetPoint(
            textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
            if (!caretPoint.HasValue)
            {
                return false;
            }

            _session = _provider._completionBroker.CreateCompletionSession(
                _textView,
                caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),
                true);

            // Subscribe to the Dismissed event on the session 
            _session.Dismissed += this.OnSessionDismissed;
            _session.Start();

            return true;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            _session.Dismissed -= this.OnSessionDismissed;
            _session = null;
        }


        public int EndExpansion()
        {
            _exSession = null;
            return VSConstants.S_OK;
        }

        public int FormatSpan(IVsTextLines pBuffer, TextSpan[] ts)
        {
            return VSConstants.S_OK;
        }

        public int GetExpansionFunction(IXMLDOMNode xmlFunctionNode, string bstrFieldName, out IVsExpansionFunction pFunc)
        {
            pFunc = null;
            return VSConstants.S_OK;
        }

        public int IsValidKind(IVsTextLines pBuffer, TextSpan[] ts, string bstrKind, out int pfIsValidKind)
        {
            pfIsValidKind = 1;
            return VSConstants.S_OK;
        }

        public int IsValidType(IVsTextLines pBuffer, TextSpan[] ts, string[] rgTypes, int iCountTypes, out int pfIsValidType)
        {
            pfIsValidType = 1;
            return VSConstants.S_OK;
        }

        public int OnAfterInsertion(IVsExpansionSession pSession)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeInsertion(IVsExpansionSession pSession)
        {
            return VSConstants.S_OK;
        }

        public int PositionCaretForEditing(IVsTextLines pBuffer, TextSpan[] ts)
        {
            return VSConstants.S_OK;
        }

        public int OnItemChosen(string pszTitle, string pszPath)
        {
            InsertAnyExpansion(null, pszTitle, pszPath);
            return VSConstants.S_OK;
        }

        private bool InsertAnyExpansion(string shortcut, string title, string path)
        {
            //first get the location of the caret, and set up a TextSpan 
            int endColumn, startLine;
            //get the column number from  the IVsTextView, not the ITextView
            _vsTextView.GetCaretPos(out startLine, out endColumn);

            TextSpan addSpan = new TextSpan();
            addSpan.iStartIndex = endColumn;
            addSpan.iEndIndex = endColumn;
            addSpan.iStartLine = startLine;
            addSpan.iEndLine = startLine;

            if (shortcut != null) //get the expansion from the shortcut
            {
                //reset the TextSpan to the width of the shortcut,  
                //because we're going to replace the shortcut with the expansion
                addSpan.iStartIndex = addSpan.iEndIndex - shortcut.Length;

                _exManager.GetExpansionByShortcut(
                    this,
                    new Guid(GuidList.guidDaxLanguageService),
                    shortcut,
                    _vsTextView,
                    new TextSpan[] { addSpan },
                    0,
                    out path,
                    out title);

            }
            if (title != null && path != null)
            {
                IVsTextLines textLines;
                _vsTextView.GetBuffer(out textLines);
                IVsExpansion bufferExpansion = (IVsExpansion)textLines;

                if (bufferExpansion != null)
                {
                    int hr = bufferExpansion.InsertNamedExpansion(
                        title,
                        path,
                        addSpan,
                        this,
                        new Guid(GuidList.guidDaxLanguageService),
                        0,
                       out _exSession);
                    if (VSConstants.S_OK == hr)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
    #endregion
}
