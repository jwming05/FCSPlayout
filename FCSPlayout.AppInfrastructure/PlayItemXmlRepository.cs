using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FCSPlayout.AppInfrastructure
{
    public class PlayItemXmlRepository
    {
        public static void SaveToXml(string fileName, IEnumerable<IPlayItem> playItems)
        {
            List<PlayItemEntity> entities = new List<PlayItemEntity>(playItems.Select(i=>i.ToEntity()));
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<PlayItemEntity>));
            serializer.Serialize(fileName, entities);
        }

        public static IList<IPlayItem> LoadFromXml(string fileName)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<PlayItemEntity>));
            List<PlayItemEntity> entities = serializer.Deserialize<List<PlayItemEntity>>(fileName);

            return entities.Select(i =>i.ToPlayItem()).ToList();
        }
    }
}