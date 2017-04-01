using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace FCSPlayout.MediaFileImporter
{
    public class MediaItemListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<BindableMediaFileItem> _mediaItemCollection;
        private BindableMediaFileItem _selectedMediaItem;
        
        private readonly DelegateCommand _addMediaItemCommand;
        private readonly DelegateCommand _deleteMediaItemCommand;
        private readonly DelegateCommand _saveMediaItemsCommand;
        private readonly DelegateCommand<IPlayableItem> _previewCommand;

        private IUploadProgressFeedback _progressFeedback;
        private DelegateBackgroundWorker _worker;
        private BindableMediaFileItem _currentUploadItem;
        private InteractionRequests _interactionRequests;
        private IPlayableItem _currentPreviewItem;

        public MediaItemListViewModel(IEventAggregator eventAggregator,
            InteractionRequests interactionRequests,
            IMediaFileImageResolver imageResolver,IUserService userService)
        {
            this.EventAggregator = eventAggregator;
            this.ImageResolver = imageResolver;
            this.UserService = userService;

            _worker = new DelegateBackgroundWorker();
            _worker.ProgressChangedHandler = this.ReportUploadProgress;
            _worker.RunCompletedHandler = this.OnUploadCompleted;
         

            _mediaItemCollection = new ObservableCollection<BindableMediaFileItem>();
            _addMediaItemCommand = new DelegateCommand(AddMediaItem);
            _deleteMediaItemCommand = new DelegateCommand(DeleteMediaItem, CanDeleteMediaItem);
            _saveMediaItemsCommand = new DelegateCommand(SaveMediaItems, CanSaveMediaItems);

            _previewCommand = new DelegateCommand<IPlayableItem>(Preview);

            _interactionRequests = interactionRequests;
        }

        private void Preview(IPlayableItem playableItem)
        {
            if(this.EventAggregator!=null && playableItem != null)
            {
                _currentPreviewItem = playableItem;
                this.EventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Publish(playableItem);
            }
        }

        private bool CanDeleteMediaItem()
        {
            return this.SelectedMediaItem != null && this.SelectedMediaItem!=_currentUploadItem;
        }

        private void DeleteMediaItem()
        {
            if (CanDeleteMediaItem())
            {
                var item = this.SelectedMediaItem;
                this.SelectedMediaItem = null;

                RemoveItem(item);
            }
        }

        private bool CanSaveMediaItems()
        {
            return _mediaItemCollection.Count > 0;
        }

        private void SaveMediaItems()
        {
            if (CanSaveMediaItems())
            {
                ProgressFeedback.Open();
                if (_mediaItemCollection.Count > 0)
                {
                    var item = _mediaItemCollection[0];
                    Upload(item);      
                }
            }
        }
     
        public ObservableCollection<BindableMediaFileItem> MediaItems
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
                _deleteMediaItemCommand.RaiseCanExecuteChanged();
                //OnSelectedMediaItemChanged();
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

        public ICommand AddMediaItemCommand
        {
            get
            {
                return _addMediaItemCommand;
            }
        }

        public ICommand DeleteMediaItemCommand
        {
            get
            {
                return _deleteMediaItemCommand;
            }
        }

        public ICommand SaveMediaItemsCommand
        {
            get
            {
                return _saveMediaItemsCommand;
            }
        }

        public IUploadProgressFeedback ProgressFeedback
        {
            get
            {
                return _progressFeedback ?? NullUploadProgressFeedback.Instance;
            }

            set
            {
                _progressFeedback = value;
            }
        }

        public IEventAggregator EventAggregator { get; private set; }

        private void AddMediaItem()
        {
            _interactionRequests.OpenFileInteractionRequest.Raise(new OpenFileDialogConfirmation() { Filter = "媒体文件(*.*)|*.*", Multiselect = true },
                n =>
                {
                    if (n.Confirmed)
                    {
                        var fileNames = n.FileNames.ToList();
                        for (int j = 0; j < fileNames.Count; j++)
                        {
                            var fileName = fileNames[j];

                            _mediaItemCollection.Add(new BindableMediaFileItem(fileName,this.UserService.CurrentUser.Id));
                            _saveMediaItemsCommand.RaiseCanExecuteChanged();

                        }
                    }
                });
        }

        #region
        private void Upload(BindableMediaFileItem item)
        {
            this.ProgressFeedback.Reset();

            _currentUploadItem = item;
            _worker.State = item;
            _worker.Run((c) => { MediaFileService.UploadFile(item.FilePath, item.FileName, c); });
        }

        private void ReportUploadProgress(int progress,object state)
        {
            ProgressFeedback.Report(progress, (MediaFileStorage)state);
        }

        private void RemoveItem(BindableMediaFileItem item)
        {
            if (_currentPreviewItem == item)
            {
                item.ClosePreview();
                _currentPreviewItem = null;
            }
            
            _mediaItemCollection.Remove(item);
            _saveMediaItemsCommand.RaiseCanExecuteChanged();
        }

        private void OnUploadCompleted(Exception error,bool cancelled,object result)
        {
            BindableMediaFileItem item = (BindableMediaFileItem)_worker.State;

            if (error==null && !cancelled)
            {
                var entity = item.Entity;
                var metadata = entity.Metadata;

                if (metadata == null)
                {
                    metadata = new Entities.MediaFileMetadata();
                    entity.Metadata = metadata;
                }

                if(metadata.Icon==null)
                {
                    var image = item.Image;
                    if (image == null)
                    {
                        //image = item.ResolveImage();
                        image = this.ResolveImage(item);
                    }
                    metadata.Icon = GetIcon(image);
                }

                MediaFileService.Add(item.Entity, App.Current.Name);
                _worker.State = null;
                RemoveItem(item);
            }
            else
            {
                _worker.State = null;
                _mediaItemCollection.Remove(item);

                // 放到尾部。
                _mediaItemCollection.Add(item);
            }

            if (_mediaItemCollection.Count > 0)
            {
                item = _mediaItemCollection[0];
                Upload(item);
            }
            else
            {
                _currentUploadItem = null;
                ProgressFeedback.Close();
            }
        }

        internal BitmapSource ResolveImage(BindableMediaFileItem item)
        {
            return this.ImageResolver.Resolve(item.FilePath, item.Duration.TotalMilliseconds / 2.0);
        }

        internal static byte[] GetIcon(BitmapSource image)
        {
            if (image != null)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    var jpegEncoder = new JpegBitmapEncoder();
                    jpegEncoder.Frames.Add(BitmapFrame.Create(image));
                    jpegEncoder.Save(ms);
                    return ms.ToArray();
                }
            }

            return null;
        }
        #endregion

        public ICommand PreviewCommand
        {
            get { return _previewCommand; }
        }

        public IMediaFileImageResolver ImageResolver
        {
            get;private set;
        }
        public IUserService UserService { get; private set; }
    }
}
