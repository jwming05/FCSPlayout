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
        private IPlaylist3 _playlist;
        private ILoopPlayToken _loopPlayToken;
        private IMediaFilePathResolver _filePathResolver;

        public IPlaylist3 Playlist
        {
            get
            {
                return _playlist;
            }
        }

        public BindablePlayout(IPlayPreview preview, IPlaylist3 playlist, 
            IMediaFilePathResolver filePathResolver)
        {
            _preview = preview;
            _playlist = playlist;
            _filePathResolver = filePathResolver;
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

        //internal bool CanForcePlay(IPlayItem playItem)
        //{
        //    return _innerPlayout != null && _innerPlayout.CanForcePlay(playItem);
        //}

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
                var playout = new PlayoutSystem(_preview, _playlist, settings, this.Log, ChannelSwitcher.Instance,
                    _filePathResolver,settings.CGItems);
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
            result.CGItems= PlayoutRepository.GetCGItems();
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

        //internal void ForcePlay(IPlayItem playItem/*, Action<IPlayItem, PlayRange, IPlayItem> action*/)
        //{
        //    if (CanForcePlay(playItem))
        //    {
        //        _innerPlayout.ForcePlay(playItem/*, action*/);
        //        OnStateChanged();
        //    }
        //}

        internal void StartDelay()
        {
            if (CanStartDelay())
            {
                //_loopPlayToken= _innerPlayout.StartDelay(/*() => { }*/);
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

        internal void OnTimer()
        {
            if (_innerPlayout != null)
            {
                _innerPlayout.OnTimer();
                UpdatePlaylistPosition();

                if (_preview.CurrentPlayItem != null)
                {
                    _preview.CurrentPlayItemPosition = _preview.CurrentPlayItem.Position;
                    return;
                }
            }

            _preview.CurrentPlayItemPosition = TimeSpan.Zero;
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

        public CG.CGItemCollection CGItems
        {
            get;set;
        }
    }
}
