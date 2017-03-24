using FCSPlayout.Domain;
using System;
using System.Windows.Controls;
using System.Windows.Threading;
namespace FCSPlayout.MediaFileImporter
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

        public void Init(MPlaylistSettings mplaylistSettings)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33);

            _viewModel = new PlayControlModel2(timer, mplaylistSettings);

            _viewModel.Opened += OnOpened;
            _viewModel.Closed += OnClosed;
            this.DataContext = _viewModel;
        }

        public IPlayableItem PlayableItem
        {
            get { return _viewModel.PlayableItem; }
            set { _viewModel.PlayableItem = value; }
        }

        private void OnOpened(object sender,EventArgs e)
        {
            this.audioMeter.SetControlledObject(_viewModel.PlayerObject);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            this.audioMeter.SetControlledObject(null);
        }

        private void rateSlider_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.rateSlider.Value = 1;
        }

        public object TestValue
        {
            get { return null; }
            set
            {
                if (value == null)
                {

                }
            }
        }
    }
}