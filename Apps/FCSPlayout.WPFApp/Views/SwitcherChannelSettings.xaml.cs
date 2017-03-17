using FCSPlayout.WPFApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FCSPlayout.WPFApp.Views
{
    /// <summary>
    /// SwitcherChannelSettings.xaml 的交互逻辑
    /// </summary>
    public partial class SwitcherChannelSettings : UserControl
    {
        private SwitcherChannelSettingsViewModel _viewModel=null;

        public SwitcherChannelSettings()
        {
            InitializeComponent();
            
        }

        public SwitcherChannelSettings(Guid switcherId):this()
        {
            _viewModel = new SwitcherChannelSettingsViewModel();
            _viewModel.Initialize(switcherId);
            this.DataContext = _viewModel;
        }

        public void Save()
        {
            if (_viewModel != null)
            {
                _viewModel.Save();
            }
        }
    }
}
