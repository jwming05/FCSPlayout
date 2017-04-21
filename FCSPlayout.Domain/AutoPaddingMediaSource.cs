using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    class AutoPaddingMediaSource : IMediaSource
    {
        private static readonly AutoPaddingMediaSource _instance =new AutoPaddingMediaSource();
        private AutoPaddingMediaSource()
        {
            this.Id = Guid.NewGuid();
        }

        internal static AutoPaddingMediaSource Instance
        {
            get
            {
                return _instance;
            }
        }

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
            get; private set;
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

        //public IMediaSource Clone()
        //{
        //    var source = this;
        //    return new AutoPaddingMediaSource() { Id = source.Id };
        //}

        public bool Equals(IMediaSource other)
        {
            var temp= other as AutoPaddingMediaSource;
            return temp != null && temp.Id == this.Id;
        }
    }
}
