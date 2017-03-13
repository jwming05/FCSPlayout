using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class PlayoutConfiguration : IPlayoutConfiguration
    {
        private static IPlayoutConfiguration _current;

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

        public virtual IMediaSource AutoPaddingMediaSource
        {
            get;protected set;
        }

        public virtual TimeSpan MinPlayDuration
        {
            get; protected set;
        }
    }
}
