using FCSPlayout.CG;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using MPLATFORMLib;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FCSPlayout.WPF.Core
{
    public class PreviewPlayer : IDisposable
    {
        public event EventHandler StatusChanged;
        private void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }

        private MPlaylistClass _mplaylist;
        private PreviewPlayerStatus _status;
        private MItem _mitem = null;
        MPlaylistSettings _mplaylistSettings;
        private IPlayableItem _playableItem;
        private double _rate=1.0;
        private CGManager _cgManager;

        public event EventHandler Opened;
        public event EventHandler Closed;

        public PreviewPlayer(MPlaylistSettings mplaylistSettings)
        {
            _mplaylistSettings = mplaylistSettings;
        }
        public PreviewPlayerStatus Status
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    OnStatusChanged();
                }
            }
        }

        public object PlayerObject { get { return _mplaylist; } }

        public string Name
        {
            get; private set;
        }

        public double Duration { get; private set; }
        public void SetAudioGain(int audioGain)
        {
            if (_mitem != null)
            {
                this.SetAudioGain((MItemClass)_mitem, audioGain);
            }
        }

        public void Play()
        {
            if (_mitem != null &&
                (this.Status == PreviewPlayerStatus.Paused || this.Status == PreviewPlayerStatus.Stopped))
            {
                if(this.Status == PreviewPlayerStatus.Stopped)
                {
                    _mplaylist.FilePosSet(0.0, 0.0);
                }
                _mplaylist.FilePlayStart();
                this.Status = PreviewPlayerStatus.Running;
            }
        }

        public void Pause()
        {
            if (_mitem != null && this.Status == PreviewPlayerStatus.Running)
            {
                _mplaylist.FilePlayPause(0.0);

                this.Status = PreviewPlayerStatus.Paused;
            }
        }

        public void Stop()
        {
            if (_mitem != null &&
                (this.Status == PreviewPlayerStatus.Paused || this.Status == PreviewPlayerStatus.Running))
            {
                _mplaylist.FilePlayStop(0.0);
                //_mplaylist.FilePosSet(0.0, 0.0);
                this.Status = PreviewPlayerStatus.Stopped;
            }
        }

        public void Close()
        {
            if (_mplaylist != null && this.Status != PreviewPlayerStatus.Closed)
            {
                if (_mitem != null)
                {
                    _mplaylist.FilePlayStop(0.0);

                    _mplaylist.PlaylistRemove(_mitem);

                    Marshal.ReleaseComObject(_mitem);
                    _mitem = null;
                }
                _mplaylist.ObjectClose();

                this.Duration = 0.0;
                this.Name = null;

                this.Status = PreviewPlayerStatus.Closed;

                OnClosed();
            }
        }

        public void Open(IPlayableItem playableItem)
        {
            this.Close();

            if (_mplaylist == null)
            {
                _mplaylist = new MPlaylistClass();
                if (_mplaylistSettings.VideoFormat != null)
                {
                    SetVideoFormat(_mplaylist, _mplaylistSettings.VideoFormat);
                }

                if (_mplaylistSettings.AudioFormat != null)
                {
                    SetAudioFormat(_mplaylist, _mplaylistSettings.AudioFormat);
                }
                _mplaylist.PropsSet("loop", "true");
                _mplaylist.OnEvent += MFile_OnEvent;

                _cgManager = new CGManager(_mplaylist);
                //_mplaylist.FileRateSet(_rate);
            }

            //_fileName = fileName;
            _playableItem = playableItem;
            //_fileName = _playableItem.FilePath;
            int index = -1;

            _mplaylist.PlaylistAdd(null, _playableItem.FilePath, "", ref index, out _mitem);
            _mplaylist.ObjectStart(null);

            _mplaylist.FileRateSet(_rate);

            SetAudioGain((MItemClass)_mitem, _playableItem.AudioGain);


            double dblIn = 0.0, dblOut = 0.0, dblDuration = 0.0;
            _mplaylist.FileInOutGet(out dblIn, out dblOut, out dblDuration);

            //_mitem.FileInOutGet(out dblIn, out dblOut, out dblDuration);

            this.Duration = dblDuration;

            string name = null;
            _mplaylist.ObjectNameGet(out name);
            this.Name = name;

            this.Status = PreviewPlayerStatus.Stopped;

            OnOpened();
        }

        private void OnOpened()
        {
            if (this.Opened != null)
            {
                this.Opened(this, EventArgs.Empty);
            }
        }

        private void OnClosed()
        {
            if (this.Closed != null)
            {
                this.Closed(this, EventArgs.Empty);
            }
        }

        private void MFile_OnEvent(string bsChannelID, string bsEventName, string bsEventParam, object pEventObject)
        {
            Debug.WriteLine("OnEvent线程：" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        public void Dispose()
        {
            if (_mplaylist != null)
            {
                if (this.Status != PreviewPlayerStatus.Closed)
                {
                    this.Close();
                }

                _cgManager.Dispose();
                _cgManager = null;

                Marshal.ReleaseComObject(_mplaylist);
                _mplaylist = null;
            }
        }

        public double GetPosition()
        {
            if (_mitem != null && this.Status != PreviewPlayerStatus.Closed && this.Status != PreviewPlayerStatus.Stopped)
            {
                double dblPos = 0.0;
                //_mplaylist.PlaylistPosGet(out index, out nextIndex, out dblPos, out dblListPos);
                _mplaylist.FilePosGet(out dblPos);
                return dblPos;
            }
            return 0.0;
        }

        public void SetPosition(double pos)
        {
            if (_mitem != null && this.Status != PreviewPlayerStatus.Closed && this.Status != PreviewPlayerStatus.Stopped)
            {
                //_mplaylist.PlaylistPosSet(0, pos, 0.0);
                _mplaylist.FilePosSet(pos, 0.0);

            }
        }

        private static void SetAudioFormat(MPlaylistClass mplaylist, string formatName)
        {
            int count = 0;
            mplaylist.FormatAudioGetCount(eMFormatType.eMFT_Convert, out count);
            int index = -1;
            MPLATFORMLib.M_AUD_PROPS audProps = new MPLATFORMLib.M_AUD_PROPS();
            string name;
            for (int i = 0; i < count; i++)
            {
                mplaylist.FormatAudioGetByIndex(eMFormatType.eMFT_Convert, i, out audProps, out name);
                if (string.Equals(name, formatName, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                mplaylist.FormatAudioSet(eMFormatType.eMFT_Convert, ref audProps);
            }
        }

        private static void SetVideoFormat(MPlaylistClass mplaylist, string formatName)
        {
            int count = 0;
            mplaylist.FormatVideoGetCount(eMFormatType.eMFT_Convert, out count);
            int index = -1;
            MPLATFORMLib.M_VID_PROPS vidProps = new MPLATFORMLib.M_VID_PROPS();
            string name;
            for (int i = 0; i < count; i++)
            {
                mplaylist.FormatVideoGetByIndex(eMFormatType.eMFT_Convert, i, out vidProps, out name);
                if (string.Equals(name, formatName, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                mplaylist.FormatVideoSet(eMFormatType.eMFT_Convert, ref vidProps);
            }
        }

        public void SetRate(double rate)
        {
            if (_rate != rate)
            {
                _rate = rate;
                if (_mplaylist != null && _mitem!=null)
                {
                    _mplaylist.FileRateSet(_rate);
                }
            }            
        }

        private void SetAudioGain(MItemClass mItem, int gain)
        {
            int audioTracks = -1;
            ((MItemClass)mItem).AudioTracksGetCount(out audioTracks);
            if(audioTracks > 0)
            {
                string empty = string.Empty;
                IMAudioTrack iMAudioTrack = null;
                mItem.AudioTrackGetByIndex(0, out empty, out iMAudioTrack);

                if (iMAudioTrack != null)
                {
                    iMAudioTrack.TrackGainSet(-1, (double)gain, 0.1);
                }
            }
        }

        #region
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
    }
}
