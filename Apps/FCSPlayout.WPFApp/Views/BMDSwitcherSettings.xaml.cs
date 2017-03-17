using FCSPlayout.WPFApp.ViewModels;
using System.Windows;

namespace FCSPlayout.WPFApp.Views
{
    /// <summary>
    /// BMDSwitcherSettings.xaml 的交互逻辑
    /// </summary>
    public partial class BMDSwitcherSettings : Window
    {
        private BMDSwitcherSettingsViewModel _viewModel;

        public BMDSwitcherSettings()
        {
            InitializeComponent();

            this.Loaded += BMDSwitcherSettings_Loaded;
        }

        private void BMDSwitcherSettings_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new BMDSwitcherSettingsViewModel();
            this.DataContext = _viewModel;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            _viewModel.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
