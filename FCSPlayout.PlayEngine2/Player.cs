using FCSPlayout.CG;
using FCSPlayout.Domain;
using MPLATFORMLib;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FCSPlayout.PlayEngine
{
    public partial class Player : IPlayer, ITimerAware
    {
        private MPlaylistClass _mplaylist;

        private IPlayPreview _preview;
        private ILog _log;
        private MRendererManager _rendererManager;
        private MRendererSettings _rendererInfo;

        private CGManager _cgManager;
        private MPlaylistSettings _playlistSettings;

        private IPlayerItem _nextItem;
        private IPlayerItem _currentItem;

        private IDateTimeService _dateTimeService;
        private IMediaFilePathResolver _filePathResolver;

        public event EventHandler<PlayerItemEventArgs> ItemLoaded;
        public event EventHandler<PlayerItemEventArgs> ItemStarted;
        public event EventHandler<PlayerItemEventArgs> ItemStopped;

        public Player(IPlayPreview preview, PlayoutSettings settings, ILog log, IDateTimeService dateTimeService,
            IMediaFilePathResolver filePathResolver)
        {
            _preview = preview;
            _log = log;
            _rendererInfo = settings.RendererSettings;
            _playlistSettings = settings.PlaylistSettings;
            _dateTimeService = dateTimeService;
            _filePathResolver = filePathResolver;
        }

        #region
        public void Load(IPlayerItem playerItem)
        {
            Debug.Assert(_nextItem == null);
            
            PlayerToken playToken = new PlayerToken(playerItem, _dateTimeService);
            if (IsMLSource(playerItem.MediaSource))
            {
                playToken.MPlaylist = _mplaylist;
                M_Load(playToken);
            }
            else
            {
                var adjustedRange = playerItem.MediaSource.Adjust(playToken.PlayRange);
                playToken.LoadRange = adjustedRange.Value;
            }

            _nextItem = playerItem;
            _nextItem.LoadTime = _dateTimeService.GetLocalNow();
            _nextItem.PlayerToken = playToken;
            OnItemLoaded(_nextItem);
        }

        public void PlayNext()
        {
            Debug.Assert(_nextItem != null);
            OnCurrentItemStopped();

            _nextItem.PlayerToken.Stopped += CurrentItem_Stopped;
            if (IsMLSource(_nextItem.MediaSource))
            {
                M_PlayNext();
            }

            _currentItem = _nextItem;
            _nextItem = null;
            _currentItem.StartTime = _dateTimeService.GetLocalNow();
            OnItemStarted(_currentItem);
        }

        public void OnTimer()
        {
            if (_currentItem != null)
            {
                _currentItem.PlayerToken.OnTimer();
            }
        }

        public void Start()
        {
            if (_mplaylist == null)
            {
                CreateMPlaylist();

                _mplaylist.OnEvent += MPlaylist_OnEvent;
                _mplaylist.ObjectStart(null);

                string objName = null;
                _mplaylist.ObjectNameGet(out objName);
                //_preview.SetPreviewUri(new Uri("mplatform://" + objName));
                _preview.MObject = _mplaylist;

                if (_rendererInfo != null && !string.IsNullOrEmpty(_rendererInfo.VideoDevice))
                {
                    _rendererManager = new MRendererManager(_rendererInfo/*, 1*/);
                    _rendererManager.AttachVideoDevice(_mplaylist);
                }

                _cgManager = new CGManager(_mplaylist);
                //GlobalEventAggregator.Instance.RaiseMPlaylistCreated(_mplaylist);
            }
        }

        public void Stop()
        {
            if (_mplaylist != null)
            {
                if (_nextItem != null) throw new InvalidOperationException();

                if (_currentItem != null)
                {
                    //if (IsMLSource(_currentItem.MediaSource))
                    //{
                    //    M_StopCurrent();
                    //}

                    OnCurrentItemStopped();
                }

                if (_rendererManager != null)
                {
                    _rendererManager.DetachVideoDevice();
                    _rendererManager.Dispose();
                    _rendererManager = null;
                }

                _preview.MObject = null;
                //_preview.SetPreviewUri(null);
                //GlobalEventAggregator.Instance.RaiseMPlaylistDestroying();

                _cgManager.Dispose();
                _cgManager = null;

                _mplaylist.ObjectClose();
                _mplaylist.OnEvent -= MPlaylist_OnEvent;
                Marshal.ReleaseComObject(_mplaylist);
                _mplaylist = null;

            }
        }

        public void Attach(CGItemCollection cgItems)
        {
            if (_mplaylist != null && _cgManager != null)
            {
                _cgManager.Attach(cgItems);
            }
        }

        public void Detach(CGItemCollection cgItems)
        {
            if (_mplaylist != null && _cgManager != null)
            {
                _cgManager.Detach(cgItems);
            }
        }

        public void Attach(CGItem cgItem)
        {
            if (_mplaylist != null && _cgManager != null)
            {
                _cgManager.Attach(cgItem);
            }
        }

        public void Detach(CGItem cgItem)
        {
            if (_mplaylist != null && _cgManager != null)
            {
                _cgManager.Detach(cgItem);
            }

        }

        #endregion



        private void OnCurrentItemStopped()
        {
            if (_currentItem != null)
            {
                _currentItem.PlayerToken.Stopped -= CurrentItem_Stopped;
                if (IsMLSource(_currentItem.MediaSource))
                {
                    M_OnCurrentItemStopped();
                }

                _currentItem.StopTime = _dateTimeService.GetLocalNow();
                _currentItem.PlayerToken.Dispose();
                var temp = _currentItem;
                _currentItem = null;
                OnItemStopped(temp);
            }
        }

        private void CurrentItem_Stopped(object sender, EventArgs e)
        {
            OnCurrentItemStopped();
        }

        private void OnItemLoaded(IPlayerItem item)
        {
            if (ItemLoaded != null)
            {
                ItemLoaded(this, new PlayerItemEventArgs(item));
            }
        }

        private void OnItemStarted(IPlayerItem item)
        {
            if (ItemStarted != null)
            {
                ItemStarted(this, new PlayerItemEventArgs(item));
            }
        }

        private void OnItemStopped(IPlayerItem item)
        {
            if (ItemStopped != null)
            {
                ItemStopped(this, new PlayerItemEventArgs(item));
            }
        }

        private void CreateMPlaylist()
        {
            _mplaylist = MPlaylistFactory.Current.Create(_playlistSettings);
        }

        

        private void MPlaylist_OnEvent(string bsChannelID, string bsEventName, string bsEventParam, object pEventObject)
        {
        }

        private void Log(string message)
        {
        }

        private PlayRange GetInOut(MItem mitem)
        {
            double dblIn = 0.0, dblOut = 0.0, dblTotal = 0.0;
            mitem.FileInOutGet(out dblIn, out dblOut, out dblTotal);

            if(dblOut==dblIn || dblOut == 0.0)
            {
                dblOut = dblTotal;
            }

            TimeSpan startPos = TimeSpan.FromSeconds(dblIn);
            TimeSpan duration = TimeSpan.FromSeconds(dblOut - dblIn); // TimeSpan.FromSeconds(dblTotal);

            return new PlayRange(startPos, duration);
        }

        private PlayRange SetInOut(MItem mitem, PlayRange playRange)
        {
            double dblIn = 0.0, dblOut = 0.0;

            dblIn = playRange.StartPosition.TotalSeconds;
            dblOut = playRange.StopPosition.TotalSeconds;

            mitem.FileInOutSet(dblIn, dblOut);

            double dblDuration;
            mitem.FileInOutGet(out dblIn, out dblOut, out dblDuration);
            if (dblOut == dblIn || dblOut == 0.0)
            {
                dblOut = dblDuration;
            }

            return new PlayRange(TimeSpan.FromSeconds(dblIn),TimeSpan.FromSeconds(dblOut-dblIn));
        }
        
        private void ApplyParameters(PlayerToken playToken)
        {
            MItem mitem = playToken.MItem;
            //IPlayParameters parameters = playToken.Parameters;
            //if (parameters != null)
            {
                //((IMProps)mitem).PropsSet("file::interlace", EnumUtils.GetInterlaceString(playToken.Parameters.MPFieldsType));
                //((IMProps)mitem).PropsSet("file::scale_type", EnumUtils.GetScaleTypeString(playToken.Parameters.StretchMode));
                //((IMProps)mitem).PropsSet("file::aspect_ratio", EnumUtils.GetAspectRatioString(playToken.Parameters.AspectRatio));
                if (playToken.HasAudio && IsMLSource(playToken.MediaSource))
                {
                    IFileMediaSource fileSource = playToken.MediaSource as IFileMediaSource;
                    
                    this.SetAudioGain((MItemClass)mitem, fileSource.AudioGain);
                }
            }

            // TODO: add other parameters
        }

        private void SetAudioGain(MItemClass mItem, int gain)
        {
            string empty = string.Empty;
            IMAudioTrack iMAudioTrack = null;
            mItem.AudioTrackGetByIndex(0, out empty, out iMAudioTrack);

            if (iMAudioTrack != null)
            {
                iMAudioTrack.TrackGainSet(-1, (double)gain, 0.1);
            }
        }

        

        private bool IsMLSource(IMediaSource mediaSource)
        {
            return mediaSource is IFileMediaSource;
        }

        private void M_Load(PlayerToken playToken)
        {
            Debug.Assert(_mplaylist != null);

            IFileMediaSource fileSource = (IFileMediaSource)playToken.MediaSource; // as IFileMediaSource;

            int index;

            MItem mitem = null;
            index = -1;
            _mplaylist.PlaylistAdd(null, _filePathResolver.Resolve(fileSource.FileName), string.Empty, ref index, out mitem);

            var adjustedRange = fileSource.Adjust(playToken.PlayRange);

            var loadRange = GetInOut(mitem);

            if (adjustedRange != null)
            {
                if (adjustedRange.Value.StartPosition != loadRange.StartPosition || adjustedRange.Value.StopPosition != loadRange.StopPosition)
                {
                    loadRange = SetInOut(mitem, adjustedRange.Value);
                }
            }
            

            playToken.LoadRange = loadRange;

            int audioTracks = -1;
            ((MItemClass)mitem).AudioTracksGetCount(out audioTracks);
            playToken.HasAudio = (audioTracks > 0);
            playToken.MItem = mitem;

            ApplyParameters(playToken);
        }

        //private void M_StopCurrent()
        //{
        //    ((PlayerToken)_currentItem.PlayerToken).MItem.FilePlayStop(0.0);
        //}

        private void M_PlayNext()
        {
            Debug.Assert(_mplaylist != null);

            ((PlayerToken)_nextItem.PlayerToken).MItem.FilePlayStart();

            if (_nextItem.CGItems != null)
            {
                _cgManager.Attach(_nextItem.CGItems);
            }
        }

        private void M_OnCurrentItemStopped()
        {
            if (_currentItem.CGItems != null)
            {
                _cgManager.Detach(_currentItem.CGItems);
            }

            if (((PlayerToken)_currentItem.PlayerToken).MItem != null)
            {
                _mplaylist.PlaylistRemove(((PlayerToken)_currentItem.PlayerToken).MItem);    
            }
        }
    }    
}
