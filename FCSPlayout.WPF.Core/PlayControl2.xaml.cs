using FCSPlayout.Domain;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// PlayControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlayControl2 : UserControl
    {
        private PlayControlModel2 _viewModel;

        public PlayControl2()
        {
            InitializeComponent();
        }

        public PlayRange PlayRange
        {
            get
            {
                return _viewModel.PlayRange;
            }
        }

        public void Init(string filePath, PlayRange playRange, MPlaylistSettings mplaylistSettings)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33);

            _viewModel = new PlayControlModel2(timer/*,this.Dispatcher*/, mplaylistSettings);

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
    }
}