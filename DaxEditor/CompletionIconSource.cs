// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System.Diagnostics;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;

namespace DaxEditor
{
    public class CompletionIconSource
    {
        public CompletionIconSource(IGlyphService glyphService)
        {
            _glyphService = glyphService;
        }

        private IGlyphService _glyphService = null;

        public ImageSource GetKeywordImage()
        {
            Debug.Assert(_glyphService != null);

            return _glyphService.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemInternal);
        }

        public ImageSource GetFunctionImage()
        {
            Debug.Assert(_glyphService != null);

            return _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, StandardGlyphItem.GlyphItemPublic);
        }

        public ImageSource GetTableImage()
        {
            Debug.Assert(_glyphService != null);

            return _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupClass, StandardGlyphItem.GlyphItemPublic);
        }

        public ImageSource GetColumnImage()
        {
            Debug.Assert(_glyphService != null);

            return _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupOperator, StandardGlyphItem.GlyphItemPublic);
        }

        public ImageSource GetMeasureImage()
        {
            Debug.Assert(_glyphService != null);

            return _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupProperty, StandardGlyphItem.GlyphItemPublic);
        }
    }
}
