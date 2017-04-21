using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public static class MediaSourceExtensions
    {
        public static IMediaSource ToMediaSource(this MediaSourceEntity entity)
        {
            MediaFileEntity fileEntity = entity as MediaFileEntity;
            if (fileEntity != null)
            {
                return new FileMediaSource(fileEntity);
            }
            else
            {
                ChannelInfo ci = entity as ChannelInfo;
                if (ci != null)
                {
                    return new ChannelMediaSource(ci);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public static IMediaSource ToMediaSource(this MediaSourceEntity entity, PlaybillItemEntity billItemEntity)
        {
            IMediaSource result = null;
            MediaFileEntity fileEntity = entity as MediaFileEntity;
            if (fileEntity != null)
            {
                var fileSource = new FileMediaSource(fileEntity);
                fileSource.AudioGain = billItemEntity.AudioGain;
                result = fileSource;
            }
            else
            {
                ChannelInfo ci = entity as ChannelInfo;
                if (ci != null)
                {
                    var channelSource = new ChannelMediaSource(ci);
                    result = channelSource;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            return result;
        }

        public static MediaSourceEntity ToEntity(this IMediaSource mediaSource)
        {
            var fileMediaSource = mediaSource as FileMediaSource;
            if (fileMediaSource != null)
            {
                MediaFileEntity entity = new MediaFileEntity();
                entity.Id = fileMediaSource.Id;
                entity.AudioGain = fileMediaSource.AudioGain;
                entity.FileName = fileMediaSource.FileName;
                entity.Title = fileMediaSource.Title;
                entity.Duration = fileMediaSource.Duration.Value.TotalSeconds;
                return entity;
            }
            else
            {
                var channelSource = mediaSource as ChannelMediaSource;
                if (channelSource != null)
                {
                    var entity = new ChannelInfo();
                    entity.Id = channelSource.Id;
                    entity.Title = channelSource.Title;
                    return entity;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }
    }
}
