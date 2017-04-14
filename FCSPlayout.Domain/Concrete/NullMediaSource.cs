using System;

namespace FCSPlayout.Domain
{
    public class NullMediaSource : IMediaSource
    {
        public NullMediaSource(string title, TimeSpan duration)
        {
            this.Title = title;
            this.Duration = duration;
            this.Id = Guid.NewGuid();
        }

        public MediaSourceCategory Category
        {
            get
            {
                return MediaSourceCategory.Null;
            }
        }

        public TimeSpan? Duration
        {
            get;private set;
        }

        public Guid Id
        {
            get; private set;
        }

        public string Title
        {
            get;private set;
        }

        public PlayRange? Adjust(PlayRange playRange)
        {
            throw new NotSupportedException();
        }

        public bool Equals(IMediaSource other)
        {
            var temp = other as NullMediaSource;
            return temp != null && temp.Id == this.Id;
        }

        //public IMediaSource Clone()
        //{
        //    var source = this;
        //    return new NullMediaSource(source.Title,source.Duration.Value)
        //    {
        //        Id = source.Id,
        //    };
        //}


    }
}
