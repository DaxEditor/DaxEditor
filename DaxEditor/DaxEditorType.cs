// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace DaxEditor
{
    internal static class DaxEditorClassificationDefinition
    {
        /// <summary>
        /// Defines the "DaxEditor" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("DaxEditor")]
        internal static ClassificationTypeDefinition _DaxEditorType = null;

        [Export]
        [Name("DaxEditor")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition _contentTypeDefinition = null;

        [Export]
        [FileExtension(".dax")]
        [ContentType("DAX")]
        internal static FileExtensionToContentTypeDefinition _fileExtensionDefinition = null;
    }
}
