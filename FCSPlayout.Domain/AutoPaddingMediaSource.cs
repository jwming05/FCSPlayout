using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    class AutoPaddingMediaSource : IMediaSource
    {
        public MediaSourceCategory Category
        {
            get
            {
                return MediaSourceCategory.External;
            }
        }

        public TimeSpan? Duration
        {
            get
            {
                return null;
            }
        }

        public Guid Id
        {
            get;set;
        }

        public string Title
        {
            get
            {
                return "自动垫片";
            }
        }

        public PlayRange? Adjust(PlayRange playRange)
        {
            return new PlayRange(playRange.Duration);
        }

        public IMediaSource Clone()
        {
            var source = this;
            return new AutoPaddingMediaSource() { Id = source.Id };
        }
    }
}
