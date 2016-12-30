namespace DaxEditor.Xml
{
    using Microsoft.AnalysisServices;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public static class XmlUtilities
    {
        private static string GenerateBackupXmla(out string tempPath)
        {
            tempPath = Path.GetTempFileName();
            return @"<Backup xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
  <Object>
    <DatabaseID>Test Database ID</DatabaseID>
  </Object>
  <File>" + tempPath + @"</File>
  <AllowOverwrite>true</AllowOverwrite>
</Backup>";
        }

        public static bool IsXml(string text)
        {
            return text[0] != '<';
        }
        private static Stream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        public static Server Deserialize(string text)
        {
            var serializer = new XmlSerializer(typeof(Server));
            return serializer.Deserialize(GenerateStreamFromString(text)) as Server;
        }

        public static string Serialize(Server server)
        {
            //var options = new SerializeOptions();
            //options.SplitMultilineStrings = true;
            //var stream = new MemoryStream();
            //var serializer = new XmlSerializer(typeof(Server));
            //serializer.Serialize(stream, server);
            //return new StreamReader(stream).ReadToEnd();
            //string path;
            //server.Connect()
            //server.SendXmlaRequest(XmlaRequestType.Execute, GenerateStreamFromString(GenerateBackupXmla(out path)));
            //return File.ReadAllText(path);
            return string.Empty;
        }
    }
}