namespace DaxEditor.Test
{
    using System.IO;
    using System.Reflection;
    
    static class Utils
    {
        public static string ReadFileFromResources(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DaxEditorSample.Test.Resources." + fileName;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
