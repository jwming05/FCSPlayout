using FCSPlayout.CG;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;

namespace FCSPlayout.AppInfrastructure
{
    public static class PlayItemExtensions
    {
        public static PlayItemEntity ToEntity(this IPlayItem playItem)
        {
            var entity = new PlayItemEntity();
            entity.Id = playItem.Id;

            entity.StartTime = playItem.StartTime;
            entity.PlayDuration = playItem.CalculatedPlayDuration.TotalSeconds;
            entity.MarkerIn = playItem.PlayRange.StartPosition.TotalSeconds;
            entity.MarkerDuration = playItem.PlayRange.Duration.TotalSeconds;

            entity.PlaybillItemId = playItem.PlaybillItem.Id;
            entity.PlaybillItem = new PlaybillItemEntity(playItem.PlaybillItem, (i)=>i.ToEntity());

            return entity;
        }

        public static PlayItemEntity ToEntity(this IPlayItem playItem, PlaybillEntity billEntity, IMediaSource replaceAutoPadding)
        {
            var dtoItem = new PlayItemEntity();
            dtoItem.Id = playItem.Id;

            dtoItem.StartTime = playItem.StartTime;
            dtoItem.PlayDuration = playItem.CalculatedPlayDuration.TotalSeconds;
            dtoItem.MarkerIn = playItem.PlayRange.StartPosition.TotalSeconds;
            dtoItem.MarkerDuration = playItem.PlayRange.Duration.TotalSeconds;

            dtoItem.PlaybillItem = ToEntity(billEntity, playItem.PlaybillItem, replaceAutoPadding); //.ToEntity();
            return dtoItem;
        }

        public static IPlayItem ToPlayItem(this PlayItemEntity entity)
        {
            IPlaybillItem playbillItem = entity.PlaybillItem.ToPlaybillItem(); // FromEntity(entity.PlaybillItem);

            if (playbillItem.ScheduleMode == PlayScheduleMode.Auto)
            {
                var autoPlayItem = new AutoPlayItem(playbillItem);
                autoPlayItem.StartTime = entity.StartTime;
                autoPlayItem.CalculatedPlayDuration = TimeSpan.FromSeconds(entity.PlayDuration);
                autoPlayItem.PlayRange = new PlayRange(TimeSpan.FromSeconds(entity.MarkerIn), TimeSpan.FromSeconds(entity.MarkerDuration));
                autoPlayItem.Id = entity.Id;
                return autoPlayItem;
            }
            else
            {
                return (TimingPlaybillItem)playbillItem;
            }
        }

        public static IPlayItem ToPlayItem(this PlayItemEntity entity, IPlaybillItem playbillItem)
        {
            if (playbillItem.ScheduleMode == PlayScheduleMode.Auto)
            {
                var autoPlayItem = new AutoPlayItem(playbillItem);
                autoPlayItem.StartTime = entity.StartTime;
                autoPlayItem.CalculatedPlayDuration = TimeSpan.FromSeconds(entity.PlayDuration);
                autoPlayItem.PlayRange = new PlayRange(TimeSpan.FromSeconds(entity.MarkerIn), TimeSpan.FromSeconds(entity.MarkerDuration));
                //autoPlayItem.PlaybillItem = playbillItem;
                autoPlayItem.Id = entity.Id;
                return autoPlayItem;
            }
            else
            {
                return (TimingPlaybillItem)playbillItem;
            }
            //throw new NotImplementedException();
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

        public static PlaybillItemEntity ToEntity(PlaybillEntity billEntity, IPlaybillItem billItem, IMediaSource replaceAutoPadding)
        {
            var dtoItem = new PlaybillItemEntity();
            dtoItem.Id = billItem.Id;

            dtoItem.Playbill = billEntity;

            dtoItem.StartTime = billItem.StartTime;
            dtoItem.MarkerIn = billItem.PlayRange.StartPosition.TotalSeconds;
            dtoItem.ScheduleMode = billItem.ScheduleMode;
            dtoItem.MarkerDuration = billItem.PlayRange.Duration.TotalSeconds;
            //_dtoItem.SegmentId = this.SegmentId;

            if (billItem.MediaSource.Category != MediaSourceCategory.Null)
            {
                dtoItem.MediaSourceId = billItem.MediaSource.Id;
            }
            else
            {
                dtoItem.MediaSourceTitle = billItem.MediaSource.Title;
                dtoItem.MediaSourceDuration = billItem.MediaSource.Duration.Value.TotalSeconds;
            }

            var autoBillItem = billItem as AutoPlaybillItem;
            if (autoBillItem != null)
            {
                if (autoBillItem.IsAutoPadding && replaceAutoPadding!=null)
                {
                    // TODO: 替换自动垫片。

                    dtoItem.MediaSourceId = replaceAutoPadding.Id;

                    //dtoItem.IsAutoPadding = autoBillItem.IsAutoPadding;
                }
            }


            var fileMediaSource = billItem.MediaSource as IFileMediaSource;
            if (fileMediaSource != null)
            {
                dtoItem.AudioGain = fileMediaSource.AudioGain;
            }

            if (billItem.CGItems != null)
            {
                dtoItem.CGContents = CGItemCollection.ToXml(billItem.CGItems);
            }

            return dtoItem;
        }

        public static IPlaybillItem ToPlaybillItem(this PlaybillItemEntity entity)
        {

            MediaSourceEntity sourceEntity = entity.MediaSource;
            IMediaSource mediaSource = null;
            if (sourceEntity == null)
            {
                mediaSource = new NullMediaSource(entity.MediaSourceTitle, TimeSpan.FromSeconds(entity.MediaSourceDuration.Value));
            }
            else
            {
                mediaSource = entity.MediaSource.ToMediaSource(entity); // FromEntity(entity, entity.MediaSource);
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

        //private static IPlaybillItem FromEntity(PlaybillItemEntity entity)
        //{
        //    MediaSourceEntity sourceEntity = entity.MediaSource;
        //    IMediaSource mediaSource = null;
        //    if (sourceEntity == null)
        //    {
        //        mediaSource = new NullMediaSource(entity.MediaSourceTitle, TimeSpan.FromSeconds(entity.MediaSourceDuration.Value));
        //    }
        //    else
        //    {
        //        mediaSource = entity.MediaSource.ToMediaSource();
        //    }

        //    IPlaySource playSource = Create(entity, mediaSource);
        //    IPlaybillItem result = null;
        //    switch (entity.ScheduleMode)
        //    {
        //        case PlayScheduleMode.Auto:
        //            result = PlaybillItem.Auto(playSource);
        //            break;
        //        case PlayScheduleMode.Timing:
        //            result = PlaybillItem.Timing(playSource, entity.StartTime.Value);
        //            break;
        //        case PlayScheduleMode.TimingBreak:
        //            result = PlaybillItem.TimingBreak(playSource, entity.StartTime.Value);
        //            break;
        //    }
        //    result.Id = entity.Id;
        //    return result;
        //}
    }
}
