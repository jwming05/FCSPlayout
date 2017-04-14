using FCSPlayout.Domain;
using System;
using System.Windows.Controls;
using System.Windows.Threading;
namespace FCSPlayout.WPF.Core
//namespace FCSPlayout.Common
{
    /// <summary>
    /// PlayControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlayControl2 : UserControl
    {
        private PlayControlModel2.Player2 _player;

        private PlayControlModel2 _viewModel;

        public PlayControl2()
        {
            InitializeComponent();
            pl2 = this;

           


        }

        public PlayRange PlayRange
        {
            get
            {
                return _viewModel.PlayRange;
            }
        }
        public void play() {
            _viewModel.Play();
           
        }
        public void pause()
        {
            _viewModel.Pause();

        }
        public bool canpause()
        {
            return _viewModel.CanPause();

        }

    

        /*
        private ;
        private ;

        private ;
        private ;
        */
        public void Init(string filePath, PlayRange playRange, MPlaylistSettings mplaylistSettings,
            Func<bool> canSetInAction, Action<double> setInAction, Func<bool> canSetOutAction, Action<double> setOutAction)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33);

            if (_player == null)
            {
                _player = new PlayControlModel2.Player2(mplaylistSettings);
            }
            _viewModel = new PlayControlModel2(timer/*,this.Dispatcher*/, mplaylistSettings, _player)
            {
                _canSetOutAction = canSetOutAction,
                _canSetInAction = canSetInAction,
                _setInAction = setInAction,
                _setOutAction = setOutAction
            };
            

            _viewModel.Opened += OnOpened;
            _viewModel.Closed += OnClosed;

            _viewModel.FileName = filePath;
            _viewModel.PlayRange = playRange;


            this.DataContext = _viewModel;
        }


     
        public void Init(string filePath, PlayRange playRange, MPlaylistSettings mplaylistSettings)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33);

            if (_player == null)
            {
                _player = new PlayControlModel2.Player2(mplaylistSettings);
            }
            _viewModel = new PlayControlModel2(timer/*,this.Dispatcher*/, mplaylistSettings, _player);


            _viewModel.Opened += OnOpened;
            _viewModel.Closed += OnClosed;

            _viewModel.FileName = filePath;
            _viewModel.PlayRange = playRange;


            this.DataContext = _viewModel;
        }
        private void OnOpened(object sender,EventArgs e)
        {
            this.audioMeter.SetControlledObject(_viewModel.PlayerObject);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            this.audioMeter.SetControlledObject(null);
        }
        public static PlayControl2 pl2;

        private void rateSlider_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.rateSlider.Value = 1;
        }
    }
}