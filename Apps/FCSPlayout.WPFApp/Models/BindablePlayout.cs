using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.PlayEngine;
using System;

namespace FCSPlayout.WPFApp.Models
{
    public class BindablePlayout
    {
        private PlayoutSystem _innerPlayout;
        private IPlayPreview _preview;
        //private IPlayItemPlayRecorder _playRecorder;
        private IPlaylist2 _playlist;
        private ILoopPlayToken _loopPlayToken;

        public IPlaylist2 Playlist
        {
            get
            {
                return _playlist;
            }

            set
            {
                _playlist = value;
            }
        }

        public BindablePlayout(IPlayPreview preview/*,IPlayItemPlayRecorder playRecorder*/)
        {
            _preview = preview;
            //_playRecorder = playRecorder;            
        }

        #region
        public bool CanStart()
        {
            return _playlist != null && _innerPlayout == null;
        }

        public bool CanStop()
        {
            return _innerPlayout != null;
        }

        internal bool CanForcePlay(IPlayItem playItem)
        {
            return _innerPlayout != null && _innerPlayout.CanForcePlay(playItem);
        }

        internal bool CanStopDelay()
        {
            return _innerPlayout != null && _loopPlayToken!=null && _innerPlayout.CanStopDelay();
        }

        internal bool CanStartDelay()
        {
            return _innerPlayout != null && _loopPlayToken==null && _innerPlayout.CanStartDelay();
        }
        #endregion

        #region
        public void Start()
        {
            if (_innerPlayout == null)
            {

                PlayoutSettings settings = GetPlayoutSettings();
                var playout = new PlayoutSystem(_preview, /*_playRecorder,*/ _playlist, settings, this.Log, /*new ChannelSwitcher()*/ChannelSwitcher.Instance);
                playout.CurrentPlayItemChanged += Playout_CurrentPlayItemChanged;
                playout.NextPlayItemChanged += Playout_NextPlayItemChanged;
                playout.Start();
                _innerPlayout = playout;

                OnStateChanged();
            }
        }

        private void Playout_NextPlayItemChanged(object sender, PlayerItemEventArgs e)
        {
        }

        private void Playout_CurrentPlayItemChanged(object sender, PlayerItemEventArgs e)
        {
            _preview.CurrentPlayItem = e.PlayerItem;
            OnStateChanged();
        }

        private PlayoutSettings GetPlayoutSettings()
        {
            var result = new PlayoutSettings();
            result.RendererSettings.VideoDevice= Properties.Settings.Default.RendererDeviceName;
            result.PlaylistSettings = PlayoutRepository.GetMPlaylistSettings();
            return result;
        }

        

        public void Stop()
        {
            if (_innerPlayout != null)
            {
                var playout = _innerPlayout;
                _innerPlayout = null;
                playout.CurrentPlayItemChanged -= Playout_CurrentPlayItemChanged;
                playout.NextPlayItemChanged -= Playout_NextPlayItemChanged;

                playout.Stop();
                playout.Dispose();

                OnStateChanged();
            }
        }

        internal void ForcePlay(IPlayItem playItem/*, Action<IPlayItem, PlayRange, IPlayItem> action*/)
        {
            if (CanForcePlay(playItem))
            {
                _innerPlayout.ForcePlay(playItem/*, action*/);
                OnStateChanged();
            }
        }

        internal void StartDelay()
        {
            if (CanStartDelay())
            {
                _loopPlayToken= _innerPlayout.StartDelay(/*() => { }*/);
                //OnStateChanged();
            }
        }

        internal void StopDelay()
        {
            if (CanStopDelay())
            {
                _loopPlayToken.Stop();
                _loopPlayToken = null;

                //_innerPlayout.StopDelay();
                //OnStateChanged();
            }
        }
        #endregion

        //private void Playout_ScheduleStateChanged(object sender, EventArgs e)
        //{
        //    OnStateChanged();
        //    //throw new NotImplementedException();
        //}

        //private void Playout_PlayItemStopped(object sender, PlayItemEventArgs e)
        //{
        //    _preview.CurrentPlayItem = null;
        //    OnStateChanged();
        //}

        //private void Playout_PlayItemStarted(object sender, PlayItemEventArgs e)
        //{
        //    _preview.CurrentPlayItem = e.PlayItem;
        //    OnStateChanged();
        //}

        internal void OnTimer()
        {
            if (_innerPlayout != null)
            {
                _innerPlayout.OnTimer();
                UpdatePlaylistPosition();

                if (_preview.CurrentPlayItem != null)
                {
                    _preview.CurrentPlayItemPosition = _preview.CurrentPlayItem.Position;
                }
            }
        }

        private void UpdatePlaylistPosition()
        {
            //_preview.PlaylistPosition = Math.Min(_playlist.Duration.TotalSeconds, Math.Max(0.0, DateTime.Now.Subtract(_playlist.StartTime).TotalSeconds));
        }

        internal ILog Log { get; set; }


        internal event EventHandler StateChanged;

        private void OnStateChanged()
        {
            if (StateChanged != null)
            {
                StateChanged(this, EventArgs.Empty);
            }
        }
    }
}
