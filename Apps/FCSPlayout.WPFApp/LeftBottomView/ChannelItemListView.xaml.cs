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

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// ChannelItemListView.xaml 的交互逻辑
    /// </summary>
    public partial class ChannelItemListView : UserControl
    {
        public ChannelItemListView()
        {
            InitializeComponent();
        }

        public ChannelItemListView(ChannelItemListViewModel viewModel)
            :this()
        {
            this.DataContext = viewModel;
        }

        public string Title
        {
            get { return "直通通道"; }
        }
    }
}
