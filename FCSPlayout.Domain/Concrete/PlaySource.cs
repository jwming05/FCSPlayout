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
            get; private set;
        }

        public IPlaySource Clone()
        {
            var result = new PlaySource(this.MediaSource.Clone(), this.PlayRange);
            if (this.CGItems != null)
            {
                result.CGItems = this.CGItems.Clone();
            }

            return result;
        }
    }
}
