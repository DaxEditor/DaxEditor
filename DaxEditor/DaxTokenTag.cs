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
            foreach (SnapshotSpan curSpan in spans)
            {
                var scanner = new LineScanner();
                ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
                int curLoc = containingLine.Start.Position;


                foreach (TokenLocation tokenLocation in new TokenScanner(containingLine.GetText(), curLoc, t => t.Token == Tokens.LEX_WHITE))
                {
                    var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Microsoft.VisualStudio.Text.Span(tokenLocation.Location, tokenLocation.Length));
                    if (tokenSpan.IntersectsWith(curSpan))
                        yield return new TagSpan<DaxTokenTag>(tokenSpan, new DaxTokenTag(TokenToTypeConverter.Convert(tokenLocation.Token)));
                }

                //string[] tokens = containingLine.GetText().ToLower().Split(' ');

                //foreach (string ookToken in tokens)
                //{
                //    if (_ookTypes.ContainsKey(ookToken))
                //    {
                //        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, ookToken.Length));
                //        if (tokenSpan.IntersectsWith(curSpan))
                //            yield return new TagSpan<DaxTokenTag>(tokenSpan,
                //                                                  new DaxTokenTag(_ookTypes[ookToken]));
                //    }

                //    //add an extra char location because of the space
                //    curLoc += ookToken.Length + 1;
                //}
            }

        }
    }
}
