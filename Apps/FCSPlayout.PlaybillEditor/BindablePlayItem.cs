using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Mvvm;
using System;
using System.Threading;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPFApp.Models
{
    public class BindablePlayItem : BindableBase//, IPlayItem,IPlayItemAdapter
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
        }

        public string Title
        {
            get
            {
                //return _playItem.Title;
                return _playItem.PlaybillItem.Title;
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
                return _playItem.CalculatedStopTime;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                //return _playItem.PlaybillItem.Duration;
                //return _playItem.PlayRange.Duration;
                return this._playItem.CalculatedPlayDuration; //.Source.Duration;
            }
        }

        public TimeSpan PlayDuration
        {
            get { return _playItem.CalculatedPlayDuration; }
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
                        //var pos = this.PlayRange.StartPosition.TotalSeconds;
                        var pos = this.PlayRange.StartPosition.TotalSeconds+(this.PlayRange.StopPosition.TotalSeconds- this.PlayRange.StartPosition.TotalSeconds) /2;

                        _startImage = sBmpSourceCache.Get(mediaSource.FileName/*.Path*/, this.PlayRange.StartPosition.TotalSeconds, this.PlayRange.StopPosition.TotalSeconds);

                        if (_startImage == null && _startImageRequest == null)
                        {
                            StartRetrieveStartImage(mediaSource.FileName/*.Path*/, pos);
                            return null;
                        }
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

        public TimeSpan? SourceDuration
        {
            get
            {
                var fileSource = this.Source as FileMediaSource;
                if (fileSource != null)
                {
                    return fileSource.Duration;
                }

                return null;
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
                return _playItem.PlayRange;
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
        //        return _playItem.PlaybillItem.PlaySource.Parameters;
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
        

        public DateTime MaxPlayTime
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private MediaFileImageRequest _startImageRequest;
        //private MediaFileImageRequest _stopImageRequest;

        private void SetStartImageInternal(object value)
        {
            var tuple = (Tuple<IntPtr, string, double>)value;
            var ptr = tuple.Item1;
            if (ptr != IntPtr.Zero)
            {
                BitmapSource bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero,
                System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                NativeMethods.DeleteObject(ptr);
                //Infrastructure.BitmapSourceCache.Instance.Add(tuple.Item1, tuple.Item2, bmpSource);
                this.StartImage = bmpSource;
                sBmpSourceCache.Add(tuple.Item2, tuple.Item3, bmpSource);
            }
            _startImageRequest = null;
        }
        
        private void StartRetrieveStartImage(string file, double pos)
        {
            if (_startImageRequest != null) return;

            _startImageRequest = new MediaFileImageRequest
            {
                Path = file,
                Position = pos,
                Complete = (ptr) =>
                {
                    _syncContext.Post(new System.Threading.SendOrPostCallback(SetStartImageInternal), Tuple.Create(ptr, file, pos));
                }
            };

            MediaFileImageExtractor.Current.GetHBitmapAsync(_startImageRequest);
        }

        public void ChangeStartTime(DateTime newStartTime)
        {
            throw new NotImplementedException();
        }

        public void ChangeMediaItem(MediaItem newMediaItem)
        {
            throw new NotImplementedException();
        }
   
        //public IPlayItem ChangeScheduleMode(PlayScheduleMode newScheduleMode)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPlayItem ChangeScheduleMode(PlayScheduleMode newScheduleMode, DateTime startTime)
        //{
        //    throw new NotImplementedException();
        //}

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


        internal IPlayItem PlayItem
        {
            get
            {
                return _playItem;
            }
        }

        public int AudioGain
        {
            get
            {
                var source = this._playItem.PlaybillItem.MediaSource as IFileMediaSource;
                if (source != null)
                {
                    return source.AudioGain;
                }
                return 0; // this._playItem.PlaybillItem.AudioGain;
            }
        }

//        public ENUM_AspectRatio AspectRatio
//        {
//            get {

//                return _playItem.PlaybillItem.AspectRatio;
//            }
//        }

//        public MPFieldsType MPFieldsType
//        {
//            get {

//                return _playItem.PlaybillItem.MPFieldsType;
//}
//        }

//        public ENUM_StretchMode StretchMode
//        {
//            get {

//                return _playItem.PlaybillItem.StretchMode;
//            }
           
//        }

        public MediaSourceCategory Category
        {
            get
            {
               
                return _playItem.PlaybillItem.MediaSource.Category;
            }
        }
        //添加新属性
      
        internal void NotifyPlayRangeChanged()
        {
            this.OnPropertyChanged(() => this.StartPosition);
            this.OnPropertyChanged(() => this.StopPosition);
            this.OnPropertyChanged(() => this.Duration);
            this.OnPropertyChanged(() => this.PlayDuration);

            this.OnPropertyChanged(() => this.StartTime);
            this.OnPropertyChanged(() => this.StopTime);
            this.OnPropertyChanged(() => this.Category);
            this.OnPropertyChanged(() => this.AudioGain);
            //this.OnPropertyChanged(() => this.StretchMode);
            //this.OnPropertyChanged(() => this.MPFieldsType);
            //this.OnPropertyChanged(() => this.AspectRatio);
        }        
    }
}
