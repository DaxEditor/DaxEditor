// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using DaxEditor.GeneratorSource;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;
using Babel;

namespace DaxEditor
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("DAX")]
    [Name("DaxEditor")]
    class DaxCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        IGlyphService GlyphService = null;

        [Import]
        internal SVsServiceProvider _serviceProvider = null;

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            var languageService = _serviceProvider.GetService(typeof(Babel.LanguageService)) as LanguageService;
            var complitionDataProvider = languageService.BismProvider as ICompletionDataSnaphotProvider;

            return new DaxCompletionSource(textBuffer, GlyphService, complitionDataProvider);
        }
    }

    class DaxCompletionSource : ICompletionSource, IDisposable
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;
        private IGlyphService _glyphService = null;
        private ICompletionDataSnaphotProvider _completionDataProvider;
        private CompletionIconSource _completionIconSource;

        public DaxCompletionSource(ITextBuffer buffer, IGlyphService glyphService, ICompletionDataSnaphotProvider completionDataProvider)
        {
            _buffer = buffer;
            _glyphService = glyphService;
            _completionDataProvider = completionDataProvider;
            _completionIconSource = new CompletionIconSource(_glyphService);
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed)
                throw new ObjectDisposedException("DaxCompletionSource");

            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

            if (triggerPoint == null)
                return;

            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = triggerPoint;

            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
            {
                start -= 1;
            }

            var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);

            var completion = _completionDataProvider.GetCompletionDataSnapshot().GetCompletionData(_completionIconSource);
            completionSets.Add(new CompletionSet("All", "All", applicableTo, completion, Enumerable.Empty<Completion>()));
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
