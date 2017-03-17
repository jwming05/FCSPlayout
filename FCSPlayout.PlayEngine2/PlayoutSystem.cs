using FCSPlayout.Domain;
using System;

namespace FCSPlayout.PlayEngine
{
    public class PlayoutSystem:IDisposable
    {
        private PlayScheduler _scheduler;
        private IPlayerItem _nextItem;
        private IPlayerItem _currentItem;
        private IPlaylist2 _playlist;
        private LoopPlayToken _loopPlayToken;

        public event EventHandler<PlayerItemEventArgs> CurrentPlayItemChanged;
        public event EventHandler<PlayerItemEventArgs> NextPlayItemChanged;

        public PlayoutSystem(IPlayPreview preview, IPlaylist2 playlist,
            PlayoutSettings settings, ILog log,IChannelSwitcher switcher)
        {
            _playlist = playlist;
            var player = new Player(preview, settings, log, DefaultDateTimeService.Instance);
            _scheduler = new PlayScheduler(player,playlist, DefaultDateTimeService.Instance, switcher);
            _scheduler.CurrentPlayItemChanged += OnCurrentPlayItemChanged;
            _scheduler.NextPlayItemChanged += OnNextPlayItemChanged;
        }

        private void OnNextPlayItemChanged(object sender, EventArgs e)
        {
            _nextItem = _scheduler.NextPlayItem;
            if (_nextItem != null)
            {
                _playlist.NextItem = _nextItem.PlayItem;
            }
            else
            {
                _playlist.NextItem = null;
            }

            if (this.NextPlayItemChanged != null)
            {
                this.NextPlayItemChanged(this, new PlayerItemEventArgs(_nextItem));
            }
        }

        private void OnCurrentPlayItemChanged(object sender, EventArgs e)
        {
            _currentItem = _scheduler.CurrentPlayItem;
            if (_currentItem != null)
            {
                _playlist.CurrentItem = _currentItem.PlayItem;
            }
            else
            {
                _playlist.CurrentItem = null;
            }

            if (this.CurrentPlayItemChanged != null)
            {
                this.CurrentPlayItemChanged(this, new PlayerItemEventArgs(_currentItem));
            }
        }

        public void OnTimer()
        {
            if (_loopPlayToken == null)
            {
                _playlist.OnTimer();
                _scheduler.OnTimer();
            }
        }

        public void Start()
        {
            _playlist.Start();
            _scheduler.Start();
        }

        public void Stop()
        {
            _scheduler.Stop();
            _playlist.Stop();
        }

        public void Dispose()
        {
        }

        #region
        public bool CanStopDelay()
        {
            return _loopPlayToken != null;
        }

        public bool CanStartDelay()
        {
            return _loopPlayToken==null && _nextItem == null && _currentItem != null && _playlist.CanEnterLoop(_currentItem.PlayItem);
        }

        public bool CanForcePlay(IPlayItem playItem)
        {
            return _loopPlayToken == null && _nextItem == null && _currentItem != null && _playlist.CanForcePlay(playItem);
        }

        public ILoopPlayToken StartDelay()
        {
            if (CanStartDelay())
            {
                _loopPlayToken = new LoopPlayToken();
                _loopPlayToken.RequestStop += LoopPlayToken_RequestStop;
                return _loopPlayToken;
            }

            throw new InvalidOperationException();
        }

        private void LoopPlayToken_RequestStop(object sender, EventArgs e)
        {
            StopDelay();
        }

        private void StopDelay()
        {
            if (CanStopDelay())
            {
                _loopPlayToken.RequestStop -= LoopPlayToken_RequestStop;
                _loopPlayToken = null;
            }
        }

        public void ForcePlay(IPlayItem playItem)
        {
            if (CanForcePlay(playItem))
            {
                _playlist.ForcePlay(playItem);
            }
        }
        #endregion

        class LoopPlayToken : ILoopPlayToken
        {
            public event EventHandler RequestStop;
            public void Stop()
            {
                if (RequestStop != null)
                {
                    RequestStop(this, EventArgs.Empty);
                }
            }
        }
    }
}
