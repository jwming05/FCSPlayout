using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using System.Windows;
using System.Windows.Input;

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// MediaItemListView.xaml 的交互逻辑
    /// </summary>
    public partial class MediaItemListView : FCSPlayout.WPF.Core.ViewBase, IUploadProgressFeedback
    {    
        private MediaItemListViewModel _viewModel;

        public MediaItemListView(MediaItemListViewModel viewModel)
            :this()
        {
            _viewModel = viewModel;
            _viewModel.ProgressFeedback = this;
            this.DataContext = _viewModel;
        }
        public MediaItemListView()
        {
            InitializeComponent();            
        }

        #region
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

        void IUploadProgressFeedback.Reset()
        {
            this.pb1.Value = 0;
            this.pb2.Value = 0;
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
        #endregion

        protected override ViewModelBase ViewModel
        {
            get
            {
                return _viewModel;
            }

            set
            {
                _viewModel = (MediaItemListViewModel)value;
            }
        }
      
        public ICommand AddMediaItemCommand
        {
            get
            {
                return _viewModel.AddMediaItemCommand;
            }
        }

        public ICommand DeleteMediaItemCommand
        {
            get
            {
                return _viewModel.DeleteMediaItemCommand;
            }
        }

        public ICommand SaveMediaItemsCommand
        {
            get
            {
                return _viewModel.SaveMediaItemsCommand;
            }
        }
    }
}
