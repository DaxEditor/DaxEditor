// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Babel;
using Babel.Parser;
using DaxEditor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace DaxEditor
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("DAX")]
    [TagType(typeof(DaxTokenTag))]
    internal sealed class DaxTokenTagProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new DaxTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class DaxTokenTag : ITag
    {
        public DaxTokenTypes type { get; private set; }

        public DaxTokenTag(DaxTokenTypes type)
        {
            this.type = type;
        }
    }

    internal sealed class DaxTokenTagger : ITagger<DaxTokenTag>
    {

        ITextBuffer _buffer;

        internal DaxTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<DaxTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var span in spans)
            {
                var line = span.Start.GetContainingLine();
                var scanner = new TokenScanner(
                    line.GetText(),
                    line.Start.Position, 
                    t => t.Token == Tokens.LEX_WHITE
                    );

                foreach (var location in scanner)
                {
                    var textSpan = new Microsoft.VisualStudio.Text.Span(location.Location, location.Length);
                    var tokenSpan = new SnapshotSpan(span.Snapshot, textSpan);
                    if (tokenSpan.IntersectsWith(span))
                        yield return new TagSpan<DaxTokenTag>(tokenSpan, new DaxTokenTag(TokenToTypeConverter.Convert(location.Token)));
                }
            }
        }
    }
}
