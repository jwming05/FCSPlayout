using FCSPlayout.AppInfrastructure;
using FCSPlayout.CG;
using FCSPlayout.Domain;
using Prism.Mvvm;
using System;
using System.Threading;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPFApp.Models
{
    public class BindablePlayItem : BindableBase //, IPlayItem //,IPlayItemAdapter
    {
        public static readonly BitmapSourceCache sBmpSourceCache = new BitmapSourceCache();

        private IPlayItem _playItem;

        private SynchronizationContext _syncContext = SynchronizationContext.Current;
        private BitmapSource _startImage;
        //private BitmapSource _stopImage;
        private static BitmapSource _errorImage;

        private static BitmapSource ErrorImage
        {
            get
            {
                if (_errorImage == null)
                {
                    _errorImage = new WriteableBitmap(100, 100, 96, 96, System.Windows.Media.PixelFormats.Rgb24, null);
                }
                return _errorImage;
            }
        }
        public BindablePlayItem(IPlayItem playItem)
        {
            _playItem = playItem;

            //var bindable = playItem as BindablePlayItem;
            //if (bindable != null)
            //{
            //    _playItem = bindable._playItem;
            //}
            //else
            //{
            //    var temp = playItem as PlayItem;
            //    if (temp == null) throw new ArgumentException();
            //    _playItem = temp;
            //}

            //if (_playItem != null) _playItem.StatusChanged += _playItem_StatusChanged;
        }
        //private void _playItem_StatusChanged(object sender, EventArgs e)
        //{
        //    OnPropertyChanged("Status");

        //}
        public string Title
        {
            get
            {
                return _playItem.PlaybillItem.Title; //.Title;
            }
        }


        //public MediaItem MediaItem
        //{
        //    get
        //    {
        //        return _playItem.PlaybillItem.MediaItem;
        //    }
        //}

        public DateTime StartTime
        {
            get
            {
                return _playItem.StartTime;
            }
        }

        public DateTime StopTime
        {
            get
            {
                return _playItem.CalculatedStopTime; //.GetStopTime();
            }
        }

        public TimeSpan Duration
        {
            get { return _playItem.CalculatedPlayDuration; }// .PlayRange.Duration; 
        }

        public BitmapSource StartImage
        {
            get
            {
                if (_startImage == null)
                {
                    var mediaSource = this.Source as FileMediaSource;
                    if (mediaSource != null)
                    {
                        //var pos = this.PlayRange.StartPosition.TotalSeconds+(this.PlayRange.StopPosition.TotalSeconds- this.PlayRange.StartPosition.TotalSeconds) /2;

                        //_startImage = sBmpSourceCache.Get(mediaSource.FileName, this.PlayRange.StartPosition.TotalSeconds, this.PlayRange.StopPosition.TotalSeconds);

                        //if (_startImage == null && _startImageRequest == null)
                        //{
                        //    StartRetrieveStartImage(mediaSource.FileName, pos);
                        //    return null;
                        //}
                    }
                }
                return _startImage;
            }

            private set
            {
                if (_startImage != value)
                {
                    _startImage = value;
                    this.OnPropertyChanged(() => this.StartImage);
                }
            }
        }

        public IMediaSource Source
        {
            get
            {
                return _playItem.PlaybillItem.MediaSource;
            }
        }

        public PlayRange PlayRange
        {
            get
            {
                return _playItem.PlayRange; //.PlaybillItem.MediaItem.PlayRange;
            }
        }

        public PlayScheduleMode ScheduleMode
        {
            get
            {
                return _playItem.PlaybillItem.ScheduleMode;
            }
        }

        //public IPlayParameters Parameters
        //{
        //    get
        //    {
        //        return _playItem.PlayParameters;
        //    }
        //}


        public TimeSpan StartPosition
        {
            get { return this.PlayRange.StartPosition; }
        }

        public TimeSpan StopPosition
        {
            get { return this.PlayRange.StopPosition; }
        }

        public CGItemCollection CGItems
        {
            get
            {
                return _playItem.PlaybillItem.CGItems;
            }

            set
            {
                // TODO: change the CGItems
                //_playItem.PlaybillItem.CGItems = value;
            }
        }

        //IPlayItem IPlayItemAdapter.Adaptee
        //{
        //    get
        //    {
        //        return _playItem;
        //    }
        //}

        

        //public Guid Id
        //{
        //    get
        //    {
        //        return _playItem.Id;
        //    }
        //}


        //public PlayItemStatus Status
        //{
        //    get
        //    {
        //        return _status;
        //        //return _playItem.Status;
        //    }

        //    set
        //    {
        //        if (_status != value)
        //        {
        //            _status = value;
        //            OnPropertyChanged("Status");
        //        }
        //    }
        //}

        //Guid IPlayItemEntity.Id
        //{
        //    get
        //    {
        //        return _playItem.Id;
        //    }

        //    set
        //    {
        //        _playItem.Id = value;
        //    }
        //}

        

        public DateTime MaxPlayTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //private MediaFileImageRequest _startImageRequest;

        //private PlayItemStatus _status;

        //private MediaFileImageRequest _stopImageRequest;
        public event EventHandler StatusChanged;
        //private void SetStartImageInternal(object value)
        //{
        //    var tuple = (Tuple<IntPtr, string, double>)value;
        //    var ptr = tuple.Item1;
        //    if (ptr != IntPtr.Zero)
        //    {
        //        BitmapSource bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero,
        //        System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

        //        NativeMethods.DeleteObject(ptr);
        //        //Infrastructure.BitmapSourceCache.Instance.Add(tuple.Item1, tuple.Item2, bmpSource);
        //        this.StartImage = bmpSource;
        //        sBmpSourceCache.Add(tuple.Item2, tuple.Item3, bmpSource);
        //    }
        //    _startImageRequest = null;
        //}
        
        //private void StartRetrieveStartImage(string file, double pos)
        //{
        //    if (_startImageRequest != null) return;

        //    _startImageRequest = new MediaFileImageRequest
        //    {
        //        Path = file,
        //        Position = pos,
        //        Complete = (ptr) =>
        //        {
        //            _syncContext.Post(new System.Threading.SendOrPostCallback(SetStartImageInternal), Tuple.Create(ptr, file, pos));
        //        }
        //    };

        //    MediaFileImageExtractor.Current.GetHBitmapAsync(_startImageRequest);
        //}

        //public void SetMaxPlayTime(DateTime maxPlayTime)
        //{
        //    _playItem.Editor.SetMaxPlayTime(maxPlayTime);
        //}

        //public void SetPlayTime(DateTime startTime, DateTime maxPlayTime)
        //{
        //    _playItem.Editor.SetPlayTime(startTime, maxPlayTime);
        //}

        
        public void ChangeStartTime(DateTime newStartTime)
        {
            throw new NotImplementedException();
        }

        public void ChangeMediaItem(MediaItem newMediaItem)
        {
            throw new NotImplementedException();
        }

        public IPlayItem ChangeScheduleMode(PlayScheduleMode newScheduleMode)
        {
            throw new NotImplementedException();
        }

        public IPlayItem ChangeScheduleMode(PlayScheduleMode newScheduleMode, DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public bool IsDirty()
        {
            throw new NotImplementedException();
        }

        public void OnSaved()
        {
            throw new NotImplementedException();
        }

        public long ModifiedTimestamp
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IPlayItem PlayItem
        {
            get
            {
                return _playItem;
            }
        }
    }
}
