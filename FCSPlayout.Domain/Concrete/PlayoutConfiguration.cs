using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlayoutConfiguration : IPlayoutConfiguration
    {
        private static IPlayoutConfiguration _current=new PlayoutConfiguration();
        private static TimeSpan _defaultDuration=TimeSpan.FromHours(1);

        public static IPlayoutConfiguration Current
        {
            get
            {
                return _current;
            }

            set
            {
                _current = value;
            }
        }

        public PlayoutConfiguration()
        {
            this.MinPlayDuration = TimeSpan.FromSeconds(5);
            this.AutoPaddingMediaSource = new AutoPaddingMediaSource();
        }

        public virtual IMediaSource AutoPaddingMediaSource
        {
            get; set;
        }

        public TimeSpan DefaultDuration
        {
            get
            {
                return _defaultDuration;
            }
            set
            {
                _defaultDuration = value;
            }
        }

        public virtual TimeSpan MinPlayDuration
        {
            get; set;
        }
    }
}
