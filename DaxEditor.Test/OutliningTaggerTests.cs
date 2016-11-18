// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using DaxEditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;

namespace DaxEditorSample.Test
{
    [TestClass]
    public class OutliningTaggerTests
    {
        [TestMethod]
        public void OutliningTagger_EmptyBuffer()
        {
            var iTextBufferFactoryService = GetTextBufferFactoryService();
            var emptyTextBuffer = iTextBufferFactoryService.CreateTextBuffer();
            Assert.IsNotNull(emptyTextBuffer);
            var outliningTagger = new OutliningTagger(emptyTextBuffer);
            var spans = new NormalizedSnapshotSpanCollection(emptyTextBuffer.CurrentSnapshot, GetTextSnapshotSpan(emptyTextBuffer.CurrentSnapshot));
            var tags = outliningTagger.GetTags(spans);
            Assert.AreEqual(0, tags.Count());
        }

        [TestMethod]
        public void OutliningTagger_OnlyComment()
        {
            var iTextBufferFactoryService = GetTextBufferFactoryService();
            var commentTextBuffer = iTextBufferFactoryService.CreateTextBuffer(@"-- this is comment only", iTextBufferFactoryService.TextContentType);
            Assert.IsNotNull(commentTextBuffer);
            var outliningTagger = new OutliningTagger(commentTextBuffer);
            var spans = new NormalizedSnapshotSpanCollection(commentTextBuffer.CurrentSnapshot, GetTextSnapshotSpan(commentTextBuffer.CurrentSnapshot));
            var tags = outliningTagger.GetTags(spans);
            Assert.AreEqual(0, tags.Count());
        }

        [TestMethod]
        public void OutliningTagger_RandomText()
        {
            var iTextBufferFactoryService = GetTextBufferFactoryService();
            var textBuffer = iTextBufferFactoryService.CreateTextBuffer(@"asd", iTextBufferFactoryService.TextContentType);
            Assert.IsNotNull(textBuffer);
            var outliningTagger = new OutliningTagger(textBuffer);
            var spans = new NormalizedSnapshotSpanCollection(textBuffer.CurrentSnapshot, GetTextSnapshotSpan(textBuffer.CurrentSnapshot));
            var tags = outliningTagger.GetTags(spans);
            Assert.AreEqual(0, tags.Count());
        }

        [TestMethod]
        public void OutliningTagger_OneLineQuery()
        {
            var iTextBufferFactoryService = GetTextBufferFactoryService();
            var OneLineQuery = iTextBufferFactoryService.CreateTextBuffer(@"EVALUATE T", iTextBufferFactoryService.TextContentType);
            Assert.IsNotNull(OneLineQuery);
            var outliningTagger = new OutliningTagger(OneLineQuery);
            var spans = new NormalizedSnapshotSpanCollection(OneLineQuery.CurrentSnapshot, GetTextSnapshotSpan(OneLineQuery.CurrentSnapshot));
            var tags = outliningTagger.GetTags(spans);
            Assert.AreEqual(1, tags.Count());
        }

        [TestMethod]
        public void OutliningTagger_TwoMeasures()
        {
            var iTextBufferFactoryService = GetTextBufferFactoryService();
            var OneLineQuery = iTextBufferFactoryService.CreateTextBuffer(@"CREATE MEASURE T[M1] = 1
CREATE MEASURE 'T T'[M2] = 2", iTextBufferFactoryService.TextContentType);
            Assert.IsNotNull(OneLineQuery);
            var outliningTagger = new OutliningTagger(OneLineQuery);
            var spans = new NormalizedSnapshotSpanCollection(OneLineQuery.CurrentSnapshot, GetTextSnapshotSpan(OneLineQuery.CurrentSnapshot));
            var tags = outliningTagger.GetTags(spans);
            Assert.AreEqual(2, tags.Count());
            var tag1 = tags.First();
            Assert.AreEqual(0, tag1.Span.Start.Position);
            Assert.AreEqual(24, tag1.Span.End.Position);
            Assert.AreEqual("T[M1]", tag1.Tag.CollapsedForm as string);
            Assert.AreEqual("CREATE MEASURE T[M1] = 1", tag1.Tag.CollapsedHintForm as string);
            var tag2 = tags.Last();
            Assert.AreEqual(26, tag2.Span.Start.Position);
            Assert.AreEqual(54, tag2.Span.End.Position);
            Assert.AreEqual("'T T'[M2]", tag2.Tag.CollapsedForm as string);
            Assert.AreEqual("CREATE MEASURE 'T T'[M2] = 2", tag2.Tag.CollapsedHintForm as string);
        }

        [TestMethod]
        public void OutliningTagger_ThreeMeasures()
        {
            var iTextBufferFactoryService = GetTextBufferFactoryService();
            var text = iTextBufferFactoryService.CreateTextBuffer(@"CREATE MEASURE T[M1] = 1;

CREATE MEASURE T[m3] = 
 (3 + 9) / 12

;

CREATE MEASURE T[M2] = 2
;

", iTextBufferFactoryService.TextContentType);
            Assert.IsNotNull(text);
            var outliningTagger = new OutliningTagger(text);
            var spans = new NormalizedSnapshotSpanCollection(text.CurrentSnapshot, GetTextSnapshotSpan(text.CurrentSnapshot));
            var tags = outliningTagger.GetTags(spans);
            Assert.AreEqual(3, tags.Count());

            var tag1 = tags.First();
            Assert.AreEqual(0, tag1.Span.Start.Position);
            Assert.AreEqual(25, tag1.Span.End.Position);
            var tag2 = tags.Skip(1).First();
            Assert.AreEqual(29, tag2.Span.Start.Position);
            Assert.AreEqual(72, tag2.Span.End.Position);
            var tag3 = tags.Skip(2).First();
            Assert.AreEqual(76, tag3.Span.Start.Position);
            Assert.AreEqual(103, tag3.Span.End.Position);
        }

        private static ITextBufferFactoryService GetTextBufferFactoryService()
        {
            var container = new CompositionContainer(new AssemblyCatalog(Assembly.LoadFrom("Microsoft.VisualStudio.Platform.VSEditor.dll")));
            Assert.IsNotNull(container, "Assembly Container is null");
            var service = container?.GetExportedValue<ITextBufferFactoryService>();
            Assert.IsNotNull(service, "TextBufferFactoryService is null");
            return service;
        }

        private static IEnumerable<Span> GetTextSnapshotSpan(ITextSnapshot textSnapshot)
        {
            yield return new Span(0, textSnapshot.Length);
        }
    }
}
