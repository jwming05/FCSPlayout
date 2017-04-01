using FCSPlayout.Domain;
using FCSPlayout.Entities;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPF.Core
{
    public class BindableMediaFileItem : BindableBase,IPlayableItem
    {
        private static MediaFileStorage _mediaFileStorage = MediaFileStorage.Primary;

        //private SynchronizationContext _syncContext = SynchronizationContext.Current;
        private BitmapSource _image;

        public event EventHandler PreviewClosing;

        //public BindableMediaFileItem(MediaFileEntity entity)
        //{
        //    this.Entity = entity;
        //    this.FilePath = ResolvePath(this.FileName);
        //}

        public BindableMediaFileItem(MediaFileEntity entity, string filePath)
        {
            this.Entity = entity;
            this.FilePath = filePath; // ResolvePath(this.FileName);
        }

        //public BindableMediaFileItem(string filePath)
        //{
        //    this.Entity = new MediaFileEntity { CreatorId = UserService.CurrentUser.Id, CreationTime = DateTime.Now };
        //    this.FilePath = filePath;

        //    this.OriginalFileName = Path.GetFileName(filePath);
        //    this.FileName = GetNewFileName(filePath);
        //    this.Title = Path.GetFileNameWithoutExtension(filePath);
        //    this.Duration = GetDuration(filePath);

        //    this.StartPosition = TimeSpan.Zero;
        //    this.PlayDuration = this.Duration;
        //}

        public BindableMediaFileItem(string filePath, Guid creatorId)
        {
            this.Entity = new MediaFileEntity { CreatorId = creatorId/*UserService.CurrentUser.Id*/, CreationTime = DateTime.Now };
            this.FilePath = filePath;

            this.OriginalFileName = Path.GetFileName(filePath);
            this.FileName = GetNewFileName(filePath);
            this.Title = Path.GetFileNameWithoutExtension(filePath);
            this.Duration = GetDuration(filePath);

            this.StartPosition = TimeSpan.Zero;
            this.PlayDuration = this.Duration;
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

        public Guid CreatorId
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
            get { return TimeSpan.FromSeconds(this.Entity.MarkerIn); }
            private set
            {
                this.Entity.MarkerIn = value.TotalSeconds;
            }
        }

        public TimeSpan StopPosition
        {
            get
            {
                return this.StartPosition + this.PlayDuration;
            }
        }

        public TimeSpan PlayDuration
        {
            get
            {
                return TimeSpan.FromSeconds(this.Entity.MarkerDuration);
            }

            private set
            {
                this.Entity.MarkerDuration = value.TotalSeconds;
            }
        }

        public PlayRange PlayRange
        {
            get
            {
                return new PlayRange(this.StartPosition, this.PlayDuration);
            }

            set
            {
                this.StartPosition = value.StartPosition;
                this.PlayDuration = value.Duration;
                this.RaisePropertyChanged(nameof(this.PlayRange));
                this.RaisePropertyChanged(nameof(this.StartPosition));
                this.RaisePropertyChanged(nameof(this.StopPosition));
                this.RaisePropertyChanged(nameof(this.PlayDuration));

                //if (_image != null)
                //{
                //    this.Image = null;
                //}
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

        internal byte[] ImageBytes
        {
            get
            {
                return this.Entity.Metadata != null ? this.Entity.Metadata.Icon : null;
            }
        }

        public BitmapSource Image
        {
            get
            {
                return _image;
            }

            internal set
            {
                if (_image != value)
                {
                    _image = value;
                    this.RaisePropertyChanged(nameof(this.Image));
                }
            }
        }

        public MediaFileEntity Entity { get; private set; }

        //internal static MediaFileStorage MediaFileStorage
        //{
        //    get { return _mediaFileStorage; }
        //    set
        //    {
        //        _mediaFileStorage = value;
        //    }
        //}

        //private static string ResolvePath(string fileName)
        //{
        //    return MediaFilePathResolver.Current.Resolve(fileName, BindableMediaFileItem.MediaFileStorage);
        //}

        private static TimeSpan GetDuration(string filePath)
        {
            return MediaFileDurationGetter.Current.GetDuration(filePath);
        }

        //internal BitmapSource ResolveImage()
        //{
        //    return MediaFileImageResolver.Instance.Resolve(this.FilePath, this.Duration.TotalMilliseconds / 2.0);
        //}
    }
}
