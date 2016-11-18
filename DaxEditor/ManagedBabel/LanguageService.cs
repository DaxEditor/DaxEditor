/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Package;

namespace Babel
{
	public abstract class BabelLanguageService : Microsoft.VisualStudio.Package.LanguageService
	{
		#region Custom Colors
		public override int GetColorableItem(int index, out IVsColorableItem item)
		{
			if (index <= Configuration.ColorableItems.Count)
			{
				item = Configuration.ColorableItems[index - 1];
				return Microsoft.VisualStudio.VSConstants.S_OK;
			}
			else
			{
				throw new ArgumentNullException("index");
			}
		}

		public override int GetItemCount(out int count)
		{
			count = Configuration.ColorableItems.Count;
			return Microsoft.VisualStudio.VSConstants.S_OK;
		}
		#endregion

		#region MPF Accessor and Factory specialisation
		private LanguagePreferences preferences;
		public override LanguagePreferences GetLanguagePreferences()
		{
			if (this.preferences == null)
			{
				this.preferences = new LanguagePreferences(this.Site,
														typeof(Babel.LanguageService).GUID,
														this.Name);
				this.preferences.Init();
			}

			return this.preferences;
		}

		public override Microsoft.VisualStudio.Package.Source CreateSource(IVsTextLines buffer)
		{
			return new Source(this, buffer, this.GetColorizer(buffer));
		}

		private IScanner scanner;
		public override IScanner GetScanner(IVsTextLines buffer)
		{
			if (scanner == null)
				this.scanner = new LineScanner();

			return this.scanner;
		}
		#endregion

		public override void OnIdle(bool periodic)
		{
            Source source = (Source) GetSource(this.LastActiveTextView);
            if (source != null && source.LastParseTime >= 0x7ffff)
                source.LastParseTime = 0;

            base.OnIdle(periodic);
		}


        public override Microsoft.VisualStudio.Package.AuthoringScope ParseSource(ParseRequest req)
        {
            Source source = (Source)this.GetSource(req.FileName);
            bool yyparseResult = false;

            // req.DirtySpan seems to be set even though no changes have occurred
            // source.IsDirty also behaves strangely
            // might be possible to use source.ChangeCount to sync instead

            if (req.Reason == ParseReason.Check
                || req.DirtySpan.iStartIndex != req.DirtySpan.iEndIndex
                || req.DirtySpan.iStartLine != req.DirtySpan.iEndLine)
            {
                Babel.Parser.ErrorHandler handler = new Babel.Parser.ErrorHandler();
                Babel.Lexer.Scanner scanner = new Babel.Lexer.Scanner(); // string interface
                Parser.Parser parser = new Parser.Parser();  // use noarg constructor

#if DEBUG
                parser.Trace = true;
#endif

                parser.scanner = scanner;
                scanner.Handler = handler;
                parser.SetHandler(handler);
                scanner.SetSourceText(req.Text);

                parser.MBWInit(req);
                yyparseResult = parser.Parse();

                // store the parse results
                // source.ParseResult = aast;
                source.ParseResult = null;
                source.Braces = parser.Braces;

                // for the time being, just pull errors back from the error handler
                if (handler.ErrNum > 0)
                {
                    foreach (Babel.Parser.Error error in handler.SortedErrorList())
                    {
                        TextSpan span = new TextSpan();
                        span.iStartLine = span.iEndLine = error.line - 1;
                        span.iStartIndex = error.column;
                        span.iEndIndex = error.column + error.length;

                        req.Sink.AddError(req.FileName, error.message, span, Severity.Error);
                    }
                }
            }

            return new AuthoringScope(source.ParseResult, source);
        }

		public override string Name
		{
			get { return Configuration.Name; }
		}
	}
}