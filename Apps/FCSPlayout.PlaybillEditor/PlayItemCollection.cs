using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FCSPlayout.PlaybillEditor
{
    public class PlayItemCollection : ObservableCollection<BindablePlayItem>, IPlayItemCollection
    {
        private IMediaFilePathResolver _filePathResolver;
        private IPlayItemEditorFactory _itemEditFactory;

        public PlayItemCollection(IMediaFilePathResolver filePathResolver,IPlayItemEditorFactory itemEditFactory)
        {
            _filePathResolver = filePathResolver;
            _itemEditFactory = itemEditFactory;
        }

        IPlayItem IPlayItemCollection.this[int index]
        {
            get
            {
                return this[index].PlayItem;
            }
        }

        int IPlayItemCollection.Count
        {
            get
            {
                return this.Count;
            }
        }

        internal BindablePlayItem CreateBindablePlayItem(IPlayItem playItem)
        {
            string filePath = null;
            if (playItem.MediaSource.Category == MediaSourceCategory.File)
            {
                filePath=_filePathResolver.Resolve(((AppInfrastructure.FileMediaSource)playItem.MediaSource).FileName);
            }
            return new BindablePlayItem(playItem,filePath,_itemEditFactory);
        }

        public void Append(IList<IPlayItem> playItems)
        {
            for(int i = 0; i < playItems.Count; i++)
            {
                this.Add(CreateBindablePlayItem(playItems[i]));
            }
        }

        bool IPlayItemCollection.Contains(IPlayItem playItem)
        {
            return this.Any(i => i.PlayItem == playItem);
        }

        void IPlayItemCollection.Insert(int index, IPlayItem playItem)
        {
            this.Insert(index, CreateBindablePlayItem(playItem));
        }

        void IPlayItemCollection.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            this.IsDirty = true;
        }

        public void ValidateTimeRange(DateTime startTime, TimeSpan duration)
        {
            ValidateStartTime(startTime);
            //var e = new TimeValidationEventArgs(startTime);
            //OnValidateStartTime(e);
            //if (!e.IsValid)
            //{
            //    throw new InvalidTimeException(startTime);
            //}

            ValidateTimeRange(startTime, startTime.Add(duration));
        }

        private void ValidateStartTime(DateTime startTime)
        {
            //throw new NotImplementedException();
        }

        private void ValidateTimeRange(DateTime startTime, DateTime stopTime)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var item = this[i].PlayItem;
                if (item.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing || item.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    var startTime2 = item.StartTime;
                    var stopTime2 = item.CalculatedStopTime;

                    if (stopTime > startTime2 && stopTime2 > startTime)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        public void ValidateTimeRange(DateTime startTime, TimeSpan duration, IPlayItem excludeItem)
        {
            ValidateStartTime(startTime);
            ValidateTimeRange(startTime, startTime.Add(duration),excludeItem);
        }

        private void ValidateTimeRange(DateTime startTime, DateTime stopTime, IPlayItem excludeItem)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var item = this[i].PlayItem;
                if (item!=excludeItem && 
                    item.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing || item.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak)
                {
                    var startTime2 = item.StartTime;
                    var stopTime2 = item.CalculatedStopTime;

                    if (stopTime > startTime2 && stopTime2 > startTime)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        public bool CanClear()
        {
            return true;
        }

        public DateTime? GetStartTime()
        {
            if (this.Count == 0)
            {
                return null;
            }
            else
            {
                return this[0].StartTime;
            }
        }

        public DateTime? GetStopTime()
        {
            if (this.Count == 0)
            {
                return null;
            }
            else
            {
                return this[this.Count-1].StopTime;
            }
        }

        public bool IsDirty
        {
            get;set;
        }
    }
}
