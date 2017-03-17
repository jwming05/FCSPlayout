using FCSPlayout.Domain;
using FCSPlayout.PlayEngine;
using FCSPlayout.WPFApp.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FCSPlayout.WPFApp.Views
{
    /// <summary>
    /// PlayoutView.xaml 的交互逻辑
    /// </summary>
    public partial class PlayoutView : UserControl
    {
        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register("Playlist", typeof(IPlaylist2), typeof(PlayoutView),
                new FrameworkPropertyMetadata(null, OnPlaylistPropertyChanged));

        public static readonly DependencyProperty TimerProperty =
            DependencyProperty.Register("Timer", typeof(ITimer), typeof(PlayoutView),
                new FrameworkPropertyMetadata(null, OnTimerPropertyChanged));

        public static readonly DependencyProperty LogProperty =
            DependencyProperty.Register("Log", typeof(ILog), typeof(PlayoutView),
                new FrameworkPropertyMetadata(null, OnLogPropertyChanged));

        public static readonly DependencyProperty SelectedPlayItemProperty =
            DependencyProperty.Register("SelectedPlayItem", typeof(IPlayItem), typeof(PlayoutView),
                new FrameworkPropertyMetadata(null, OnSelectedPlayItemPropertyChanged));

        private static void OnPlaylistPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlayoutView)dpObj).OnPlaylistChanged((IPlaylist)e.OldValue, (IPlaylist)e.NewValue);
        }

        private static void OnTimerPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlayoutView)dpObj).OnTimerChanged((ITimer)e.OldValue, (ITimer)e.NewValue);
        }

        private static void OnLogPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlayoutView)dpObj).OnLogChanged((ILog)e.OldValue, (ILog)e.NewValue);
        }

        private static void OnSelectedPlayItemPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((PlayoutView)dpObj).OnSelectedPlayItemChanged((IPlayItem)e.OldValue, (IPlayItem)e.NewValue);
        }

        private void OnSelectedPlayItemChanged(IPlayItem oldValue, IPlayItem newValue)
        {
            _viewModel.SelectedPlayItem = this.SelectedPlayItem;
        }

        private void OnLogChanged(ILog oldValue, ILog newValue)
        {
            _viewModel.Log = this.Log;
        }

        private void OnTimerChanged(ITimer oldValue, ITimer newValue)
        {
            _viewModel.Timer = this.Timer;
        }

        private PlayoutViewModel _viewModel;

        public PlayoutView()
        {
            InitializeComponent();
            

            _viewModel = new PlayoutViewModel();
            //_viewModel.Playlist = this.Playlist;
            _viewModel.ForcePlayed += ViewModel_ForcePlayed;
            this.DataContext = _viewModel;

            //GlobalEventAggregator.Instance.MPlaylistCreated += Instance_MPlaylistCreated;
            //GlobalEventAggregator.Instance.MPlaylistDestroying += Instance_MPlaylistDestroying;
            Pv = this;
    }

        private void ViewModel_ForcePlayed(object sender, ForcePlayEventArgs e)
        {
            OnForcePlayed(e);
        }

        public event EventHandler<ForcePlayEventArgs> ForcePlayed;

        private void OnForcePlayed(ForcePlayEventArgs eventArgs)
        {
            if (ForcePlayed != null)
            {
                this.ForcePlayed(this, eventArgs);
            }
        }

        private void Instance_MPlaylistDestroying(object sender, EventArgs e)
        {
            this.audioMeter.SetControlledObject(null);
        }

        private void Instance_MPlaylistCreated(object sender, EventArgs e)
        {
            //this.audioMeter.SetControlledObject(GlobalEventAggregator.Instance.MPlaylist);
        }

        public IPlaylist2 Playlist
        {
            get { return (IPlaylist2)GetValue(PlaylistProperty); }
            set { SetValue(PlaylistProperty, value); }
        }

        public ITimer Timer
        {
            get { return (ITimer)GetValue(TimerProperty); }
            set { SetValue(TimerProperty, value); }
        }

        public ILog Log
        {
            get { return (ILog)GetValue(LogProperty); }
            set { SetValue(LogProperty, value); }
        }

        public IPlayItem SelectedPlayItem
        {
            get { return (IPlayItem)GetValue(SelectedPlayItemProperty); }
            set { SetValue(SelectedPlayItemProperty, value); }
        }

        public ICommand ForcePlayCommand
        {
            get { return _viewModel.ForcePlayCommand; }
        }
        private void OnPlaylistChanged(IPlaylist oldValue, IPlaylist newValue)
        {
            _viewModel.Playlist = this.Playlist;
        }
        public ICommand StartPlayoutCommand
        {
            get { return _viewModel.StartPlayoutCommand; }
        }

        public ICommand StopPlayoutCommand
        {
            get { return _viewModel.StopPlayoutCommand; }
        }

        public ICommand StartDelayCommand
        {
            get { return _viewModel.StartDelayCommand; }
        }

        public ICommand StopDelayCommand
        {
            get { return _viewModel.StopDelayCommand; }
        }
        public static PlayoutView Pv;
    }
}
