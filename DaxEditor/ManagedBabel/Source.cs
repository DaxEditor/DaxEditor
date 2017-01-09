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
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Diagnostics;
using DaxEditor;
using EnvDTE;

namespace Babel
{
	public class Source : Microsoft.VisualStudio.Package.Source
	{

        public Source(BabelLanguageService service, IVsTextLines textLines, Colorizer colorizer)
			: base(service, textLines, colorizer)
        {
            bismInforProvider = null;
		}

		private object parseResult;
		public object ParseResult
		{
			get { return parseResult; }
			set { parseResult = value; }
		}

		private IList<TextSpan[]> braces;
		public IList<TextSpan[]> Braces
		{
			get { return braces; }
			set { braces = value; }
		}

        private BismInfoProvider bismInforProvider;
        public BismInfoProvider BismInfoProvider
        {
            get { return bismInforProvider; }
            set { bismInforProvider = value; }
        }

        public override CommentInfo GetCommentFormat()
        {
             return Configuration.MyCommentInfo;
        }

        #region Reformatting


        /// <summary>
        /// This method formats the given span using the given EditArray. 
        /// An empty input span means reformat the entire document.
        /// </summary>
        public override void ReformatSpan(EditArray mgr, TextSpan span)
        {
            Babel.LanguageService langService = this.LanguageService as Babel.LanguageService;
            DaxFormatter formatter = new DaxFormatter(this, mgr, span, Environment.NewLine, this.LanguageService.Preferences, langService.FormattingPage.IndentDepthInFunctions, langService.FormattingPage.FormatterType);
            formatter.Format();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="info"></param>
        /// <param name="reason"></param>
        public override void Completion(IVsTextView textView, TokenInfo info, ParseReason reason)
        {
            base.Completion(textView, info, reason);
        }
    }
}
