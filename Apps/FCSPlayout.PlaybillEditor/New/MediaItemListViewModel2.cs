using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FCSPlayout.PlaybillEditor
{
    public class MediaItemListViewModel2 : ViewModelBase
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

        public MediaItemListViewModel2(IEventAggregator eventAggregator, IMediaFilePathResolver filePathResolver, 
            IMediaFileImageResolver imageResolver)
        {
            this.EventAggregator = eventAggregator;
            this.FilePathResolver = filePathResolver;
            this.ImageResolver = imageResolver;

            _mediaItemCollection = new ObservableCollection<BindableMediaFileItem>();

            _deleteMediaItemCommand = new DelegateCommand(DeleteMediaItem, CanDeleteMediaItem);
            _editMediaItemCommand = new DelegateCommand<object>(EditMediaItem, CanEditMediaItem);

            //_saveMediaItemsCommand = new DelegateCommand(SaveMediaItems, CanSaveMediaItems);

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
                _mediaItemCollection.Add(new BindableMediaFileItem(item,this.ResolvePath(item.FileName)));
            }

            e.Result = result;
        }

        //private static string ResolvePath(string fileName)
        //{
        //    return MediaFilePathResolver.Current.Resolve(fileName, BindableMediaFileItem.MediaFileStorage);
        //}

        private string ResolvePath(string fileName)
        {
            return this.FilePathResolver.Resolve(fileName/*, BindableMediaFileItem.MediaFileStorage*/);
        }

        private bool CanDeleteMediaItem()
        {
            return this.SelectedMediaItem != null && 
                (UserService.CurrentUser.IsAdmin() || this.SelectedMediaItem.CreatorId==UserService.CurrentUser.Id);
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

                MediaFileService.DeleteMediaFile(item.Entity, App.Current.Name);
                _mediaItemCollection.Remove(item);
                //_saveMediaItemsCommand.RaiseCanExecuteChanged();
            }
        }

        //private bool CanSaveMediaItems()
        //{
        //    return _mediaItemCollection.Count > 0;
        //}

        //private void SaveMediaItems()
        //{
        //    if (CanSaveMediaItems())
        //    {
        //        if (_mediaItemCollection.Count > 0)
        //        {
        //            var item = _mediaItemCollection[0];
        //        }
        //    }
        //}

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

        //public event EventHandler SelectedMediaItemChanged;
        //private void OnSelectedMediaItemChanged()
        //{
        //    if (SelectedMediaItemChanged != null)
        //    {
        //        SelectedMediaItemChanged(this, EventArgs.Empty);
        //    }
        //}

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

        //public ICommand SaveMediaItemsCommand
        //{
        //    get
        //    {
        //        return _saveMediaItemsCommand;
        //    }
        //}

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
    }
}
