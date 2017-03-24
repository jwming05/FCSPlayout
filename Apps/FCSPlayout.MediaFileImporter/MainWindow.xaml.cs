
using FCSPlayout.Domain;
using FCSPlayout.Entities;
using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
           //mw = this;
         
        

            Debug.WriteLine("主线程：" + System.Threading.Thread.CurrentThread.ManagedThreadId);
          
            this.KeyUp += MainWindow_KeyUp;

            //this.previewInteractionRequestTrigger.sou
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {

                //if (this.playControl.canpause())
                //{ 
                //    this.playControl.pause();
                //}
                //else
                //{
                //    this.playControl.play();
                //}
            }
           
            
        }

        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
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
            timer.Start();
            timer.Tick += OnTimer_Tick;
            var viewModel= new MainViewModel(new TimerAdapter(timer));
            this.playControl.Init(PlayoutRepository.GetMPlaylistSettings());
            viewModel.Player = this.playControl;
            this.DataContext = viewModel;

            //CbType.ItemsSource = Enum.GetValues(typeof(MPFieldsType));
            //CbMode.ItemsSource = Enum.GetValues(typeof(ENUM_StretchMode));
            //CbRatio.ItemsSource = Enum.GetValues(typeof(ENUM_AspectRatio));
            comboBoxAudioGain_Loaded(CbAg);
            comboBoxMediaFileCategoryId_Loaded(CbMediaFileCategoryId);
          

      
            mymediafilechannel(cece);
        }

    
        private void OnTimer_Tick(object sender, EventArgs e)
        {
            //this.logView.OnTimer();
          
            
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



        public void mymediafilechannel(ComboBox CbAg)
        {

            CbAg.ItemsSource = PlayoutRepository.MediaFileChannels;
            CbAg.SelectedValuePath = "Key";
            CbAg.DisplayMemberPath = "Value";

            CbMediaFileCategoryText.ItemsSource = PlayoutRepository.MediaFileCategories;
            CbMediaFileCategoryText.SelectedValuePath = "Key";
            CbMediaFileCategoryText.DisplayMemberPath = "Value";

            CbMediaFileChannelText.ItemsSource = PlayoutRepository.MediaFileChannels;
            CbMediaFileChannelText.SelectedValuePath = "Key";
            CbMediaFileChannelText.DisplayMemberPath = "Value";

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

        void comboBoxMediaFileCategoryId_Loaded(ComboBox CbAg)
        {
            CbAg.ItemsSource = PlayoutRepository.MediaFileCategories;
            CbAg.SelectedValuePath = "Key";
            CbAg.DisplayMemberPath = "Value";
            //CbAg.SelectedIndex =0;
            //CbAg.SelectedItem = 0;


        }


        //public static MainWindow mw;


        //public WPF.Core.PlayControl2 mw1
        //{
        //    get
        //    {
        //        return playControl;
        //    }
        //    set
        //    {
        //        this.playControl = value;

        //        //_playRange = _playRange.ModifyByStopPosition(value);
        //    }
        //}

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.pagingControl.RaiseRequestPagingItems(0);
            PlaybillEditor.datagridHepler.SetShowRowIndexProperty(mediaItemListView2.dgMediaItem, true);
        }

        private void pagingControl_RequestPagingItems(object sender, RequestPagingItemsEventArgs e)
        {
            e.Result = this.mediaItemListView2.LoadMediaItems(e.PagingInfo);
        }


    }
}

















