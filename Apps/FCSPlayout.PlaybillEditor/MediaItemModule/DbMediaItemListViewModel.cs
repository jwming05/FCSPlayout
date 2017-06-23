using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Regions;

namespace FCSPlayout.PlaybillEditor
{
    public class DbMediaItemListViewModel : ViewModelBase, Prism.IActiveAware
    {
        private readonly ObservableCollection<BindableFileMediaItem> _mediaItemCollection;

        private BindableFileMediaItem _selectedMediaItem;
        private readonly DelegateCommand<object> _editMediaItemCommand;
        private readonly DelegateCommand<IPlayableItem> _previewCommand;
        private readonly DelegateCommand<RequestPagingItemsEventArgs> _searchCommand;
        
        private IPlayableItem _currentPreviewItem;
        private int _pageSize = 30;

        public DbMediaItemListViewModel(IEventAggregator eventAggregator, IMediaFilePathResolver filePathResolver, 
            IMediaFileImageResolver imageResolver,IRegionManager regionManager,IMediaFileService mediaService)
        {
            this.EventAggregator = eventAggregator;
            this.FilePathResolver = filePathResolver;
            this.ImageResolver = imageResolver;
            this.MediaFileService = mediaService;

            _regionManager = regionManager;

            _mediaItemCollection = new ObservableCollection<BindableFileMediaItem>();

            _editMediaItemCommand = new DelegateCommand<object>(EditMediaItem, CanEditMediaItem);

            _searchCommand = new DelegateCommand<RequestPagingItemsEventArgs>(SearchMediaItems);

            _previewCommand = new DelegateCommand<IPlayableItem>(Preview);
            this.SearchOptions = new MediaItemSearchOptions();
        }

        public ICommand PreviewCommand
        {
            get { return _previewCommand; }
        }

        private void Preview(IPlayableItem playableItem)
        {
            if (this.EventAggregator != null && playableItem != null)
            {
                _currentPreviewItem = playableItem;
                this.EventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Publish(playableItem);
            }
        }

        private void SearchMediaItems(RequestPagingItemsEventArgs e)
        {
            if (_currentPreviewItem != null)
            {
                _currentPreviewItem.ClosePreview();
                _currentPreviewItem = null;
            }

            _mediaItemCollection.Clear();
            
            var result = MediaFileService.GetMediaFiles(this.SearchOptions, e.PagingInfo);

            foreach (var item in result.Items)
            {
                _mediaItemCollection.Add(new BindableFileMediaItem(item,this.ResolvePath(item.FileName)));
            }

            e.Result = result;
        }

        private string ResolvePath(string fileName)
        {
            return this.FilePathResolver.Resolve(fileName);
        }
        
        public ObservableCollection<BindableFileMediaItem> MediaItemView
        {
            get
            {
                return _mediaItemCollection;
            }
        }

        public BindableFileMediaItem SelectedMediaItem
        {
            get { return _selectedMediaItem; }
            set
            {
                _selectedMediaItem = value;
                _editMediaItemCommand.RaiseCanExecuteChanged();

                if (_isActive && this.MediaItemSelector != null)
                {
                    this.MediaItemSelector.SelectedMediaItem = 
                        this.SelectedMediaItem == null ? (MediaItem?)null : new MediaItem(this.SelectedMediaItem.MediaSource, this.SelectedMediaItem.PlayRange);
                }

                this.RaisePropertyChanged(nameof(this.SelectedMediaItem));
            }
        }

        public ICommand EditMediaItemCommand
        {
            get
            {
                return _editMediaItemCommand;
            }
        }

        public MediaItemSearchOptions SearchOptions
        {
            get;private set;
        }

        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand;
            }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                this.RaisePropertyChanged(nameof(PageSize));
            }
        }

        private IEventAggregator EventAggregator { get; set; }
        public IMediaFilePathResolver FilePathResolver { get; private set; }
        public IMediaFileImageResolver ImageResolver { get; private set; }

        private bool CanEditMediaItem(object cmdParameter)
        {
            return false;
        }
      
        private void EditMediaItem(object cmdParameter)
        {
            if (CanEditMediaItem(cmdParameter))
            {
            }
        }

        public event EventHandler IsActiveChanged;
        private bool _isActive;
        private IRegionManager _regionManager;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (_isActive && this.MediaItemSelector!=null)
                {
                    this.MediaItemSelector.SelectedMediaItem = this.SelectedMediaItem == null ? (MediaItem?)null : new MediaItem(this.SelectedMediaItem.MediaSource,this.SelectedMediaItem.PlayRange);
                }
            }
        }

        public IMediaItemSelector MediaItemSelector
        {
            get { return _regionManager.Regions["mediaItemRegion"].Context as IMediaItemSelector; }
        }

        public IMediaFileService MediaFileService { get; private set; }
    }
}
