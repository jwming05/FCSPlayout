using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
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

        public PlayControl2(PlayControlModel2 viewModel)
            :this()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33);

            _viewModel = viewModel;
            _viewModel.Timer = new TimerAdapter(timer);

            this.DataContext = _viewModel;
        }
    }
}