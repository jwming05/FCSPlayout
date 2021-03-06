﻿using FCSPlayout.Domain;
using FCSPlayout.Entities;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPF.Core
{
    public class BindableMediaFileItem : BindableBase,IPlayableItem,IImageItem
    {
        public static IImageSourceDecoder ImageSourceDecoder;
        public static ImageSource PlaceholderImage;

        private ImageSource _image;
        private PlayRange _playRange;

        public event EventHandler PreviewClosing;

        public BindableMediaFileItem(MediaFileEntity entity, string filePath)
        {
            this.Entity = entity;
            this.PlayRange = new PlayRange(TimeSpan.FromSeconds(this.Entity.MarkerIn), TimeSpan.FromSeconds(this.Entity.MarkerDuration));
            this.FilePath = filePath;
        }

        //private BindableMediaFileItem(string filePath, Guid creatorId, TimeSpan duration)
        //{
        //    this.Entity = new MediaFileEntity
        //    {
        //        CreatorId = creatorId,
        //        CreationTime = DateTime.Now,
        //        OriginalFileName = filePath,
        //        FileName= GetNewFileName(filePath),
        //        Title= Path.GetFileNameWithoutExtension(filePath),
        //        Duration=duration.TotalSeconds
        //    };
        //    this.FilePath = filePath;

        //    //this.OriginalFileName = filePath;
        //    //this.FileName = GetNewFileName(filePath);
        //    //this.Title = Path.GetFileNameWithoutExtension(filePath);
        //    //this.Duration = duration;

        //    this.PlayRange = new PlayRange(this.Duration);
        //}

        public BindableMediaFileItem(string filePath, Guid creatorId, TimeSpan duration, byte[] thumbnailBytes)
            //:this(filePath,creatorId,duration)
        {
            this.Entity = new MediaFileEntity
            {
                CreatorId = creatorId,
                CreationTime = DateTime.Now,
                OriginalFileName = filePath,
                FileName = GetNewFileName(filePath),
                Title = Path.GetFileNameWithoutExtension(filePath),
                Duration = duration.TotalSeconds
            };
            this.FilePath = filePath;

            //this.OriginalFileName = filePath;
            //this.FileName = GetNewFileName(filePath);
            //this.Title = Path.GetFileNameWithoutExtension(filePath);
            //this.Duration = duration;

            this.PlayRange = new PlayRange(this.Duration);
            this.Entity.Metadata = new MediaFileMetadata { Icon = thumbnailBytes };
        }

        public void ClosePreview()
        {
            if (PreviewClosing != null)
            {
                PreviewClosing(this, EventArgs.Empty);
            }
        }

        private string GetNewFileName(string filePath)
        {
            return Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(filePath);
        }

        public TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromSeconds(this.Entity.Duration);
            }

            private set
            {
                if (this.Duration != value)
                {
                    this.Entity.Duration = value.TotalSeconds;
                }
            }
        }

        public Guid? CreatorId
        {
            get
            {
                return this.Entity.CreatorId;
            }
        }

        public DateTime CreationTime
        {
            get
            {
                return this.Entity.CreationTime;
            }
        }
        public string Title
        {
            get { return this.Entity.Title; }
            set
            {
                this.Entity.Title = value;
                this.RaisePropertyChanged(nameof(this.Title));
            }
        }


        public TimeSpan StartPosition
        {
            get
            {
                return this.PlayRange.StartPosition;
            }
        }

        public TimeSpan StopPosition
        {
            get
            {
                return this.PlayRange.StopPosition;
            }
        }

        public TimeSpan PlayDuration
        {
            get
            {
                return this.PlayRange.Duration;
            }
        }

        public PlayRange PlayRange
        {
            get
            {
                return _playRange;
            }

            set
            {
                _playRange = value;
                this.Entity.MarkerIn = value.StartPosition.TotalSeconds;
                this.Entity.MarkerDuration = value.Duration.TotalSeconds;

                this.RaisePropertyChanged(nameof(this.PlayRange));
                this.RaisePropertyChanged(nameof(this.StartPosition));
                this.RaisePropertyChanged(nameof(this.StopPosition));
                this.RaisePropertyChanged(nameof(this.PlayDuration));
            }
        }

        public string FileName
        {
            get { return this.Entity.FileName; }
            private set { this.Entity.FileName = value; }
        }

        public string OriginalFileName
        {
            get
            { return this.Entity.OriginalFileName; }
            private set { this.Entity.OriginalFileName = value; }
        }

        public string FilePath
        {
            get; private set;
        }

        public int AudioGain
        {
            get { return this.Entity.AudioGain; }
            set
            {
                if (this.Entity.AudioGain != value)
                {
                    this.Entity.AudioGain = value;
                    this.RaisePropertyChanged(nameof(this.AudioGain));
                }
            }
        }

        public Guid MediaFileCategoryId
        {

            get { return this.Entity.MediaFileCategoryId ?? Guid.Empty; }
            set
            {
                if (this.Entity.MediaFileCategoryId != value)
                {
                    this.Entity.MediaFileCategoryId = value == Guid.Empty ? (Guid?)null : value;
                }
                this.RaisePropertyChanged(nameof(this.MediaFileCategoryId));
            }
        }

        public Guid MediaFileChannelId
        {

            get { return this.Entity.MediaFileChannelId ?? Guid.Empty; }
            set
            {
                if (this.Entity.MediaFileChannelId != value)
                {
                    this.Entity.MediaFileChannelId = value == Guid.Empty ? (Guid?)null : value;
                }
                this.RaisePropertyChanged(nameof(this.MediaFileChannelId));
            }
        }

        public byte[] ImageBytes
        {
            get
            {
                return this.Entity.Metadata != null ? this.Entity.Metadata.Icon : null;
            }
        }

        public ImageSource Image
        {
            get
            {
                if(_image==null && this.ImageBytes!=null && this.ImageBytes.Length > 0 && this.Duration>TimeSpan.Zero)
                {
                    _image = ImageSourceDecoder.Decode(this.ImageBytes);
                }
                return _image ?? PlaceholderImage;
            }

            set
            {
                if (_image != value)
                {
                    _image = value;
                    this.RaisePropertyChanged(nameof(this.Image));
                }
            }
        }

        public MediaFileEntity Entity { get; private set; }

        IPlayItem IPlayableItem.PlayItem
        {
            get
            {
                throw new NotImplementedException();
            }
        }


    }
}
