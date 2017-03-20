using FCSPlayout.CG;
using FCSPlayout.Domain;
using System;
using System.Diagnostics;

namespace FCSPlayout.PlayEngine
{
    public class PlayScheduler
    {
        private IDateTimeService _dateTimeService;
        private IPlaylist2 _playlist;
        private IPlayer _player;

        private PlayItemWrapper _nextPlayItem;
        private PlayItemWrapper _currentPlayItem;

        private IChannelSwitcher _switcher;
        public event EventHandler CurrentPlayItemChanged;
        public event EventHandler NextPlayItemChanged;

        public PlayScheduler(IPlayer player, IPlaylist2 playlist, IDateTimeService dateTimeService, IChannelSwitcher switcher)
        {
            _player = player;
            _player.ItemLoaded += Player_ItemLoaded;
            _player.ItemStarted += Player_ItemStarted;
            _player.ItemStopped += Player_ItemStopped;
            _playlist = playlist;
            _dateTimeService = dateTimeService;
            _switcher = switcher;
        }

        private void Player_ItemStopped(object sender, PlayerItemEventArgs e)
        {
            this.CurrentPlayItem.OnStopped();
            this.CurrentPlayItem = null;
        }

        private void Player_ItemStarted(object sender, PlayerItemEventArgs e)
        {
            this.NextPlayItem = null;
            this.CurrentPlayItem = (PlayItemWrapper)e.PlayerItem;
            this.CurrentPlayItem.OnStarted();
        }

        private void Player_ItemLoaded(object sender, PlayerItemEventArgs e)
        {
            this.NextPlayItem = (PlayItemWrapper)e.PlayerItem;
            this.NextPlayItem.OnLoaded();
        }

        internal void OnTimer()
        {
            _player.OnTimer();

            this.Schedule();
        }

        internal PlayItemWrapper NextPlayItem
        {
            get
            {
                return _nextPlayItem;
            }

            private set
            {
                _nextPlayItem = value;
                OnNextPlayItemChanged();
            }
        }

        internal void Start()
        {
            _player.Start();
        }

        internal void Stop()
        {
            _player.Stop();
        }

        internal PlayItemWrapper CurrentPlayItem
        {
            get
            {
                return _currentPlayItem;
            }

            private set
            {
                //if (_currentPlayItem != null)
                //{
                //    _currentPlayItem.OnStopped();
                //}

                _currentPlayItem = value;
                OnCurrentPlayItemChanged();
            }
        }

        private void OnCurrentPlayItemChanged()
        {
            if (CurrentPlayItemChanged != null)
            {
                CurrentPlayItemChanged(this, EventArgs.Empty);
            }
        }

        private void OnNextPlayItemChanged()
        {
            if (NextPlayItemChanged != null)
            {
                NextPlayItemChanged(this, EventArgs.Empty);
            }
        }

        

        private void Schedule()
        {
            if (this.NextPlayItem != null)
            {
                if (ShouldPlayNext())
                {
                    PlayNext();
                }
            }
            else
            {
                TryLoadNextPlayItem();
            }
        }

        

        private bool ShouldPlayNext()
        {
            DateTime now = _dateTimeService.GetLocalNow();
            return this.NextPlayItem.ExpectedPlayTime.Subtract(now) <= PlayoutConfiguration.Current.PlayTimeTolerance;
        }

        private void PlayNext()
        {
            if (this.NextPlayItem != null)
            {
                _player.PlayNext();
                _switcher.SwitchChannelFor(this.CurrentPlayItem.PlayItem.PlaybillItem.PlaySource.MediaSource);
            }
        }

        private bool TryLoadNextPlayItem()
        {
            DateTime now = _dateTimeService.GetLocalNow();
            DateTime minStartTime = now.Add(PlayoutConfiguration.Current.MinLoadDelay);
            DateTime maxStartTime = now.Add(PlayoutConfiguration.Current.MaxLoadDelay);

            var item = _playlist.GetNextPlayItem(minStartTime, maxStartTime);
            if (item == null) return false;


            PlayItemWrapper nextItem = null;
            if (item.StartTime < minStartTime)
            {
                nextItem = new PlayItemWrapper(item, minStartTime/*, true*/);
            }
            else
            {
                nextItem = new PlayItemWrapper(item);
            }

            _player.Load(nextItem);
            return true;
        }

