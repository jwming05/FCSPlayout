using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    public class MediaSourceConverter : IMediaSourceConverter
    {
        public T FromEntity<T>(IMediaSourceEntity entity) where T : IMediaSource
        {
            IMediaSource result = null;
            MediaFileEntity fileEntity = entity as MediaFileEntity;
            if (fileEntity != null)
            {
                var fileSource = new FileMediaSource(fileEntity);
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
            return (T)result;
        }

        public T ToEntity<T>(IMediaSource mediaSource) where T : IMediaSourceEntity
        {
            IMediaSourceEntity result = null;
            var fileMediaSource = mediaSource as FileMediaSource;
            if (fileMediaSource != null)
            {
                MediaFileEntity entity = new MediaFileEntity();
                entity.Id = fileMediaSource.Id;
                entity.AudioGain = fileMediaSource.AudioGain;
                entity.FileName = fileMediaSource.FileName;
                entity.Title = fileMediaSource.Title;
                //entity.OriginalFileName=fileMediaSource.
                //entity.MediaFileCategory=fileMediaSource.me
                entity.Duration = fileMediaSource.Duration.Value.TotalSeconds;
                //entity.MarkerIn=fileMediaSource.m
                result = entity;
                return (T)result;
            }
            else
            {
                var channelSource = mediaSource as ChannelMediaSource;
                if (channelSource != null)
                {
                    var entity = new ChannelInfo();
                    entity.Id = channelSource.Id;
                    entity.Title = channelSource.Title;
                    //entity.Special
                    result = entity;
                    return (T)result;
                }
                else
                {
                    throw new ArgumentException();
                }
            }            
        }
    }
}
