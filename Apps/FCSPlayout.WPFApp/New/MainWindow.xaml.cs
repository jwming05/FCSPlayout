using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using FCSPlayout.WPFApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
            this.Loaded += MainWindow_Loaded;

            //this.playoutView.ForcePlayed += PlayoutView_ForcePlayed;
        }

        public MainWindow(MainViewModel viewModel)
            :this()
        {
            this.DataContext = viewModel;
        }

        private void PlayoutView_ForcePlayed(object sender, ViewModels.ForcePlayEventArgs e)
        {
            //this.playbillView.OnForcePlay(e);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += OnTimer_Tick;
            timer.Start();
            
            //this.DataContext = new MainViewModel(new TimerAdapter(timer));
        }

        private void OnTimer_Tick(object sender, EventArgs e)
        {
            MLLicenseLib.MLLicenseManager.Instance.Timer();
            //this.logView.OnTimer();
        }

        private void miRendererSettings_Click(object sender, RoutedEventArgs e)
        {
            var renderName = Settings.Default.RendererDeviceName;
            var render = new RendererWindow(renderName);
            
            //if (!string.IsNullOrEmpty(renderName))
            //{
            //    render.SetRendererName(renderName);
            //}

            if (render.ShowDialog() == true)
            {
                Settings.Default.RendererDeviceName = render.RendererName;
                Settings.Default.Save();

                //var t = Settings.Default.Properties[""];  
                //Settings.Default.RendererDeviceName = render.RendererName;
            }
        }

        private void miFormatSettings_Click(object sender, RoutedEventArgs e)
        {
            
            //var videoFormatName = Settings.Default.VideoFormatName;
            //var audioFormatName = Settings.Default.AudioFormatName;
            var formatWindow = new FormatWindow(PlayoutRepository.GetMPlaylistSettings());
            //formatWindow.SetFormats(videoFormatName, audioFormatName);

            if (formatWindow.ShowDialog() == true)
            {
                PlayoutRepository.SaveMPlaylistSettings(formatWindow.MPlaylistSettings);

                //Settings.Default.VideoFormatName = formatWindow.VideoFormat;
                //Settings.Default.AudioFormatName = formatWindow.AudioFormat;
                //Settings.Default.Save();
            }
        }

        private void miSwitcherSettings_Click(object sender, RoutedEventArgs e)
        {
            Window dialog = new Views.BMDSwitcherSettings();
            dialog.ShowDialog();
        }

        private void miChannelSettings_Click(object sender, RoutedEventArgs e)
        {
            Window dialog = new Views.ChannelSettings();
            dialog.ShowDialog();
        }

        private void miSwitcherChannelSettings_Click(object sender, RoutedEventArgs e)
        {
            Window dialog = new Views.SwitcherChannelSettingsWindow();
            dialog.ShowDialog();
        }
    }
}
