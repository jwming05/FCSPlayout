using FCSPlayout.Domain;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace FCSPlayout.MediaFileImporter
{
    internal partial class PlayControlModel2 : BindableBase
    {
        private Player2 _player;
        //private string _fileName;
        private IPlayableItem _playableItem;

        private readonly DelegateCommand _playCommand;
        private readonly DelegateCommand _pauseCommand;
        private readonly DelegateCommand _stopCommand;
        private readonly DelegateCommand _setInCommand;
        private readonly DelegateCommand _setOutCommand;
        private readonly DelegateCommand _goInCommand;
        private readonly DelegateCommand _goOutCommand;
        private readonly DelegateCommand _nextFrameCommand;
        private readonly DelegateCommand _previousFrameCommand;

        private readonly DelegateCommand _applyCommand;

        private double _maxPosition;
        private string _sourceName;
        private DispatcherTimer _timer;
        private PlayRange _playRange;
        private double _playRate = 1.0;
        private int _audioGain=0;

        //internal Func<bool> _canSetInAction;
        //internal Action<double> _setInAction;

        //internal Func<bool> _canSetOutAction;
        //internal Action<double> _setOutAction;

        public PlayControlModel2(DispatcherTimer timer, MPlaylistSettings mplaylistSettings/*, Player2 player*/)
        {
            _timer = timer;

            _timer.Tick += Timer_Tick;

            _player = new Player2(mplaylistSettings); //player;
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

            _applyCommand = new DelegateCommand(Apply, CanApply);
        }

        

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

        //public string FileName
        //{
        //    get { return _fileName; }
        //    set
        //    {
        //        if (_player.Status != PlayerStatus.Closed)
        //        {
        //            _player.Close();
        //        }
        //        _fileName = value;

        //        if (!string.IsNullOrEmpty(_fileName) && System.IO.File.Exists(_fileName))
        //        {
        //            _player.Open(_fileName);

        //            _player.SetRate(this.PlayRate);
        //        }
        //    }
        //}

        

        #region Commands
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

        public ICommand ApplyCommand
        {
            get { return _applyCommand; }
        }
        #endregion Commands

        #region Public Properties
        public object PlayerObject { get { return _player.PlayerObject; } }

        public IPlayableItem PlayableItem
        {
            get { return _playableItem; }
            set
            {
                if (_playableItem == value) return;

                if (_player.Status != PlayerStatus.Closed)
                {
                    _player.Close();
                }

                if(value!=null && !string.IsNullOrEmpty(value.FilePath) && System.IO.File.Exists(value.FilePath))
                {
                    _playableItem = value;
                }
                else
                {
                    _playableItem = null;
                }
                

                if (_playableItem!=null)
                {
                    this.PlayRange = _playableItem.PlayRange;

                    //_audioGain = _playableItem.AudioGain;
                    //this.OnPropertyChanged(() => this.AudioGain);
                    SetAudioGain(_playableItem.AudioGain, false);
                    _player.Open(_playableItem);

                    //_player.SetRate(this.PlayRate);

                    //this.AudioGain = _playableItem.AudioGain;

                    this.Play();
                    this.Pause();
                }
                else
                {
                    _player.Close();
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

                    if (_player != null)
                    {
                        _player.SetRate(_playRate);
                    }
                    //if (_player.Status != PlayerStatus.Closed)
                    //{
                    //    _player.SetRate(_playRate);
                    //}

                    this.OnPropertyChanged(() => this.PlayRate);
                }
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
            set
            {
                //_playRange = _playRange.ModifyByStartPosition(TimeSpan.FromSeconds(value));

                var startPos = TimeSpan.FromSeconds(value);
                var duration = _playRange.StopPosition - startPos;
                if (duration >= PlayoutConfiguration.Current.MinPlayDuration)
                {
                    _playRange = new PlayRange(startPos, duration);
                    this.OnPropertyChanged<double>(() => this.InPosition);
                }
            }
        }

        public double OutPosition
        {
            get { return _playRange.StopPosition.TotalSeconds; }
            set
            {
                //_playRange = _playRange.ModifyByStopPosition(TimeSpan.FromSeconds(value));

                var stopPos = TimeSpan.FromSeconds(value);
                var duration = stopPos - _playRange.StartPosition;
                if (duration >= PlayoutConfiguration.Current.MinPlayDuration)
                {
                    _playRange = new PlayRange(_playRange.StartPosition, duration);
                    this.OnPropertyChanged<double>(() => this.OutPosition);
                }
                
            }
        }
        #endregion Public Properties

        #region Command Methods
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

        public bool CanPause()
        {
            return _player.Status == PlayerStatus.Running;
        }

        public void Pause()
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
            return _player.Status == PlayerStatus.Paused/* && (_canSetOutAction == null || _canSetOutAction())*/;
        }

        private void SetOut()
        {
            if (this.CanSetOut())
            {
                this.OutPosition = this.Position;
                //if (_setOutAction != null)
                //{
                //    _setOutAction(this.Position);
                //}

            }
        }

        private bool CanSetIn()
        {
            return _player.Status == PlayerStatus.Paused/* && (_canSetInAction == null || _canSetInAction())*/;
        }

        private void SetIn()
        {
            if (this.CanSetIn())
            {

                this.InPosition = this.Position;
                //if (_setInAction != null)
                //{
                //    _setInAction(this.Position);
                //}
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

        private bool CanApply()
        {
            return this.PlayableItem != null && _player.Status!=PlayerStatus.Closed;
        }

        private void Apply()
        {
            if (this.CanApply())
            {
                this.PlayableItem.AudioGain = this.AudioGain;
                this.PlayableItem.PlayRange = this.PlayRange;
            }
        }
        #endregion Command Methods

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

            _applyCommand.RaiseCanExecuteChanged();

            if (_player.Status == PlayerStatus.Stopped)
            {
                this.OnPropertyChanged(() => this.Position);
            }

            //
            SetSourceUri(_player.Name);
            this.MaxPosition = _player.Duration;

            this.OnPropertyChanged(() => this.Seekable);
        }

        private void SetSourceUri(string name)
        {
            if (_sourceName != name)
            {
                _sourceName = name;
                base.OnPropertyChanged<Uri>(() => this.SourceUri);
            }
        }

        private void MoveByFrames(int frames)
        {
            var pos = this.Position;
            pos += (frames * TimeCodeUtils.MillisecondsPerFrame) / 1000.0;

            this.Position = Math.Min(this.MaxPosition, Math.Max(0.0, pos));
        }

        public int AudioGain
        {
            get { return _audioGain; }
            set
            {
                SetAudioGain(value, true);
            }
        }

        private void SetAudioGain(int audioGain, bool forceUpdate)
        {
            if (_audioGain != audioGain)
            {
                _audioGain = audioGain;
                if (forceUpdate && _player != null && this.PlayableItem != null)
                {
                    _player.SetAudioGain(_audioGain);
                }

                this.OnPropertyChanged(() => this.AudioGain);
            }
        }

        public bool Seekable
        {
            get
            {
                return _player != null && _player.Status != PlayerStatus.Closed && _player.Status != PlayerStatus.Stopped;
            }
        }
    }
}
