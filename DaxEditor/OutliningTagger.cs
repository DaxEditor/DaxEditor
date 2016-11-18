using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Babel;
using BabelParser = Babel.Parser;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using DaxEditor;

namespace DaxEditor
{
    public sealed class OutliningTagger : ITagger<IOutliningRegionTag>
    {
        class PartialRegion
        {
            public BabelParser.Tokens StartToken { get; set; }
            public string TableName { get; set; }
            public string MeasureName { get; set; }
            public int StartLine { get; set; }
            public string GetHoverText()
            {
                if (string.IsNullOrEmpty(TableName) || string.IsNullOrEmpty(MeasureName))
                    return "...";

                return string.Format("{0}{1}", TableName, MeasureName);
            }
        }

        class Region : PartialRegion
        {
            public int EndLine { get; set; }
        }

        ITextBuffer buffer;
        ITextSnapshot snapshot;
        List<Region> regions;
        IList<BabelParser.Tokens> _startRegionTokens = new List<BabelParser.Tokens>() { BabelParser.Tokens.KWCREATE, BabelParser.Tokens.KWEVALUATE, BabelParser.Tokens.KWDEFINE };

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public OutliningTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
            this.snapshot = buffer.CurrentSnapshot;
            this.regions = new List<Region>();
            this.Parse();
            this.buffer.Changed += BufferChanged;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

            List<Region> currentRegions = this.regions;
            ITextSnapshot currentSnapshot = this.snapshot;
            SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;
            foreach (var region in currentRegions)
            {
                if (region.StartLine <= endLineNumber &&
                    region.EndLine >= startLineNumber)
                {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(startLine.Start,
                        endLine.End),
                        new OutliningRegionTag(false, false, region.GetHoverText(), currentSnapshot.GetText(startLine.Start.Position, endLine.End.Position - startLine.Start.Position)));
                }
            }
        }

        void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event). 
            if (e.After != buffer.CurrentSnapshot)
                return;

            this.Parse();
        }

        private void Parse()
        {
            ITextSnapshot newSnapshot = buffer.CurrentSnapshot;
            List<Region> newRegions = new List<Region>();

            PartialRegion currentRegion = null;
            int lineOfPreviousToken = -1;

            foreach (var line in newSnapshot.Lines)
            {
                int curLoc = line.Start.Position;

                foreach (TokenLocation tokenLocation in new TokenScanner(line.GetText(), curLoc, t => t.Token == BabelParser.Tokens.LEX_WHITE))
                {
                    if (_startRegionTokens.Contains(tokenLocation.Token))
                    {
                        if (currentRegion == null)
                        {   // Init currentRegion for the first time
                            currentRegion = new PartialRegion()
                            {
                                StartToken = tokenLocation.Token,
                                StartLine = line.LineNumber,
                            };
                        }
                        else
                        {   // Save currentRegion in newRegions, refresh currentRegion 
                            newRegions.Add(new Region()
                            {
                                StartToken = currentRegion.StartToken,
                                StartLine = currentRegion.StartLine,
                                TableName = currentRegion.TableName,
                                MeasureName = currentRegion.MeasureName,
                                EndLine = lineOfPreviousToken
                            });

                            currentRegion = new PartialRegion()
                            {
                                StartToken = tokenLocation.Token,
                                StartLine = line.LineNumber,
                            };
                        }
                    }
                    // Get table name and measure name for hover text
                    if (currentRegion != null)
                    {
                        if (tokenLocation.Token == BabelParser.Tokens.ESCAPEDTABLENAME || tokenLocation.Token == BabelParser.Tokens.TABLENAME && currentRegion.TableName == null)
                            currentRegion.TableName = newSnapshot.GetText(tokenLocation.Location, tokenLocation.Length);
                        if (tokenLocation.Token == BabelParser.Tokens.COLUMNNAME && currentRegion.MeasureName == null)
                            currentRegion.MeasureName = newSnapshot.GetText(tokenLocation.Location, tokenLocation.Length);
                    }

                    lineOfPreviousToken = line.LineNumber; // Line where the last not white-space token is located
                }
            }

            if (currentRegion != null && _startRegionTokens.Contains(currentRegion.StartToken))
            {
                newRegions.Add(new Region()
                {
                    StartToken = currentRegion.StartToken,
                    StartLine = currentRegion.StartLine,
                    TableName = currentRegion.TableName,
                    MeasureName = currentRegion.MeasureName,
                    EndLine = lineOfPreviousToken
                });
            }

            // Determine the changed span, and send a changed event with the new spans
            List<Span> oldSpans =
                new List<Span>(this.regions.Select(r => AsSnapshotSpan(r, this.snapshot)
                    .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
                    .Span));
            List<Span> newSpans =
                    new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

            NormalizedSpanCollection oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            NormalizedSpanCollection newSpanCollection = new NormalizedSpanCollection(newSpans);

            // The changed regions are regions that appear in one set or the other, but not both.
            NormalizedSpanCollection removed =
            NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

            int changeStart = int.MaxValue;
            int changeEnd = -1;

            if (removed.Count > 0)
            {
                changeStart = removed[0].Start;
                changeEnd = removed[removed.Count - 1].End;
            }

            if (newSpans.Count > 0)
            {
                changeStart = Math.Min(changeStart, newSpans[0].Start);
                changeEnd = Math.Max(changeEnd, newSpans[newSpans.Count - 1].End);
            }

            this.snapshot = newSnapshot;
            this.regions = newRegions;

            if (changeStart <= changeEnd)
            {
                ITextSnapshot snap = this.snapshot;
                if (this.TagsChanged != null)
                    this.TagsChanged(this, new SnapshotSpanEventArgs(
                        new SnapshotSpan(this.snapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = (region.StartLine == region.EndLine) ? startLine
                 : snapshot.GetLineFromLineNumber(region.EndLine);
            return new SnapshotSpan(startLine.Start, endLine.End);
        }
    }
}
