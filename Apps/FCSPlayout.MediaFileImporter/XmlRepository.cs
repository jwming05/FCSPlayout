using FCSPlayout.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FCSPlayout.MediaFileImporter
{
    class XmlRepository
    {
        public static void Save(string fileName, IEnumerable<MediaFileEntity> entities)
        {
            List<MediaFileEntity> list = new List<MediaFileEntity>(entities);

            XmlSerializer serializer = new XmlSerializer(typeof(List<MediaFileEntity>));
            using(var fs=new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs,list);
            }
        }

        public static IEnumerable<MediaFileEntity> Load(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<MediaFileEntity>));
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (List<MediaFileEntity>)serializer.Deserialize(fs);
            }
        }
    }
}
