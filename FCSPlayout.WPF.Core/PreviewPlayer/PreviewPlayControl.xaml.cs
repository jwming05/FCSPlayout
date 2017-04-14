using System;
using System.Windows.Controls;
using System.Windows.Threading;
namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// PlayControl.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewPlayControl : UserControl
    {
        private PreviewPlayControlModel _viewModel;

        public PreviewPlayControl()
        {
            InitializeComponent();
        }

        public PreviewPlayControl(PreviewPlayControlModel viewModel)
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