using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FCSPlayout.MediaFileImporter
{
    public class BindableMediaFileItem : BindableBase,IPlayableItem
    {
        private static MediaFileStorage _mediaFileStorage = MediaFileStorage.Primary;

        private SynchronizationContext _syncContext = SynchronizationContext.Current;
        private MediaFileImageRequest _request;
        private BitmapSource _image;
        //private DelegateCommand _previewCommand;

        public BindableMediaFileItem(MediaFileEntity entity)
        {
            this.Entity = entity;
            this.FilePath = ResolvePath(this.FileName);

            //_previewCommand = new DelegateCommand(Preview, CanPreview);
        }

        

        public BindableMediaFileItem(string filePath)
        {
            this.Entity = new MediaFileEntity { CreatorId = UserService.CurrentUser.Id, CreationTime = DateTime.Now };
            this.FilePath = filePath;

            this.OriginalFileName = Path.GetFileName(filePath);
            this.FileName = GetNewFileName(filePath);
            this.Title = Path.GetFileNameWithoutExtension(filePath);
            this.Duration = GetDuration(filePath);

            this.StartPosition = TimeSpan.Zero;
            this.PlayDuration = this.Duration;

            //_previewCommand = new DelegateCommand(Preview, CanPreview);
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
                this.OnPropertyChanged(() => this.Title);
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
                this.OnPropertyChanged(() => this.PlayRange);
                this.OnPropertyChanged(() => this.StartPosition);
                this.OnPropertyChanged(() => this.StopPosition);
                this.OnPropertyChanged(() => this.PlayDuration);
                if (_image != null)
                {
                    this.Image = null;
                }
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
                    OnPropertyChanged(() => this.AudioGain);
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

                OnPropertyChanged(() => this.MediaFileCategoryId);
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

                OnPropertyChanged(() => this.MediaFileChannelId);
            }
        }

        public BitmapSource Image
        {
            get
            {
                //if (_image == null)
                //{
                //    var pos = this.StartPosition.TotalSeconds + (this.PlayDuration.TotalSeconds / 2.0);
                //    if (_image == null && _request == null)
                //    {
                //        StartRetrieveImage(this.FilePath, pos);
                //        return null;
                //    }
                //}
                return _image;
            }

            /*private*/internal set
            {
                if (_image != value)
                {
                    _image = value;
                    this.OnPropertyChanged(() => this.Image);
                }
            }
        }

        internal MediaFileEntity Entity { get; private set; }

        internal static MediaFileStorage MediaFileStorage
        {
            get { return _mediaFileStorage; }
            set
            {
                _mediaFileStorage = value;
            }
        }
        //private void SetImageInternal(object value)
        //{
        //    var ptr = (IntPtr)value;
        //    if (ptr != IntPtr.Zero)
        //    {
        //        BitmapSource bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero,
        //        System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

        //        NativeMethods.DeleteObject(ptr);
        //        this.Image = bmpSource;
        //    }
        //    _request = null;
        //}
        //private void StartRetrieveImage(string file, double pos)
        //{
        //    _request = new MediaFileImageRequest
        //    {
        //        Path = file,
        //        Position = pos,
        //        Complete = (ptr) =>
        //        {
        //            _syncContext.Post(new System.Threading.SendOrPostCallback(SetImageInternal), ptr);
        //        }
        //    };

        //    MediaFileImageExtractor.Current.GetHBitmap(_request);
        //}

        private static string ResolvePath(string fileName)
        {
            return MediaFilePathResolver.Current.Resolve(fileName, BindableMediaFileItem.MediaFileStorage);
        }

        private static TimeSpan GetDuration(string filePath)
        {
            return MediaFileDurationGetter.Current.GetDuration(filePath);
        }

        //public ICommand PreviewCommand
        //{
        //    get { return _previewCommand; }
        //}

        //private bool CanPreview()
        //{
        //    return true;
        //}

        //private void Preview()
        //{
        //    System.Diagnostics.Debug.WriteLine("Request preview");
        //}
    }
}
