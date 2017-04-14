using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FCSPlayout.Domain
{
    public static class XmlSerializerExtensions
    {
        public static void Serialize<T>(this XmlSerializer serializer, string fileName, T obj)
        {
            serializer.Serialize<T>(fileName, obj, Encoding.UTF8);
        }

        public static void Serialize<T>(this XmlSerializer serializer, string fileName, T obj, Encoding encoding)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var writer = new StreamWriter(fs, encoding))
                {
                    serializer.Serialize(writer, obj);
                }
            }
        }

        public static T Deserialize<T>(this XmlSerializer serializer, string fileName)
        {
            return serializer.Deserialize<T>(fileName, Encoding.UTF8);
        }

        public static T Deserialize<T>(this XmlSerializer serializer, string fileName, Encoding encoding)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = new StreamReader(fs, encoding))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
        }
    }
}
