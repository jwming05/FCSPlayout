using Microsoft.Win32;
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

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// FilePicker.xaml 的交互逻辑
    /// </summary>
    public partial class FilePicker : UserControl
    {
        public static readonly DependencyProperty LabelVisibilityProperty =
            DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(FilePicker), 
                new FrameworkPropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(string), typeof(FilePicker), 
                new FrameworkPropertyMetadata(string.Empty));

        

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(FilePicker), 
                new FrameworkPropertyMetadata("文件名:"));


        private OpenFileDialog _openFileDlg;
        public FilePicker()
        {
            InitializeComponent();
            _openFileDlg = new OpenFileDialog();
        }

        public Visibility LabelVisibility
        {
            get { return (Visibility)GetValue(LabelVisibilityProperty); }
            set { SetValue(LabelVisibilityProperty, value); }
        }

        public string SelectedFile
        {
            get { return (string)GetValue(SelectedFileProperty); }
            set { SetValue(SelectedFileProperty, value); }
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (_openFileDlg.ShowDialog() == true)
            {
                this.SelectedFile = _openFileDlg.FileName;
            }
        }
    }
}
