using FCSPlayout.CG;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.PlaybillEditor
{
    class PlayItemXmlRepository
    {
        public static void SaveToXml(string fileName, IEnumerable<IPlayItem> playItems, IMediaSourceConverter mediaSourceConverter)
        {
            List<PlayItemEntity> entities = new List<PlayItemEntity>(playItems.Select(i=>new PlayItemEntity(i, mediaSourceConverter)));
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<PlayItemEntity>));
            serializer.Serialize(fileName, entities);
        }

        public static IList<IPlayItem> LoadFromXml(string fileName, IMediaSourceConverter mediaSourceConverter)
        {
            //List<PlayItemEntity> entities = new List<PlayItemEntity>(playItems.Select(i => new PlayItemEntity(i, mediaSourceConverter)));
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<PlayItemEntity>));
            List<PlayItemEntity> entities = serializer.Deserialize<List<PlayItemEntity>>(fileName);

            return entities.Select(i =>);
        }

        private static IPlaybillItem FromEntity(PlaybillItemEntity entity, IMediaSourceConverter mediaSourceConverter)
        {

            MediaSourceEntity sourceEntity = entity.MediaSource;
            IMediaSource mediaSource = null;
            if (sourceEntity == null)
            {
                mediaSource = new NullMediaSource(entity.MediaSourceTitle, TimeSpan.FromSeconds(entity.MediaSourceDuration.Value));
            }
            else
            {
                mediaSource = mediaSourceConverter.FromEntity<IMediaSource>(entity.MediaSource); // FromEntity(entity, entity.MediaSource);
            }

            IPlaySource playSource = Create(entity, mediaSource);
            IPlaybillItem result = null;
            switch (entity.ScheduleMode)
            {
                case PlayScheduleMode.Auto:
                    result = PlaybillItem.Auto(playSource);
                    break;
                case PlayScheduleMode.Timing:
                    result = PlaybillItem.Timing(playSource, entity.StartTime.Value);
                    break;
                case PlayScheduleMode.TimingBreak:
                    result = PlaybillItem.TimingBreak(playSource, entity.StartTime.Value);
                    break;
            }
            result.Id = entity.Id;
            return result;
        }

        private static IPlaySource Create(PlaybillItemEntity entity, IMediaSource mediaSource)
        {
            var range = new PlayRange(TimeSpan.FromSeconds(entity.MarkerIn), TimeSpan.FromSeconds(entity.MarkerDuration));

            CGItemCollection cgItems = null;


            if (entity.CGContents != null)
            {
                cgItems = CGItemCollection.FromXml(entity.CGContents);
            }

            if (cgItems == null)
            {
                return new PlaySource(mediaSource, range);
            }
            else
            {
                return new PlaySource(mediaSource, range, cgItems);
            }
        }
    }


}
