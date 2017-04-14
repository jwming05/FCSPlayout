using FCSPlayout.CG;
using System;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class PlaySource : IPlaySource
    {
        internal static PlaySource CreateAutoPadding(TimeSpan duration)
        {
            return new PlaySource(PlayoutConfiguration.Current.AutoPaddingMediaSource, new Domain.PlayRange(duration));
        }

        private PlayRange _playRange;

        public PlaySource(IMediaSource mediaSource)
        {
            if (mediaSource.Duration != null)
            {
                PlayoutUtils.ValidatePlayDuration(mediaSource.Duration.Value);
            }
            this.MediaSource = mediaSource;
            TimeSpan duration = mediaSource.Duration == null ? PlayoutConfiguration.Current.DefaultDuration : mediaSource.Duration.Value;
            this.PlayRange = new PlayRange(duration);
        }

        public PlaySource(IMediaSource mediaSource,PlayRange playRange)
        {
            if (mediaSource.Duration != null)
            {
                PlayoutUtils.ValidatePlayDuration(playRange.Duration);
            }
                
            //mediaSource.ValidatePlayRange(playRange);
            this.MediaSource = mediaSource;
            this.PlayRange = playRange;
        }

        public PlaySource(IMediaSource mediaSource, CGItemCollection cgItems)
            :this(mediaSource)
        {
            if (cgItems != null)
            {
                this.CGItems = cgItems.Clone();
            }
        }

        public PlaySource(IMediaSource mediaSource, PlayRange playRange, CGItemCollection cgItems)
            : this(mediaSource, playRange)
        {
            if (cgItems != null)
            {
                this.CGItems = cgItems.Clone();
            }
        }

        public IMediaSource MediaSource
        {
            get;private set;
        }

        public PlayRange PlayRange
        {
            get { return _playRange; }
            private set
            {
                if (this.MediaSource != null)
                {
                    this.MediaSource.ValidatePlayRange(value);
                }
                
                _playRange = value;
            }
        }

        public string Title
        {
            get
            {
                return this.MediaSource.Title;
            }
        }

        public CGItemCollection CGItems
        {
            get; set;
        }

        public IPlaySource Clone()
        {
            var result = new PlaySource(this.MediaSource/*.Clone()*/, this.PlayRange);
            if (this.CGItems != null)
            {
                result.CGItems = this.CGItems.Clone();
            }

            return result;
        }

        public IPlaySource Clone(PlayRange newRange)
        {
            var result = new PlaySource(this.MediaSource/*.Clone()*/, newRange);
            if (this.CGItems != null)
            {
                result.CGItems = this.CGItems.Clone();
            }

            return result;
        }

        public bool CanMerge(IPlaySource playSource)
        {
            if (!this.MediaSource.Equals(playSource.MediaSource))
            {
                return false;
            }

            if (!Equals(this.CGItems, playSource.CGItems))
            {
                return false;
            }

            return FCSPlayout.Domain.PlayRange.CanMerge(this.PlayRange, playSource.PlayRange);
        }

        public IPlaySource Merge(IPlaySource playSource)
        {
            if (!CanMerge(playSource))
            {
                throw new InvalidOperationException();
            }

            var range = FCSPlayout.Domain.PlayRange.Merge(this.PlayRange, playSource.PlayRange);

            return this.Clone(range);
        }

        private bool Equals(CGItemCollection item1,CGItemCollection item2)
        {
            if (item1 == null || item1.Count==0)
            {
                return item2 == null || item2.Count == 0;
            }

            return false;
        }
    }
}
