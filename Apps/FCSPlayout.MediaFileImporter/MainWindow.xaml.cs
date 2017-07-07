using System;
using System.Windows;
using System.Windows.Threading;

namespace FCSPlayout.MediaFileImporter
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
        }

        public MainWindow(MainViewModel viewModel)
            :this()
        {
            this.DataContext = viewModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Start();
            timer.Tick += OnTimer_Tick;
        }

    
        private void OnTimer_Tick(object sender, EventArgs e)
        {   
        }
    }
}

















