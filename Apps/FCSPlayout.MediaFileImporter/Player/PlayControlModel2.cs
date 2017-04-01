using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using MPLATFORMLib;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace FCSPlayout.MediaFileImporter
{
    public partial class PlayControlModel2 : BindableBase
    {
        private Player2 _player;
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
        private readonly DelegateCommand _resetRateCommand;

        private double _maxPosition;
        private ITimer _timer;
        private PlayRange _playRange;
        private double _playRate = 1.0;
        private int _audioGain=0;
        private IMObject _mobject;
        

        public PlayControlModel2(MPlaylistSettings mplaylistSettings, IEventAggregator eventAggregator)
        {

            eventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Subscribe(Preview);

            _player = new Player2(mplaylistSettings);

            _player.Opened += Player_Opened;
            _player.Closed += Player_Closed;
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
            _resetRateCommand = new DelegateCommand(ResetRate, CanResetRate);
        }

        private void Preview(IPlayableItem playableItem)
        {
            if (_playableItem == playableItem) return;

            if (_player.Status != PlayerStatus.Closed)
            {
                _player.Close();
            }

            if (_playableItem != null)
            {
                _playableItem.PreviewClosing -= PlayableItem_PreviewClosing;
            }

            if (playableItem != null && !string.IsNullOrEmpty(playableItem.FilePath) && System.IO.File.Exists(playableItem.FilePath))
            {
                _playableItem = playableItem;
            }
            else
            {
                _playableItem = null;
            }


            if (_playableItem != null)
            {
                _playableItem.PreviewClosing += PlayableItem_PreviewClosing;
                this.PlayRange = _playableItem.PlayRange;

                SetAudioGain(_playableItem.AudioGain, false);
                _player.Open(_playableItem);

                this.Play();
                this.Pause();
            }
        }

        private void PlayableItem_PreviewClosing(object sender, EventArgs e)
        {
            Preview(null);
        }

        private void Player_Closed(object sender, EventArgs e)
        {
            this.MObject = null;
        }

        private void Player_Opened(object sender, EventArgs e)
        {
            this.MObject = _player.PlayerObject as IMObject;
        }

        public IMObject MObject
        {
            get { return _mobject; }
            set
            {
                _mobject = value;
                this.RaisePropertyChanged(nameof(this.MObject));
            }
        }

        public ITimer Timer
        {
            get { return _timer; }
            set
            {
                if (_timer != null)
                {
                    throw new InvalidOperationException();
                }

                _timer = value;
                _timer.Tick += Timer_Tick;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_player != null && _player.Status == PlayerStatus.Running)
            {
                this.RaisePropertyChanged(nameof(this.Position));
            }
        }
        
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

        public ICommand ResetRateCommand
        {
            get { return _resetRateCommand; }
        }
        #endregion Commands

        #region Public Properties
        public bool Seekable
        {
            get
            {
                return _player != null && _player.Status != PlayerStatus.Closed && _player.Status != PlayerStatus.Stopped;
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

                    this.RaisePropertyChanged(nameof(this.PlayRate));
                }
            }
        }

        public PlayRange PlayRange
        {
            get { return _playRange; }
            set
            {
                _playRange = value;
                this.RaisePropertyChanged(nameof(this.InPosition));
                this.RaisePropertyChanged(nameof(this.OutPosition));
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
                        this.RaisePropertyChanged(nameof(this.Position));
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
                    this.RaisePropertyChanged(nameof(this.MaxPosition));
                }
            }
        }

        public double InPosition
        {
            get { return _playRange.StartPosition.TotalSeconds; }
            set
            {
                var startPos = TimeSpan.FromSeconds(value);
                var duration = _playRange.StopPosition - startPos;
                if (duration >= PlayoutConfiguration.Current.MinPlayDuration)
                {
                    _playRange = new PlayRange(startPos, duration);
                    this.RaisePropertyChanged(nameof(this.InPosition));
                }
            }
        }

        public double OutPosition
        {
            get { return _playRange.StopPosition.TotalSeconds; }
            set
            {
                var stopPos = TimeSpan.FromSeconds(value);
                var duration = stopPos - _playRange.StartPosition;
                if (duration >= PlayoutConfiguration.Current.MinPlayDuration)
                {
                    _playRange = new PlayRange(_playRange.StartPosition, duration);
                    this.RaisePropertyChanged(nameof(this.OutPosition));
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
                    this.RaisePropertyChanged(nameof(this.Position));
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
                    this.RaisePropertyChanged(nameof(this.Position));
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
            return _playableItem != null && _player.Status!=PlayerStatus.Closed;
        }

        private void Apply()
        {
            if (this.CanApply())
            {
                this._playableItem.AudioGain = this.AudioGain;
                this._playableItem.PlayRange = this.PlayRange;
            }
        }

        private bool CanResetRate()
        {
            return true;
        }

        private void ResetRate()
        {
            this.PlayRate = 1.0;
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
                this.RaisePropertyChanged(nameof(this.Position));
            }

            //SetSourceUri(_player.Name);
            this.MaxPosition = _player.Duration;

            this.RaisePropertyChanged(nameof(this.Seekable));
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
                if (forceUpdate && _player != null && this._playableItem != null)
                {
                    _player.SetAudioGain(_audioGain);
                }

                this.RaisePropertyChanged(nameof(this.AudioGain));
            }
        }
    }
}
