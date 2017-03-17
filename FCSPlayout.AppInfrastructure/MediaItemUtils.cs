using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    public static class MediaItemUtils
    {
        
        public static MediaItem FromEntity(MediaSourceEntity entity)
        {
            MediaSourceBase source = MediaSourceBase.Create(entity);

            var fileEntity = entity as MediaFileEntity;
            PlayRange playRange;
            if (fileEntity != null)
            {
                playRange = new PlayRange(TimeSpan.FromSeconds(fileEntity.MarkerIn),
                    TimeSpan.FromSeconds(fileEntity.MarkerDuration));
            }
            else
            {
                playRange = new PlayRange(PlayoutConfiguration.Current.DefaultDuration);
            }

            return new MediaItem(source, playRange);
        }

        public static MediaItem FromEntity(MediaFileEntity entity)
        {
            MediaSourceBase source = new FileMediaSource(entity);
            PlayRange playRange = new PlayRange(TimeSpan.FromSeconds(entity.MarkerIn), TimeSpan.FromSeconds(entity.MarkerDuration));

            return new MediaItem(source, playRange);
        }

        public static MediaSourceEntity ToEntity(MediaItem mediaItem)
        {

            MediaSourceEntity entity = mediaItem.Source.GetSourceEntity(); //.Entity;

            var rangeMarkable = entity as IRangeMarkable;
            if (rangeMarkable != null)
            {
                rangeMarkable.MarkerIn = mediaItem.PlayRange.StartPosition.TotalSeconds;
                rangeMarkable.MarkerDuration = mediaItem.PlayRange.Duration.TotalSeconds;
            }

            return entity;
        }

        private static MediaSourceEntity GetSourceEntity(this IMediaSource source)
        {
            var temp = source as MediaSourceBase;
            if (temp != null)
            {
                return temp.Entity;
            }
            return null;
        }
    }
}
