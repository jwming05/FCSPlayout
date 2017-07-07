using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FCSPlayout.MediaFileImporter
{
    public class DbMediaItemListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<BindableMediaFileItem> _mediaItemCollection;

        private BindableMediaFileItem _selectedMediaItem;
        private readonly DelegateCommand _deleteMediaItemCommand;
        private readonly DelegateCommand<object> _editMediaItemCommand;
        //private readonly DelegateCommand _saveMediaItemsCommand;
        private readonly DelegateCommand<IPlayableItem> _previewCommand;
        private readonly DelegateCommand<RequestPagingItemsEventArgs> _searchCommand;
        
        private IPlayableItem _currentPreviewItem;
        private int _pageSize = 30;
        private MediaFileItemManager _mediaFileItemManager;

        public DbMediaItemListViewModel(IEventAggregator eventAggregator, /*IMediaFilePathResolver filePathResolver,*/ 
            IMediaFileImageResolver imageResolver,IMediaFileService mediaFileService, MediaFileItemManager mediaFileItemManager)
        {
            this.EventAggregator = eventAggregator;
            //this.FilePathResolver = filePathResolver;
            this.ImageResolver = imageResolver;
            this.MediaFileService = mediaFileService;
            _mediaItemCollection = new ObservableCollection<BindableMediaFileItem>();
            _mediaFileItemManager = mediaFileItemManager;

            _deleteMediaItemCommand = new DelegateCommand(DeleteMediaItem, CanDeleteMediaItem);
            _editMediaItemCommand = new DelegateCommand<object>(EditMediaItem, CanEditMediaItem);

            //_saveMediaItemsCommand = new DelegateCommand(SaveMediaItems, CanSaveMediaItems);

            _searchCommand = new DelegateCommand<RequestPagingItemsEventArgs>(SearchMediaItems);

            _previewCommand = new DelegateCommand<IPlayableItem>(Preview);
            this.SearchOptions = new MediaItemSearchOptions();
        }

        public ObservableCollection<BindableMediaFileItem> MediaItemView
        {
            get
            {
                return _mediaItemCollection;
            }
        }

        public BindableMediaFileItem SelectedMediaItem
        {
            get { return _selectedMediaItem; }
            set
            {
                _selectedMediaItem = value;
                _editMediaItemCommand.RaiseCanExecuteChanged();
                _deleteMediaItemCommand.RaiseCanExecuteChanged();

                //OnSelectedMediaItemChanged();
                this.RaisePropertyChanged(nameof(this.SelectedMediaItem));
            }
        }

        public MediaItemSearchOptions SearchOptions
        {
            get;private set;
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
        //public IMediaFilePathResolver FilePathResolver { get; private set; }
        public IMediaFileImageResolver ImageResolver { get; private set; }
        public IMediaFileService MediaFileService { get; private set; }

        //private string ResolvePath(string fileName)
        //{
        //    return this.FilePathResolver.Resolve(fileName);
        //}

        #region Command Methods
        private bool CanDeleteMediaItem()
        {
            return this.SelectedMediaItem != null &&
                (UserService.CurrentUser.IsAdmin() || this.SelectedMediaItem.CreatorId == UserService.CurrentUser.Id);
        }

        private void DeleteMediaItem()
        {
            if (CanDeleteMediaItem())
            {
                var item = this.SelectedMediaItem;
                this.SelectedMediaItem = null;

                if (item == _currentPreviewItem)
                {
                    _currentPreviewItem.ClosePreview();
                    _currentPreviewItem = null;
                }

                this.MediaFileService.DeleteMediaFile(item.Entity, App.Current.Name);
                _mediaItemCollection.Remove(item);
                //_saveMediaItemsCommand.RaiseCanExecuteChanged();
            }
        }

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

        private async void SearchMediaItems(RequestPagingItemsEventArgs e)
        {
            if (_currentPreviewItem != null)
            {
                _currentPreviewItem.ClosePreview();
                _currentPreviewItem = null;
            }

            _mediaItemCollection.Clear();

            //var result = this.MediaFileService.GetMediaFiles(this.SearchOptions, e.PagingInfo);
            var result = await this.MediaFileService.GetMediaFilesAsync(this.SearchOptions, e.PagingInfo);
            foreach (var item in result.Items)
            {
                _mediaItemCollection.Add(_mediaFileItemManager.Create(item));
                //_mediaItemCollection.Add(new BindableMediaFileItem(item, this.ResolvePath(item.FileName)));
            }

            e.Result = result;
        }

        private void Preview(IPlayableItem playableItem)
        {
            if (this.EventAggregator != null && playableItem != null)
            {
                _currentPreviewItem = playableItem;
                this.EventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Publish(playableItem);
            }
        }
        #endregion Command Methods

        #region Commands
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand;
            }
        }

        public ICommand EditMediaItemCommand
        {
            get
            {
                return _editMediaItemCommand;
            }
        }

        public ICommand DeleteMediaItemCommand
        {
            get
            {
                return _deleteMediaItemCommand;
            }
        }

        public ICommand PreviewCommand
        {
            get { return _previewCommand; }
        }
        #endregion Commands
    }
}
