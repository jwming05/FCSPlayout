﻿using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Regions;
using Prism.Interactivity.InteractionRequest;

namespace FCSPlayout.WPFApp
{
    public class MediaItemListViewModel : ViewModelBase, Prism.IActiveAware
    {
        //private readonly ObservableCollection<BindableMediaFileItem> _mediaItemCollection;
        private readonly ObservableCollection<BindableFileMediaItem> _mediaItemCollection;

        private BindableFileMediaItem _selectedMediaItem;
        //private readonly DelegateCommand _deleteMediaItemCommand;
        private readonly DelegateCommand<object> _editMediaItemCommand;
        private readonly DelegateCommand<IPlayableItem> _previewCommand;
        private readonly DelegateCommand<RequestPagingItemsEventArgs> _searchCommand;
        
        private IPlayableItem _currentPreviewItem;
        private int _pageSize = 30;

        public MediaItemListViewModel(IEventAggregator eventAggregator, IMediaFilePathResolver filePathResolver, 
            IMediaFileImageResolver imageResolver,IRegionManager regionManager,InteractionRequests interactionRequests,
            IMediaFileService mediaFileService)
        {
            this.EventAggregator = eventAggregator;
            this.FilePathResolver = filePathResolver;
            this.ImageResolver = imageResolver;
            this.MediaFileService = mediaFileService;
            _interactionRequests = interactionRequests;
            _regionManager = regionManager;

            //_mediaItemCollection = new ObservableCollection<BindableMediaFileItem>();
            _mediaItemCollection = new ObservableCollection<BindableFileMediaItem>();

            //_deleteMediaItemCommand = new DelegateCommand(DeleteMediaItem, CanDeleteMediaItem);
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
            if (/*this.EventAggregator != null && */playableItem != null)
            {
                _currentPreviewItem = playableItem;
                this.PreviewInteractionRequest.Raise(new PreviewRequestConfirmation(playableItem) { Title = "预览", PlayItemEditorFactory =null },
                (c) =>
                {
                    if (c.Confirmed)
                    {

                    }

                    playableItem.ClosePreview();
                });

                //this.EventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Publish(playableItem);
            }
        }

        public InteractionRequest<PreviewRequestConfirmation> PreviewInteractionRequest
        {
            get { return _interactionRequests.PreviewInteractionRequest; }
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

        //private bool CanDeleteMediaItem()
        //{
        //    return this.SelectedMediaItem != null && 
        //        (UserService.CurrentUser.IsAdmin() || this.SelectedMediaItem.CreatorId==UserService.CurrentUser.Id);
        //}

        //private void DeleteMediaItem()
        //{
        //    if (CanDeleteMediaItem())
        //    {
        //        var item = this.SelectedMediaItem;
        //        this.SelectedMediaItem = null;

        //        if (item == _currentPreviewItem)
        //        {
        //            _currentPreviewItem.ClosePreview();
        //            _currentPreviewItem = null;
        //        }

        //        MediaFileService.DeleteMediaFile(item.Entity, App.Current.Name);
        //        _mediaItemCollection.Remove(item);
        //        //_saveMediaItemsCommand.RaiseCanExecuteChanged();
        //    }
        //}

        

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
                //_deleteMediaItemCommand.RaiseCanExecuteChanged();

                if (_isActive && this.MediaItemSelector != null)
                {
                    this.MediaItemSelector.SelectedMediaItem = 
                        this.SelectedMediaItem == null ? (MediaItem?)null : new MediaItem(this.SelectedMediaItem.MediaSource, this.SelectedMediaItem.PlayRange);
                    this.EventAggregator.GetEvent<PubSubEvent<MediaItem?>>().Publish(this.MediaItemSelector.SelectedMediaItem);
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

        //public ICommand DeleteMediaItemCommand
        //{
        //    get
        //    {
        //        return _deleteMediaItemCommand;
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

        public event EventHandler IsActiveChanged;
        private bool _isActive;
        private IRegionManager _regionManager;
        private InteractionRequests _interactionRequests;

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
