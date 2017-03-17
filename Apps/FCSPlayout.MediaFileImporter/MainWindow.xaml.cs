using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IUploadProgressFeedback
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.mediaItemListView.UploadProgressFeedback = this;
           mw = this;

            Debug.WriteLine("主线程：" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow(App.Name);
            login.Owner = this;
            if (login.ShowDialog() != true)
            {
                Application.Current.Shutdown();
                return;
            }

            var timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += OnTimer_Tick;
            timer.Start();
            
            this.DataContext = new MainViewModel(new TimerAdapter(timer));
            //CbType.ItemsSource = Enum.GetValues(typeof(MPFieldsType));
            //CbMode.ItemsSource = Enum.GetValues(typeof(ENUM_StretchMode));
            //CbRatio.ItemsSource = Enum.GetValues(typeof(ENUM_AspectRatio));
            comboBoxAudioGain_Loaded(CbAg);

        }

        private void OnTimer_Tick(object sender, EventArgs e)
        {
            //this.logView.OnTimer();
            MLLicenseLib.MLLicenseManager.Instance.Timer();
            FCSPlayout.AppInfrastructure.WPFApplicationManager.Current.OnTimer();
        }

        void IUploadProgressFeedback.Open()
        {
            this.pb1.Value = 0;
            this.pb2.Value = 0;
            pbPanel.Visibility = Visibility.Visible;
        }

        void IUploadProgressFeedback.Close()
        {
            pbPanel.Visibility = Visibility.Collapsed;
        }

        void IUploadProgressFeedback.Report(int progress, MediaFileStorage locationCategory)
        {
            switch (locationCategory)
            {
                case MediaFileStorage.Primary:
                    this.pb1.Value = progress;
                    break;
                case MediaFileStorage.Secondary:
                    this.pb2.Value = progress;
                    break;
            }
        }
    
      void comboBoxAudioGain_Loaded(ComboBox CbAg)
        {
           
          
            List<int> items = new List<int>();
            items.Add(60);
            items.Add(50);
            items.Add(40);
            items.Add(30);
            items.Add(20);
            items.Add(10);
            items.Add(0);
            items.Add(-10);
            items.Add(-20);
            items.Add(-30);
            items.Add(-40);
            items.Add(-50);
            items.Add(-60);
            CbAg.ItemsSource = items;

        }

        public static MainWindow mw;

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.pagingControl.RaiseRequestPagingItems(0);
        }

        private void pagingControl_RequestPagingItems(object sender, RequestPagingItemsEventArgs e)
        {
            e.Result = this.mediaItemListView2.LoadMediaItems(e.PagingInfo);
        }
    }
}






