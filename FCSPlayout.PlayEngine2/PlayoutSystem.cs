using FCSPlayout.CG;
using FCSPlayout.Domain;
using System;

namespace FCSPlayout.PlayEngine
{
    public class PlayoutSystem:IDisposable
    {
        private PlayScheduler _scheduler;
        private IPlayerItem _nextItem;
        private IPlayerItem _currentItem;
        private IPlaylist3 _playlist;
        private LoopPlayToken _loopPlayToken;
        private bool _isRunning=false;
        private bool _forcePlayRequest = false;

        public event EventHandler<PlayerItemEventArgs> CurrentPlayItemChanged;
        public event EventHandler<PlayerItemEventArgs> NextPlayItemChanged;

        public PlayoutSystem(IPlayPreview preview, IPlaylist3 playlist,
            PlayoutSettings settings, ILog log,IChannelSwitcher switcher, IMediaFilePathResolver filePathResolver,
            CGItemCollection cgItems)
        {
            _playlist = playlist;
            var player = new Player(preview, settings, log, DefaultDateTimeService.Instance,filePathResolver);
            _scheduler = new PlayScheduler(player,playlist, DefaultDateTimeService.Instance, switcher,cgItems);
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
            if (_isRunning && _loopPlayToken == null)
            {
                _playlist.OnTimer();
                _scheduler.OnTimer();

                if (_forcePlayRequest)
                {
                    _forcePlayRequest = false;
                }
            }
        }

        public void Start()
        {
            _playlist.Start();
            _scheduler.Start();

            _isRunning = true;
        }

        public void Stop()
        {
            _scheduler.Stop();
            _playlist.Stop();

            _isRunning = false;
            _loopPlayToken = null;
            _forcePlayRequest = false;
        }

        public void Dispose()
        {
        }

        #region
        public bool CanStopDelay(ILoopPlayToken token=null)
        {
            return _isRunning && !_forcePlayRequest && _loopPlayToken != null && _loopPlayToken==token;
        }

        public bool CanStartDelay()
        {
            return _isRunning && !_forcePlayRequest && _loopPlayToken == null && _nextItem == null && _currentItem != null && _currentItem.MediaSource.Category == MediaSourceCategory.External;
        }

        //public bool CanForcePlay(IPlayItem playItem)
        //{
        //    return _isRunning && !_forcePlayRequest && _loopPlayToken == null && 
        //        _nextItem == null && _currentItem != null && _currentItem.PlayItem != playItem && _playlist.CanForcePlay(playItem);
        //}

        public bool StartDelay(out ILoopPlayToken token)
        {
            token = null;
            if (CanStartDelay())
            {
                if (_currentItem.PlayerToken.RemainTime >= TimeSpan.FromSeconds(2.0))
                {
                    _loopPlayToken = new LoopPlayToken();
                    _loopPlayToken.RequestStop += LoopPlayToken_RequestStop;
                    token = _loopPlayToken;
                    return true;
                }
            }

            return false;
        }

        private void LoopPlayToken_RequestStop(object sender, EventArgs e)
        {
            StopDelay(sender as ILoopPlayToken);
        }

        private void StopDelay(ILoopPlayToken token)
        {
            if (CanStopDelay(token))
            {
                _loopPlayToken.RequestStop -= LoopPlayToken_RequestStop;
                _loopPlayToken = null;
            }
            else
            {
                throw new InvalidOperationException();
            }
            
        }

        //public bool ForcePlay(IPlayItem playItem)
        //{
        //    bool result = false;
        //    if (CanForcePlay(playItem))
        //    {
        //        DateTime now = DefaultDateTimeService.Instance.GetLocalNow();
        //        var remainDuration = _currentItem.LoadRange.Duration - now.Subtract(_currentItem.StartTime);
        //        if (remainDuration >= TimeSpan.FromSeconds(2.0))
        //        {
        //            using (var editor = (IPlaylistEditor2)_playlist.Edit())
        //            {
        //                result= editor.ForcePlay(playItem, now.AddSeconds(1.0));
        //            }

        //            if (result)
        //            {
        //                _forcePlayRequest = true;
        //                //this.OnTimer();
        //            }
        //        }
        //    }
        //    return result;
        //}
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
