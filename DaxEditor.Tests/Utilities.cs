namespace DaxEditor.Tests
{
    using System.IO;
    using System.Reflection;
    
    internal static class Utilities
    {
        public static string ReadFileFromResources(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var projectDir = Path.GetDirectoryName(
                Path.GetDirectoryName(Path.GetDirectoryName(assembly.Location)));
            var resourcesDir = Path.Combine(projectDir, "Resources");

            return File.ReadAllText(Path.Combine(resourcesDir, name));
        }
    }
}
