// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System.Collections.Generic;
using System.Linq;
using DaxEditor.GeneratorSource;
using Microsoft.VisualStudio.Language.Intellisense;

namespace DaxEditor
{
    public class CompletionDataSnapshot
    {
        internal class InputData
        {
            internal string Name { get; set; }
            internal string Description { get; set; }
        }

        List<InputData> _keywords;
        List<InputData> _functions;
        List<string> _tables;
        List<string> _columnList;
        List<string> _measureList;
        IEnumerable<Completion> _sortedCompletion;

        public CompletionDataSnapshot()
        {
            _keywords = new List<InputData>();
            foreach (var keywordDescription in new DaxKeywords().GetDeclarations())
            {
                _keywords.Add(new InputData() { Name = keywordDescription.DisplayText, Description = keywordDescription.Description });
            }

            _functions = new List<InputData>();
            foreach (var daxFunctionDescription in new DaxFunctions())
            {
                _functions.Add(new InputData() { Name = daxFunctionDescription.Name, Description = daxFunctionDescription.Description });
            }

            _tables = new List<string>();
            _columnList = new List<string>();
            _measureList = new List<string>();

            _sortedCompletion = null;
        }

        public CompletionDataSnapshot(CompletionDataSnapshot original, List<DaxFunction> daxFunctions)
        {
            _keywords = original._keywords;
            _tables = original._tables;
            _columnList = original._columnList;
            _measureList = original._measureList;

            _functions = new List<InputData>();
            foreach (var daxFunctionDescription in daxFunctions)
            {
                _functions.Add(new InputData() { Name = daxFunctionDescription.Name, Description = daxFunctionDescription.Description });
            }

            _sortedCompletion = null;
        }

        public CompletionDataSnapshot(CompletionDataSnapshot original, List<string> tableList, List<string> columnList, List<string> measureList)
        {
            _keywords = original._keywords;
            _functions = original._functions;

            _tables = tableList;
            _columnList = columnList;
            _measureList = measureList;

            _sortedCompletion = null;
        }

        public IEnumerable<Completion> GetCompletionData(CompletionIconSource completionIconSource)
        {
            if (_sortedCompletion == null)
            {
                var completions = new List<Completion>();
                completions.AddRange(GetKeywordCompletionDataList(completionIconSource));
                completions.AddRange(GetFunctionCompletionDataList(completionIconSource));
                completions.AddRange(GetTableCompletionDataList(completionIconSource));

                completions.Sort((i, k) => string.Compare(i.DisplayText, k.DisplayText));
                _sortedCompletion = completions;
            }

            return _sortedCompletion;
        }

        private IEnumerable<Completion> GetKeywordCompletionDataList(CompletionIconSource completionIconSource)
        {
            var completions = new List<Completion>();
            foreach (var keyword in _keywords)
            {
                completions.Add(new Completion()
                {
                    DisplayText = keyword.Name,
                    InsertionText = keyword.Name,
                    Description = keyword.Description,
                    IconSource = completionIconSource.GetKeywordImage(),
                    IconAutomationText = null,
                });
            };

            return completions;
        }

        private IEnumerable<Completion> GetFunctionCompletionDataList(CompletionIconSource completionIconSource)
        {
            var completions = new List<Completion>();
            foreach (var function in _functions)
            {
                completions.Add(new Completion()
                {
                    DisplayText = function.Name,
                    InsertionText = function.Name,
                    Description = function.Description,
                    IconSource = completionIconSource.GetFunctionImage(),
                    IconAutomationText = null,
                });
            };

            return completions;
        }

        private IEnumerable<Completion> GetTableCompletionDataList(CompletionIconSource completionIconSource)
        {
            var completions = new List<Completion>();
            foreach (var table in _tables)
            {
                completions.Add(new Completion()
                {
                    DisplayText = table,
                    InsertionText = table,
                    Description = table,
                    IconSource = completionIconSource.GetTableImage(),
                    IconAutomationText = null,
                });
            };

            foreach (var column in _columnList)
            {
                completions.Add(new Completion()
                {
                    DisplayText = column,
                    InsertionText = column,
                    Description = column,
                    IconSource = completionIconSource.GetColumnImage(),
                    IconAutomationText = null,
                });
            };

            foreach (var measure in _measureList)
            {
                completions.Add(new Completion()
                {
                    DisplayText = measure,
                    InsertionText = measure,
                    Description = measure,
                    IconSource = completionIconSource.GetMeasureImage(),
                    IconAutomationText = null,
                });
            };

            return completions;
        }

    }
}
