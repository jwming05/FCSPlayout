using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FCSPlayout.WPFApp
{
    public class PlayItemCollection : ObservableCollection<BindablePlayItem>, IPlayItemCollection
    {
        private IMediaFilePathResolver _filePathResolver;
        //private int _currentIndex = -1;
        private int _nextIndex = -1;

        private IPlayItem _currentItem;
        private bool _running;
        private ObservableCollection<BindablePlayItem> _playedCollection;

        public PlayItemCollection(IMediaFilePathResolver filePathResolver, 
            [Microsoft.Practices.Unity.Dependency("playedCollection")]ObservableCollection<BindablePlayItem> playedCollection)
        {
            _filePathResolver = filePathResolver;
            _playedCollection = playedCollection;

            //_itemEditFactory = itemEditFactory;
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
            return new BindablePlayItem(playItem,filePath/*,_itemEditFactory*/);
        }

        internal IPlayItem GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime)
        {
            if (_nextIndex != -1) return null;

            int i = this.CurrentItem==null ? 0 : 1;

            while (i<this.Count)
            {
                var item = this[i];
                //if (item == this.CurrentItem)
                //{
                //    i++;
                //    continue;
                //}

                if(item.StartTime > maxStartTime)
                {
                    return null;
                }

                if (item.StartTime >= minStartTime)
                {
                    if (item.Duration >= PlayoutConfiguration.Current.MinPlayDuration)
                    {
                        _nextIndex = i;
                        return item.PlayItem;
                    }
                    else
                    {
                        // 删除当前项。
                        //this.RemoveAt(i);
                        //continue;

                        // 等以后调用再删除。
                        return null;
                    }
                }
                else  // item.StartTime < minStartTime
                {
                    if(item.StopTime.Subtract(minStartTime) >= PlayoutConfiguration.Current.MinPlayDuration)
                    {
                        _nextIndex = i;
                        return item.PlayItem;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            // 删除当前项。
                            this.RemoveAt(i);
                            this.PlayedCollection.Add(item);
                            continue;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                //i++;
            }

            return null;
        }

        internal void OnTimer()
        {
            
        }

        internal void OnStop()
        {
            _running = false;
            //_currentIndex = -1;
            _nextIndex = -1;
        }

        internal void OnStart()
        {
            _running = true;
        }

        public void Append(IList<IPlayItem> playItems)
        {
            var maxStopTime = playItems[0].StartTime;
            for (int i = this.Count - 1; i >= 0; i--)
            {
                var temp = this[i];
                if (temp.StopTime <= maxStopTime)
                {
                    break;
                }

                temp.Truncate(maxStopTime);

                //var duration = temp.StartTime < maxStopTime ? maxStopTime.Subtract(temp.StartTime) : TimeSpan.Zero;
                //temp.PlayDuration = duration;

                //if (temp.ScheduleMode != PlayScheduleMode.Auto)
                //{
                //    conflictCount++;
                //}
            }

            for (int i = 0; i < playItems.Count; i++)
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
                if (item.PlaybillItem.ScheduleMode == PlayScheduleMode.Timing || 
                    item.PlaybillItem.ScheduleMode == PlayScheduleMode.TimingBreak)
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
        public IPlayItem NextItem { get; internal set; }
        public IPlayItem CurrentItem
        {
            get { return _currentItem; }
            internal set
            {
                if (_currentItem != value)
                {
                    if (value==null && _currentItem != null && this.Count>0 /*&& _currentIndex!=-1*/)
                    {
                        this.PlayedCollection.Add(this[0]);
                        //var item = this[_currentIndex];
                        this.RemoveAt(0/*_currentIndex*/);
                    }

                    _currentItem = value;
                    if(_currentItem!=null/*&& _nextIndex != -1*/)
                    {
                        //_currentIndex = _nextIndex;
                        _nextIndex = -1;
                    }
                }
            }
        }

        public ObservableCollection<BindablePlayItem> PlayedCollection
        {
            get
            {
                return _playedCollection;
            }
        }

        public event EventHandler Committed;

        internal void OnCommitted()
        {
            if (Committed != null)
            {
                Committed(this, EventArgs.Empty);
            }
        }

        internal bool CanForcePlay(BindablePlayItem selectedPlayItem)
        {
            if (_running)
            {
                return this.NextItem==null && this.CurrentItem!=null &&
                    selectedPlayItem.PlayItem != this.CurrentItem && selectedPlayItem.PlayItem != this.NextItem;
            }
            return false;
        }

        //internal void ForcePlay(BindablePlayItem selectedPlayItem)
        //{
        //    if (CanForcePlay(selectedPlayItem))
        //    {
        //        // 复制当前播放项。

        //    }
        //}
    }
}
