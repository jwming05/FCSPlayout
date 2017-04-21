using FCSPlayout.CG;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FCSPlayout.AppInfrastructure
{
    public class Playbill
    {
        private PlaybillEntity _entity;

        public Playbill()
        {
        }

        private Playbill(PlaybillEntity billEntity)
        {
            _entity = billEntity;
        }

        public IPlayItemCollection PlayItemCollection { get; set; }

        public void Save(IMediaSource replaceAutoPadding,IUser currentUser)
        {
            ValidatePlayItems();
            if (_entity == null)
            {
                _entity = new PlaybillEntity();
            }

            var playItemEntities = ToEntities(_entity, this.PlayItemCollection, replaceAutoPadding);

            _entity.StartTime = playItemEntities[0].StartTime;
            _entity.Duration = playItemEntities[playItemEntities.Count - 1].StopTime.Subtract(_entity.StartTime).TotalSeconds;

            var oldId = _entity.Id;
            var oldEditorId = _entity.LastEditorId;

            try
            {
                PlayoutRepository.SavePlaybill(_entity, playItemEntities,currentUser);
            }
            catch(Exception ex)
            {
                _entity.Id = oldId;
                _entity.LastEditorId = oldEditorId;
                throw;
            }
        }

        public static IEnumerable<PlaybillEntity> LoadPlaybills(DateTime minStopTime)
        {
            return PlayoutRepository.LoadPlaybills(minStopTime);

            //List<PlaybillEntity> result = new List<PlaybillEntity>();
            //using (var ctx = new PlayoutDbContext())
            //{
            //    var temp = ctx.Playbills.ToList();
            //    foreach (var item in temp)
            //    {
            //        result.Add(new BindablePlaybill(item));
            //    }
            //}
            //return result;
        }

        private void ValidatePlayItems()
        {
            
        }

        private IList<PlayItemEntity> ToEntities(PlaybillEntity billEntity, IPlayItemCollection playItemCollection, IMediaSource replaceAutoPadding)
        {
            List<PlayItemEntity> result = new List<PlayItemEntity>();

            for(int i = 0; i < playItemCollection.Count; i++)
            {
                result.Add(playItemCollection[i].ToEntity(billEntity, replaceAutoPadding));
                //result.Add(ToEntity(billEntity, playItemCollection[i]));
            }

            return result;
        }

        //public PlayItemEntity ToEntity(PlaybillEntity billEntity, IPlayItem playItem)
        //{
        //    var dtoItem = new PlayItemEntity();
        //    dtoItem.Id = playItem.Id;

        //    dtoItem.StartTime = playItem.StartTime;
        //    dtoItem.PlayDuration = playItem.CalculatedPlayDuration.TotalSeconds;
        //    dtoItem.MarkerIn = playItem.PlayRange.StartPosition.TotalSeconds;
        //    dtoItem.MarkerDuration = playItem.PlayRange.Duration.TotalSeconds;

        //    dtoItem.PlaybillItem = ToEntity(billEntity, playItem.PlaybillItem);
        //    return dtoItem;
        //}

        //public PlaybillItemEntity ToEntity(PlaybillEntity billEntity, IPlaybillItem billItem)
        //{
        //    var dtoItem = new PlaybillItemEntity();
        //    dtoItem.Id = billItem.Id;

        //    dtoItem.Playbill = billEntity;

        //    dtoItem.StartTime = billItem.StartTime;
        //    dtoItem.MarkerIn = billItem.PlayRange.StartPosition.TotalSeconds;
        //    dtoItem.ScheduleMode = billItem.ScheduleMode;
        //    dtoItem.MarkerDuration = billItem.PlayRange.Duration.TotalSeconds;

        //    if (billItem.MediaSource.Category!=MediaSourceCategory.Null)
        //    {
        //        dtoItem.MediaSourceId = billItem.MediaSource.Id;
        //    }
        //    else
        //    {
        //        dtoItem.MediaSourceTitle = billItem.MediaSource.Title;
        //        dtoItem.MediaSourceDuration = billItem.MediaSource.Duration.Value.TotalSeconds;
        //    }

        //    var autoBillItem = billItem as AutoPlaybillItem;
        //    if (autoBillItem != null)
        //    {
        //        dtoItem.IsAutoPadding = autoBillItem.IsAutoPadding;
        //    }
            

        //    var fileMediaSource = billItem.MediaSource as IFileMediaSource;
        //    if (fileMediaSource != null)
        //    {
        //        dtoItem.AudioGain = fileMediaSource.AudioGain;
        //    }

        //    if (billItem.CGItems != null)
        //    {
        //        dtoItem.CGContents = CGItemCollection.ToXml(billItem.CGItems);
        //    }
            
        //    return dtoItem;
        //}

        public static Playbill Load(Guid id, IList<IPlayItem> playItemList)
        {
            PlaybillEntity billEntity = PlayoutRepository.GetPlaybill(id); // GetEntity(id);
            Playbill playbill = null;
            Dictionary<Guid,IPlaybillItem> playbillItemDict = new Dictionary<Guid, IPlaybillItem>();
            if (billEntity != null)
            {
                playbill= new Playbill(billEntity);
                for(int i = 0; i < billEntity.PlayItems.Count; i++)
                {
                    PlayItemEntity playItemEntity = billEntity.PlayItems[i];
                    IPlaybillItem playbillItem = null;
                    if (!playbillItemDict.ContainsKey(playItemEntity.PlaybillItemId))
                    {
                        playbillItem = playItemEntity.PlaybillItem.ToPlaybillItem(); 
                        // FromEntity(playItemEntity.PlaybillItem);
                        playbillItemDict.Add(playItemEntity.PlaybillItemId, playbillItem);
                    }
                    else
                    {
                        playbillItem = playbillItemDict[playItemEntity.PlaybillItemId];
                    }

                    IPlayItem playItem = playItemEntity.ToPlayItem(playbillItem); // FromEntity(playItemEntity,playbillItem);
                    playItemList.Add(playItem);
                }
            }
            return playbill;
        }

        //private static IPlayItem FromEntity(PlayItemEntity entity, IPlaybillItem playbillItem)
        //{
        //    if (playbillItem.ScheduleMode == PlayScheduleMode.Auto)
        //    {
        //        var autoPlayItem = new AutoPlayItem(playbillItem);
        //        autoPlayItem.StartTime = entity.StartTime;
        //        autoPlayItem.CalculatedPlayDuration = TimeSpan.FromSeconds(entity.PlayDuration);
        //        autoPlayItem.PlayRange = new PlayRange(TimeSpan.FromSeconds(entity.MarkerIn), TimeSpan.FromSeconds(entity.MarkerDuration));
        //        //autoPlayItem.PlaybillItem = playbillItem;
        //        autoPlayItem.Id = entity.Id;
        //        return autoPlayItem;
        //    }
        //    else
        //    {
        //        return (TimingPlaybillItem)playbillItem;
        //    }
        //    //throw new NotImplementedException();
        //}

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
        //        mediaSource = FromEntity(entity, entity.MediaSource);
        //    }

        //    IPlaySource playSource = Create(entity, mediaSource);
        //    IPlaybillItem result = null;
        //    switch (entity.ScheduleMode)
        //    {
        //        case PlayScheduleMode.Auto:
        //            result= PlaybillItem.Auto(playSource);
        //            break;
        //        case PlayScheduleMode.Timing:
        //            result= PlaybillItem.Timing(playSource, entity.StartTime.Value);
        //            break;
        //        case PlayScheduleMode.TimingBreak:
        //            result= PlaybillItem.TimingBreak(playSource, entity.StartTime.Value);
        //            break;
        //    }
        //    result.Id = entity.Id;
        //    return result;
        //}

        //private static IPlaySource Create(PlaybillItemEntity entity, IMediaSource mediaSource)
        //{
        //    var range = new PlayRange(TimeSpan.FromSeconds(entity.MarkerIn), TimeSpan.FromSeconds(entity.MarkerDuration));
        //    var mediaItem = new MediaItem(mediaSource, range);

        //    CGItemCollection cgItems = null;
        //    //var playSource = new PlaySource(mediaItem.Source,mediaItem.PlayRange);
            

        //    if (entity.CGContents != null)
        //    {
        //        cgItems/*playSource.CGItems*/ = CGItemCollection.FromXml(entity.CGContents);
        //    }

        //    if (cgItems == null)
        //    {
        //        return new PlaySource(mediaItem.Source, mediaItem.PlayRange);
        //    }
        //    else
        //    {
        //        return new PlaySource(mediaItem.Source, mediaItem.PlayRange, cgItems);
        //    }
        //    //return playSource;
        //}

        //private static IMediaSource FromEntity(PlaybillItemEntity billItemEntity, MediaSourceEntity entity)
        //{
        //    IMediaSource result = null;
        //    MediaFileEntity fileEntity = entity as MediaFileEntity;
        //    if (fileEntity != null)
        //    {
        //        var fileSource= new FileMediaSource(fileEntity);
        //        fileSource.AudioGain = billItemEntity.AudioGain;
        //        result = fileSource;
        //    }
        //    else
        //    {
        //        ChannelInfo ci = entity as ChannelInfo;
        //        if (ci != null)
        //        {
        //            var channelSource= new ChannelMediaSource(ci);
        //            result = channelSource;
        //        }
        //        else
        //        {
        //            throw new ArgumentException();
        //        }
        //    }
        //    return result;
        //}
    }
}
