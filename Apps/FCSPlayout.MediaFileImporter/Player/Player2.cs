using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using MPLATFORMLib;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FCSPlayout.MediaFileImporter
{
    public class Player2 : IDisposable
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
        private PlayerStatus _status;
        private MItem _mitem = null;
        MPlaylistSettings _mplaylistSettings;
        private IPlayableItem _playableItem;
        private double _rate=1.0;

        public event EventHandler Opened;
        public event EventHandler Closed;

        public Player2(MPlaylistSettings mplaylistSettings)
        {
            _mplaylistSettings = mplaylistSettings;
        }
        internal PlayerStatus Status
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
        internal void SetAudioGain(int audioGain)
        {
            if (_mitem != null)
            {
                this.SetAudioGain((MItemClass)_mitem, audioGain);
            }
        }

        public void Play()
        {
            if (_mitem != null &&
                (this.Status == PlayerStatus.Paused || this.Status == PlayerStatus.Stopped))
            {
                if(this.Status == PlayerStatus.Stopped)
                {
                    _mplaylist.FilePosSet(0.0, 0.0);
                }
                _mplaylist.FilePlayStart();
                this.Status = PlayerStatus.Running;
            }
        }

        public void Pause()
        {
            if (_mitem != null && this.Status == PlayerStatus.Running)
            {
                _mplaylist.FilePlayPause(0.0);

                this.Status = PlayerStatus.Paused;
            }
        }

        public void Stop()
        {
            if (_mitem != null &&
                (this.Status == PlayerStatus.Paused || this.Status == PlayerStatus.Running))
            {
                _mplaylist.FilePlayStop(0.0);
                //_mplaylist.FilePosSet(0.0, 0.0);
                this.Status = PlayerStatus.Stopped;
            }
        }

        public void Close()
        {
            if (_mplaylist != null && this.Status != PlayerStatus.Closed)
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

                this.Status = PlayerStatus.Closed;

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

            this.Status = PlayerStatus.Stopped;

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
                if (this.Status != PlayerStatus.Closed)
                {
                    this.Close();
                }

                Marshal.ReleaseComObject(_mplaylist);
                _mplaylist = null;
            }
        }

        internal double GetPosition()
        {
            if (_mitem != null && this.Status != PlayerStatus.Closed && this.Status != PlayerStatus.Stopped)
            {
                double dblPos = 0.0;
                //_mplaylist.PlaylistPosGet(out index, out nextIndex, out dblPos, out dblListPos);
                _mplaylist.FilePosGet(out dblPos);
                return dblPos;
            }
            return 0.0;
        }

        internal void SetPosition(double pos)
        {
            if (_mitem != null && this.Status != PlayerStatus.Closed && this.Status != PlayerStatus.Stopped)
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
    }
}