        internal class PlayItemWrapper : IPlayerItem
        {
            //private IPlayItem _playItem;
            private PlayRange _playRange;
            private IPlayerToken _playerToken;

            public PlayItemWrapper(IPlayItem item)
            {
                this.PlayItem = item;
                _playRange =new PlayRange(item.PlayRange.StartPosition,item.PlayDuration);

                this.ExpectedPlayTime = item.StartTime;
                if (item.PlaybillItem.PlaySource.CGItems != null)
                {
                    this.CGItems = item.PlaybillItem.PlaySource.CGItems.Clone();
                }
                
                this.MediaSource = item.PlaybillItem.PlaySource.MediaSource.Clone();
            }

            public PlayItemWrapper(IPlayItem item, DateTime expectedPlayTime) : this(item)
            {
                Debug.Assert(expectedPlayTime > item.StartTime);

                this.ExpectedPlayTime = expectedPlayTime;

                //var oldRange = item.PlayRange;
                TimeSpan delta = expectedPlayTime.Subtract(item.StartTime);

                var newStartPos = _playRange.StartPosition + delta;

                _playRange = new PlayRange(newStartPos, _playRange.StopPosition - newStartPos);
            }

            public DateTime ExpectedPlayTime { get; private set; }

            public PlayRange PlayRange
            {
                get
                {
                    return _playRange;
                }
            }

            #region Internal Members
            public DateTime StartTime { get; set; }
            public DateTime StopTime { get; set; }
            public DateTime LoadTime
            {
                get; set;
            }

            public TimeSpan Position
            {
                get
                {
                    return (this.PlayerToken == null) ? TimeSpan.Zero : this.PlayerToken.Position;
                    //var duration = DateTime.Now.Subtract(this.StartTime);
                    //return duration > this.PlayRange.Duration ? this.PlayRange.Duration : duration;
                }
            }
            internal PlayerItemStatus Status
            {
                get;private set;
                //get { return _playItem.Status; }
                //private set
                //{
                //    _playItem.Status = value;
                //}
            }

            public CGItemCollection CGItems
            {
                //get
                //{
                //    return _playItem.PlaybillItem.PlaySource.CGItems;
                //}
                get;
                private set;
            }

            public IMediaSource MediaSource
            {
                //get
                //{
                //    return _playItem.PlaybillItem.PlaySource.MediaSource; 
                //}
                get;
                private set;
            }

            //IPlayParameters IPlayerItem.PlayParameters
            //{
            //    get
            //    {
            //        return _playItem.Parameters;
            //    }
            //}

            //public string Title
            //{
            //    //get { return _playItem.PlaybillItem.PlaySource.Title; }
            //}

            public IPlayItem PlayItem
            {
                //get
                //{
                //    return _playItem;
                //}
                get;private set;
            }

            internal void SetStatus(PlayerItemStatus status)
            {
                this.Status = status;
            }

            internal void OnLoaded()
            {
                this.SetStatus(PlayerItemStatus.Loaded);
            }

            internal void OnStarted()
            {
                this.SetStatus(PlayerItemStatus.Started);

                // TODO:添加播放记录
                
                //var playRange = this.PlayItem.PlayRange;
            }

            internal void OnStopped()
            {
                this.SetStatus(PlayerItemStatus.Stopped);

                //var record=new FCSPlayou
                PlayoutRecordService.Current.Add(this);

                
                //this.PlayToken = null; ???

                // TODO:更新播放记录
            }
            #endregion

            public IPlayerToken PlayerToken
            {
                get { return _playerToken; }
                set
                {
                    _playerToken = value;
                    if (_playerToken != null)
                    {
                        this.LoadRange = _playerToken.LoadRange;
                    }  
                }
            }
            public PlayRange LoadRange { get; private set; }
        }
    }
}
