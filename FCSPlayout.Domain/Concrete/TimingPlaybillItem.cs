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
                return this.GetPlayRange();
            }
        }

        TimeSpan IPlayItem.PlayDuration
        {
            get
            {
                return base.PlaySource.GetDuration();
            }
        }

        DateTime IPlayItem.StartTime
        {
            get
            {
                return base.StartTime.Value;
            }
        }

        public long EditId
        {
            get;set;
        }

        IPlayItem IPlayItem.Clone()
        {
            return (IPlayItem)this.Clone();
        }

        protected override PlaybillItem Clone()
        {
            var result= new TimingPlaybillItem(this.PlaySource.Clone(),
                this.StartTime.Value, this.ScheduleMode == PlayScheduleMode.TimingBreak);
            result.Id = this.Id;
            return result;
        }
    }
}
