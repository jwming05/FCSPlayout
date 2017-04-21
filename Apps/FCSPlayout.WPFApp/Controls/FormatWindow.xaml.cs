using FCSPlayout.Domain;
using MPLATFORMLib;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// LiveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FormatWindow : Window
    {
        MPlaylistClass _mplaylist;
        //private string _videoFormat;
        //private string _audioFormat;
        private MPlaylistSettings _mplaylistSettings;

        public MPlaylistSettings MPlaylistSettings
        {
            get
            {
                return _mplaylistSettings;
            }

            private set
            {
                _mplaylistSettings = value;
            }
        }

        //public string VideoFormat { get; private set; }
        //public string AudioFormat { get; private set; }


        public FormatWindow()
        {
            InitializeComponent();
            _mplaylistSettings = new MPlaylistSettings();
            this.Loaded += FormatWindow_Loaded;
            this.Closed += FormatWindow_Closed;
        }

        public FormatWindow(/*string videoFormat,string audioFormat*/MPlaylistSettings mplaylistSettings)
            :this()
        {
            _mplaylistSettings = mplaylistSettings;
            //_videoFormat = videoFormat;
            //_audioFormat = audioFormat;
        }
        
        private void FormatWindow_Closed(object sender, EventArgs e)
        {
            Marshal.ReleaseComObject(_mplaylist);
            _mplaylist = null;
        }

        private void FormatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _mplaylist = new MPlaylistClass();
            this.videoFormatComboBox.MFormatObject = _mplaylist;
            this.audioFormatComboBox.MFormatObject = _mplaylist;
            if (!string.IsNullOrEmpty(_mplaylistSettings.VideoFormat))
            {
                //this.videoFormatComboBox.SelectedFormat = _videoFormat;
                this.videoFormatComboBox.SelectedItem = _mplaylistSettings.VideoFormat;
            }

            if (!string.IsNullOrEmpty(_mplaylistSettings.AudioFormat))
            {
                //this.audioFormatComboBox.SelectedFormat = _audioFormat;
                this.audioFormatComboBox.SelectedItem = _mplaylistSettings.AudioFormat;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _mplaylistSettings.VideoFormat = this.videoFormatComboBox.SelectedFormat;
            _mplaylistSettings.AudioFormat = this.audioFormatComboBox.SelectedFormat;
            //this.RendererName = this.rendererComboBox.SelectedDevice;
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
