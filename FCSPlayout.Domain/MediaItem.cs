using System;

namespace FCSPlayout.Domain
{
    public struct MediaItem
    {
        public MediaItem(IMediaSource source)
        {
            this.Source = source;
            this.PlayRange = source.Duration==null ? 
                new PlayRange(PlayoutConfiguration.Current.DefaultDuration) : new PlayRange(source.Duration.Value);
        }

        public MediaItem(IMediaSource source, PlayRange playRange)
        {
            source.ValidatePlayRange(playRange);
            this.Source = source;
            this.PlayRange = playRange;
        }

        public IMediaSource Source { get; private set; }
        public PlayRange PlayRange { get; private set; }

        public TimeSpan Duration
        {
            get { return this.PlayRange.Duration; }
        }

        //public static MediaItem ChangePlayRange(MediaItem mediaItem, PlayRange newPlayRange)
        //{
        //    if (mediaItem.PlayRange != newPlayRange)
        //    {
        //        return new MediaItem(mediaItem.Source, newPlayRange);
        //    }

        //    return mediaItem;
        //}
    }
}
