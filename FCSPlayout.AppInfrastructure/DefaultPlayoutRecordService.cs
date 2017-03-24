using FCSPlayout.Entities;
using FCSPlayout.PlayEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.AppInfrastructure
{
    public class DefaultPlayoutRecordService : PlayoutRecordService
    {
        private static readonly DefaultPlayoutRecordService _instance = new DefaultPlayoutRecordService();

        public static DefaultPlayoutRecordService Instance
        {
            get
            {
                return _instance;
            }
        }

        private DefaultPlayoutRecordService()
        {

        }
        public override void Add(IPlayerItem playerItem)
        {
            var record = new PlayRecord();
            record.PlayItemId = playerItem.PlayItem.Id;
            record.PlaybillItemId = playerItem.PlayItem.PlaybillItem.Id;
            record.ScheduleMode = playerItem.PlayItem.PlaybillItem.ScheduleMode;

            record.SourceId = playerItem.PlayItem.PlaybillItem.MediaSource.Id;
            record.SourceTitle = playerItem.PlayItem.PlaybillItem.MediaSource.Title;
            var sourceDuration = playerItem.PlayItem.PlaybillItem.MediaSource.Duration;
            if (sourceDuration != null)
            {
                record.SourceDuration = sourceDuration.Value.TotalSeconds;
            }

            record.SourceCategory = playerItem.PlayItem.PlaybillItem.MediaSource.Category;


            record.ActualStartTime = playerItem.StartTime;
            //var loadTime = this.LoadTime;
            record.ActualStopTime = playerItem.StopTime;

            record.PlayItemStartTime = playerItem.PlayItem.StartTime;
            record.LoadMarkerIn = playerItem.LoadRange.StartPosition.TotalSeconds;
            record.LoadMarkerDuration = playerItem.LoadRange.Duration.TotalSeconds;

            record.PlayItemMarkerIn = playerItem.PlayItem.PlayRange.StartPosition.TotalSeconds;
            record.PlayItemDuration = playerItem.PlayItem.CalculatedPlayDuration.TotalSeconds;

            PlayoutRepository.AddPlayRecord(record);
        }
    }
}
