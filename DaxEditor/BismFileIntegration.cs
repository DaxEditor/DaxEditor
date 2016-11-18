// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DaxEditor
{
    /// <summary>
    /// Operations handling DAX editor interation with BIM file
    /// </summary>
    class BimFileIntegration
    {
        const string BimExtension = ".bim";
        const string DaxExtension = ".dax";
        const string BeginCustomScript  = @"-- MDX SCRIPT --";
        const string EndCustomScript = @"-- MDX SCRIPT --";

        /// <summary>
        /// Is BIM file available
        /// </summary>
        internal static bool IsAvailable(string daxFilePath)
        {
            //return System.IO.File.Exists(GetModelFileName(daxFilePath));
            return true;//lets just enable integration for all .dax files and show error message if something goes wrong
        }

        /// <summary>
        /// Returns BIM file path for given DAX file
        /// </summary>
        public static string ConvertDaxPathToBimPath(string daxFilePath)
        {
            return ConvertPath(daxFilePath, DaxExtension, BimExtension);
        }

        /// <summary>
        /// Returns DAX file path for given DAX file
        /// </summary>
        public static string ConvertBimPathToDaxPath(string daxFilePath)
        {
            return ConvertPath(daxFilePath, BimExtension, DaxExtension);
        }

        /// <summary>
        /// Changes provided path, replaces extension
        /// </summary>
        /// <param name="daxFilePath">Path to a file</param>
        /// <param name="fromExtension">File extension in the daxFilePath</param>
        /// <param name="toExtension">New file extension</param>
        /// <returns></returns>
        private static string ConvertPath(string daxFilePath, string fromExtension, string toExtension)
        {
            if (daxFilePath.EndsWith(fromExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                return daxFilePath.Substring(0, daxFilePath.Length - fromExtension.Length) + toExtension;
            }
            throw new ArgumentException(string.Format("{0} file expected. Actual: {1}.", fromExtension, daxFilePath));
        }

        /// <summary>
        /// Gets Measures section of BIM file
        /// </summary>
        internal static string GetMeasuresFromBimFile(string bimFilePath)
        {
            try
            {
                if (!File.Exists(bimFilePath))
                {
                    throw new FileNotFoundException(string.Format("Model file {0} doesn't exist.", bimFilePath));
                }
                string bimScript = File.ReadAllText(bimFilePath);
                var mc = MeasuresContainer.ParseText(bimScript);
                return mc.GetDaxText();
            }
            catch (Exception e)
            {
                throw new DaxException(string.Format("Error while reading measures from file '{0}'", bimFilePath), e);
            }
        }

        /// <summary>
        /// Saves DAX file to Measures section of BIM file
        /// </summary>
        internal static void SaveMeasuresToBimFile(string daxFilePath)
        {
            string daxFileContent = File.ReadAllText(daxFilePath);
            string customScript = null;

            //TL: If DAX file includes custom MDX script, remove it to avoid interference with parser
            if (daxFileContent.Contains(BeginCustomScript))
            {
                // read custom script
                customScript = daxFileContent.Substring(daxFileContent.IndexOf(BeginCustomScript, StringComparison.InvariantCultureIgnoreCase), daxFileContent.IndexOf(EndCustomScript, EndCustomScript.Length, StringComparison.InvariantCultureIgnoreCase) + EndCustomScript.Length);
                // remove custom script
                daxFileContent = daxFileContent.Replace(customScript, String.Empty);
            }         
            
            var mc = MeasuresContainer.ParseDaxScript(daxFileContent);
            string bimFilePath = ConvertDaxPathToBimPath(daxFilePath);
            if (!File.Exists(bimFilePath))
            {
                throw new FileNotFoundException(string.Format("Model file {0} doesn't exist", bimFilePath));
            }
            if (new FileInfo(bimFilePath).IsReadOnly)
            {
                throw new InvalidOperationException(string.Format("Model file {0} is read only", bimFilePath));
            }

            // TL -- injecting custom MDX script verbatum to the bim file
            if (!string.IsNullOrEmpty(customScript))
            {
                // insert customScript as first measure so it gets serialized first
                DaxMeasure m = new DaxMeasure();
                m.Name = "CustomMDXScript";
                m.FullText = customScript;
                mc.Measures.Insert(0, m);
            }

            string bimScript = System.IO.File.ReadAllText(bimFilePath);
            var updatedScript = mc.UpdateMeasures(bimScript);

            File.WriteAllText(bimFilePath, updatedScript);
        }

        /// <summary>
        /// Create DAX file from active BIM document
        /// </summary>
        /// <param name="activeDocumentPath">Path to currently opened BIM model</param>
        /// <returns>Path to created DAX file</returns>
        public static string CreateDaxFile(string activeDocumentPath)
        {
            string extension = Path.GetExtension(activeDocumentPath);
            if (!extension.Equals(BimExtension, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("DAX file can only be created from opened BIM model");
            string daxFilePath = activeDocumentPath.Substring(0, activeDocumentPath.Length - BimExtension.Length) + DaxExtension;
            if (File.Exists(daxFilePath))
                throw new ApplicationException(string.Format("DAX file {0} already exists", daxFilePath));

            string fileContents = BimFileIntegration.GetMeasuresFromBimFile(daxFilePath);
            File.WriteAllText(daxFilePath, string.IsNullOrEmpty(fileContents) ? "" : fileContents);
            return daxFilePath;
        }
    }
}
