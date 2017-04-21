using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace FileComparator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog _openFileDlg;
        private MD5Generator _generator;
        public MainWindow()
        {
            InitializeComponent();
            _openFileDlg = new OpenFileDialog();
            _generator = new MD5Generator();
        }

        private async void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDlg.ShowDialog() == true)
            {
                this.txtHash.Text = await _generator.GenerateAsync(_openFileDlg.FileName);
                //using(var fs=new FileStream(_openFileDlg.FileName,System.IO.FileMode.Open,File))
            }
        }
    }
}
