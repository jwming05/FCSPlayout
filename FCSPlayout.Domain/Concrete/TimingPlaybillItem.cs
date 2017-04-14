using System;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class TimingPlaybillItem : PlaybillItem, IPlayItem
    {
        public TimingPlaybillItem(IPlaySource playSource, DateTime startTime, bool isBreak)
            :base(playSource,isBreak ? PlayScheduleMode.TimingBreak : PlayScheduleMode.Timing)
        {
            
            base.StartTime = startTime;
        }

        

        IPlaybillItem IPlayItem.PlaybillItem
        {
            get
            {
                return this;
            }
        }

        PlayRange IPlayItem.PlayRange
        {
            get
            {
                return base.PlayRange;
            }
        }

        TimeSpan IPlayItem.CalculatedPlayDuration
        {
            get
            {
                return this.PlayRange.Duration; //.GetDuration();
            }
            set
            {
                throw new InvalidOperationException("不能修改定时播或定时插播的播放时长。");
            }
        }

        DateTime IPlayItem.StartTime
        {
            get
            {
                return base.StartTime.Value;
            }
            set
            {
                throw new InvalidOperationException("不能修改定时播或定时插播的开始时间。");
            }
        }

        public void Split(TimeSpan duration, out IPlayItem first, out IPlayItem second)
        {
            throw new InvalidOperationException("不能对定时播或定时插播进行分割。");
        }

        //[NonSerialized]
        //private long? _editId;
        //public long? EditId
        //{
        //    get { return _editId; }
        //    set { _editId = value; }
        //}

        [NonSerialized]
        private IPlaylistEditor _editor;
        public IPlaylistEditor Editor
        {
            get { return _editor; }
            set { _editor = value; }
        }

        public PlayRange CalculatedPlayRange
        {
            get
            {
                return this.PlayRange;
            }
        }

        public DateTime CalculatedStopTime
        {
            get
            {
                return this.StartTime.Value.Add(this.PlayRange.Duration);
            }
        }

        //IPlayItem IPlayItem.Clone()
        //{
        //    return (IPlayItem)this.Clone();
        //}

        //protected override PlaybillItem Clone()
        //{
        //    var result= new TimingPlaybillItem(this.PlaySource.Clone(),
        //        this.StartTime.Value, this.ScheduleMode == PlayScheduleMode.TimingBreak);
        //    result.Id = this.Id;
        //    return result;
        //}

        public override IPlaybillItem Clone(PlayRange newRange)
        {
            throw new NotImplementedException();
        }
    }
}
