using FCSPlayout.Domain;
using FCSPlayout.Entities;
using System;

namespace FCSPlayout.AppInfrastructure
{
    [Serializable]
    public class FileMediaSource : MediaSourceBase, IFileMediaSource
    {
        public FileMediaSource(MediaFileEntity entity) : base(MediaSourceCategory.File, entity)
        {
            this.FileName = MediaFilePathResolver.Current.Resolve(entity.FileName);
            this.Id = entity.Id;

            if (string.IsNullOrEmpty(this.Title))
            {
                this.Title = System.IO.Path.GetFileNameWithoutExtension(this.FileName);
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

        //public ENUM_AspectRatio AspectRatio
        //{
        //    get { return this.FileEntity.AspectRatio; }
        //    set
        //    {
        //        if (this.FileEntity.AspectRatio != value)
        //        {
        //            this.FileEntity.AspectRatio = value;
        //        }
        //    }
        //}

        //public MPFieldsType MPFieldsType
        //{
        //    get { return this.FileEntity.MPFieldsType; }
        //    set
        //    {
        //        if (this.FileEntity.MPFieldsType != value)
        //        {
        //            this.FileEntity.MPFieldsType = value;
        //        }

        //    }
        //}

        //public ENUM_StretchMode StretchMode
        //{
        //    get { return this.FileEntity.StretchMode; }
        //    set
        //    {
        //        if (this.FileEntity.StretchMode != value)
        //        {
        //            this.FileEntity.StretchMode = value;
        //        }

        //    }
        //}

        private MediaFileEntity FileEntity { get { return (MediaFileEntity)base.Entity; } }

        public override PlayRange? Adjust(PlayRange playRange)
        {
            if(playRange.StartPosition==TimeSpan.Zero && playRange.Duration == this.Duration.Value)
            {
                return null;
            }
            return playRange;
        }

        public override IMediaSource Clone()
        {
            var result = new FileMediaSource(this.FileEntity);
            result.AudioGain = this.AudioGain;
            return result;
        }
    }
}