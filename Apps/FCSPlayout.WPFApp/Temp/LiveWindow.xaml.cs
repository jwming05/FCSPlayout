using MPLATFORMLib;
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

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// LiveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LiveWindow : Window
    {
        MLiveClass mlive;
        public LiveWindow()
        {
            InitializeComponent();
            this.Loaded += LiveWindow_Loaded;
        }

        private void LiveWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mlive = new MLiveClass();
            this.videoComboBox.MDevice = mlive;
            this.audioComboBox.MDevice = mlive;
        }
    }
}
