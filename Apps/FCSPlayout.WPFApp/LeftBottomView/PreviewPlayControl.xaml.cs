using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Windows.Controls;
using System.Windows.Threading;
namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// PlayControl.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewPlayControl : UserControl
    {
        private PreviewPlayControlModel2 _viewModel;

        public PreviewPlayControl()
        {
            InitializeComponent();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(33);

            _viewModel = new PreviewPlayControlModel2(ServiceLocator.Current.GetInstance<MPlaylistSettings>()); // viewModel;
            _viewModel.Timer = new TimerAdapter(timer);

            this.DataContext = _viewModel;
        }

        internal void Preview(IPlayableItem playableItem)
        {
            _viewModel.Preview(playableItem);
        }

        

        internal void Apply(IPlayableItemEditorFactory playItemEditorFactory)
        {
            //if (_viewModel.ApplyCommand.CanExecute(null))
            //{
            //    _viewModel.ApplyCommand.Execute(null);
            //}

            if (_viewModel.CanApply())
            {
                _viewModel.Apply(playItemEditorFactory);
            }
        }
        //public PreviewPlayControl(PreviewPlayControlModel2 viewModel)
        //    :this()
        //{
        //    var timer = new DispatcherTimer();
        //    timer.Interval = TimeSpan.FromMilliseconds(33);

        //    _viewModel = viewModel;
        //    _viewModel.Timer = new TimerAdapter(timer);

        //    this.DataContext = _viewModel;
        //}
    }
}