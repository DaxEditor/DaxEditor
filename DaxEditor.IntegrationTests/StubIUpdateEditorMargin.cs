using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaxEditor.IntegrationTests
{
    internal class StubIUpdateEditorMargin : IUpdateEditorMargin
    {
        Action<System.Data.DataTable> _updateResult = null;
        Action<string> _updateSchema = null;
        Action<string> _updateXmlaResult = null;
        Action<string, bool> _logMessage = null;

        public StubIUpdateEditorMargin(Action<System.Data.DataTable> updateResult = null, Action<string> updateSchema = null, Action<string> updateXmlaResult = null, Action<string, bool> logMessage = null)
        {
            _updateResult = updateResult;
            _updateSchema = updateSchema;
            _updateXmlaResult = updateXmlaResult;
            _logMessage = logMessage;
        }

        public void UpdateResult(System.Data.DataTable result)
        {
            if (_updateResult != null)
                _updateResult(result);
        }

        public void UpdateSchema(string schemaHtml)
        {
            if (_updateSchema != null)
                _updateSchema(schemaHtml);
        }

        public void UpdateXmlaResult(string xmlaResult)
        {
            if (_updateXmlaResult != null)
                _updateXmlaResult(xmlaResult);
        }

        public void WriteLog(string logMessage, bool switchToLogWindow)
        {
            if (_logMessage != null)
                _logMessage(logMessage, switchToLogWindow);
        }
    }
}
