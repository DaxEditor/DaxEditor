// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace DaxEditor
{
    #region Provider definition
    /// <summary>
    /// This class causes a classifier to be added to the set of classifiers.
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [ContentType("DAX")]
    [TagType(typeof(ClassificationTag))]
    internal class DaxEditorProvider : ITaggerProvider
    {
        /// <summary>
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService _classificationRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService _aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<DaxTokenTag> tagAggregator = _aggregatorFactory.CreateTagAggregator<DaxTokenTag>(buffer);
            return new DaxEditor(tagAggregator, _classificationRegistry) as ITagger<T>;
        }
    }
    #endregion //provider def

    #region Classifier
    /// <summary>
    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
    /// </summary>
    class DaxEditor : ITagger<ClassificationTag>
    {
        ITagAggregator<DaxTokenTag> _aggregator;
        List<IClassificationType> _classificationTypes;

        internal DaxEditor(ITagAggregator<DaxTokenTag> aggregator, IClassificationTypeRegistryService typeService)
        {
            _aggregator = aggregator;

            // Define classification types
            _classificationTypes = new List<IClassificationType>(9);
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.Keyword));  // DaxTokenTypes.Keyword
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.Comment));  // DaxTokenTypes.Comment
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.Identifier));  // DaxTokenTypes.Identifier
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.String));  // DaxTokenTypes.StringLiteral
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.Number));  // DaxTokenTypes.NumberLiteral
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.ExcludedCode));  // DaxTokenTypes.SpecialFormat
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.Literal));  // DaxTokenTypes.Text
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.Other));  // DaxTokenTypes.Error
            _classificationTypes.Add(typeService.GetClassificationType(PredefinedClassificationTypeNames.PreprocessorKeyword));  // DaxTokenTypes.Function
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (var tagSpan in this._aggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return
                    new TagSpan<ClassificationTag>(tagSpans[0],
                                                   new ClassificationTag(_classificationTypes[(int)tagSpan.Tag.type]));
            }
        }
    }
    #endregion //Classifier
}
