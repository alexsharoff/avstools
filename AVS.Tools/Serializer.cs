using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace AVS.Tools
{
    public static class Serializer
    {
        public static void Serialize<T>(T o, TextWriter writer)
        {
            var bsSerializer = new XmlSerializer(typeof(T));
            bsSerializer.Serialize(writer, o);
        }
        public static void Serialize<T>(T obj, string path, FileMode fileMode = FileMode.Create)
        {
            if (Path.GetDirectoryName(path).Length == 0)
            {
                path = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), path);
            }
            var writer = new StreamWriter(File.Open(path, fileMode));
            Serialize<T>(obj, writer);
            writer.Close();
        }

        public static T Deserialize<T>(TextReader reader)
        {
            var bsSerializer = new XmlSerializer(typeof(T));
            return (T)bsSerializer.Deserialize(reader);
        }

        public static T Deserialize<T>(string path)
        {
            if (Path.GetDirectoryName(path).Length == 0)
            {
                path = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), path);
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            else
            {
                var reader = new StringReader(File.ReadAllText(path));
                T obj = Deserialize<T>(reader);
                reader.Close();
                return obj;
            }
        }
    }
}
