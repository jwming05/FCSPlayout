using FCSPlayout.Domain;
using MPLATFORMLib;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace FCSPlayout.WPF.Core
{
    internal partial class PlayControlModel2 : BindableBase
    {
        private Player2 _player;
        private string _fileName;

        private readonly DelegateCommand _playCommand;
        private readonly DelegateCommand _pauseCommand;
        private readonly DelegateCommand _stopCommand;
        private readonly DelegateCommand _setInCommand;
        private readonly DelegateCommand _setOutCommand;
        private readonly DelegateCommand _goInCommand;
        private readonly DelegateCommand _goOutCommand;
        private readonly DelegateCommand _nextFrameCommand;
        private readonly DelegateCommand _previousFrameCommand;

        private double _maxPosition;
        private string _sourceName;
        private DispatcherTimer _timer;
        private PlayRange _playRange;
        private double _playRate=1.0;

        public PlayControlModel2(DispatcherTimer timer, MPlaylistSettings mplaylistSettings)
        {
            _timer = timer;

            _timer.Tick += Timer_Tick;
            
            _player = new Player2(mplaylistSettings);
            _player.StatusChanged += Player_StatusChanged;

            _playCommand = new DelegateCommand(Play, CanPlay);
            _pauseCommand = new DelegateCommand(Pause, CanPause);
            _stopCommand = new DelegateCommand(Stop, CanStop);

            _setInCommand = new DelegateCommand(SetIn, CanSetIn);
            _setOutCommand = new DelegateCommand(SetOut, CanSetOut);

            _goInCommand = new DelegateCommand(GoIn, CanGoIn);
            _goOutCommand = new DelegateCommand(GoOut, CanGoOut);

            _nextFrameCommand = new DelegateCommand(GoNextFrame, CanGoNextFrame);
            _previousFrameCommand = new DelegateCommand(GoPreviousFrame, CanGoPreviousFrame);
        }

        public object PlayerObject { get { return _player.PlayerObject; } }

        public event EventHandler Opened
        {
            add { _player.Opened += value; }
            remove { _player.Opened -= value; }
        }

        public event EventHandler Closed
        {
            add { _player.Closed += value; }
            remove { _player.Closed -= value; }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_player != null && _player.Status == PlayerStatus.Running)
            {
                OnPropertyChanged<double>(() => this.Position);
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_player.Status != PlayerStatus.Closed)
                {
                    _player.Close();
                }
                _fileName = value;

                if (!string.IsNullOrEmpty(_fileName) && System.IO.File.Exists(_fileName))
                {
                    _player.Open(_fileName);

                    _player.SetRate(this.PlayRate);
                }
            }
        }

        public double PlayRate
        {
            get { return _playRate; }
            set
            {
                if (_playRate != value)
                {
                    _playRate = value;

                    if(_player.Status != PlayerStatus.Closed)
                    {
                        _player.SetRate(_playRate);
                    }

                    this.OnPropertyChanged(() => this.PlayRate);
                }
            }
        }

        public ICommand PlayCommand
        {
            get
            {
                return _playCommand;
            }
        }

        public ICommand PauseCommand
        {
            get
            {
                return _pauseCommand;
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return _stopCommand;
            }
        }

        public ICommand SetInCommand
        {
            get
            {
                return _setInCommand;
            }
        }

        public ICommand SetOutCommand
        {
            get
            {
                return _setOutCommand;
            }
        }

        public ICommand GoInCommand
        {
            get
            {
                return _goInCommand;
            }
        }

        public ICommand GoOutCommand
        {
            get
            {
                return _goOutCommand;
            }
        }

        public PlayRange PlayRange
        {
            get { return _playRange; }
            set
            {
                _playRange = value;
                this.OnPropertyChanged<double>(() => this.InPosition);
                this.OnPropertyChanged<double>(() => this.OutPosition);
            }
        }

        public Uri SourceUri
        {
            get
            {
                if (_player == null || _player.Status == PlayerStatus.Closed)
                {
                    return null;
                }
                return _sourceName == null ? null : new Uri("mplatform://" + _sourceName);
            }
        }

        public double Position
        {
            get
            {
                if (_player != null && _player.Status != PlayerStatus.Closed)
                {
                    return _player.GetPosition();
                }
                return 0.0;
            }

            set
            {
                if (_player != null && (_player.Status != PlayerStatus.Closed && _player.Status != PlayerStatus.Stopped))
                {
                    _player.SetPosition(value);
                    if (_player.Status == PlayerStatus.Paused)
                    {
                        this.OnPropertyChanged<double>(() => this.Position);
                    }
                }
            }
        }

        public double MaxPosition
        {
            get
            {
                if (_player != null && _player.Status != PlayerStatus.Closed)
                {
                    return _maxPosition;
                }
                return 1.0;
            }

            set
            {
                if (_maxPosition != value)
                {
                    _maxPosition = value;
                    this.OnPropertyChanged<double>(() => this.MaxPosition);
                }
            }
        }

        public double InPosition
        {
            get { return _playRange.StartPosition.TotalSeconds; }
            private set
            {
                //_playRange = _playRange.ModifyByStartPosition(TimeSpan.FromSeconds(value));

                var startPos = TimeSpan.FromSeconds(value);
                var duration = _playRange.StopPosition - startPos;
                _playRange = new PlayRange(startPos, duration);

                this.OnPropertyChanged<double>(() => this.InPosition);
            }
        }

        public double OutPosition
        {
            get { return _playRange.StopPosition.TotalSeconds; }
            private set
            {
                //_playRange = _playRange.ModifyByStopPosition(TimeSpan.FromSeconds(value));

                var stopPos = TimeSpan.FromSeconds(value);
                var duration = stopPos - _playRange.StartPosition;
                _playRange = new PlayRange(_playRange.StartPosition, duration);
                this.OnPropertyChanged<double>(() => this.OutPosition);
            }
        }

        public ICommand NextFrameCommand
        {
            get
            {
                return _nextFrameCommand;
            }
        }

        public ICommand PreviousFrameCommand
        {
            get
            {
                return _previousFrameCommand;
            }
        }

        private void Player_StatusChanged(object sender, EventArgs e)
        {
            _playCommand.RaiseCanExecuteChanged();
            _pauseCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();

            _setInCommand.RaiseCanExecuteChanged();
            _setOutCommand.RaiseCanExecuteChanged();

            _goInCommand.RaiseCanExecuteChanged();
            _goOutCommand.RaiseCanExecuteChanged();

            _nextFrameCommand.RaiseCanExecuteChanged();
            _previousFrameCommand.RaiseCanExecuteChanged();

            if (_player.Status == PlayerStatus.Stopped)
            {
                this.OnPropertyChanged(() => this.Position);
            }

            //
            SetSourceUri(_player.Name);
            this.MaxPosition = _player.Duration;
        }

        private void SetSourceUri(string name)
        {
            if (_sourceName != name)
            {
                _sourceName = name;
                base.OnPropertyChanged<Uri>(() => this.SourceUri);
            }
        }

        private bool CanStop()
        {
            return _player.Status == PlayerStatus.Running || _player.Status == PlayerStatus.Paused;
        }

        private void Stop()
        {
            if (this.CanStop())
            {
                _player.Stop();
                _timer.Stop();
            }
        }

        private bool CanPause()
        {
            return _player.Status == PlayerStatus.Running;
        }

        private void Pause()
        {
            if (this.CanPause())
            {
                _player.Pause();
                _timer.Stop();
            }
        }

        private bool CanPlay()
        {
            return _player.Status == PlayerStatus.Stopped || _player.Status == PlayerStatus.Paused;
        }

        private void Play()
        {
            if (this.CanPlay())
            {
                _player.Play();
                _timer.Start();
            }
        }

        private bool CanSetOut()
        {
            return _player.Status == PlayerStatus.Paused;
        }

        private void SetOut()
        {
            if (this.CanSetOut())
            {
                this.OutPosition = this.Position;
            }
        }

        private bool CanSetIn()
        {
            return _player.Status == PlayerStatus.Paused;
        }

        private void SetIn()
        {
            if (this.CanSetIn())
            {
                this.InPosition = this.Position;
            }
        }

        private bool CanGoOut()
        {
            return _player.Status == PlayerStatus.Paused || _player.Status == PlayerStatus.Running;
        }

        private void GoOut()
        {
            if (this.CanGoOut())
            {
                _player.SetPosition(this.OutPosition);
                if (_player.Status == PlayerStatus.Paused)
                {
                    this.OnPropertyChanged<double>(() => this.Position);
                }
            }
        }

        private bool CanGoIn()
        {
            return _player.Status == PlayerStatus.Paused || _player.Status == PlayerStatus.Running;
        }

        private void GoIn()
        {
            if (this.CanGoIn())
            {
                _player.SetPosition(this.InPosition);
                if (_player.Status == PlayerStatus.Paused)
                {
                    this.OnPropertyChanged<double>(() => this.Position);
                }
            }
        }

        private bool CanGoPreviousFrame()
        {
            return _player.Status == PlayerStatus.Paused;
        }

        private void GoPreviousFrame()
        {
            if (this.CanGoPreviousFrame())
            {
                MoveByFrames(-1);
            }
        }

        private bool CanGoNextFrame()
        {
            return _player.Status == PlayerStatus.Paused;
        }

        private void GoNextFrame()
        {
            if (this.CanGoNextFrame())
            {
                MoveByFrames(1);
            }
        }

        private void MoveByFrames(int frames)
        {
            var pos = this.Position;
            pos += (frames * TimeCodeUtils.MillisecondsPerFrame) / 1000.0;

            this.Position = Math.Min(this.MaxPosition, Math.Max(0.0, pos));
        }
    }

    internal partial class PlayControlModel2
    {
        enum PlayerStatus
        {
            Closed,
            Stopped,
            Running,
            Paused
        }

        class Player2 : IDisposable
        {
            public event EventHandler StatusChanged;
            private void OnStatusChanged()
            {
                if (StatusChanged != null)
                {
                    StatusChanged(this, EventArgs.Empty);
                }
            }

            //private MFileClass _mfileObject;
            private MPlaylistClass _mplaylist;
            private PlayerStatus _status;
            private string _fileName;
            private MItem _mitem=null;
            MPlaylistSettings _mplaylistSettings;

            public event EventHandler Opened;
            public event EventHandler Closed;

            public Player2(MPlaylistSettings mplaylistSettings)
            {
                _mplaylistSettings = mplaylistSettings;
            }
            public PlayerStatus Status
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

            public void Play()
            {
                if (_mitem != null &&
                    (this.Status == PlayerStatus.Paused || this.Status == PlayerStatus.Stopped))
                {
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

            

            public void Open(string fileName)
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
                }

                //if (_mitem != null)
                //{
                //    _mplaylist.FilePlayStop(0.0);

                //    _mplaylist.PlaylistRemove(_mitem);
                //    Marshal.ReleaseComObject(_mitem);  // ?
                //    _mitem = null;
                //}

                _fileName = fileName;
                int index = -1;

                _mplaylist.PlaylistAdd(null, _fileName, "", ref index, out _mitem);
                _mplaylist.ObjectStart(null);


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
                Debug.WriteLine("OnEvent线程："+System.Threading.Thread.CurrentThread.ManagedThreadId);
                //if ((string.Equals(bsChannelID, this._fileName, StringComparison.OrdinalIgnoreCase) &&
                //    string.Equals(bsEventName, "eof", StringComparison.OrdinalIgnoreCase))||
                //    string.Equals(bsEventName, "eol", StringComparison.OrdinalIgnoreCase))
                //{
                //    //this.Stop();
                //    this.Status = PlayerStatus.Stopped;
                //}
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
                    double dblPos = 0.0; // dblListPos=0.0;
                    //int index = 0, nextIndex = 0;
                    
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
                if (_mitem != null && this.Status != PlayerStatus.Closed)
                {
                    _mplaylist.FileRateSet(rate);
                }
            }
        }
    }
}