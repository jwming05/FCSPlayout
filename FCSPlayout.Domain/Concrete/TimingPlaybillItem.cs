using System;

namespace FCSPlayout.Domain
{
    public class TimingPlaybillItem : PlaybillItem, IPlayItem
    {
        public TimingPlaybillItem(IPlaySource playSource, DateTime startTime, bool isBreak)
            :base(playSource,isBreak ? PlaybillItemCategory.TimingBreak : PlaybillItemCategory.Timing)
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
    }
}
