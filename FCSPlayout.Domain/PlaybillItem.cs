using System;

namespace FCSPlayout.Domain
{
    public abstract class PlaybillItem : IPlaybillItem
    {
        protected PlaybillItem(IPlaySource playSource, PlayScheduleMode scheduleMode)
        {
            this.Id = Guid.NewGuid();
            this.PlaySource = playSource;
            this.ScheduleMode = scheduleMode;
        }

        public PlayScheduleMode ScheduleMode
        {
            get;private set;
        }

        public IPlaySource PlaySource
        {
            get;private set;
        }

        public DateTime? StartTime
        {
            get;protected set;
        }

        public Guid Id { get; set; }
        public static PlaybillItem Timing(MediaItem mediaItem, DateTime startTime)
        {
            return new TimingPlaybillItem(new PlaySource(mediaItem), startTime, false);
        }

        public static PlaybillItem Timing(IPlaySource playSource, DateTime startTime)
        {
            return new TimingPlaybillItem(playSource, startTime, false);
        }

        public static PlaybillItem Auto(MediaItem mediaItem)
        {
            return new AutoPlaybillItem(new PlaySource(mediaItem));
        }

        public static PlaybillItem Auto(IPlaySource playSource)
        {
            return new AutoPlaybillItem(playSource);
        }

        public static PlaybillItem TimingBreak(MediaItem mediaItem, DateTime startTime)
        {
            return new TimingPlaybillItem(new PlaySource(mediaItem), startTime, true);
        }

        public static PlaybillItem TimingBreak(IPlaySource playSource, DateTime startTime)
        {
            return new TimingPlaybillItem(playSource, startTime, true);
        }
    }
}
