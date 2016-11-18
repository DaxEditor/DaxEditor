// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DaxEditor;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Babel
{
    [Guid(GuidList.guidDaxLanguageService)]
    public partial class LanguageService : BabelLanguageService
    {
        #region Public static fields

        public const int BASE_COLUMN_GLYPH_INDEX = 0;
        public const int CALC_COLUMN_GLYPH_INDEX = 1;
        public const int MEASURE_GLYPH_INDEX = 2;
        public const int TABLE_GLYPH_INDEX = 3;
        public const int FUNCTION_GLYPH_INDEX = 72;
        public const int KEYWORD_GLYPH_INDEX = 36;

        public const char TABLE_BEGIN_CHAR  = '\'';
        public const char TABLE_END_CHAR    = '\'';
        public const char MEMBER_BEGIN_CHAR = '[';
        public const char MEMBER_END_CHAR   = ']';

        #endregion

        public override string GetFormatFilterList()
        {
            return "DAX files (*.dax)\n*.dax";
        }

        public BismInfoProvider BismProvider { get; private set; }

        /// <summary>
        /// Creates DAX Document Properties object, stores in private member and returns the reference to it.
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns>Reference to DAX Document Properties.</returns>
        public override DocumentProperties CreateDocumentProperties(CodeWindowManager mgr)
        {
            DaxDocumentProperties daxDocumentProperties = new DaxDocumentProperties(mgr);

            // Initialize BismProvider for given Document/Source/CodeWindowManager
            Babel.Source babelSource = mgr.Source as Babel.Source;
            if (babelSource != null)
            {
                BismProvider = new BismInfoProvider(daxDocumentProperties);
                babelSource.BismInfoProvider = BismProvider;
            }

            return daxDocumentProperties;
        }

          public override Microsoft.VisualStudio.Package.AuthoringScope ParseSource(ParseRequest req)
        {
            return base.ParseSource(req);
        }

        public override ImageList GetImageList()
        {
            Color background = Color.Magenta;

            ImageList ilist = new ImageList();
            ilist.ImageSize = new Size(16, 16);
            ilist.TransparentColor = background;
            
            // Obtain icons from the assembly
            Assembly assembly = typeof(LanguageService).Assembly;
            // Save BaseColumn Glyph under index=0
            Stream stm = assembly.GetManifestResourceStream("DaxEditor.Resources.BaseColumnGlyph.ico");
            ilist.Images.Add(new Icon(stm));
            // Save Measure Glyph under index=1
            stm = assembly.GetManifestResourceStream("DaxEditor.Resources.CalcColumnGlyph.ico");
            ilist.Images.Add(new Icon(stm));
            // Save Measure Glyph under index=2
            stm = assembly.GetManifestResourceStream("DaxEditor.Resources.MeasureGlyph.ico");
            ilist.Images.Add(new Icon(stm));
            // Save Table Glyph under index=3
            stm = assembly.GetManifestResourceStream("DaxEditor.Resources.TableGlyph.ico");
            ilist.Images.Add(new Icon(stm));
            
            return ilist;
        }


        public DaxFormattingPage FormattingPage { get; set; }

        // Help for functions and keywords.  Initiated by pressing F1.
        public override void UpdateLanguageContext(LanguageContextHint hint, IVsTextLines buffer, TextSpan[] ptsSelection, IVsUserContext context)
        {
            string searchingKeyword = null;
            // Search keyword as the function that presented where cursor stays or just before it
            Source source = (Source)this.GetSource(buffer);
            Debug.Assert(ptsSelection.Length > 0);
            int line = ptsSelection[0].iStartLine;
            int endPosition = ptsSelection[0].iEndIndex;
            var colorState = new DaxEditor.DaxFormatter.DummyColorState();
            TokenInfo[] lineInfos = source.GetColorizer().GetLineInfo(buffer, line, colorState);
            foreach(var tokenInfo in lineInfos)
            {
                if(!(tokenInfo.Type == TokenType.Identifier || tokenInfo.Type == TokenType.Keyword))
                    continue;
                var span = new TextSpan();
                span.iStartLine = line;
                span.iEndLine = line;
                span.iStartIndex = tokenInfo.StartIndex;
                span.iEndIndex = tokenInfo.EndIndex + 1;

                searchingKeyword = source.GetText(span);

                if (span.iEndIndex >= endPosition)
                    break;
            }

            if (!string.IsNullOrEmpty(searchingKeyword))
            {
                ErrorHandler.ThrowOnFailure(context.RemoveAttribute(null, null));
                ErrorHandler.ThrowOnFailure(context.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Lookup, "keyword", "SQL11.AS.DAXREF." + searchingKeyword + ".F1"));
            }
        }
    }
}
