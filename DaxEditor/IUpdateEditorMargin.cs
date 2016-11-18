// The project released under MS-PL license https://daxeditor.codeplex.com/license


using System.Data;

namespace DaxEditor
{
    public interface IUpdateEditorMargin
    {
        void UpdateResult(DataTable result);
        void UpdateSchema(string schemaHtml);
        void UpdateXmlaResult(string xmlaResult);
        void WriteLog(string logMessage, bool switchToLogWindow);
    }
}
