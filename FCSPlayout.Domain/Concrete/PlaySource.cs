using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    internal class PlaySource : IPlaySource
    {
        internal static PlaySource CreateAutoPadding(TimeSpan duration)
        {
            return new PlaySource(PlayoutConfiguration.Current.AutoPaddingMediaSource, new Domain.PlayRange(duration));
        }

        public PlaySource(IMediaSource mediaSource)
        {
            if (mediaSource.Duration != null)
            {
                PlayoutUtils.ValidatePlayDuration(mediaSource.Duration.Value);
            }

            this.MediaSource = mediaSource;
        }

        public PlaySource(IMediaSource mediaSource,PlayRange playRange)
            :this(mediaSource)
        {
            PlayoutUtils.ValidatePlayDuration(playRange.Duration);
            this.PlayRange = playRange;
        }

        public IMediaSource MediaSource
        {
            get;private set;
        }

        public PlayRange? PlayRange
        {
            get;private set;
        }
    }
}
