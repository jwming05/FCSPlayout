using FCSPlayout.Entities;
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
using System.Windows.Shapes;

namespace FCSPlayout.WPFApp.Views
{
    /// <summary>
    /// SwitcherChannelSettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SwitcherChannelSettingsWindow : Window
    {
        private List<SwitcherChannelSettings> _controls = new List<SwitcherChannelSettings>();
        public SwitcherChannelSettingsWindow()
        {
            InitializeComponent();
            foreach(var info in PlayoutRepository.GetBMDSwitcherInfos())
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = info.Name;
                var uc = new SwitcherChannelSettings(info.Id);
                tabItem.Content = uc;
                _controls.Add(uc);
                this.tabControl.Items.Add(tabItem);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            foreach(var uc in _controls)
            {
                uc.Save();
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
