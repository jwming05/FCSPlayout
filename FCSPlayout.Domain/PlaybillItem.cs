using System;
using FCSPlayout.CG;

namespace FCSPlayout.Domain
{
    [Serializable]
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
            get; private set;
        }

        public DateTime? StartTime
        {
            get;protected set;
        }

        //IPlaybillItem IPlaybillItem.Clone()
        //{
        //    return this.Clone();
        //}

        //protected abstract PlaybillItem Clone();

        public Guid Id { get; set; }

        public IMediaSource MediaSource
        {
            get
            {
                return this.PlaySource.MediaSource;
            }
        }

        public PlayRange PlayRange
        {
            get
            {
                return this.PlaySource.PlayRange;
            }
        }

        public virtual string Title
        {
            get
            {
                return this.PlaySource.Title;
            }
        }

        public CGItemCollection CGItems
        {
            get
            {
                return this.PlaySource.CGItems;
            }
            set
            {
                this.PlaySource.CGItems = value;
            }
        }

        public static PlaybillItem Timing(MediaItem mediaItem, DateTime startTime)
        {
            return new TimingPlaybillItem(new PlaySource(mediaItem.Source,mediaItem.PlayRange), startTime, false);
        }

        public static PlaybillItem Timing(IPlaySource playSource, DateTime startTime)
        {
            return new TimingPlaybillItem(playSource, startTime, false);
        }

        public static PlaybillItem Auto(MediaItem mediaItem)
        {
            return new AutoPlaybillItem(new PlaySource(mediaItem.Source,mediaItem.PlayRange));
        }

        public static PlaybillItem Auto(IPlaySource playSource)
        {
            return new AutoPlaybillItem(playSource);
        }

        public static PlaybillItem TimingBreak(MediaItem mediaItem, DateTime startTime)
        {
            return new TimingPlaybillItem(new PlaySource(mediaItem.Source,mediaItem.PlayRange), startTime, true);
        }

        public static PlaybillItem TimingBreak(IPlaySource playSource, DateTime startTime)
        {
            return new TimingPlaybillItem(playSource, startTime, true);
        }

        public abstract IPlaybillItem Clone(PlayRange newRange);

        public virtual bool CanMerge(IPlaybillItem playbillItem)
        {
            throw new NotSupportedException();
        }

        public virtual IPlaybillItem Merge(IPlaybillItem playbillItem)
        {
            throw new NotSupportedException();
        }
    }
}
