using FCSPlayout.CG;
using System;

namespace FCSPlayout.Domain
{
    [Serializable]
    public class PlaySource : IPlaySource
    {
        internal static PlaySource CreateAutoPadding(TimeSpan duration)
        {
            var mediaItem = new MediaItem(PlayoutConfiguration.Current.AutoPaddingMediaSource, new Domain.PlayRange(duration));
            return new PlaySource(mediaItem);
        }

        private PlayRange _playRange;

        public PlaySource(MediaItem mediaItem)
        {
            if (mediaItem.Source.Duration != null)
            {
                PlayoutUtils.ValidatePlayDuration(mediaItem.Source.Duration.Value);
            }

            //mediaItem.Source.ValidatePlayRange(mediaItem.PlayRange);
            this.MediaSource = mediaItem.Source;
            this.PlayRange = mediaItem.PlayRange;
        }

        public IMediaSource MediaSource
        {
            get;private set;
        }

        public PlayRange PlayRange
        {
            get { return _playRange; }
            set
            {
                this.MediaSource.ValidatePlayRange(value);
                _playRange = value;
            }
        }

        //public IPlayParameters Parameters
        //{
        //    get;set;
        //}

        public string Title
        {
            get
            {
                return this.MediaSource.Title;
            }
        }

        public CGItemCollection CGItems
        {
            get;set;
        }

        public IPlaySource Clone()
        {
            //var source = this;
            var mediaItem = new MediaItem(this.MediaSource.Clone(), this.PlayRange);
            var result = new PlaySource(mediaItem);
            if (this.CGItems != null)
            {
                result.CGItems = this.CGItems.Clone();
            }
            
            return result;
        }
    }
}
