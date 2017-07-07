using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Mvvm;
using System;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FCSPlayout.WPFApp
{
    public class BindablePlayItem : BindableBase, IImageItem, IItemWrapper<IPlayItem>,IPlayableItemWithCG //, IPlayItem, IPlayItemAdapter
    {
        private static ImageSource _errorImage;
        private static ImageSource ErrorImage
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

        private IPlayItem _playItem;
        private ImageSource _image;
        private string _resolvedFilePath;
        //private IPlayItemEditorFactory _itemEditorFactory;

        public BindablePlayItem(IPlayItem playItem,string resolvedFilePath/*, IPlayItemEditorFactory itemEditorFactory*/)
        {
            _playItem = playItem;

            _resolvedFilePath = resolvedFilePath;
            //_itemEditorFactory = itemEditorFactory;

            if (playItem.MediaSource.Category != MediaSourceCategory.File)
            {
                this.Image = ErrorImage;
            }
            else
            {
                var metadata = ((Entities.MediaFileEntity)((FileMediaSource)_playItem.MediaSource).Entity).Metadata;
                if (metadata != null)
                {
                    this.ImageBytes = metadata.Icon;
                }
            }
        }

        

        public string Title
        {
            get
            {
                return _playItem.PlaybillItem.Title;
            }
        }

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
                return this._playItem.CalculatedPlayDuration;
            }
        }

        public TimeSpan PlayDuration
        {
            get { return _playItem.CalculatedPlayDuration; }
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

        public TimeSpan StartPosition
        {
            get { return this.PlayRange.StartPosition; }
        }

        public TimeSpan StopPosition
        {
            get { return this.PlayRange.StopPosition; }
            
        }
        
        //public void ChangeStartTime(DateTime newStartTime)
        //{
        //    throw new NotImplementedException();
        //}

        //public void ChangeMediaItem(MediaItem newMediaItem)
        //{
        //    throw new NotImplementedException();
        //}
   

        //public bool IsDirty()
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnSaved()
        //{
        //    throw new NotImplementedException();
        //}

        //public long ModifiedTimestamp
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}


        public IPlayItem PlayItem
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
                return 0;
            }
        }

        internal void Truncate(DateTime maxStopTime)
        {
            var duration = this.StartTime < maxStopTime ? maxStopTime.Subtract(this.StartTime) : TimeSpan.Zero;
            _playItem.CalculatedPlayDuration = duration;

            this.RaisePropertyChanged(nameof(PlayDuration));
            this.RaisePropertyChanged(nameof(Duration));
        }

        public MediaSourceCategory Category
        {
            get
            {
               
                return _playItem.PlaybillItem.MediaSource.Category;
            }
        }

        public string FilePath { get { return _resolvedFilePath; } }

        public ImageSource Image
        {
            get { return _image; }
            set
            {
                SetProperty(ref _image, value);
            }
        }

        public byte[] ImageBytes
        {
            get;
            private set;
        }

        int IPlayableItem.AudioGain
        {
            get
            {
                return this.AudioGain;
            }

            set
            {
                var source = this._playItem.PlaybillItem.MediaSource as IFileMediaSource;
                if (source != null)
                {
                    source.AudioGain=value;
                }
                //return 0;
            }
        }

        
        //string IPlayableItem.FilePath
        //{
        //    get
        //    {
        //        return _filePath;
        //    }
        //}

        PlayRange IPlayableItem.PlayRange
        {
            get
            {
                return this.PlayRange;
            }

            set
            {
                //using(var editor = this._itemEditorFactory.CreateEditor())
                //{
                //    editor.ChangePlayRange(this.PlayItem, value);
                //}

                throw new NotImplementedException();
            }
        }

        public event EventHandler PreviewClosing;
        void IPlayableItem.ClosePreview()
        {
            if (PreviewClosing != null)
            {
                PreviewClosing(this, EventArgs.Empty);
            }
        }

        public IPlayableItem PlayableItem
        {
            get
            {
                if (_playItem.MediaSource.Category == MediaSourceCategory.File)
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }
        }

        IPlayItem IItemWrapper<IPlayItem>.WrappedItem
        {
            get
            {
                return _playItem;
            }
        }

        public CG.CGItemCollection CGItems
        {
            get { return _playItem.CGItems; }
        }

        internal bool Skipped()
        {
            return this.PlayDuration == TimeSpan.Zero;
        }

        //添加新属性

        //internal void NotifyPlayRangeChanged()
        //{
        //    this.RaisePropertyChanged(nameof(this.StartPosition));
        //    this.RaisePropertyChanged(nameof(this.StopPosition));
        //    this.RaisePropertyChanged(nameof(this.Duration));
        //    this.RaisePropertyChanged(nameof(this.PlayDuration));
        //    this.RaisePropertyChanged(nameof(this.StartTime));
        //    this.RaisePropertyChanged(nameof(this.StopTime));
        //    this.RaisePropertyChanged(nameof(this.Category));
        //    this.RaisePropertyChanged(nameof(this.AudioGain));
        //}        
    }
}
