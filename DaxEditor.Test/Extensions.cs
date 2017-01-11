// The project released under MS-PL license https://daxeditor.codeplex.com/license

using DaxEditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using DaxEditor.StringExtensions;

namespace DaxEditorSample.Test
{
    public static class WindiffAssert
    {
        private static string _tempLogFolder = null;
        private static int _counter = 0;

        /// <summary>
        ///  Normalizes expected and actual string by removing whitespaces in the beggining of lines for all element before comparison
        /// </summary>
        /// <param name="expected">Expected XMLA</param>
        /// <param name="actual">Actual XMLA</param>
        public static void AreEqualNormalizedXmla(string expected, string actual)
        {
            string normalizedExpected = NormalizeWhitespacesInText(NormalizeXmla(expected));
            string normalizedActual = NormalizeWhitespacesInText(NormalizeXmla(actual));

            if (!string.Equals(normalizedExpected, normalizedActual))
            {
                if(_tempLogFolder == null)
                {
                    _tempLogFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(_tempLogFolder);
                }

                _counter++;

                string expectedFileName = Path.Combine(_tempLogFolder, _counter.ToString("00") + "_expected.txt");
                File.WriteAllText(expectedFileName, expected);
                string actualFileName = Path.Combine(_tempLogFolder, _counter.ToString("00") + "_actual.txt");
                File.WriteAllText(actualFileName, actual);

                string expectedNormalizedFileName = Path.Combine(_tempLogFolder, _counter.ToString("00") + "_expectedNormalized.txt");
                File.WriteAllText(expectedNormalizedFileName, normalizedExpected);
                string actualNormalizedFileName = Path.Combine(_tempLogFolder, _counter.ToString("00") + "_actualNormalized.txt");
                File.WriteAllText(actualNormalizedFileName, normalizedActual);

                string winDiffCommandLine = string.Format(@"Windiff.exe ""{0}"" ""{1}""", expectedFileName, actualFileName);
                string winDiffCommandLineForNormalized = string.Format(@"Windiff.exe ""{0}"" ""{1}""", expectedNormalizedFileName, actualNormalizedFileName);
                var failMessage = $@"Different XMLA.  Normalized diff:
                {winDiffCommandLineForNormalized}
                Different XMLA.  Actual diff:
{winDiffCommandLine}";
                Assert.Fail(failMessage);
            }
        }

        public static string GetFCDiff(string expected, string actual)
        {
            try
            {
                string expectedFileName = "expected.txt.temp";
                string actualFileName = "actual.txt.temp";
                File.WriteAllText(expectedFileName, expected);
                File.WriteAllText(actualFileName, actual);
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "fc.exe",
                        Arguments = $"\"{expectedFileName}\" \"{actualFileName}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                var lines = new List<string>();
                while (!proc.StandardOutput.EndOfStream)
                {
                    lines.Add(proc.StandardOutput.ReadLine());
                }
                File.Delete(expectedFileName);
                File.Delete(actualFileName);
                return string.Join("\n", lines);
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// Сomparison without normalization
        /// </summary>
        /// <param name="expected">Expected text</param>
        /// <param name="actual">Actual text</param>
        public static void AreEqual(string expected, string actual)
        {
            if (!string.Equals(expected, actual))
            {
                Assert.Fail($@"
Different texts. Diff:
{GetFCDiff(expected, actual)}");
            }
        }

        public static void AreEqualIgnoreEmptyLinesInExpressions(string expected, string actual)
        {
            AreEqual(
                expected.Replace(" ", "").Replace("\"", "").Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace(",", ""), 
                actual.Replace(" ", "").Replace("\"", "").Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace(",", "")
            );
        }

        /// <summary>
        /// Normalizes XMLA by replacing different const strings in MdxScript with common text
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string NormalizeXmla(string input)
        {
            if (input == null)
                return input;

            var result = input;
            result = result.Replace(ServerCommandProducer.DoNotModify1100, $"-- normalized header{Environment.NewLine}");
            result = result.Replace(ServerCommandProducer.DoNotModify1103, $"-- normalized header{Environment.NewLine}");
            var calculateNormalizeCandidates = new List<string>() { ServerCommandProducer.CommonCommandText1100, ServerCommandProducer.CommonCommandText1103, ServerCommandProducer.CommonCommandTextUnknownVersion };
            foreach(var calculateNormalizeCandidate in calculateNormalizeCandidates)
            {
                result = result.Replace(calculateNormalizeCandidate, $"CALCULATE; -- normalized CALCULATE{Environment.NewLine}");
            }

            result = result.Replace("<CalculationReference>Measures.[__No measures defined]</CalculationReference>", "<CalculationReference>-- normalized</CalculationReference>");
            result = result.Replace("<CalculationReference>[__XL_Count of Models]</CalculationReference>", "<CalculationReference>-- normalized</CalculationReference>");

            return result;
        }

        /// <summary>
        /// Normalizes string by removing whitespaces from beggining of every line
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Normalized string</returns>
        private static string NormalizeWhitespacesInText(string input)
        {
            if (input == null)
            {
                return input;
            }

            var builder = new StringBuilder();
            foreach (var line in input.ToLines())
            {
                builder.AppendLine(line.TrimStart(new char[] { ' ', '\t' }));
            }

            return builder.ToString();
        }
    }
}
