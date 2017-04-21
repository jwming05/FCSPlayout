using MPLATFORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// RendererWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RendererWindow : Window
    {
        private MRendererClass _mrenderer;
        private string _deviceName;

        public RendererWindow()
        {
            InitializeComponent();
            this.Loaded += RendererWindow_Loaded;
            this.Closed += RendererWindow_Closed;
        }

        public RendererWindow(string deviceName):this()
        {
            _deviceName = deviceName;
        }
        private void RendererWindow_Closed(object sender, EventArgs e)
        {
            Marshal.ReleaseComObject(_mrenderer);
            _mrenderer = null;
        }

        private void RendererWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _mrenderer = new MRendererClass();
            this.rendererComboBox.MDevice = _mrenderer;

            if (!string.IsNullOrEmpty(_deviceName))
            {
                this.rendererComboBox.SelectedItem = _deviceName;
                //this.rendererComboBox.SelectedDevice = _deviceName;
            }
        }

        public String RendererName
        {
            get;
            private set;
        }

        //public void SetRendererName(string name)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        this.rendererComboBox.SelectedDevice = name;
        //    }
        //}

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.RendererName = this.rendererComboBox.SelectedDevice;
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
