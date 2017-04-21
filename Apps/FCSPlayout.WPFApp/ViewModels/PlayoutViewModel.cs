using FCSPlayout.Domain;
using FCSPlayout.PlayEngine;
using FCSPlayout.WPFApp.Models;
using FCSPlayout.WPFApp.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace FCSPlayout.WPFApp.ViewModels
{
    public class PlayoutViewModel : BindableBase/*, IPlayItemPlayRecorder*/, IPlayPreview
    {
        private readonly DelegateCommand _startPlayoutCommand;
        private readonly DelegateCommand _stopPlayoutCommand;

        private BindablePlayout _playout;
        private Uri _sourceUri;
        private ITimer _timer;
        private DispatcherTimer timer;
        //private ProcessCount processCount;
        //private double _playlistDuration;
        private double _playlistPosition;
        private DateTime? _playlistStartTime;
        private DateTime? _playlistStopTime;
        private readonly DelegateCommand _startDelayCommand;
        private readonly DelegateCommand _stopDelayCommand;
        //private readonly DelegateCommand<object> _forcePlayCommand;

        public PlayoutViewModel(IPlaylist3 playlist)
        {
            _playout = new BindablePlayout(this, playlist);
            _playout.StateChanged += OnPlayout_StateChanged;
            _startPlayoutCommand = new DelegateCommand(StartPlayout, CanStartPlayout);
            _stopPlayoutCommand = new DelegateCommand(StopPlayout, CanStopPlayout);

            _startDelayCommand = new DelegateCommand(StartDelay, CanStartDelay);
            _stopDelayCommand = new DelegateCommand(StopDelay, CanStopDelay);

            //_forcePlayCommand = new DelegateCommand<object>(ForcePlay,CanForcePlay);
        }

        private void OnPlayout_StateChanged(object sender, EventArgs e)
        {
            _startPlayoutCommand.RaiseCanExecuteChanged();
            _stopPlayoutCommand.RaiseCanExecuteChanged();

            _startDelayCommand.RaiseCanExecuteChanged();
            _stopDelayCommand.RaiseCanExecuteChanged();

            //_forcePlayCommand.RaiseCanExecuteChanged();
        }

        //private bool CanForcePlay(object obj)
        //{
        //    return this.SelectedPlayItem != null && _playout.CanForcePlay(this.SelectedPlayItem);

        //    //var eventArgs = obj as Prism.Interactivity.InteractionRequest.InteractionRequestedEventArgs;
        //    //if (eventArgs != null)
        //    //{
        //    //    var playItem = eventArgs.Context.Content as BindablePlayItem;
        //    //    if (playItem != null)
        //    //    {
        //    //        return _playout.CanForcePlay(playItem);
        //    //    }
        //    //}

        //    //return false;
        //}

        //private void ForcePlay(object obj)
        //{
        //    if (CanForcePlay(obj))
        //    {
        //        _playout.ForcePlay(this.SelectedPlayItem);
        //        //_playout.ForcePlay(this.SelectedPlayItem, (curItem,range,forcedItem) => 
        //        //{
        //        //    var eventArgs = new ForcePlayEventArgs();
        //        //    eventArgs.CurrentPlayItem = curItem;
        //        //    eventArgs.CurrentRemainRange = range;
        //        //    eventArgs.ForcePlayItem = forcedItem;
        //        //    OnForcePlayed(eventArgs);
        //        //    // 删除forcedItem
        //        //    // 根据curItem和range产生一个新的项并插入到curItem之后（修改curItem的时长）
        //        //    //curItem.PlayRange.Offset(TimeSpan.Zero, range.StartPosition - curItem.PlayRange.StartPosition);
        //        //});
        //    }

        //    //var playItem = obj as BindablePlayItem;
        //    //if (playItem != null)
        //    //{
        //    //    _playout.ForcePlay(playItem, () => { });
        //    //}

        //    //var eventArgs = obj as Prism.Interactivity.InteractionRequest.InteractionRequestedEventArgs;
        //    //if (eventArgs != null)
        //    //{
        //    //    var playItem = eventArgs.Context.Content as BindablePlayItem;
        //    //    if (playItem != null)
        //    //    {
        //    //        _playout.ForcePlay(playItem, () => { });
        //    //    }
        //    //}
        //}

        private void OnForcePlayed(ForcePlayEventArgs eventArgs)
        {
            if (ForcePlayed != null)
            {
                this.ForcePlayed(this, eventArgs);
            }
        }

        public event EventHandler<ForcePlayEventArgs> ForcePlayed;
        private bool CanStopDelay()
        {
            return _playout.CanStopDelay();
        }

        private void StopDelay()
        {
            if (CanStopDelay())
            {
                _playout.StopDelay();
                
            }
        }

        private bool CanStartDelay()
        {
            return _playout.CanStartDelay();
        }

        private void StartDelay()
        {
            if (CanStartDelay())
            {
                _playout.StartDelay();
            }
        }

        public Uri SourceUri
        {
            get { return _sourceUri; }
            set
            {
                _sourceUri = value;
                this.RaisePropertyChanged(nameof(this.SourceUri));
            }
        }

        public ICommand StartPlayoutCommand
        {
            get { return _startPlayoutCommand; }
        }

        public ICommand StopPlayoutCommand
        {
            get { return _stopPlayoutCommand; }
        }

        public ICommand StartDelayCommand
        {
            get { return _startDelayCommand; }
        }

        public ICommand StopDelayCommand
        {
            get { return _stopDelayCommand; }
        }

        //public IPlaylist2 Playlist
        //{
        //    get { return _playout.Playlist; }
        //    internal set
        //    {
        //        if (_playout.Playlist != null)
        //        {
        //            //_playout.Playlist.CollectionChanged -= Playlist_CollectionChanged;
        //        }
        //        _playout.Playlist = value;

        //        if (_playout.Playlist != null)
        //        {
        //            //_playout.Playlist.CollectionChanged += Playlist_CollectionChanged;
        //        }
        //        _startPlayoutCommand.RaiseCanExecuteChanged();
        //        _stopPlayoutCommand.RaiseCanExecuteChanged();

        //        //_startDelayCommand.RaiseCanExecuteChanged();
        //        //_stopDelayCommand.RaiseCanExecuteChanged();s
        //    }
        //}

        private void Playlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //this.PlaylistDuration = _playout.Playlist.Duration.TotalSeconds;
            
            this.PlaylistStartTime = _playout.Playlist.StartTime;
            //if (this.PlaylistStartTime != null)
            //{

            //}
            //this.PlaylistStopTime = _playout.Playlist.StartTime.Add(_playout.Playlist.Duration);
        }

        private bool CanStartPlayout()
        {
            return _playout.CanStart();
        }

        private void StartPlayout()
        {
            if (CanStartPlayout())
            {
                _playout.Start();
                //timer = new DispatcherTimer();
                //timer.Interval = new TimeSpan(10000000);   //时间间隔为一秒
                //timer.Tick += new EventHandler(timer_Tick);

                //var now = DateTime.Now;
                
                //string sTime = _playlistStartTime.ToString();
                //DateTime historyTime = Convert.ToDateTime(sTime);
                //TimeSpan ts = historyTime - now;

                ////处理倒计时的类
                //processCount = new ProcessCount(ts.Days + ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds);
                //CountDown += new CountDownHandler(processCount.ProcessCountDown);

                //////开启定时器
                //timer.Start();

            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (OnCountDown())
            {


                //PlayoutView.Pv.djs.Text = processCount.GetHour() + ":" + processCount.GetMinute() + ":" + processCount.GetSecond();
                //HourArea.Text = processCount.GetHour();
                //MinuteArea.Text = processCount.GetMinute();
                //SecondArea.Text = processCount.GetSecond();

            }
            else
            {
                timer.Stop();
                //PlayoutView.Pv.djs.Text = "00:00:00";
            }
        }

        /// <summary>
        /// 处理事件
        /// </summary>
        public event CountDownHandler CountDown;
        public bool OnCountDown()
        {
            if (CountDown != null)
                return CountDown();

            return false;
        }


        /// <summary>
        /// 处理倒计时的委托
        /// </summary>
        /// <returns></returns>
        public delegate bool CountDownHandler();


        private bool CanStopPlayout()
        {
            return _playout.CanStop();
        }

        private void StopPlayout()
        {
            if (CanStopPlayout())
            {
                _playout.Stop();
                timer.Stop();
                //PlayoutView.Pv.djs.Text = "00:00:00";
            }
        }

     

        void IPlayPreview.SetPreviewUri(Uri uri)
        {
            this.SourceUri = uri;
        }

        
        public double PlaylistDuration
        {
            get
            {
                if (this.PlaylistStopTime != null)
                {
                    return this.PlaylistStopTime.Value.Subtract(this.PlaylistStartTime.Value).TotalSeconds;
                }
                else
                {
                    return 0.0;
                }
                //return _playlistDuration;
            }
            //set
            //{
            //    if (_playlistDuration != value)
            //    {
            //        _playlistDuration = value;
            //        OnPropertyChanged(() => this.PlaylistDuration);
            //    }
            //}
        }

        public DateTime? PlaylistStartTime
        {
            get { return _playlistStartTime; }
            set
            {
                _playlistStartTime = value;

                this.RaisePropertyChanged(nameof(this.PlaylistStartTime));

                if (_playlistStartTime != null)
                {
                    this.PlaylistStopTime = _playlistStartTime.Value.Add(_playout.Playlist.Duration);
                }
                else
                {
                    this.PlaylistStopTime = null;
                }
            }
        }

        public DateTime? PlaylistStopTime
        {
            get { return _playlistStopTime; }
            set
            {
                _playlistStopTime = value;
                this.RaisePropertyChanged(nameof(this.PlaylistStopTime));
                this.RaisePropertyChanged(nameof(this.PlaylistDuration));
            }
        }

        public double PlaylistPosition
        {
            get { return _playlistPosition; }
            set
            {
                if (_playlistPosition != value)
                {
                    _playlistPosition = value;
                    this.RaisePropertyChanged(nameof(this.PlaylistPosition));
                }

            }
        }

        public ITimer Timer
        {
            get
            {
                return _timer;
            }

            set
            {
                if (_timer != null)
                {
                    _timer.Tick -= OnTimer_Tick;
                }
                _timer = value;
                if (_timer != null)
                {
                    _timer.Tick += OnTimer_Tick;
                }
            }
        }

        public BindablePlayout Playout
        {
            get
            {
                return _playout;
            }

            set
            {
                _playout = value;
            }
        }

        internal ILog Log
        {
            get { return _playout.Log; }
            set
            {
                _playout.Log = value;
            }
        }

        //public ICommand ForcePlayCommand
        //{
        //    get
        //    {
        //        return _forcePlayCommand;
        //    }
        //}

        private IPlayerItem _currentPlayItem;
        public IPlayerItem CurrentPlayItem
        {
            get
            {
                return _currentPlayItem;
            }

            set
            {
                _currentPlayItem = value;

                //DataGrid dg = PlaybillView.w1.dgPlayItems;

                //for (int i = 0; i < _playout.Playlist.Count; i++)
                //{
                //    if (_playout.Playlist.Count < 0) return;

                //    if (_playout.Playlist[i].Status == PlayItemStatus.Playing)
                //    {
                //        DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(i + 1);

                //        if (row != null)
                //        {
                //            //滚动条偏移量
                //            var sv = FindScrollViewer(dg);
                //            if (sv != null)
                //            {
                //                Double height = sv.ViewportHeight * 30 / 2.8;

                //                double offsetHeight = Math.Min(sv.ScrollableHeight * 30, Math.Max(0, i * 30 - height));
                //                var num = offsetHeight / 30;
                //                sv.ScrollToVerticalOffset(num);
                //            }
                //        }
                //        else
                //        {
                //            dg.ScrollIntoView(dg.Items[i]);
                //        }
                //    }
                //}
                this.RaisePropertyChanged(nameof(this.CurrentPlayItem));
            }
        }

        private ScrollViewer FindScrollViewer(DependencyObject dpObj)
        {
            ScrollViewer sv = null;
            int count = VisualTreeHelper.GetChildrenCount(dpObj);
            for (int i = 0; i < count; i++)
            {
                var obj = VisualTreeHelper.GetChild(dpObj, i);
                sv = obj as ScrollViewer;
                if (sv != null)
                {
                    return sv;
                }

                sv = FindScrollViewer(obj);
                if (sv != null)
                {
                    return sv;
                }
            }

            return null;
        }

        private TimeSpan _currentPlayItemPosition;
        private IPlayItem _selectedPlayItem;
        
        public TimeSpan CurrentPlayItemPosition
        {
            get
            {
                return _currentPlayItemPosition;
            }

            set
            {
                _currentPlayItemPosition = value;
                this.RaisePropertyChanged(nameof(this.CurrentPlayItemPosition));
            }
        }

        internal IPlayItem SelectedPlayItem
        {
            get { return _selectedPlayItem; }
            set
            {
                _selectedPlayItem = value;
                //_forcePlayCommand.RaiseCanExecuteChanged();
            }
        }

        private void OnTimer_Tick(object sender, EventArgs e)
        {
            _playout.OnTimer();
        }

        //public void Submit(IPlayItemPlayRecord record)
        //{
        //    if (this.Log != null)
        //    {
        //        this.Log.Log(record.ToString());
        //    }
        //}
    }
}
