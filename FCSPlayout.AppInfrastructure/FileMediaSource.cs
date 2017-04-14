using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;

namespace FCSPlayout.AppInfrastructure
{
    [Serializable]
    public class FileMediaSource : MediaSourceBase, IFileMediaSource
    {
        public FileMediaSource(MediaFileEntity entity) 
            : base(MediaSourceCategory.File, entity)
        {
            this.FileName = entity.FileName; // resolvedFileName; // MediaFilePathResolver.Current.Resolve(entity.FileName);
            //this.Id = entity.Id;

            if (string.IsNullOrEmpty(this.Title))
            {
                this.Title = System.IO.Path.GetFileNameWithoutExtension(/*this.FileName*/entity.OriginalFileName);
            }
        }

        public string FileName
        {
            get; private set;
        }


        public int AudioGain
        {
            get { return this.FileEntity.AudioGain; }
            set
            {
                if (this.FileEntity.AudioGain != value)
                {
                    this.FileEntity.AudioGain = value;
                }

            }
        }

        private MediaFileEntity FileEntity { get { return (MediaFileEntity)base.Entity; } }

        public override PlayRange? Adjust(PlayRange playRange)
        {
            if(playRange.StartPosition==TimeSpan.Zero && playRange.Duration == this.Duration.Value)
            {
                return null;
            }
            return playRange;
        }

        //public override IMediaSource Clone()
        //{
        //    var result = new FileMediaSource(this.FileEntity);
        //    result.AudioGain = this.AudioGain;
        //    return result;
        //}

        public override bool Equals(IMediaSource other)
        {
            FileMediaSource temp = other as FileMediaSource;
            return temp != null && temp.Id == this.Id;
        }
    }
}