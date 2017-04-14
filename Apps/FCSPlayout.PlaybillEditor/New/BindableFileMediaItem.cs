using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FCSPlayout.PlaybillEditor
{
    public class BindableFileMediaItem:BindableBase, IPlayableItem, IImageItem
    {
        //private static MediaFileStorage _mediaFileStorage = MediaFileStorage.Primary;
        private BitmapSource _image;
        private PlayRange _playRange;

        public event EventHandler PreviewClosing;

        public BindableFileMediaItem(MediaFileEntity entity, string filePath)
        {
            this.MediaSource = new FileMediaSource(entity); // mediaSource;
            //this.StartPosition= TimeSpan.FromSeconds(this.Entity.MarkerIn);
            //this.PlayDuration= TimeSpan.FromSeconds(this.Entity.MarkerDuration);

            this.PlayRange= new PlayRange(TimeSpan.FromSeconds(this.Entity.MarkerIn), TimeSpan.FromSeconds(this.Entity.MarkerDuration));

            this.FilePath = filePath;
        }

        public void ClosePreview()
        {
            if (PreviewClosing != null)
            {
                PreviewClosing(this, EventArgs.Empty);
            }
        }

        //private string GetNewFileName(string filePath)
        //{
        //    return Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(filePath);
        //}

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
            get { return this.PlayRange.StartPosition; }

            //get
            //{

            //    return TimeSpan.FromSeconds(this.Entity.MarkerIn);
            //}
            //private set
            //{
            //    this.Entity.MarkerIn = value.TotalSeconds;
            //}
        }

        public TimeSpan StopPosition
        {
            get
            {
                return this.PlayRange.StopPosition; // this.StartPosition + this.PlayDuration;
            }
        }

        public TimeSpan PlayDuration
        {
            get { return this.PlayRange.Duration; }
            //get
            //{
            //    return TimeSpan.FromSeconds(this.Entity.MarkerDuration);
            //}

            //private set
            //{
            //    this.Entity.MarkerDuration = value.TotalSeconds;
            //}
        }

        public PlayRange PlayRange
        {
            get
            {
                return _playRange; // new PlayRange(this.StartPosition, this.PlayDuration);
            }

            set
            {
                _playRange = value;
                //this.StartPosition = value.StartPosition;
                //this.PlayDuration = value.Duration;
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

        public byte[] ImageBytes
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

            set
            {
                if (_image != value)
                {
                    _image = value;
                    this.RaisePropertyChanged(nameof(this.Image));
                }
            }
        }

        internal MediaFileEntity Entity
        {
            get { return (MediaFileEntity)this.MediaSource.Entity; }
        }

        public FileMediaSource MediaSource { get; private set; }

        private static TimeSpan GetDuration(string filePath)
        {
            return MediaFileDurationGetter.Current.GetDuration(filePath);
        }
    }
}
