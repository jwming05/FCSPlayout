using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using Prism.Mvvm;
using System;
using System.Threading;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPFApp.Models
{
    public class BindableMediaItem : BindableBase
    {
        private MediaItem _mediaItem;

        private SynchronizationContext _syncContext = SynchronizationContext.Current;
        private BitmapSource _image;

        public BindableMediaItem(MediaItem other)
        {
            _mediaItem = other;
        }

        public MediaItem MediaItem
        {
            get
            {
                return _mediaItem;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                //return this.PlayRange.Duration;
                return this.Source.Duration ?? TimeSpan.Zero;
            }
        }

        public TimeSpan PlayDuration
        {
            get
            {
                //return this.PlayRange.Duration;
                return this.PlayRange.Duration;
            }
        }

        public string Title
        {
            get { return this.Source.Title; }
        }

        public TimeSpan StartPosition
        {
            get { return this.PlayRange.StartPosition; }
        }

        public TimeSpan StopPosition
        {
            get { return this.PlayRange.StopPosition; }
        }

        public PlayRange PlayRange
        {
            get
            {
                return _mediaItem.PlayRange;
            }

            set
            {
                //_mediaItem.PlayRange = value;
                _mediaItem = new MediaItem(_mediaItem.Source, value);
                //_mediaItem = _mediaItem.ChangePlayRange(value); 
                this.OnPropertyChanged(() => this.PlayRange);
                this.OnPropertyChanged(() => this.StartPosition);
                this.OnPropertyChanged(() => this.StopPosition);
                this.OnPropertyChanged(() => this.Duration);
                this.OnPropertyChanged(() => this.PlayDuration);
                this.OnPropertyChanged(() => this.MediaItem);
                if (_image != null)
                {
                    this.Image = null;
                }
            }
        }

        public IMediaSource Source
        {
            get
            {
                return _mediaItem.Source;
            }
        }

        public BitmapSource Image
        {
            get
            {
                if (_image == null)
                {
                    var mediaSource = this.MediaItem.Source as FileMediaSource;
                    if (mediaSource != null)
                    {
                        var pos = (this.PlayRange.StartPosition.TotalSeconds + this.PlayRange.StopPosition.TotalSeconds) / 2;

                        //_image = Infrastructure.BitmapSourceCache.Instance.TryGet(mediaSource.Path, pos);
                        if (_image == null && _request == null)
                        {
                            StartRetrieveImage(mediaSource.FileName/*.Path*/, pos);
                            return null;
                        }
                    }
                }
                return _image;
            }

            private set
            {
                if (_image != value)
                {
                    _image = value;
                    this.OnPropertyChanged(() => this.Image);
                }
            }
        }

        private void SetImageInternal(object value)
        {
            var ptr = (IntPtr)value;
            if (ptr != IntPtr.Zero)
            {
                BitmapSource bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero,
                System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                NativeMethods.DeleteObject(ptr);
                this.Image = bmpSource;
            }
            _request = null;
        }

        private MediaFileImageRequest _request;

        private void StartRetrieveImage(string file, double pos)
        {
            _request = new MediaFileImageRequest { Path = file, Position = pos, Complete = (ptr) => 
            {
                _syncContext.Post(new System.Threading.SendOrPostCallback(SetImageInternal), ptr);
            } };

            MediaFileImageExtractor.Current.GetHBitmap(_request);
        }
    }
}
