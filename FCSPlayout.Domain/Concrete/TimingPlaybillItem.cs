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
                return base.PlayRange.Duration; //.GetDuration();
            }
        }

        DateTime IPlayItem.StartTime
        {
            get
            {
                return base.StartTime.Value;
            }
        }

        [NonSerialized]
        private long? _editId;
        public long? EditId
        {
            get { return _editId; }
            set { _editId = value; }
        }

        public PlayRange CalculatedPlayRange
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime CalculatedStopTime
        {
            get
            {
                throw new NotImplementedException();
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
    }
}
