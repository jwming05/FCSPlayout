using FCSPlayout.Domain;
using FCSPlayout.WPFApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FCSPlayout.WPFApp
{
    public class PlayItemCollection2 : ObservableCollection<BindablePlayItem>, IPlayItemCollection2
    {
        private ListView _listView;
        private IPlayControlService _playControlService;

        private System.Threading.SynchronizationContext _syncContext;

        public PlayItemCollection2(IPlayControlService playControlService)
        {
            _syncContext = System.Threading.SynchronizationContext.Current;

            _playControlService = playControlService;
            _playControlService.PlaylistRequestReceived += PlayControlService_PlaylistSyncRequestReceived;
            _playControlService.PlaylistResponseReceived += PlayControlService_PlaylistResponseReceived;

            _playControlService.SendPlaylistRequest(null, PlaylistRequestCategory.GetPlaylist);
        }

        private void PlayControlService_PlaylistResponseReceived(object sender, PlaylistResponseEventArgs e)
        {
            switch (e.ResponseMessage.Category)
            {
                case PlaylistRequestCategory.GetPlaylist:
                    _syncContext.Send(new System.Threading.SendOrPostCallback(UpdateFromRemote), e.ResponseMessage.PlayItems);
                    break;
            }
            //throw new NotImplementedException();
        }

        private void UpdateFromRemote(object state)
        {
            IPlayItem[] playItems = (IPlayItem[])state;
            this.Clear();
            for (int i = 0; i < playItems.Length; i++)
            {
                this.Add(new BindablePlayItem(playItems[i]));
            }

            if (_listView != null)
            {
                _listView.Reset();
            }
        }

        private void SendGetPlaylistResponse(Guid sender,Guid requestId, PlaylistRequestCategory requestCategory)
        {
            IPlayItem[] playItems = CopyPlayItems();

            _playControlService.SendPlaylistResponse(sender,requestId, playItems, requestCategory);
        }

        private void PlayControlService_PlaylistSyncRequestReceived(object sender, PlaylistRequestEventArgs e)
        {
            switch (e.RequestMessage.Category)
            {
                case PlaylistRequestCategory.SyncPlaylist:
                    _syncContext.Send(new System.Threading.SendOrPostCallback(UpdateFromRemote), e.RequestMessage.PlayItems);
                    break;
                case PlaylistRequestCategory.GetPlaylist:
                    SendGetPlaylistResponse(e.RequestMessage.Sender, e.RequestMessage.RequestId, e.RequestMessage.Category);
                    break;
                case PlaylistRequestCategory.StartDelay:
                    break;
                case PlaylistRequestCategory.StopDelay:
                    break;
            }
        }

        IPlayItem IPlayItemCollection.this[int index]
        {
            get
            {
                if (_listView != null)
                {
                    return _listView[index].PlayItem;
                }
                else
                {
                    return this[index].PlayItem;
                }
                
            }
        }

        int IPlayItemCollection.Count
        {
            get
            {
                if (_listView != null)
                {
                    return _listView.Count;
                }
                else
                {
                    return this.Count;
                }
                
            }
        }

        void IPlayItemCollection.Append(IList<IPlayItem> playItems)
        {
            if (_listView != null)
            {
                for (int i = 0; i < playItems.Count; i++)
                {
                    _listView.Add(new BindablePlayItem(playItems[i]));
                }
            }
            else
            {
                for(int i = 0; i < playItems.Count; i++)
                {
                    this.Add(new BindablePlayItem(playItems[i]));
                }
            }
        }

        void IPlayItemCollection.Clear()
        {
            if (_listView != null)
            {
                _listView.Clear();
            }
            else
            {
                this.Clear();
            }
        }

        bool IPlayItemCollection.Contains(IPlayItem playItem)
        {
            if (_listView != null)
            {
                return _listView.Contains(playItem);
            }
            else
            {
                return this.Any(i=>i.PlayItem==playItem);
            }
        }

        IPlayItem IPlayItemCollection2.GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime)
        {
            if (_listView != null)
            {
                return _listView.GetNextPlayItem(minStartTime, maxStartTime);
            }
            else
            {
                return null; // this.Count;
            }
        }

        void IPlayItemCollection.Insert(int index, IPlayItem playItem)
        {
            if (_listView != null)
            {
                _listView.Insert(index, new BindablePlayItem(playItem));
            }
            else
            {
                this.Insert(index, new BindablePlayItem(playItem));
            }
        }

        void IPlayItemCollection2.OnTimer()
        {
            if (_listView != null)
            {
                _listView.OnTimer();
            }
            
        }

        void IPlayItemCollection.RemoveAt(int index)
        {
            if (_listView != null)
            {
                _listView.RemoveAt(index);
            }
            else
            {
                this.RemoveAt(index);
            }
        }

        void IPlayItemCollection2.Start()
        {
            _listView = new ListView(this);
        }

        void IPlayItemCollection2.Stop()
        {
            _listView = null;
        }

        public IPlayItem NextPlayItem
        {
            get
            {
                if (_listView != null)
                {
                    return _listView.NextPlayItem;
                }
                return null;
            }
            set
            {
                if (_listView != null)
                {
                    _listView.NextPlayItem=value;
                }
            }
        }

        public IPlayItem CurrentPlayItem
        {
            get
            {
                if (_listView != null)
                {
                    return _listView.CurrentPlayItem;
                }
                return null;
            }
            set
            {
                if (_listView != null)
                {
                    _listView.CurrentPlayItem = value;
                }
            }
        }

        public bool CanDelete(IPlayItem playItem)
        {
            if (_listView != null)
            {
                return _listView.CanDelete(playItem);
            }

            return true;
        }

        public bool CanForcePlay(IPlayItem playItem)
        {
            if (_listView != null)
            {
                return _listView.CanForcePlay(playItem);
            }
            return false;
        }

        //public void ForcePlay(IPlayItem playItem)
        //{
        //    if (CanForcePlay(playItem))
        //    {
        //        _listView.ForcePlay(playItem);
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException();
        //    }

        //}

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            this.IsDirty = true;

            if (_listView != null)
            {
                _listView.IsDirty = true;
                _listView.PreLoadItem = null;
            }
        }

        
        public bool IsDirty
        {
            get; set;
        }

        public void SendPlaylistSyncRequest()
        {
            IPlayItem[] playItems = CopyPlayItems();

            _playControlService.SendPlaylistRequest(playItems, PlaylistRequestCategory.SyncPlaylist);
        }

        private IPlayItem[] CopyPlayItems()
        {
            IPlayItem[] playItems = new IPlayItem[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                playItems[i] = this[i].PlayItem;
            }

            return playItems;
        }

        public void ValidateTimeRange(DateTime startTime, TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public void ValidateTimeRange(DateTime startTime, TimeSpan duration, IPlayItem excludeItem)
        {
            throw new NotImplementedException();
        }

        public bool CanClear()
        {
            if (_listView == null)
            {
                return true;
            }
            else
            {
                return _listView.CanClear();
            }
        }

        public DateTime? GetStartTime()
        {
            if (_listView == null)
            {
                return null;
            }
            else
            {
                return _listView.GetStartTime();
            }
        }

        class ListView
        {
            private IList<BindablePlayItem> _playItemList;
            private int _offset;
            internal IPlayItem PreLoadItem { get; set; }

            public ListView(IList<BindablePlayItem> playItemList)
            {
                _offset = 0;
                _playItemList = playItemList;
            }

            private int CalcIndex(int index)
            {
                return _offset + index;
            }
            public BindablePlayItem this[int index]
            {
                get
                {
                    return _playItemList[CalcIndex(index)];
                }
            }

            public int Count
            {
                get { return _playItemList.Count - _offset; }
            }

            public void RemoveAt(int index)
            {
                _playItemList.RemoveAt(CalcIndex(index));
            }

            public void OnTimer()
            {
                TryGetPreLoadItem();
            }

            private void TryGetPreLoadItem()
            {
                if (PreLoadItem == null)
                {

                    DateTime now = DefaultDateTimeService.Instance.GetLocalNow();
                    var maxStartTime = now.AddMinutes(1);
                    var minStartTime = now.AddSeconds(1.1);

                    while (this.Count > 0)
                    {
                        IPlayItem playItem = this.Peek();
                        if (playItem.StartTime > maxStartTime)
                        {
                            break;
                        }

                        TimeSpan expiredDuration = TimeSpan.Zero;
                        if (playItem.StartTime < minStartTime)
                        {
                            expiredDuration = minStartTime.Subtract(playItem.StartTime);
                        }

                        
                        if (playItem.CalculatedPlayDuration - expiredDuration < PlayoutConfiguration.Current.MinPlayDuration)
                        {
                            // skip
                            this.Take();
                            continue;
                        }
                        else
                        {
                            PreLoadItem = playItem;
                            break;
                        }
                    }
                }
            }

            
            public void Insert(int index, BindablePlayItem bindablePlayItem)
            {
                _playItemList.Insert(CalcIndex(index),bindablePlayItem);
            }

            public void Clear()
            {
                for(int i = _playItemList.Count - 1; i >= _offset; i--)
                {
                    _playItemList.RemoveAt(i);
                }
            }

            

            public bool Contains(IPlayItem playItem)
            {
                for(int i = _offset; i < _playItemList.Count; i++)
                {
                    if (_playItemList[i].PlayItem == playItem)
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Add(BindablePlayItem bindablePlayItem)
            {
                _playItemList.Add(bindablePlayItem);
            }

            public IPlayItem GetNextPlayItem(DateTime minStartTime, DateTime maxStartTime)
            {
                IPlayItem result = null;
                //while (this.Count>0)
                //{
                //    IPlayItem playItem = this.Peek();
                //    if (playItem.StartTime > maxStartTime)
                //    {
                //        break;
                //    }

                //    TimeSpan expiredDuration=TimeSpan.Zero;
                //    if (playItem.StartTime < minStartTime)
                //    {
                //        expiredDuration = minStartTime.Subtract(playItem.StartTime);
                //    }

                //    this.Take();
                //    if (playItem.PlayDuration- expiredDuration < PlayoutConfiguration.Current.MinPlayDuration)
                //    {                        
                //        continue;
                //    }
                //    else
                //    {
                //        result = playItem;
                //        break;
                //    }       
                //}

                if (PreLoadItem!=null && PreLoadItem.StartTime <= maxStartTime)
                {
                    this.Take();
                    result = PreLoadItem;
                    PreLoadItem = null;
                }
                return result;
            }

            public IPlayItem NextPlayItem { get; set; }
            public IPlayItem CurrentPlayItem { get; set; }

            public bool CanForcePlay(IPlayItem playItem)
            {
                return this.Contains(playItem) && playItem.PlaybillItem.ScheduleMode == PlayScheduleMode.Auto;
            }

            //public void ForcePlay(IPlayItem playItem)
            //{
            //    if (CanForcePlay(playItem))
            //    {

            //    }
            //    throw new InvalidOperationException();
            //}

            private void Take()
            {
                _offset++;
            }

            private IPlayItem Peek()
            {
                return this[0].PlayItem;
            }

            public bool CanDelete(IPlayItem playItem)
            {
                return this.Contains(playItem);
            }

            internal void Reset()
            {
                _offset = 0;
                this.PreLoadItem = null;
            }

            internal bool CanClear()
            {
                return true;
            }

            internal DateTime? GetStartTime()
            {
                throw new NotImplementedException();
            }

            public bool IsDirty
            {
                get;set;
            }
        }
    }
}
