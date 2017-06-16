using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.WPF.Core;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
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
        private readonly DelegateCommand _saveXmlCommand;
        private readonly DelegateCommand _openXmlCommand;
        private readonly DelegateCommand _clearCommand;

        private IUploadProgressFeedback _progressFeedback;
        private DelegateBackgroundWorker _worker;
        private BindableMediaFileItem _currentUploadItem;
        private InteractionRequests _interactionRequests;
        private IPlayableItem _currentPreviewItem;

        public MediaItemListViewModel(IEventAggregator eventAggregator,
            InteractionRequests interactionRequests,
            IMediaFileImageResolver imageResolver,IUserService userService,IMediaFileService mediaFileService)
        {
            this.EventAggregator = eventAggregator;
            this.ImageResolver = imageResolver;
            this.UserService = userService;
            this.MediaFileService = mediaFileService;

            _interactionRequests = interactionRequests;

            _worker = new DelegateBackgroundWorker();
            _worker.ProgressChangedHandler = this.ReportUploadProgress;
            _worker.RunCompletedHandler = this.OnUploadCompleted;
         

            _mediaItemCollection = new ObservableCollection<BindableMediaFileItem>();
            _mediaItemCollection.CollectionChanged += MediaItemCollection_CollectionChanged;

            _addMediaItemCommand = new DelegateCommand(AddMediaItem);
            _deleteMediaItemCommand = new DelegateCommand(DeleteMediaItem, CanDeleteMediaItem);
            _saveMediaItemsCommand = new DelegateCommand(SaveMediaItems, CanSaveMediaItems);
            _saveXmlCommand = new DelegateCommand(SaveXml, CanSaveXml);
            _openXmlCommand = new DelegateCommand(OpenXml);
            _previewCommand = new DelegateCommand<IPlayableItem>(Preview);
            _clearCommand = new DelegateCommand(Clear, CanClear);
        }

        

        private void MediaItemCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _saveMediaItemsCommand.RaiseCanExecuteChanged();
            _saveXmlCommand.RaiseCanExecuteChanged();
            _clearCommand.RaiseCanExecuteChanged();
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

        
        #region
        private void Upload(BindableMediaFileItem item)
        {
            var entity = item.Entity;
            var metadata = entity.Metadata;

            if (metadata == null)
            {
                metadata = new Entities.MediaFileMetadata();
                entity.Metadata = metadata;
            }

            if (metadata.Icon == null)
            {
                var image = item.Image;
                if (image == null)
                {
                    image = this.ResolveImage(item);
                }
                metadata.Icon = GetIcon(image);
            }

            this.ProgressFeedback.Reset();

            CurrentUploadItem = item;
            _worker.State = item;
            _worker.Run((c) => 
            {
                this.DoUpload(item, c);
                //this.MediaFileService.UploadFile(item.FilePath, item.FileName, c);
            });
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
        }

        private void DoUpload(BindableMediaFileItem item, IBackgroundWorkContext c)
        {
            this.MediaFileService.UploadFile(item.FilePath, item.FileName, c);
            this.MediaFileService.Add(item.Entity, App.Current.Name);
        }

        private void OnUploadCompleted(Exception error,bool cancelled,object result)
        {
            BindableMediaFileItem item = (BindableMediaFileItem)_worker.State;

            if (error==null && !cancelled)
            {
                //var entity = item.Entity;
                //var metadata = entity.Metadata;

                //if (metadata == null)
                //{
                //    metadata = new Entities.MediaFileMetadata();
                //    entity.Metadata = metadata;
                //}

                //if(metadata.Icon==null)
                //{
                //    var image = item.Image;
                //    if (image == null)
                //    {
                //        image = this.ResolveImage(item);
                //    }
                //    metadata.Icon = GetIcon(image);
                //}

                //this.MediaFileService.Add(item.Entity, App.Current.Name);
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
                CurrentUploadItem = null;
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

        

        public IMediaFileImageResolver ImageResolver
        {
            get;private set;
        }
        public IUserService UserService { get; private set; }

        public BindableMediaFileItem CurrentUploadItem
        {
            get
            {
                return _currentUploadItem;
            }

            set
            {
                _currentUploadItem = value;

                _saveMediaItemsCommand.RaiseCanExecuteChanged();
                _saveXmlCommand.RaiseCanExecuteChanged();
                _deleteMediaItemCommand.RaiseCanExecuteChanged();
                _clearCommand.RaiseCanExecuteChanged();
            }
        }

        public IMediaFileService MediaFileService { get; private set; }

        #region Commands
        public ICommand PreviewCommand
        {
            get { return _previewCommand; }
        }

        public ICommand SaveXmlCommand
        {
            get
            {
                return _saveXmlCommand;
            }
        }

        public ICommand OpenXmlCommand
        {
            get
            {
                return _openXmlCommand;
            }
        }

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

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand;
            }
        }

        
        #endregion Commands

        #region Command Methods
        private void AddMediaItem()
        {
            _interactionRequests.OpenFileInteractionRequest.Raise(
                new OpenFileDialogConfirmation() { Filter = "媒体文件(*.*)|*.*", Multiselect = true },
                n =>
                {
                    if (n.Confirmed)
                    {
                        AddMediaItems(n.FileNames);
                    }
                });
        }

        private void AddMediaItems(string[] fileNames)
        {
            for (int j = 0; j < fileNames.Length; j++)
            {
                var fileName = fileNames[j];
                _mediaItemCollection.Add(new BindableMediaFileItem(fileName, this.UserService.CurrentUser.Id));
            }
        }

        private bool CanDeleteMediaItem()
        {
            return this.SelectedMediaItem != null && this.SelectedMediaItem != CurrentUploadItem;
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

        private bool CanClear()
        {
            return _mediaItemCollection.Count > 0 && this.CurrentUploadItem == null;
        }

        private void Clear()
        {
            if (CanClear())
            {
                _mediaItemCollection.Clear();
            }
        }

        private void Preview(IPlayableItem playableItem)
        {
            if (this.EventAggregator != null && playableItem != null)
            {
                _currentPreviewItem = playableItem;
                this.EventAggregator.GetEvent<PubSubEvent<IPlayableItem>>().Publish(playableItem);
            }
        }

        private void OpenXml()
        {
            _interactionRequests.OpenFileInteractionRequest.Raise(new OpenFileDialogConfirmation() { Filter = "XML文件(*.xml)|*.xml", Multiselect = false },
                n =>
                {
                    if (n.Confirmed)
                    {
                        foreach (var item in LoadFromXml(n.FileName))
                        {
                            if (!string.IsNullOrEmpty(item.FilePath) && System.IO.File.Exists(item.FilePath))
                            {
                                _mediaItemCollection.Add(item);
                            }

                        }
                    }
                });
        }

        private bool CanSaveXml()
        {
            return _mediaItemCollection.Count > 0 && this.CurrentUploadItem == null;
        }

        private void SaveXml()
        {
            if (CanSaveXml())
            {
                _interactionRequests.SaveFileInteractionRequest.Raise(new SaveFileDialogConfirmation() { Filter = "XML文件(*.xml)|*.xml", OverwritePrompt = true },
                n =>
                {
                    if (n.Confirmed)
                    {
                        SaveToXml(n.FileName, _mediaItemCollection);
                    }
                });
            }
        }

        private bool CanSaveMediaItems()
        {
            return _mediaItemCollection.Count > 0 && this.CurrentUploadItem == null;
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

        private void SaveToXml(string fileName, IList<BindableMediaFileItem> items)
        {
            XmlRepository.Save(fileName, items.Select(i => i.Entity));
        }

        private IEnumerable<BindableMediaFileItem> LoadFromXml(string fileName)
        {
            return XmlRepository.Load(fileName).Select(i => new BindableMediaFileItem(i, i.OriginalFileName));
        }
        #endregion Command Methods
    }
}
