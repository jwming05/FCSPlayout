//using FCSPlayout.Domain;
using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using FCSPlayout.MediaFileImporter;
using FCSPlayout.WPF.Core;
using FCSPlayout.WPFApp.Views;
//using FCSPlayout.WPFApp.Models;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FCSPlayout.WPFApp.ViewModels
{
    public class MediaItemListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<BindableMediaFileItem> _mediaItemCollection;
        private readonly CollectionView _mediaItemView;
        private BindableMediaFileItem _selectedMediaItem;
        private readonly DelegateCommand _addMediaItemCommand;
        private readonly DelegateCommand _deleteMediaItemCommand;
        private readonly DelegateCommand<object> _editMediaItemCommand;
        private readonly DelegateCommand _saveMediaItemsCommand;
        //private readonly DelegateCommand _loadMediaItemsCommand;

        //private InteractionRequest<AddNullMediaItemConfirmation> _addNullMediaItemInteractionRequest;
        //private InteractionRequest<AddChannelMediaItemsConfirmation> _addChannelMediaItemsInteractionRequest;
        public MediaItemListViewModel(/*IUploadProgressFeedback progressFeedback*/)
        {
            //ProgressFeedback = progressFeedback;
            _worker = new DelegateBackgroundWorker();
            _worker.ProgressChangedHandler = this.ReportUploadProgress;
            _worker.RunCompletedHandler = this.OnUploadCompleted;


            _mediaItemCollection = new ObservableCollection<BindableMediaFileItem>();
            _mediaItemView = new CollectionView(_mediaItemCollection);

            
            //_addNullMediaItemInteractionRequest = new InteractionRequest<AddNullMediaItemConfirmation>();
            //_addChannelMediaItemsInteractionRequest = new InteractionRequest<AddChannelMediaItemsConfirmation>();

            _addMediaItemCommand = new DelegateCommand(AddMediaItem);

            _deleteMediaItemCommand = new DelegateCommand(DeleteMediaItem, CanDeleteMediaItem);
            _editMediaItemCommand = new DelegateCommand<object>(EditMediaItem, CanEditMediaItem);

            _saveMediaItemsCommand = new DelegateCommand(SaveMediaItems, CanSaveMediaItems);
            //_loadMediaItemsCommand = new DelegateCommand(LoadMediaItems);
        }

        private bool CanDeleteMediaItem()
        {
            return this.SelectedMediaItem != null;
        }

        private void DeleteMediaItem()
        {
            if (CanDeleteMediaItem())
            {
                _mediaItemCollection.Remove(this.SelectedMediaItem);

                _saveMediaItemsCommand.RaiseCanExecuteChanged();
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


                    //List<BindableMediaFileItem> list = new List<BindableMediaFileItem>();
                    //list.Add(_mediaItemCollection[0]);
                    //MainWindow.mw.dgMediaItem1.ItemsSource = list;
                    //_mediaItemCollection.RemoveAt(0);
                }
                //_progressFeedback.Close();
            }
        }

        

        //private void LoadMediaItems()
        //{
        //    this.OpenFileInteractionRequest.Raise(new OpenFileDialogConfirmation()
        //    { Title = "打开", Multiselect = true, Filter = "XML文件 (*.xml)|*.xml" }, n =>
        //    {
        //        if (n.Confirmed)
        //        {
        //            foreach (var fileName in n.FileNames)
        //            {
        //                try
        //                {
        //                    var mediaItems = _mediaItemRepository.Load(fileName);
        //                    foreach (var item in mediaItems)
        //                    {
        //                        _mediaItemCollection.Add(new BindableMediaItem(item));
        //                    }
        //                }
        //                catch
        //                {

        //                }
        //            }
        //        }
        //    });
        //}

        public CollectionView MediaItemView
        {
            get
            {
                return _mediaItemView;
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
                OnSelectedMediaItemChanged();
            }
        }

        public event EventHandler SelectedMediaItemChanged;
        private void OnSelectedMediaItemChanged()
        {
            if (SelectedMediaItemChanged != null)
            {
                SelectedMediaItemChanged(this, EventArgs.Empty);
            }
        }

        public ICommand AddMediaItemCommand
        {
            get
            {
                return _addMediaItemCommand;
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

        public ICommand SaveMediaItemsCommand
        {
            get
            {
                return _saveMediaItemsCommand;
            }
        }

        //public ICommand LoadMediaItemsCommand
        //{
        //    get
        //    {
        //        return _loadMediaItemsCommand;
        //    }
        //}



        //public InteractionRequest<AddNullMediaItemConfirmation> AddNullMediaItemInteractionRequest
        //{
        //    get { return _addNullMediaItemInteractionRequest; }
        //}

        //public InteractionRequest<AddChannelMediaItemsConfirmation> AddChannelMediaItemsInteractionRequest
        //{
        //    get { return _addChannelMediaItemsInteractionRequest; }
        //}

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

        public object DataGridRowHelper { get; private set; }

        private void AddMediaItem()
        {
            if (OpenFileInteractionRequest != null)
            {
                OpenFileInteractionRequest.Raise(new OpenFileDialogConfirmation() { Filter = "媒体文件(*.*)|*.*", Multiselect = true },
                n =>
                {
                    if (n.Confirmed)
                    {
                        var fileNames = n.FileNames.ToList();
                        for (int j=0;j<fileNames.Count;j++)
                        {
                            var fileName = fileNames[j];

                            TimeSpan duration = GetFileDuration(fileName);
                            //var mediaItem = new MediaItem(new FileMediaSource(fileName,duration));
                            _mediaItemCollection.Add(new BindableMediaFileItem(fileName));
                            PlaybillEditor.datagridHepler.SetShowRowIndexProperty(MediaItemListView.mv.dgMediaItem, true);
                            _saveMediaItemsCommand.RaiseCanExecuteChanged();
                        }
                    }
                });
            }
        }

        private TimeSpan GetFileDuration(string fileName)
        {
            return MediaFileDurationGetter.Current.GetDuration(fileName);
        }

        private bool CanEditMediaItem(object cmdParameter)
        {
            Views.DataGridRowDoubleClickEventArgs args = cmdParameter as Views.DataGridRowDoubleClickEventArgs;
            if (args != null)
            {
                BindableMediaFileItem item = args.Item as BindableMediaFileItem;
                return item != null;
            }
            return false;
        }

        private void EditMediaItem(object cmdParameter)
        {
            if (CanEditMediaItem(cmdParameter))
            {
                Views.DataGridRowDoubleClickEventArgs args = cmdParameter as Views.DataGridRowDoubleClickEventArgs;
                BindableMediaFileItem bindable = args.Item as BindableMediaFileItem;
                //System.Diagnostics.Debug.WriteLine(cmdParameter);
                //bindable = this.SelectedMediaItem as BindableMediaFileItem;
                if (bindable != null)
                {
                    var strPath = bindable.FilePath; // ((FileMediaSource)).Path;
                    var range = new PlayRange(bindable.StartPosition, bindable.StopPosition- bindable.StartPosition);// new PlayRange(bindable.Duration, bindable.StartPosition, bindable.StopPosition);
                    EditMediaItemInteractionRequest.Raise(new WPF.Core.EditMediaItemConfirmation(strPath, range/*.MediaItem*/)
                    { Title = bindable.Title },
                    n =>
                    {
                        if (n.Confirmed)
                        {
                            bindable.PlayRange =new PlayRange(n.PlayRange.StartPosition,n.PlayRange.Duration);
                        }
                    });

                }
                else
                {
                }
            }
        }

        #region
        private void Upload(BindableMediaFileItem item)
        {
            _worker.State = item;
            //string destFileName = GetDestFileName(item.FilePath);

            _worker.Run((c) => { MediaFileService.UploadFile(item.FilePath, item.FileName, c); });
            //SaveToDB(item);
        }

        //private static void SaveToDB(BindableMediaFileItem item)
        //{
            

        //    //using (var context = new PlayoutDbContext())
        //    //{
                
        //    //    entity.CreatorId = UserService.CurrentUser.Id;

        //    //    context.MediaFiles.Add(entity);
        //    //    context.SaveChanges();
        //    //}
        //}

        //private string GetDestFileName(string filePath)
        //{
        //    return Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(filePath);
        //}

        private void ReportUploadProgress(int progress,object state)
        {
            ProgressFeedback.Report(progress, (MediaFileStorage)state);
        }

        //public static List<object> list;
        private void OnUploadCompleted(Exception error,bool cancelled,object result)
        {
            BindableMediaFileItem item = (BindableMediaFileItem)_worker.State;

            if (error==null && !cancelled)
            {
                var entity = item.Entity;
                MediaFileService.Add(item.Entity, App.Name);

                //SaveToDB(item);

                _worker.State = null;
                _mediaItemCollection.Remove(item);
                //list = new List<object>();
                //list.Add(item);

                //MainWindow.mw.dgMediaItem1.Items.Add(list);
            }
            else
            {
                _worker.State = null;
                _mediaItemCollection.Remove(item);

                // 放到尾部。
                _mediaItemCollection.Add(item);
            }

            //PlaybillEditor.datagridHepler.SetShowRowIndexProperty(MainWindow.mw.dgMediaItem1, true);

            //MainWindow.mw.dgMediaItem1.ItemsSource =list;

            if (_mediaItemCollection.Count > 0)
            {
                item = _mediaItemCollection[0];
                Upload(item);
                //_mediaItemCollection.RemoveAt(0);
            }
            else
            {
                ProgressFeedback.Close();
            }
        }
       
        //添加行号
    

        //private UploadStreamCreator _uploadStreamCreator = FileSystemUploadStreamCreator.Instance;

        //private UploadStreamCreator _uploadStreamCreatorSecondary;

        private IUploadProgressFeedback _progressFeedback;
        private DelegateBackgroundWorker _worker;
        #endregion
    }
}
