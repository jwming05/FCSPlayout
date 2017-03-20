using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;

namespace FCSPlayout.AppInfrastructure
{
    [Serializable]
    public abstract class MediaSourceBase : IMediaSource
    {
        internal static MediaSourceBase Create(MediaSourceEntity entity)
        {
            var file = entity as MediaFileEntity;
            if (file != null)
            {
                return new FileMediaSource(file);
            }

            //new ChannelMediaSource()
            throw new ArgumentException();
        }



        protected MediaSourceBase(MediaSourceCategory category)
        {
            this.Category = category;
        }

        protected MediaSourceBase(MediaSourceCategory category, MediaSourceEntity entity)
            :this(category)
        {
            //this.Category = category;

            this.Title = entity.Title;
            var fileEntity = entity as MediaFileEntity;
            if (fileEntity != null)
            {
                this.Duration = TimeSpan.FromSeconds(fileEntity.Duration);
            }
            
            this.Entity = entity;
        }

        public string Title
        {
            get; set;
        }

        public virtual TimeSpan? Duration
        {
            get; protected set;
        }

        public MediaSourceEntity Entity
        {
            get; protected set;
        }

        public MediaSourceCategory Category
        {
            get; private set;
        }

        public Guid Id
        {
            get;set;
        }

        public virtual PlayRange? Adjust(PlayRange playRange)
        {
            return playRange;
        }

        public abstract IMediaSource Clone();
    }
}
