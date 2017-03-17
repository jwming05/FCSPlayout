using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FCSPlayout.CG;

namespace FCSPlayout.Domain
{
    public class PlaySource : IPlaySource
    {
        internal static PlaySource CreateAutoPadding(TimeSpan duration)
        {
            var mediaItem = new MediaItem(PlayoutConfiguration.Current.AutoPaddingMediaSource, new Domain.PlayRange(duration));
            return new PlaySource(mediaItem);
        }

        
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
            get;private set;
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
    }
}
